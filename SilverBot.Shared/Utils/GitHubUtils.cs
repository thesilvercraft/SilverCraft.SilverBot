/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using DSharpPlus.Entities;

namespace SilverBot.Shared.Utils;

public partial class GitHubUtils
{
    private static readonly Regex R = MyRegex();

    public class Repo
    {
        public Repo(string user, string reponame)
        {
            User = user;
            Reponame = reponame;
        }

        public Repo()
        {
        }

        public string User { get; set; }

        public string Reponame { get; set; }

        public static bool TryParseUrl(string url, out Repo? r)
        {
            var m = R.Match(url);
            if (!m.Success)
            {
                r = null;
                return false;
            }

            r= new Repo
            {
                User = m.Groups["user"].Value,
                Reponame = m.Groups["repo"].Value
            };
            return true;

        }
    }

    public class CommitInfo
    {
        [JsonPropertyName("sha")] public string Sha { get; set; } = null!;

        [JsonPropertyName("node_id")] public string Node_id { get; set; } = null!;

        [JsonPropertyName("commit")] public Commit Commit { get; set; } = null!;

        [JsonPropertyName("url")] public string Url { get; set; } = null!;

        [JsonPropertyName("html_url")] public string HtmlUrl { get; set; } = null!;

        [JsonPropertyName("comments_url")] public string CommentsUrl { get; set; } = null!;

        [JsonPropertyName("author")] public Author1 Author { get; set; } = null!;

        [JsonPropertyName("committer")] public Committer1 Committer { get; set; } = null!;

        [JsonPropertyName("parents")] public Parent[] Parents { get; set; } = null!;

        [JsonPropertyName("stats")] public Stats Stats { get; set; } = null!;

        [JsonPropertyName("files")] public File[] Files { get; set; } = null!;

        public static async Task<CommitInfo> GetLatestFromRepoAsync(Repo repo, HttpClient client)
        {
            return await GetLatestFromRepoAsync(repo, "master", client);
        }

        public static async Task<CommitInfo> GetLatestFromRepoAsync(Repo repo, string branch, HttpClient client)
        {
            var uri = new UriBuilder($"https://api.github.com/repos/{repo.User}/{repo.Reponame}/commits/{branch}");
            var rm = await client.GetAsync(uri.Uri);
            if (rm.StatusCode == HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<CommitInfo>(await rm.Content.ReadAsStringAsync());
            }

            return await Task.FromException<CommitInfo>(
                new Exception($"Request yielded a statuscode that isnt OK it is {rm.StatusCode}"));
        }
    }

    public class Commit
    {
        [JsonPropertyName("author")] public CommitAuthor Author { get; set; } = null!;

        [JsonPropertyName("committer")] public Committer Committer { get; set; } = null!;

        [JsonPropertyName("message")] public string Message { get; set; } = null!;

        [JsonPropertyName("tree")] public Tree Tree { get; set; } = null!;

        [JsonPropertyName("url")] public string Url { get; set; } = null!;

        [JsonPropertyName("comment_count")] public int CommentCount { get; set; }

        [JsonPropertyName("verification")] public Verification Verification { get; set; } = null!;
    }

    public class CommitAuthor
    {
        [JsonPropertyName("name")] public string Name { get; set; } = null!;

        [JsonPropertyName("email")] public string Email { get; set; } = null!;

        [JsonPropertyName("date")] public DateTime Date { get; set; }
    }

    public class Committer
    {
        [JsonPropertyName("name")] public string Name { get; set; } = null!;

        [JsonPropertyName("email")] public string Email { get; set; } = null!;

        [JsonPropertyName("date")] public DateTime Date { get; set; }
    }

    public class Tree
    {
        [JsonPropertyName("sha")] public string Sha { get; set; } = null!;

        [JsonPropertyName("url")] public string Url { get; set; } = null!;
    }

    public class Verification
    {
        [JsonPropertyName("verified")] public bool Verified { get; set; }

        [JsonPropertyName("reason")] public string Reason { get; set; } = null!;

        [JsonPropertyName("signature")] public string Signature { get; set; } = null!;

        [JsonPropertyName("payload")] public string Payload { get; set; } = null!;
    }

    public class Author1
    {
        [JsonPropertyName("login")] public string Login { get; set; } = null!;

        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("node_id")] public string NodeId { get; set; } = null!;

        [JsonPropertyName("avatar_url")] public string AvatarUrl { get; set; } = null!;

        [JsonPropertyName("gravatar_id")] public string GravatarId { get; set; } = null!;

        [JsonPropertyName("url")] public string Url { get; set; } = null!;

        [JsonPropertyName("html_url")] public string HtmlUrl { get; set; } = null!;

        [JsonPropertyName("followers_url")] public string Followers_url { get; set; } = null!;

        [JsonPropertyName("following_url")] public string Following_url { get; set; } = null!;

        [JsonPropertyName("gists_url")] public string Gists_url { get; set; } = null!;

        [JsonPropertyName("starred_url")] public string Starred_url { get; set; } = null!;

        [JsonPropertyName("subscriptions_url")]
        public string Subscriptions_url { get; set; } = null!;

        [JsonPropertyName("organizations_url")]
        public string Organizations_url { get; set; } = null!;

        [JsonPropertyName("repos_url")] public string Repos_url { get; set; } = null!;

        [JsonPropertyName("events_url")] public string Events_url { get; set; } = null!;

        [JsonPropertyName("received_events_url")]
        public string Received_events_url { get; set; } = null!;

        [JsonPropertyName("type")] public string Type { get; set; } = null!;

        [JsonPropertyName("site_admin")] public bool Site_admin { get; set; }
    }

    public class Committer1
    {
        [JsonPropertyName("login")] public string Login { get; set; } = null!;

        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("node_id")] public string Node_id { get; set; } = null!;

        [JsonPropertyName("avatar_url")] public string Avatar_url { get; set; } = null!;

        [JsonPropertyName("gravatar_id")] public string Gravatar_id { get; set; } = null!;

        [JsonPropertyName("url")] public string Url { get; set; } = null!;

        [JsonPropertyName("html_url")] public string Html_url { get; set; } = null!;

        [JsonPropertyName("followers_url")] public string Followers_url { get; set; } = null!;

        [JsonPropertyName("following_url")] public string Following_url { get; set; } = null!;

        [JsonPropertyName("gists_url")] public string Gists_url { get; set; } = null!;

        [JsonPropertyName("starred_url")] public string Starred_url { get; set; } = null!;

        [JsonPropertyName("subscriptions_url")]
        public string Subscriptions_url { get; set; } = null!;

        [JsonPropertyName("organizations_url")]
        public string Organizations_url { get; set; } = null!;

        [JsonPropertyName("repos_url")] public string Repos_url { get; set; } = null!;

        [JsonPropertyName("events_url")] public string Events_url { get; set; } = null!;

        [JsonPropertyName("received_events_url")]
        public string Received_events_url { get; set; } = null!;

        [JsonPropertyName("type")] public string Type { get; set; } = null!;

        [JsonPropertyName("site_admin")] public bool SiteAdmin { get; set; }
    }

    public class Stats
    {
        [JsonPropertyName("total")] public int Total { get; set; }

        [JsonPropertyName("additions")] public int Additions { get; set; }

        [JsonPropertyName("deletions")] public int Deletions { get; set; }
    }

    public class Parent
    {
        [JsonPropertyName("sha")] public string Sha { get; set; } = null!;

        [JsonPropertyName("url")] public string Url { get; set; } = null!;

        [JsonPropertyName("html_url")] public string Htmlurl { get; set; } = null!;
    }

    public class File
    {
        [JsonPropertyName("sha")] public string Sha { get; set; } = null!;

        [JsonPropertyName("filename")] public string Filename { get; set; } = null!;

        [JsonPropertyName("status")] public string Status { get; set; } = null!;

        [JsonPropertyName("additions")] public int Additions { get; set; }

        [JsonPropertyName("deletions")] public int Deletions { get; set; }

        [JsonPropertyName("changes")] public int Changes { get; set; }

        [JsonPropertyName("blob_url")] public string Bloburl { get; set; } = null!;

        [JsonPropertyName("raw_url")] public string Rawurl { get; set; } = null!;

        [JsonPropertyName("contents_url")] public string Contents_url { get; set; } = null!;

        [JsonPropertyName("patch")] public string Patch { get; set; } = null!;
    }

    public class Release
    {
        [JsonPropertyName("url")] public string Url { get; set; } = null!;

        [JsonPropertyName("assets_url")] public string AssetsUrl { get; set; } = null!;

        [JsonPropertyName("upload_url")] public string UploadUrl { get; set; } = null!;

        [JsonPropertyName("html_url")] public string HtmlUrl { get; set; } = null!;

        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("author")] public Author Author { get; set; } = null!;

        [JsonPropertyName("node_id")] public string NodeId { get; set; } = null!;

        [JsonPropertyName("tag_name")] public string TagName { get; set; } = null!;

        [JsonPropertyName("target_commitish")] public string TargetCommitish { get; set; } = null!;

        [JsonPropertyName("name")] public string Name { get; set; } = null!;

        [JsonPropertyName("draft")] public bool Draft { get; set; }

        [JsonPropertyName("prerelease")] public bool Prerelease { get; set; }

        [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }

        [JsonPropertyName("published_at")] public DateTime PublishedAt { get; set; }

        [JsonPropertyName("assets")] public Asset[] Assets { get; set; } = null!;

        [JsonPropertyName("tarball_url")] public string TarballUrl { get; set; } = null!;

        [JsonPropertyName("zipball_url")] public string ZipballUrl { get; set; } = null!;

        [JsonPropertyName("body")] public string Body { get; set; } = null!;

        public static async Task DownloadLatestAsync(Release release, HttpClient client)
        {
            using var rm = await client.GetAsync(release.Assets[0].BrowserDownloadUrl);
            var uri = new Uri(release.Assets[0].BrowserDownloadUrl);
            var filename = Path.GetFileName(uri.LocalPath);
            await using var fs = new FileStream(
                Path.Combine(Environment.CurrentDirectory,filename),
                FileMode.CreateNew);
            await rm.Content.CopyToAsync(fs);
        }

        public async Task DownloadLatestAsync(HttpClient client)
        {
            await DownloadLatestAsync(this, client);
        }

        public static async Task<Release> GetLatestFromRepoAsync(Repo repo, HttpClient client)
        {
            var uri = new UriBuilder($"https://api.github.com/repos/{repo.User}/{repo.Reponame}/releases/latest");
            var rm = await client.GetAsync(uri.Uri);
            if (rm.StatusCode == HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<Release>(await rm.Content.ReadAsStringAsync());
            }

            return await Task.FromException<Release>(
                new Exception($"Request yielded a statuscode that isnt OK it is {rm.StatusCode}"));
        }
    }

    public class Author
    {
        [JsonPropertyName("login")] public string Login { get; set; } = null!;

        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("node_id")] public string NodeId { get; set; } = null!;

        [JsonPropertyName("avatar_url")] public string AvatarUrl { get; set; } = null!;

        [JsonPropertyName("gravatar_id")] public string GravatarId { get; set; } = null!;

        [JsonPropertyName("url")] public string Url { get; set; } = null!;

        [JsonPropertyName("html_url")] public string HtmlUrl { get; set; } = null!;

        [JsonPropertyName("followers_url")] public string FollowersUrl { get; set; } = null!;

        [JsonPropertyName("following_url")] public string FollowingUrl { get; set; } = null!;

        [JsonPropertyName("gists_url")] public string GistsUrl { get; set; } = null!;

        [JsonPropertyName("starred_url")] public string StarredUrl { get; set; } = null!;

        [JsonPropertyName("subscriptions_url")]
        public string SubscriptionsUrl { get; set; } = null!;

        [JsonPropertyName("organizations_url")]
        public string OrganizationsUrl { get; set; } = null!;

        [JsonPropertyName("repos_url")] public string ReposUrl { get; set; } = null!;

        [JsonPropertyName("events_url")] public string EventsUrl { get; set; } = null!;

        [JsonPropertyName("received_events_url")]
        public string ReceivedEventsUrl { get; set; } = null!;

        [JsonPropertyName("type")] public string Type { get; set; } = null!;

        [JsonPropertyName("site_admin")] public bool SiteAdmin { get; set; }
    }

    public class Asset
    {
        [JsonPropertyName("url")] public string Url { get; set; } = null!;

        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("node_id")] public string NodeId { get; set; } = null!;

        [JsonPropertyName("name")] public string Name { get; set; } = null!;

        [JsonPropertyName("label")] public string Label { get; set; } = null!;

        [JsonPropertyName("uploader")] public Uploader Uploader { get; set; } = null!;

        [JsonPropertyName("content_type")] public string ContentType { get; set; } = null!;

        [JsonPropertyName("state")] public string State { get; set; } = null!;

        [JsonPropertyName("size")] public int Size { get; set; }

        [JsonPropertyName("download_count")] public int DownloadCount { get; set; }

        [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("browser_download_url")]
        public string BrowserDownloadUrl { get; set; } = null!;
    }

    public class Uploader
    {
        [JsonPropertyName("login")] public string Login { get; set; } = null!;

        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("node_id")] public string NodeId { get; set; } = null!;

        [JsonPropertyName("avatar_url")] public string AvatarUrl { get; set; } = null!;

        [JsonPropertyName("gravatar_id")] public string GravatarId { get; set; } = null!;

        [JsonPropertyName("url")] public string Url { get; set; } = null!;

        [JsonPropertyName("html_url")] public string HtmlUrl { get; set; } = null!;

        [JsonPropertyName("followers_url")] public string FollowersUrl { get; set; } = null!;

        [JsonPropertyName("following_url")] public string FollowingUrl { get; set; } = null!;

        [JsonPropertyName("gists_url")] public string GistsUrl { get; set; } = null!;

        [JsonPropertyName("starred_url")] public string StarredUrl { get; set; } = null!;

        [JsonPropertyName("subscriptions_url")]
        public string SubscriptionsUrl { get; set; } = null!;

        [JsonPropertyName("organizations_url")]
        public string OrganizationsUrl { get; set; } = null!;

        [JsonPropertyName("repos_url")] public string ReposUrl { get; set; } = null!;

        [JsonPropertyName("events_url")] public string EventsUrl { get; set; } = null!;

        [JsonPropertyName("received_events_url")]
        public string ReceivedEventsUrl { get; set; } = null!;

        [JsonPropertyName("type")] public string Type { get; set; } = null!;

        [JsonPropertyName("site_admin")] public bool SiteAdmin { get; set; }
    }

    [GeneratedRegex("(?:https?://)?github.com/(?<user>.+)/(?<repo>.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex MyRegex();
}