using System.Text.Json.Serialization;

namespace SilverBot.Shared
{
    public class EmojiPack
    {
        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("author")] public string Author { get; set; }

        [JsonPropertyName("emotes")] public Emote[] Emotes { get; set; }
    }
}