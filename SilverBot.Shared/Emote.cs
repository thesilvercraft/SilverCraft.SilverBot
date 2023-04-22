using System.Text.Json.Serialization;

namespace SilverBot.Shared
{
    public class Emote
    {
        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("url")] public string Url { get; set; }
    }
}