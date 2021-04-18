using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static SilverBotDS.Utils.GitHubUtils;

namespace SilverBotDS
{
    internal static class VersionInfo
    {
        public const string VNumber = ThisAssembly.Git.Commit + "-" + ThisAssembly.Git.Branch + "-" + ThisAssembly.Git.CommitDate;

        private static void LogLine(string line)
        {
            Program.SendLog($"[VersionInfo] {line}", true);
        }

        public static async Task Checkforupdates(HttpClient client)
        {
            try
            {
                LogLine("Running on " + Environment.OSVersion.VersionString);
                LogLine("Getting latest version info from GitHub");
                Repo repo = new("thesilvercraft", "silverbot");
                var info = await CommitInfo.GetLatestFromRepoAsync(repo, client);
                if (info.Sha == ThisAssembly.Git.Sha)
                {
                    LogLine($"You are running {VNumber} which has the same Sha as the newest commit on master.");
                }
                else
                {
                    LogLine($"You are running {VNumber} which DOES NOT HAVE the same Sha as the newest commit on master. ({info.Sha} | {ThisAssembly.Git.Sha})");
                }
                if (ThisAssembly.Git.IsDirty)
                {
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS0162 // Unreachable code detected
                    //At least one of these will warn lmao
                    LogLine($"You are running a dirty version of silverbot");
#pragma warning restore IDE0079 // Remove unnecessary suppression
#pragma warning restore CS0162 // Unreachable code detected
                }
            }
            catch (WebException ex)
            {
                Program.SendLog(ex);
            }
        }
    }
}