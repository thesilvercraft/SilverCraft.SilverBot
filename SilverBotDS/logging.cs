using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SilverBotDS
{
    internal class Logging
    {
        public static void Log(DiscordMessage message)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\Logging"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\Logging");
            }
            if (!Directory.Exists(Environment.CurrentDirectory + "\\Logging\\Messages"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\Logging\\Messages");
            }
            using StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + $"\\Logging\\Messages\\{message.Channel.Id}.slvrlog", true);
            var logmessage = new Logmessage
            {
                Content = message.Content,
                Jumplink = message.JumpLink.ToString(),
                Time = message.Timestamp.UtcDateTime,
                Attachments = message.Attachments.ToList(),
            };
            sw.Write(System.Text.Json.JsonSerializer.Serialize(logmessage));
        }
    }

    public class Logmessage
    {
        public string Content { get; set; }
        public string Jumplink { get; set; }
        public DateTime Time { get; set; }
        public List<DiscordAttachment> Attachments { get; set; }
    }
}