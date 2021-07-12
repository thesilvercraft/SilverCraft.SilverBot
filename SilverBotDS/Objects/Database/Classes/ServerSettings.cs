using SilverBotDS.Objects.Database.Classes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;

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
            Prefixes = System.Array.Empty<string>();
        }
        [Key]
        public ulong ServerId { get; init; }

        public string LangName { get; set; }
        public bool EmotesOptin { get; set; }
        public ulong? ServerStatsCategoryId { get; set; }
        public List<ServerStatString> ServerStatsTemplates { get; set; }
        public bool RepeatThings { get; set; }
        [NotMapped]
        public string[] Prefixes { get=>JsonSerializer.Deserialize<string[]>(PrefixesInJson); set=> PrefixesInJson = JsonSerializer.Serialize(value); }
        public string PrefixesInJson { get; set; }
    }
    public class FakeString
    {
        [Key]
        public string Value { get; set; }
        public FakeString()
        {

        }
        public FakeString(string v)
        {
            Value = v;
        }
        public static implicit operator string(FakeString s)
        {
            return s.Value;
        }
       
    }
}