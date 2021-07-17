using SilverBotDS.Objects.Database.Classes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System;
namespace SilverBotDS.Objects
{
    public class ServerSettings
    {
        public ServerSettings()
        {
            LangName = "en";
            EmotesOptin = false;
            ServerStatsCategoryId = null;
            ServerStatsTemplates = new();
            RepeatThings = false;
            Prefixes = Array.Empty<string>();
        }
        [Key]
        public ulong ServerId { get; init; }

        public string LangName { get; set; }
        public bool EmotesOptin { get; set; }
        public ulong? ServerStatsCategoryId { get; set; }
        public List<ServerStatString> ServerStatsTemplates { get; set; }
        public bool RepeatThings { get; set; }
        [NotMapped]
        public string[] Prefixes { get=>JsonSerializer.Deserialize<string[]>(string.IsNullOrWhiteSpace(PrefixesInJson)?"{}": PrefixesInJson); set=> PrefixesInJson = JsonSerializer.Serialize(value); }
        public string PrefixesInJson { get; set; }
    }
}