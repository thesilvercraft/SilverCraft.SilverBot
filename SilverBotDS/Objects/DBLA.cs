using RestSharp;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SilverBotDS
{
    internal class DBLA
    {
        public class Client
        {
            private readonly string token;
            private readonly bool selfbot;

            /// <summary>
            /// Makes a new client
            /// </summary>
            /// <param name="tokentouse">The Token to use</param>
            /// <param name="toselfbot">Set <see cref="true"/> for cookie auth, set <see cref="false"/> for auth auth</param>
            public Client(string tokentouse, bool toselfbot)
            {
                token = tokentouse;
                selfbot = toselfbot;
                return;
            }

            public async Task<Bot> GetViaIdAsync(ulong id)
            {
                try
                {
                    var client = new RestClient($"https://top.gg/api/bots/{id}")
                    {
                        Timeout = -1
                    };
                    var request = new RestRequest(Method.GET);
                    if (selfbot)
                    {
                        request.AddHeader("Cookie", token);
                    }
                    else
                    {
                        request.AddHeader("Authorization", token);
                    }

                    IRestResponse response = await client.ExecuteAsync(request);

                    var bot = JsonSerializer.Deserialize<Bot>(response.Content);
                    if (!string.IsNullOrEmpty(bot.Error))
                    {
                        _ = Program.SendLogAsync(bot.Error, new());
                        return null;
                    }
                    else
                    {
                        return bot;
                    }
                }
                catch (HttpRequestException ex)
                {
                    _ = Program.SendLogAsync(ex.StatusCode.ToString(), new());
                    return null;
                }
            }
        }

        /// <summary>
        /// Ment to represent a bot listing on shit.gg™
        /// </summary>
        public class Bot
        {
            [JsonPropertyName("error")]
            public string Error { get; set; }

            [JsonPropertyName("defAvatar")]
            public string DefAvatar { get; set; }

            [JsonPropertyName("invite")]
            public string Invite { get; set; }

            [JsonPropertyName("website")]
            public string Website { get; set; }

            [JsonPropertyName("support")]
            public string Support { get; set; }

            [JsonPropertyName("github")]
            public string Github { get; set; }

            [JsonPropertyName("longdesc")]
            public string Longdesc { get; set; }

            [JsonPropertyName("shortdesc")]
            public string Shortdesc { get; set; }

            [JsonPropertyName("prefix")]
            public string Prefix { get; set; }

            [JsonPropertyName("lib")]
            public string Lib { get; set; }

            [JsonPropertyName("clientid")]
            public string Clientid { get; set; }

            [JsonPropertyName("avatar")]
            public string Avatar { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("discriminator")]
            public string Discriminator { get; set; }

            [JsonPropertyName("username")]
            public string Username { get; set; }

            [JsonPropertyName("date")]
            public DateTime Date { get; set; }

            [JsonPropertyName("server_count")]
            public ulong Server_count { get; set; }

            [JsonPropertyName("shard_count")]
            public int Shard_count { get; set; }

            [JsonPropertyName("guilds")]
            public string[] Guilds { get; set; }

            [JsonPropertyName("shards")]
            public int[] Shards { get; set; }

            [JsonPropertyName("monthlyPoints")]
            public ulong MonthlyPoints { get; set; }

            [JsonPropertyName("points")]
            public ulong Points { get; set; }

            [JsonPropertyName("certifiedBot")]
            public bool CertifiedBot { get; set; }

            [JsonPropertyName("owners")]
            public string[] Owners { get; set; }

            [JsonPropertyName("tags")]
            public string[] Tags { get; set; }

            [JsonPropertyName("donatebotguildid")]
            public string Donatebotguildid { get; set; }
        }
    }
}