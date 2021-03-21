using System;
using System.Net;
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

        public static async void Checkforupdates()
        {
            try
            {
                LogLine("Running on " + Environment.OSVersion.VersionString);
                var client = Program.GetHttpClient();
                LogLine("Getting latest version info from GitHub");
                Repo repo = new("thesilvercraft", "silverbot");
                var info = await CommitInfo.GetLatestFromRepoAsync(repo);
                if (info.Sha == ThisAssembly.Git.Sha)
                {
                    LogLine($"You are running {VNumber} which has the same Sha as the newest commit on master.");
                }
                else
                {
                    LogLine($"You are running {VNumber} which DOES NOT HAVE the same Sha as the newest commit on master. ({info.Sha} | {ThisAssembly.Git.Sha})");
                }
            }
            catch (WebException ex)
            {
                Program.SendLog(ex);
            }
        }
    }
}