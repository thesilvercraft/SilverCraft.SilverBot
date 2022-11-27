﻿using DSharpPlus;
using Microsoft.Extensions.Logging;
using SilverBotDS.Objects;
using System.Diagnostics;

namespace SilverBot.SysAdminModule
{
    public class SysAdminModule : IService
    {
        public DiscordClient client { set; private get; }
        IPackageManager pm=new ScoopPackageManager();
        public async Task Start()
        {
       
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User)?.Contains("scoop")==true)
            {
                Console.WriteLine("Scoop version: " + pm.Version);
                Console.WriteLine("Upgrading index");
                pm.UpgradeIndex();
                Console.WriteLine("Installed packages:");
                foreach (var package in pm.GetInstalledPackages())
                {
                    Console.WriteLine($"{package.Name}\t\t{package.Version}\t\t{package.Source}");
                }
                Console.WriteLine("Packages to upgrade:");
                foreach (var package in pm.GetPackagesReadyToUpdate())
                {
                    Console.WriteLine($"{package.Name}\t\t{package.Version}\t\t{((ScoopPackage)package).NewVersion}");
                }
                Console.WriteLine("Upgrading packages");
                pm.UpgradePackages();
            }
        }

        public async Task Stop()
        {

        }
    }
    public interface IPackage
    {
        /// <summary>
        /// Name of package
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Version of package
        /// </summary>
        public string Version { get; }
        /// <summary>
        /// Generic field for description, may be left out if getting it requires another process call
        /// </summary>
        public string? Description { get; }
        /// <summary>
        /// Calling this field might take a considarable amount of time and should be avoided because not all package manager wrappers might support it
        /// </summary>
        public string? FullDescription { get; }

        /// <summary>
        /// Generic field for anything, could be a platform (e.g. Github, Flatpak), could be a package manager specific field like a bucket ("main", "extra", "silvercraft")
        /// </summary>
        public string? Source { get;  }
    }
    public interface IPackageManager
    {
        /// <summary>
        /// The package manager's name
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The package manager's version
        /// </summary>
        public string Version { get; }
        /// <summary>
        /// Gets a list of all installed packages
        /// </summary>
        /// <returns>a list of all installed packages</returns>
        public IEnumerable<IPackage> GetInstalledPackages();
        /// <summary>
        /// Gets a list of packages ready to update
        /// </summary>
        /// <returns>a list of packages ready to update</returns>
        public IEnumerable<IPackage> GetPackagesReadyToUpdate();
        /// <summary>
        /// Upgrades list of avaliable packages
        /// </summary>
        /// <returns>nothing</returns>
        public void UpgradeIndex();
        /// <summary>
        /// Upgrades a package
        /// </summary>
        /// <param name="id">the packages name/id depending on the package manager</param>
        public void UpgradePackage(string id);
        /// <summary>
        /// Installs a package
        /// </summary>
        /// <param name="id">the packages name/id depending on the package manager</param>
        public void InstallPackage(string id);
        /// <summary>
        /// Upgrades all packages ready to update
        /// </summary>
        public void UpgradePackages();

    }
    public class ScoopPackageManager : IPackageManager
    {
        public string Name =>  "Scoop";

        public string Version => GetScoopVer();
        public string GetScoopVer()
        {
            var c= RunCommand("-v");
            c.WaitForExit();
            var s =c.StandardOutput.ReadToEnd();
            var x = s.Split("\n");
            var y = Array.IndexOf(x, "Current Scoop version:");         
            return x[y+1];
        }
        public Process? RunCommand(string args)
        {
            return Process.Start(new ProcessStartInfo() { Arguments= $"/c \"scoop {args}\"", FileName="cmd", CreateNoWindow=true, RedirectStandardOutput=true, RedirectStandardError=true, UseShellExecute=false});
        }
        public IEnumerable<IPackage> GetInstalledPackages()
        {
            var c = RunCommand("list");
            c.WaitForExit();
            var s = c.StandardOutput.ReadToEnd();
            var x = s.Split("\n");
            int y;
            for (y=0;y<x.Length;y++)
            {
                if (x[y].StartsWith("----"))
                {
                    break;
                }
            }
            for (int y2 = y+1; y2 < x.Length; y2++)
            {
                if (!string.IsNullOrWhiteSpace( x[y2]))
                {
                    var fields = x[y2].Split("  ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    yield return new ScoopPackage(fields[0], fields[1], fields[2], fields[3], fields[4]);
                }
            }
        }

        public IEnumerable<IPackage> GetPackagesReadyToUpdate()
        {
            var c = RunCommand("status");
            c.WaitForExit();
            var s = c.StandardOutput.ReadToEnd();
            var x = s.Split("\n");
            int y;
            for (y = 0; y < x.Length; y++)
            {
                if (x[y].StartsWith("----"))
                {
                    break;
                }
            }
            for (int y2 = y + 1; y2 < x.Length; y2++)
            {
                if (!string.IsNullOrWhiteSpace(x[y2]))
                {
                    var fields = x[y2].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    yield return new ScoopPackage(fields[0], fields[1], fields[2]);
                }
            }
        }


        public void InstallPackage(string id)
        {
            var c = RunCommand($"install {id}");
            c.OutputDataReceived += (s, e) => {
                Console.WriteLine(e.Data);
            };
            c.ErrorDataReceived += (s, e) => {
                Console.WriteLine(e.Data);
            };
            c.BeginErrorReadLine();
            c.BeginOutputReadLine();
            c.WaitForExit();
        }

        public void UpgradeIndex()
        {
            var c = RunCommand("update");
            c.OutputDataReceived += (s, e) => {
                Console.WriteLine(e.Data);
            };
            c.ErrorDataReceived += (s, e) => {
                Console.WriteLine(e.Data);
            };
            c.BeginErrorReadLine();
            c.BeginOutputReadLine();
            c.WaitForExit();
        }

        public void UpgradePackage(string id)
        {
            var c = RunCommand($"update {id}");
            c.OutputDataReceived += (s, e) => {
                Console.WriteLine(e.Data);
            };
            c.ErrorDataReceived += (s, e) => {
                Console.WriteLine(e.Data);
            };
            c.BeginErrorReadLine();
            c.BeginOutputReadLine();
            c.WaitForExit();
        }

        public void UpgradePackages()
        {
            var c = RunCommand("update *");
            c.OutputDataReceived += (s, e) => {
                Console.WriteLine(e.Data);
            };
            c.ErrorDataReceived += (s, e) => {
                Console.WriteLine(e.Data);
            };
            c.BeginErrorReadLine();
            c.BeginOutputReadLine();
            c.WaitForExit();
        }
    }

}