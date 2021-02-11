using DSharpPlus.Entities;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SilverBotDS
{
    public class GitHubUtils
    {
        private static readonly Regex r = new Regex("(?:https?://)?github.com/(?P<user>.+)/(?P<repo>.+)", RegexOptions.IgnoreCase);

        public class Repo
        {
            public string User;
            public string Reponame;

            public Repo(string user, string reponame)
            {
                User = user;
                Reponame = reponame;
            }

            public Repo()
            {
            }

            public static Optional<Repo> TryParseUrl(string url)
            {
                Match m = r.Match(url);
                var Repo = new Repo();
                if (m.Success)
                {
                    Repo.User = m.Groups["user"].Value;
                    Repo.Reponame = m.Groups["repo"].Value;
                    return Optional.FromValue(Repo);
                }
                else
                {
                    return Optional.FromNoValue<Repo>();
                }
            }
        }

        public class Release
        {
            public static async Task DownloadLatestAsync(Release release)
            {
                HttpClient client = Webclient.Get();
                using HttpResponseMessage rm = await client.GetAsync(release.Assets[0].Browser_download_url);
                using var fs = new FileStream(
        Environment.CurrentDirectory + "\\lavalink.jar",
        FileMode.CreateNew);
                await rm.Content.CopyToAsync(fs);
            }

            public async Task DownloadLatestAsync()
            {
                HttpClient client = Webclient.Get();
                using HttpResponseMessage rm = await client.GetAsync(Assets[0].Browser_download_url);
                using var fs = new FileStream(
        Environment.CurrentDirectory + "\\lavalink.jar",
        FileMode.CreateNew);
                await rm.Content.CopyToAsync(fs);
            }

            public static async Task<Release> GetLatestFromRepoAsync(Repo repo)
            {
                HttpClient httpClient = Webclient.Get();
                UriBuilder uri = new UriBuilder($"https://api.github.com/repos/{repo.User}/{repo.Reponame}/releases/latest");
                HttpResponseMessage RM = await httpClient.GetAsync(uri.Uri);
                if (RM.StatusCode == HttpStatusCode.OK)
                {
                    return JsonSerializer.Deserialize<Release>(await RM.Content.ReadAsStringAsync());
                }
                else
                {
                    return await Task.FromException<Release>(new Exception($"Request yielded a statuscode that isnt OK it is {RM.StatusCode}"));
                }
            }

            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("assets_url")]
            public string Assets_url { get; set; }

            [JsonPropertyName("upload_url")]
            public string Upload_url { get; set; }

            [JsonPropertyName("html_url")]
            public string Html_url { get; set; }

            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("author")]
            public Author Author { get; set; }

            [JsonPropertyName("node_id")]
            public string Node_id { get; set; }

            [JsonPropertyName("tag_name")]
            public string Tag_name { get; set; }

            [JsonPropertyName("target_commitish")]
            public string Target_commitish { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("draft")]
            public bool Draft { get; set; }

            [JsonPropertyName("prerelease")]
            public bool Prerelease { get; set; }

            [JsonPropertyName("created_at")]
            public DateTime Created_at { get; set; }

            [JsonPropertyName("published_at")]
            public DateTime Published_at { get; set; }

            [JsonPropertyName("assets")]
            public Asset[] Assets { get; set; }

            [JsonPropertyName("tarball_url")]
            public string Tarball_url { get; set; }

            [JsonPropertyName("zipball_url")]
            public string Zipball_url { get; set; }

            [JsonPropertyName("body")]
            public string Body { get; set; }
        }

        public class Author
        {
            [JsonPropertyName("login")]
            public string Login { get; set; }

            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("node_id")]
            public string Node_id { get; set; }

            [JsonPropertyName("avatar_url")]
            public string Avatar_url { get; set; }

            [JsonPropertyName("gravatar_id")]
            public string Gravatar_id { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("html_url")]
            public string Html_url { get; set; }

            [JsonPropertyName("followers_url")]
            public string Followers_url { get; set; }

            [JsonPropertyName("following_url")]
            public string Following_url { get; set; }

            [JsonPropertyName("gists_url")]
            public string Gists_url { get; set; }

            [JsonPropertyName("starred_url")]
            public string Starred_url { get; set; }

            [JsonPropertyName("subscriptions_url")]
            public string Subscriptions_url { get; set; }

            [JsonPropertyName("organizations_url")]
            public string Organizations_url { get; set; }

            [JsonPropertyName("repos_url")]
            public string Repos_url { get; set; }

            [JsonPropertyName("events_url")]
            public string Events_url { get; set; }

            [JsonPropertyName("received_events_url")]
            public string Received_events_url { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("site_admin")]
            public bool Site_admin { get; set; }
        }

        public class Asset
        {
            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("node_id")]
            public string Node_id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("label")]
            public string Label { get; set; }

            [JsonPropertyName("uploader")]
            public Uploader Uploader { get; set; }

            [JsonPropertyName("content_type")]
            public string Content_type { get; set; }

            [JsonPropertyName("state")]
            public string State { get; set; }

            [JsonPropertyName("size")]
            public int Size { get; set; }

            [JsonPropertyName("download_count")]
            public int Download_count { get; set; }

            [JsonPropertyName("created_at")]
            public DateTime Created_at { get; set; }

            [JsonPropertyName("updated_at")]
            public DateTime Updated_at { get; set; }

            [JsonPropertyName("browser_download_url")]
            public string Browser_download_url { get; set; }
        }

        public class Uploader
        {
            [JsonPropertyName("login")]
            public string Login { get; set; }

            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("node_id")]
            public string Node_id { get; set; }

            [JsonPropertyName("avatar_url")]
            public string Avatar_url { get; set; }

            [JsonPropertyName("gravatar_id")]
            public string Gravatar_id { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("html_url")]
            public string Html_url { get; set; }

            [JsonPropertyName("followers_url")]
            public string Followers_url { get; set; }

            [JsonPropertyName("following_url")]
            public string Following_url { get; set; }

            [JsonPropertyName("gists_url")]
            public string Gists_url { get; set; }

            [JsonPropertyName("starred_url")]
            public string Starred_url { get; set; }

            [JsonPropertyName("subscriptions_url")]
            public string Subscriptions_url { get; set; }

            [JsonPropertyName("organizations_url")]
            public string Organizations_url { get; set; }

            [JsonPropertyName("repos_url")]
            public string Repos_url { get; set; }

            [JsonPropertyName("events_url")]
            public string Events_url { get; set; }

            [JsonPropertyName("received_events_url")]
            public string Received_events_url { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("site_admin")]
            public bool Site_admin { get; set; }
        }
    }
}