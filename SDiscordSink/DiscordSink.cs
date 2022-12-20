/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus;
using DSharpPlus.Entities;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

#nullable enable

namespace SDiscordSink;

public static class DiscordSinkExtensions
{
    public static LoggerConfiguration DiscordSink(
        this LoggerSinkConfiguration loggerConfiguration, params Tuple<ulong, string>[] webhooks)
    {
        return loggerConfiguration.Sink(new DiscordSink(webhooks));
    }

    public static LoggerConfiguration DiscordSink(
       this LoggerSinkConfiguration loggerConfiguration, ulong id, string token)
    {
        return loggerConfiguration.Sink(new DiscordSink(id, token));
    }
}

public class DiscordSink : ILogEventSink
{
    private readonly DiscordWebhookClient _webhookClient;

    public DiscordSink(ulong id, string token)
    {
        _webhookClient = new DiscordWebhookClient();
        _webhookClient.AddWebhookAsync(id, token).Wait();
    }

    public DiscordSink(params Tuple<ulong, string>[] webhooks)
    {
        _webhookClient = new DiscordWebhookClient();
        foreach (var webhook in webhooks)
        {
            _webhookClient.AddWebhookAsync(webhook.Item1, webhook.Item2).Wait();
        }
    }

    private readonly Dictionary<LogEventLevel, Tuple<string, DiscordColor?>> k = new()
    {
        { LogEventLevel.Verbose, new("[VER]", DiscordColor.DarkGray) },
        { LogEventLevel.Debug, new("[DEB]", null) },
        { LogEventLevel.Information, new("[INF]", DiscordColor.DarkBlue) },
        { LogEventLevel.Warning, new("[WRN]", DiscordColor.Orange) },
        { LogEventLevel.Error, new("[ERR]", DiscordColor.DarkRed) },
        { LogEventLevel.Fatal, new("[FTL]", DiscordColor.Red) }
    };

    private readonly Regex VBUErr = new(@"^Error `(P<error>.+?)` encountered.\nGuild `(P<guild_id>\d+|None)`, channel `(P<channel_id>\d+|None)`, user `(P<user_id>\d+|None)`\n```\n(P<command_invoke>.+?)\n```$", RegexOptions.Multiline | RegexOptions.Compiled);
    public void Emit(LogEvent logEvent)
    {
        var builder = new DiscordEmbedBuilder();
        var message = logEvent.RenderMessage();
        if (logEvent.Exception != null && VBUErr.IsMatch(message))
        {
            var msc = $"{logEvent.Exception.Message ?? "null"}\nStack trace:\n{logEvent.Exception.StackTrace ?? "null"}\nSource\n{logEvent.Exception.Source ?? "null"}\nHelp link:\n{logEvent.Exception.HelpLink ?? "null"}";
            var dwb = new DiscordWebhookBuilder().WithContent("Test content").AddFile(
            "error.cs",
                new MemoryStream(Encoding.UTF8.GetBytes(
                   msc)),
               true).WithUsername("sb - Error");
            _webhookClient.BroadcastMessageAsync(dwb);
        }
        else
        {
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
                    builder.WithTitle(message[..firstLineEnd]);
                    if (message[firstLineEnd] == '\r' && message.Length > firstLineEnd + 1 &&
                        message[firstLineEnd + 1] == '\n')
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
            if (k.ContainsKey(logEvent.Level))
            {
                var a = k[logEvent.Level];
                sb.Append(a.Item1);
                if (a.Item2 != null)
                {
                    builder.WithColor((DiscordColor)a.Item2);
                }
            }
            else
            {
                sb.Append("[DEF]");
            }
            sb.Append("` | ");
            sb.Append(Formatter.Timestamp(logEvent.Timestamp.LocalDateTime, TimestampFormat.LongDateTime));
            if (logEvent.Exception != null)
            {
                sb.Append(" | `");
                sb.Append(logEvent.Exception.GetType());
                sb.Append("` |");
                sb.AppendLine();
                var exMessage = logEvent.Exception.Message;
                sb.Append(exMessage?.Length <= 1024 ? exMessage : exMessage?[..1024]);
            }
            if (restMessage != null)
            {
                if (sb.Length + restMessage.Length < 2020)
                {
                    sb.Insert(0, '\n');
                    sb.Insert(0, restMessage);
                }
                else
                {
                    var capacity = 2020 - sb.Length;
                    sb.Insert(0, '\n');
                    sb.Insert(0, "…");
                    sb.Insert(0, restMessage.AsSpan(0, capacity));
                }
            }
            builder.WithDescription(sb.ToString());
            _webhookClient.BroadcastMessageAsync(new DiscordWebhookBuilder().AddEmbed(builder.Build()));
        }
    }
}
