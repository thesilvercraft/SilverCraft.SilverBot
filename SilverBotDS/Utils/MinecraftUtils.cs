using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SilverBotDS.Utils
{
    internal class MinecraftUtils
    {
        private const string GetProfileUrl = "https://api.mojang.com/users/profiles/minecraft/{0}";

        public static async Task<Player> GetPlayerAsync(string name)
        {
            var httpClient = Program.GetHttpClient();
            var uri = new UriBuilder(string.Format(GetProfileUrl, name));
            var rm = await httpClient.GetAsync(uri.Uri);
            if (rm.StatusCode == HttpStatusCode.OK)
            {
                var player = JsonSerializer.Deserialize<Player>(await rm.Content.ReadAsStringAsync());
                if (!string.IsNullOrEmpty(player.Error))
                {
                    return await Task.FromException<Player>(new Exception($"Mojang returned an error: {player.Error} {player.ErrorMessage}"));
                }
                return player;
            }
            else
            {
                return await Task.FromException<Player>(new Exception($"Request yielded a statuscode that isnt OK it is {rm.StatusCode}"));
            }
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

            public string GetAvatarUrl()
            {
                return string.Format("https://crafatar.com/avatars/{0}", Id);
            }

            public string GetHeadUrl()
            {
                return string.Format("https://crafatar.com/renders/head/{0}", Id);
            }

            public string GetBodyUrl()
            {
                return string.Format("https://crafatar.com/renders/body/{0}", Id);
            }
        }
    }
}