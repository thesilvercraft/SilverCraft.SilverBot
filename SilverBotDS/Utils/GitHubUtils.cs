using DSharpPlus.Entities;
using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebClient = SilverBotDS.Objects.WebClient;

namespace SilverBotDS.Utils
{
    public abstract class GitHubUtils
    {
        private static readonly Regex R = new Regex("(?:https?://)?github.com/(?<user>.+)/(?<repo>.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
                var m = R.Match(url);
                var repo = new Repo();
                if (m.Success)
                {
                    repo.User = m.Groups["user"].Value;
                    repo.Reponame = m.Groups["repo"].Value;
                    return Optional.FromValue(repo);
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
                System.Net.Http.HttpClient client = WebClient.Get();
                using System.Net.Http.HttpResponseMessage rm = await client.GetAsync(release.Assets[0].BrowserDownloadUrl);
                Uri uri = new Uri(release.Assets[0].BrowserDownloadUrl);
                string filename = Path.GetFileName(uri.LocalPath);
                await using FileStream fs = new FileStream(
        Environment.CurrentDirectory + $"\\{filename}",
        FileMode.CreateNew);
                await rm.Content.CopyToAsync(fs);
            }

            public async Task DownloadLatestAsync() => await DownloadLatestAsync(this);

            public static async Task<Release> GetLatestFromRepoAsync(Repo repo)
            {
                var httpClient = WebClient.Get();
                var uri = new UriBuilder($"https://api.github.com/repos/{repo.User}/{repo.Reponame}/releases/latest");
                var rm = await httpClient.GetAsync(uri.Uri);
                if (rm.StatusCode == HttpStatusCode.OK)
                {
                    return JsonSerializer.Deserialize<Release>(await rm.Content.ReadAsStringAsync());
                }
                else
                {
                    return await Task.FromException<Release>(new Exception($"Request yielded a statuscode that isnt OK it is {rm.StatusCode}"));
                }
            }

            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("assets_url")]
            public string AssetsUrl { get; set; }

            [JsonPropertyName("upload_url")]
            public string UploadUrl { get; set; }

            [JsonPropertyName("html_url")]
            public string HtmlUrl { get; set; }

            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("author")]
            public Author Author { get; set; }

            [JsonPropertyName("node_id")]
            public string NodeId { get; set; }

            [JsonPropertyName("tag_name")]
            public string TagName { get; set; }

            [JsonPropertyName("target_commitish")]
            public string TargetCommitish { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("draft")]
            public bool Draft { get; set; }

            [JsonPropertyName("prerelease")]
            public bool Prerelease { get; set; }

            [JsonPropertyName("created_at")]
            public DateTime CreatedAt { get; set; }

            [JsonPropertyName("published_at")]
            public DateTime PublishedAt { get; set; }

            [JsonPropertyName("assets")]
            public Asset[] Assets { get; set; }

            [JsonPropertyName("tarball_url")]
            public string TarballUrl { get; set; }

            [JsonPropertyName("zipball_url")]
            public string ZipballUrl { get; set; }

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
            public string NodeId { get; set; }

            [JsonPropertyName("avatar_url")]
            public string AvatarUrl { get; set; }

            [JsonPropertyName("gravatar_id")]
            public string GravatarId { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("html_url")]
            public string HtmlUrl { get; set; }

            [JsonPropertyName("followers_url")]
            public string FollowersUrl { get; set; }

            [JsonPropertyName("following_url")]
            public string FollowingUrl { get; set; }

            [JsonPropertyName("gists_url")]
            public string GistsUrl { get; set; }

            [JsonPropertyName("starred_url")]
            public string StarredUrl { get; set; }

            [JsonPropertyName("subscriptions_url")]
            public string SubscriptionsUrl { get; set; }

            [JsonPropertyName("organizations_url")]
            public string OrganizationsUrl { get; set; }

            [JsonPropertyName("repos_url")]
            public string ReposUrl { get; set; }

            [JsonPropertyName("events_url")]
            public string EventsUrl { get; set; }

            [JsonPropertyName("received_events_url")]
            public string ReceivedEventsUrl { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("site_admin")]
            public bool SiteAdmin { get; set; }
        }

        public class Asset
        {
            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("node_id")]
            public string NodeId { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("label")]
            public string Label { get; set; }

            [JsonPropertyName("uploader")]
            public Uploader Uploader { get; set; }

            [JsonPropertyName("content_type")]
            public string ContentType { get; set; }

            [JsonPropertyName("state")]
            public string State { get; set; }

            [JsonPropertyName("size")]
            public int Size { get; set; }

            [JsonPropertyName("download_count")]
            public int DownloadCount { get; set; }

            [JsonPropertyName("created_at")]
            public DateTime CreatedAt { get; set; }

            [JsonPropertyName("updated_at")]
            public DateTime UpdatedAt { get; set; }

            [JsonPropertyName("browser_download_url")]
            public string BrowserDownloadUrl { get; set; }
        }

        public class Uploader
        {
            [JsonPropertyName("login")]
            public string Login { get; set; }

            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("node_id")]
            public string NodeId { get; set; }

            [JsonPropertyName("avatar_url")]
            public string AvatarUrl { get; set; }

            [JsonPropertyName("gravatar_id")]
            public string GravatarId { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("html_url")]
            public string HtmlUrl { get; set; }

            [JsonPropertyName("followers_url")]
            public string FollowersUrl { get; set; }

            [JsonPropertyName("following_url")]
            public string FollowingUrl { get; set; }

            [JsonPropertyName("gists_url")]
            public string GistsUrl { get; set; }

            [JsonPropertyName("starred_url")]
            public string StarredUrl { get; set; }

            [JsonPropertyName("subscriptions_url")]
            public string SubscriptionsUrl { get; set; }

            [JsonPropertyName("organizations_url")]
            public string OrganizationsUrl { get; set; }

            [JsonPropertyName("repos_url")]
            public string ReposUrl { get; set; }

            [JsonPropertyName("events_url")]
            public string EventsUrl { get; set; }

            [JsonPropertyName("received_events_url")]
            public string ReceivedEventsUrl { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("site_admin")]
            public bool SiteAdmin { get; set; }
        }
    }
}