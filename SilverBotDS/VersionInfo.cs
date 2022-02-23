using Serilog.Core;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static SilverBotDS.Utils.GitHubUtils;
using File = System.IO.File;

namespace SilverBotDS;

internal static class VersionInfo
{
    public const string VNumber =
        ThisAssembly.Git.Commit + "-" + ThisAssembly.Git.Branch + "-" + ThisAssembly.Git.CommitDate;

    public static async Task Checkforupdates(HttpClient client, Logger log)
    {
        try
        {
            log.Information("Running on {OS}", Environment.OSVersion.VersionString);
            log.Information("Getting latest version info from {Source}", "GitHub");
            Repo repo = new("thesilvercraft", "silverbot");
            var info = await CommitInfo.GetLatestFromRepoAsync(repo, client);
            if (info.Sha == ThisAssembly.Git.Sha)
            {
                log.Information("You are running {VNumber} which has the same Sha as the newest commit on master",
                    VNumber);
            }
            else
            {
                log.Information(
                    "You are running {VNumber} which DOES NOT HAVE the same Sha as the newest commit on master ({Sha1} | {Sha2})",
                    VNumber, info.Sha, ThisAssembly.Git.Sha);
                string gamer = string.Empty;
                if (File.Exists("autoupdater.sh"))
                {
                    gamer = "autoupdater.sh";
                }
                else if (File.Exists("autoupdater.cmd"))
                {
                    gamer = "autoupdater.cmd";
                }
                else if (File.Exists("autoupdater.bat"))
                {
                    gamer = "autoupdater.bat";
                }
                if (!string.IsNullOrEmpty(gamer))
                {
                    var rls = await Release.GetLatestFromRepoAsync(repo, client);
                    if (rls.Body.Contains(info.Sha))
                    {
                        log.Information("TRYING TO DOWNLOAD RELEASE {Rls} from url {Url} AS AUTOUPDATER WAS DETECTED",
                            rls.HtmlUrl, rls.AssetsUrl);
                        switch (Environment.OSVersion.Platform)
                        {
                            case PlatformID.Unix:
                                {
                                    ProcessStartInfo processInfo = new()
                                    {
                                        Arguments =
                                            $"-c \"echo \\\"./{gamer}\\\" \\\"{rls.Assets[0].BrowserDownloadUrl}\\\" | at now\"",
                                        CreateNoWindow = true,
                                        FileName = "/bin/bash",
                                        UseShellExecute = false,
                                        WindowStyle = ProcessWindowStyle.Hidden
                                    };
                                    Process.Start(processInfo);
                                    break;
                                }
                            case PlatformID.Win32NT:
                                {
                                    //TODO: Add working Windows support
                                    Process p = new();
                                    p.StartInfo.FileName = "CMD.exe";
                                    p.StartInfo.Arguments = $"{gamer} \"{rls.Assets[0].BrowserDownloadUrl}\"";
                                    p.StartInfo.CreateNoWindow = false;
                                    p.Start();
                                    break;
                                }
                        }
                    }
                }
            }

            if (ThisAssembly.Git.IsDirty)
            {
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS0162 // Unreachable code detected
                //At least one of these will warn lmao
                log.Information("You are running a {Dirty} version of silverbot", "dirty");
#pragma warning restore CS0162 // Unreachable code detected
#pragma warning restore IDE0079 // Remove unnecessary suppression
            }
        }
        catch (WebException ex)
        {
            Program.SendLog(ex);
        }
    }
}