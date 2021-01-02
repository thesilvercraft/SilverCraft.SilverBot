using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS
{
    internal class logging
    {
        public static void log(DiscordMessage message)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\Logging"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\Logging");
            }
            if (!Directory.Exists(Environment.CurrentDirectory + "\\Logging\\Messages"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\Logging\\Messages");
            }
            using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + $"\\Logging\\Messages\\{message.Channel.Id}.slvrlog", true))
            {
                var logmessage = new logmessage
                {
                    content = message.Content,
                    jumplink = message.JumpLink.ToString(),
                    time = message.Timestamp.UtcDateTime,
                    attachments = message.Attachments.ToList(),
                };
                sw.Write(System.Text.Json.JsonSerializer.Serialize(logmessage));
            }
        }
    }

    public class logmessage
    {
        public string content { get; set; }
        public string jumplink { get; set; }
        public DateTime time { get; set; }
        public List<DiscordAttachment> attachments { get; set; }
    }
}