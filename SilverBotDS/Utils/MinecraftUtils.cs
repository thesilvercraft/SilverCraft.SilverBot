using SilverBotDS.Exceptions;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SilverBotDS.Utils
{
    internal class MinecraftUtils
    {
#pragma warning disable S1075 // URIs should not be hardcoded
        private const string GetProfileUrl = "https://api.mojang.com/users/profiles/minecraft/{0}";
        private const string CrafatarBaseUrl = "https://crafatar.com/";
#pragma warning restore S1075 // URIs should not be hardcoded

        public static async Task<Player> GetPlayerAsync(string name, HttpClient httpClient)
        {
            var uri = new UriBuilder(string.Format(GetProfileUrl, name));
            var rm = await httpClient.GetAsync(uri.Uri);
            var player = JsonSerializer.Deserialize<Player>(await rm.Content.ReadAsStringAsync());
            if (!string.IsNullOrEmpty(player.Error))
            {
                throw new MojangException(player.Error, player.ErrorMessage);
            }
            return player;
        }

        public class Player
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("error")]
            public string Error { get; set; }

            [JsonPropertyName("errorMessage")]
            public string ErrorMessage { get; set; }

            [JsonPropertyName("demo")]
            public bool Demo { get; set; }

            public string GetAvatarUrl() => $"{CrafatarBaseUrl}avatars/{Id}";

            public string GetHeadUrl() => $"{CrafatarBaseUrl}renders/head/{Id}";

            public string GetBodyUrl() => $"{CrafatarBaseUrl}renders/body/{Id}";
        }
    }
}