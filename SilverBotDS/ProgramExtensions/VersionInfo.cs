/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Mono.Cecil;
using Serilog.Core;
using static SilverBot.Shared.Utils.GitHubUtils;
using File = System.IO.File;

namespace SilverBotDS.ProgramExtensions;

public static class VersionInfo
{
  
    public const string VNumber =
        "";

    public static string GitRepo { get; private set; }= "https://github.com/thesilvercraft/SilverCraft.SilverBot/L";

    public static void SetRepoURL()
    {
        var definition = AssemblyDefinition.ReadAssembly( typeof(VersionInfo).Assembly.Location);
        CustomAttribute attribute = null;
        foreach (var ca in definition.CustomAttributes)
        {
            if (ca.AttributeType.FullName == typeof(AssemblyMetadataAttribute).FullName &&
                ca.ConstructorArguments.Count == 2 &&
                ca.ConstructorArguments[0].Value is string key &&
                key == "RepositoryUrl")
            {
                attribute = ca;
                break;
            }
        }

// get the value from the second constructor argument
        if (attribute != null && attribute.ConstructorArguments[1].Value is string url)
        {
            GitRepo = url;
            // do something with the value
        }

    }
    public static async Task Checkforupdates(HttpClient client, Logger log)
    {
        try
        {
            log.Information("Running on {OS}", Environment.OSVersion.VersionString);
        
           
              
                if (Repo.TryParseUrl(GitRepo, out var repo) )
                {
                    log.Information("Getting latest version info from {Source}", "GitHub");
                    var info = await CommitInfo.GetLatestFromRepoAsync(repo, client);
                    log.Information("The latest sha is {Sha}", info.Sha);
                }
           /* 
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
                var gamer = string.Empty;
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
                            default:
                                Console.WriteLine("AutoUpdater module does not support your platform");
                                break;
                        }
                    }
                }
            }

            if (ThisAssembly.Git.IsDirty)
            {
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS0162 // Unreachable code detected
                //At least one of these will warn lmao
                log.Information("You are running a {Dirty} version of silverbot", "DIRTY");
#pragma warning restore CS0162 // Unreachable code detected
#pragma warning restore IDE0079 // Remove unnecessary suppression
            }*/
        }
        catch (WebException ex)
        {
            Program.SendLog(ex);
        }
    }
}
