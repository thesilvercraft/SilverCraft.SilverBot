/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Text.Json;
using System.Text.Json.Serialization;

namespace SilverBot.Shared.Utils
{
    internal static class NuGetUtils
    {
        /// <summary>
        ///     Searches for a query on the nuget
        /// </summary>
        /// <param name="query">The query to search</param>
        /// <returns>A list of packages</returns>
        /// <exception cref="Exception">given when the webserver didnt return a OK</exception>
        public static async Task<Datum[]> SearchAsync(string query, HttpClient httpClient)
        {
            var uri = new UriBuilder("https://azuresearch-usnc.nuget.org/query")
            {
                Query = $"?q={query}"
            };
            var rm = await httpClient.GetAsync(uri.Uri);
            return JsonSerializer.Deserialize<Rootobject>(await rm.Content.ReadAsStringAsync())?.Data;
        }

        public class Rootobject
        {
            [JsonPropertyName("context")] public Context Context { get; set; }

            [JsonPropertyName("totalHits")] public int TotalHits { get; set; }

            [JsonPropertyName("data")] public Datum[] Data { get; set; }
        }

        public class Context
        {
            [JsonPropertyName("vocab")] public string Vocab { get; set; }

            [JsonPropertyName("_base")] public string Base { get; set; }
        }

        public class Datum
        {
            [JsonPropertyName("@id")] public string Atid { get; set; }

            [JsonPropertyName("@type")] public string Type { get; set; }

            [JsonPropertyName("registration")] public string Registration { get; set; }

            [JsonPropertyName("id")] public string Id { get; set; }

            [JsonPropertyName("version")] public string Version { get; set; }

            [JsonPropertyName("description")] public string Description { get; set; }

            [JsonPropertyName("summary")] public string Summary { get; set; }

            [JsonPropertyName("title")] public string Title { get; set; }

            [JsonPropertyName("iconUrl")] public string IconUrl { get; set; }

            [JsonPropertyName("licenseUrl")] public string LicenseUrl { get; set; }

            [JsonPropertyName("projectUrl")] public string ProjectUrl { get; set; }

            [JsonPropertyName("tags")] public string[] Tags { get; set; }

            [JsonPropertyName("authors")] public string[] Authors { get; set; }

            [JsonPropertyName("totalDownloads")] public int? TotalDownloads { get; set; }

            [JsonPropertyName("verified")] public bool? Verified { get; set; }

            [JsonPropertyName("packageTypes")] public Packagetype[] PackageTypes { get; set; }

            [JsonPropertyName("versions")] public Version[] Versions { get; set; }
        }

        public class Packagetype
        {
            [JsonPropertyName("name")] public string Name { get; set; }
        }

        public class Version
        {
            [JsonPropertyName("version")] public string StrVersion { get; set; }

            [JsonPropertyName("downloads")] public int Downloads { get; set; }

            [JsonPropertyName("id")] public string Id { get; set; }
        }
    }
}