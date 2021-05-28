using SilverBotDS.Objects.Database.Classes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SilverBotDS.Objects
{
    public class ServerSettings
    {
        [Key]
        public ulong ServerId { get; init; }

        public string LangName { get; set; }
        public bool EmotesOptin { get; set; }
        public ulong? ServerStatsCategoryId { get; set; }
        public List<ServerStatString> ServerStatsTemplates { get; set; }
    }
}