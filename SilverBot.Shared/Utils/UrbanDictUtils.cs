/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace SilverBot.Shared.Utils
{
    internal static class UrbanDictUtils
    {
        public static async Task<Defenition[]> GetDefinition(string word, HttpClient httpClient)
        {
            var uri = new UriBuilder($"http://api.urbandictionary.com/v0/define?term={HttpUtility.UrlEncode(word)}");
            var rm = await httpClient.GetAsync(uri.Uri);
            if (rm.StatusCode == HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<Rootobject>(await rm.Content.ReadAsStringAsync()).List;
            }

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

            [JsonPropertyName("defid")] public int DefId { get; set; }

            [JsonPropertyName("current_vote")] public string CurrentVote { get; set; }

            [JsonPropertyName("written_on")] public DateTime WrittenOn { get; set; }

            [JsonPropertyName("example")] public string Example { get; set; }

            [JsonPropertyName("thumbs_down")] public int ThumbsDown { get; set; }
        }
    }
}