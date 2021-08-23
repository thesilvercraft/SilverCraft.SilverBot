using Serilog.Core;
using Serilog.Events;
using System;
using DSharpPlus;
using DSharpPlus.Entities;
using System.Text;
using System.Globalization;
using Serilog;
using Serilog.Configuration;
using System.Text.RegularExpressions;
using System.IO;

#nullable enable

namespace SDiscordSink
{
    public static class DiscordSinkExtensions
    {
        public static LoggerConfiguration DiscordSink(
                  this LoggerSinkConfiguration loggerConfiguration, params Tuple<ulong, string>[] webhooks)
        {
            return loggerConfiguration.Sink(new DiscordSink(webhooks));
        }
    }

    public class DiscordSink : ILogEventSink
    {
        private readonly DiscordWebhookClient webhookClient = new();

        public DiscordSink(params Tuple<ulong, string>[] webhooks)
        {
            webhookClient = new();
            foreach (var webhook in webhooks)
            {
                webhookClient.AddWebhookAsync(webhook.Item1, webhook.Item2).Wait();
            }
        }
        Regex VBUErr = new(@"^Error `(P<error>.+?)` encountered.\nGuild `(P<guild_id>\d+|None)`, channel `(P<channel_id>\d+|None)`, user `(P<user_id>\d+|None)`\n```\n(P<command_invoke>.+?)\n```$",RegexOptions.Multiline|RegexOptions.Compiled);
        public void Emit(LogEvent logEvent)
        {
            var builder = new DiscordEmbedBuilder();
            var message = logEvent.RenderMessage();
            if (logEvent.Exception != null && message.StartsWith("Error"))
            {
                webhookClient.BroadcastMessageAsync(new DiscordWebhookBuilder().WithContent($"{message}\n").AddFile("error.cs",new MemoryStream(Encoding.Unicode.GetBytes($"{ logEvent.Exception.Message ?? "null" }\nStack trace:\n{ logEvent.Exception.StackTrace ?? "null"}\nSource\n{ logEvent.Exception.Source ?? "null"}\nHelp link:\n{ logEvent.Exception.HelpLink ?? "null"}")),true).WithUsername("sb - Error"));
            }
            string? restMessage = null;
            if (message.Length > 230)
            {
                var firstLineEnd = message.IndexOfAny(new[] { '\r', '\n' });
                if (firstLineEnd < 0 || firstLineEnd > 230)
                {
                    builder.WithTitle(string.Concat(message.AsSpan(0, 230), "…"));
                    restMessage = "…" + message[230..];
                }
                else
                {
                    builder.WithTitle(message.Substring(0, firstLineEnd));
                    if (message[firstLineEnd] == '\r' && message.Length > firstLineEnd + 1 && message[firstLineEnd + 1] == '\n')
                    {
                        firstLineEnd++;
                    }
                    restMessage = message[(firstLineEnd + 1)..];
                }
            }
            else
            {
                builder.WithTitle(message);
            }
            var sb = new StringBuilder();
            sb.Append('`');
            switch (logEvent.Level)
            {
                case LogEventLevel.Verbose:
                    builder.WithColor(DiscordColor.DarkGray);
                    sb.Append("[VER]");
                    break;
                case LogEventLevel.Debug:
                    sb.Append("[DEB]");
                    break;
                case LogEventLevel.Information:
                    builder.WithColor(DiscordColor.DarkBlue);
                    sb.Append("[INF]");
                    break;
                case LogEventLevel.Warning:
                    builder.WithColor(DiscordColor.Orange);
                    sb.Append("[WRN]");
                    break;
                case LogEventLevel.Error:
                    sb.Append("[ERR]");
                    builder.WithColor(DiscordColor.DarkRed);
                    break;
                case LogEventLevel.Fatal:
                    sb.Append("[FTL]");
                    builder.WithColor(DiscordColor.Red);
                    break;
                default:
                    sb.Append("[DEF]");
                    break;
            }
            sb.Append("` | ");
            sb.Append(Formatter.Timestamp(logEvent.Timestamp.LocalDateTime,TimestampFormat.LongDateTime));
            if (logEvent.Exception != null)
            {
                sb.Append(" | `");
                sb.Append(logEvent.Exception.GetType());
                sb.Append("` |");
                sb.AppendLine();
                var exMessage = logEvent.Exception.Message;
                sb.Append(exMessage?.Length <= 1024 ? exMessage : exMessage?.Substring(0, 1024));
            }
            if (restMessage != null)
            {
                if (sb.Length + restMessage.Length < 2030)
                {
                    sb.Insert(0, '\n');
                    sb.Insert(0, restMessage);
                }
                else
                {
                    var capacity = 2030 - sb.Length;
                    sb.Insert(0, '\n');
                    sb.Insert(0, "…");
                    sb.Insert(0, restMessage.AsSpan(0, capacity));
                }
            }
            builder.WithDescription(sb.ToString());
            webhookClient.BroadcastMessageAsync(new DiscordWebhookBuilder().AddEmbed(builder.Build()).WithUsername("Non-VBU compliant log entry"));
        }
    }
}