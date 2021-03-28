using RestSharp;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SilverBotDS.Objects
{
    public class Dbla
    {
        public static Dbla CreateInstance()
        {
            return new Dbla();
        }

        public class Client
        {
            private readonly string _token;
            private readonly bool _selfbot;

            /// <summary>
            /// Makes a new client
            /// </summary>
            /// <param name="tokentouse">The Token to use</param>
            /// <param name="toselfbot">Set <see cref="true"/> for cookie auth, set <see cref="false"/> for auth auth</param>
            public Client(string tokentouse, bool toselfbot)
            {
                _token = tokentouse;
                _selfbot = toselfbot;
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
                    request.AddHeader(_selfbot ? "Cookie" : "Authorization", _token);

                    var response = await client.ExecuteAsync(request);

                    var bot = JsonSerializer.Deserialize<Bot>(response.Content);
                    if (bot != null && !string.IsNullOrEmpty(bot.Error))
                    {
                        Program.SendLog(bot.Error);
                        return null;
                    }
                    else
                    {
                        return bot;
                    }
                }
                catch (HttpRequestException ex)
                {
                    Program.SendLog(ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// Ment to represent a bot listing on shit.gg™
        /// </summary>
        public class Bot
        {
            public Bot(string prefix, string shortdesc, string website, string error)
            {
                Prefix = prefix;
                Shortdesc = shortdesc;
                Website = website;
                Error = error;
            }

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
            public ulong ServerCount { get; set; }

            [JsonPropertyName("shard_count")]
            public int ShardCount { get; set; }

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