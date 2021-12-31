using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace SilverBotDS.Utils;

internal static class UrbanDictUtils
{
    public static async Task<Defenition[]> GetDefenition(string word, HttpClient httpClient)
    {
        var uri = new UriBuilder($"http://api.urbandictionary.com/v0/define?term={HttpUtility.UrlEncode(word)}");
        var rm = await httpClient.GetAsync(uri.Uri);
        if (rm.StatusCode == HttpStatusCode.OK)
            return JsonSerializer.Deserialize<Rootobject>(await rm.Content.ReadAsStringAsync()).List;
        return await Task.FromException<Defenition[]>(
            new Exception($"Request yielded a statuscode that isnt OK it is {rm.StatusCode}"));
    }

    public class Rootobject
    {
        [JsonPropertyName("list")] public Defenition[] List { get; set; }
    }

    public class Defenition
    {
        [JsonPropertyName("definition")] public string Definition { get; set; }

        [JsonPropertyName("permalink")] public string Permalink { get; set; }

        [JsonPropertyName("thumbs_up")] public int ThumbsUp { get; set; }

        [JsonPropertyName("sound_urls")] public object[] SoundUrls { get; set; }

        [JsonPropertyName("author")] public string Author { get; set; }

        [JsonPropertyName("word")] public string Word { get; set; }

        [JsonPropertyName("defid")] public int Defid { get; set; }

        [JsonPropertyName("current_vote")] public string CurrentVote { get; set; }

        [JsonPropertyName("written_on")] public DateTime WrittenOn { get; set; }

        [JsonPropertyName("example")] public string Example { get; set; }

        [JsonPropertyName("thumbs_down")] public int ThumbsDown { get; set; }
    }
}