using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace SilverBotDS.Objects
{
    internal class Logging
    {
        public static async Task Log(DiscordMessage message)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\Logging"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\Logging");
            }
            if (!Directory.Exists(Environment.CurrentDirectory + "\\Logging\\Messages"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\Logging\\Messages");
            }

            await using var sw = new StreamWriter(Environment.CurrentDirectory + $"\\Logging\\Messages\\{message.Channel.Id}.slvrlog", true);
            var logmessage = new Logmessage
            {
                Content = message.Content,
                Jumplink = message.JumpLink.ToString(),
                Time = message.Timestamp.UtcDateTime,
                Attachments = message.Attachments.ToList(),
            };
            await sw.WriteAsync(System.Text.Json.JsonSerializer.Serialize(logmessage));
        }
    }
}