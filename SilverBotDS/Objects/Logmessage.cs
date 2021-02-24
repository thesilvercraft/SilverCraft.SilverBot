using System;
using System.Collections.Generic;
using DSharpPlus.Entities;

namespace SilverBotDS.Objects
{
    public class Logmessage
    {
        public string Content { get; set; }
        public string Jumplink { get; set; }
        public DateTime Time { get; set; }
        public List<DiscordAttachment> Attachments { get; set; }
    }
}