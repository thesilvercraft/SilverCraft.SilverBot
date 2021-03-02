using CodenameGenerator;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using NetClient = SilverBotDS.Objects.NetClient;

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
                var client = NetClient.Get();
                LogLine("Getting latest version info from silverdimond.tk");
                var rm = await client.GetAsync("https://silverdimond.tk/silvercraftbot/version-info.txt");
                var content = await rm.Content.ReadAsStringAsync();
                var strings = content.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                var uptodate = true;
                if (strings.Length != 3)
                {
                    LogLine($"Oh no, silverdimond.tk returned more than three lines, it returned {strings.Length} lines, this probably means its using a different format or someone made an error.");
                }
                if (strings[0] != VNumber)
                {
                    LogLine($"You are running {VNumber} while the latest version is {strings[0]}, consider updating.");

                    uptodate = false;
                }
                if (uptodate)
                {
                    LogLine($"You are currently running {VNumber}, which is the latest version (according to silverdimond.tk)");
                }
                else
                {
                    LogLine($"You should go to {strings[2]} to download a new version of SilverBot");
                }
#if DEBUG
                if (uptodate || (Environment.UserDomainName != "DESKTOP-QK1H9BG")) return;
                await using (var sw = new StreamWriter("version-info.txt"))
                {
                    await sw.WriteLineAsync(VNumber);
                    var generator = new Generator
                    {
                        Separator = "-"
                    };
                    await sw.WriteLineAsync($"codename {generator.Generate()}");
                    await sw.WriteLineAsync(ThisAssembly.Git.RepositoryUrl);
                }
                Process.Start("notepad", "version-info.txt");

#endif
            }
            catch (WebException ex)
            {
                Program.SendLog(ex);
            }
        }
    }
}