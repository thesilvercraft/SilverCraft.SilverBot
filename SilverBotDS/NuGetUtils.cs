using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SilverBotDS
{
    internal class NuGetUtils
    {
        /// <summary>
        /// Searches for a query on the nugetz
        /// </summary>
        /// <param name="query">The query to search</param>
        /// <returns>A list of packages</returns>
        /// <exception cref="Exception">given when the webserver didnt return a OK</exception>
        public static async Task<Datum[]> SearchAsync(string query)
        {
            HttpClient httpClient = Webclient.Get();
            UriBuilder uri = new UriBuilder("https://azuresearch-usnc.nuget.org/query")
            {
                Query = $"?q={query}"
            };
            HttpResponseMessage RM = await httpClient.GetAsync(uri.Uri);
            if (RM.StatusCode == HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<Rootobject>(await RM.Content.ReadAsStringAsync()).data;
            }
            else
            {
                return await Task.FromException<Datum[]>(new Exception($"Request yielded a statuscode that isnt OK it is {RM.StatusCode}"));
            }
        }

        public class Rootobject
        {
            public Context context { get; set; }
            public int totalHits { get; set; }
            public Datum[] data { get; set; }
        }

        public class Context
        {
            public string vocab { get; set; }
            public string _base { get; set; }
        }

        public class Datum
        {
            [JsonPropertyName("@id")]
            public string atid { get; set; }

            [JsonPropertyName("@type")]
            public string type { get; set; }

            public string registration { get; set; }

            public string id { get; set; }
            public string version { get; set; }

            public string description { get; set; }
            public string summary { get; set; }
            public string title { get; set; }
            public string iconUrl { get; set; }
            public string licenseUrl { get; set; }
            public string projectUrl { get; set; }
            public string[] tags { get; set; }
            public string[] authors { get; set; }
            public int totalDownloads { get; set; }
            public bool verified { get; set; }
            public Packagetype[] packageTypes { get; set; }
            public Version[] versions { get; set; }
        }

        public class Packagetype
        {
            public string name { get; set; }
        }

        public class Version
        {
            public string version { get; set; }
            public int downloads { get; set; }
            public string id { get; set; }
        }
    }
}