﻿/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using SilverBotDS.Attributes;
using SilverBotDS.Utils;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using Towel;

namespace SilverBotDS.Objects;

[Serializable]
public class Config
{
    private const ulong CurrentConfVer = 49;

    [XmlDescription("Array of prefixes the bot will respond to")]
    public string[] Prefix { get; set; } =
    {
        "sd!",
        "sb!",
        "~",
        "ß",
        "hey silverbot",
        "ok silverbot",
        "this is so sad silverbot"
    };

    [XmlDescription(
        "how much is the bot allowed to log, Values can be Trace, Debug, Information, Warning, Error or Critical")]
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

    [XmlDescription("Should silverbot keep its logs on a log.txt file in its directory")]
    public bool UseTxtFilesAsLogs { get; set; } = true;

    [XmlDescription("The Discord token, can be had at https://discord.com/developers/")]
    public string Token { get; set; } = "Discord_Token_Here";

    [XmlDescription("What modules should silverbot load")]
    public string[] ModulesToLoad { get; set; } =
    {
        "SilverBotDS.Commands.Genericcommands",
        "SilverBotDS.Commands.ModCommands",
        "SilverBotDS.Commands.Giphy",
        "SilverBotDS.Commands.OwnerOnly",
        "SilverBotDS.Commands.Gamering.SteamCommands",
        "SilverBotDS.Commands.Bubot",
        "SilverBotDS.Commands.BibiCommands",
        "SilverBotDS.Commands.BibiLib",
        "SilverBotDS.Commands.Audio",
        "SilverBotDS.Commands.MiscCommands",
        "SilverBotDS.Commands.Gamering.MinecraftModule",
        "SilverBotDS.Commands.UserQuotesModule",
        "SilverBotDS.Commands.TranslatorCommands",
        "SilverBotDS.Commands.ServerStatsCommands",
        "SilverBotDS.Commands.ImageModule",
        "SilverBotDS.Commands.Experience",
        "SilverBotDS.Commands.AdminCommands",
        "SilverBotDS.Commands.ReactionRoleCommands",
        "SilverBotDS.Commands.ReminderCommands",
        "SilverBotDS.Commands.Slash.GeneralCommands",
        "SilverBotDS.Commands.Slash.AudioSlash",
        "SilverBotDS.Commands.Slash.BubotSlash",
    };

    [XmlDescription("What services should silverbot load from external dlls")]
    public SerializableDictionary<string, string> ServicesToLoadExternal { get; set; } = new()
    {
        {"SilverBot.SysAdminModule.dll","SilverBot.SysAdminModule.SysAdminModule" }
    };

    [XmlDescription("What modules should silverbot load from external dlls")]
    public string[] ModulesFilesToLoadExternal { get; set; } = new[]
    {
        "SilverBotDS.AnimeModule.dll"
    };

    [XmlDescription("The Giphy token, can be got from https://developers.giphy.com/")]
    public string Gtoken { get; set; } = "Giphy_Token_Here";

    [XmlDescription("The FortniteAPI token, can be got from https://dash.fortnite-api.com/")]
    public string FApiToken { get; set; } = "Fortnite_Token_Here";

    [XmlDescription("Location of java.exe to be able to launch lavalink (type java if in path")]
    public string JavaLoc { get; set; } = "C:\\Program Files\\Java\\jdk-13\\bin\\java.exe";

    [XmlDescription("(ulong)Id of main server")]
    public ulong ServerId { get; set; } = 679353407667961877;

    [XmlDescription("Call the garbage collector when the splash changes (the gc is just snake oil)")]
    public bool CallGcOnSplashChange { get; set; } = false;

    [XmlDescription("Enable reaction role related functions, essential if you load SilverBotDS.Commands.ReactionRoleCommands")]
    public bool ReactionRolesEnabled { get; set; } = true;

    [XmlDescription("Should this instance of silverbot host a webserver?")]
    public bool HostWebsite { get; set; } = true;

    [XmlDescription("Should this instance of silverbot clear dead tasks? (answer is usually yes unless you are debugging)")]
    public bool ClearTasks { get; set; } = true;

    [XmlDescription(
        "Interval to use so discord dont ban us, in ms, is int32 so use -1 if you want no splash changes, defaults to 30m (1800000ms)")]
    public int MsInterval { get; set; } = 1800000;

    [XmlDescription("The current config version, don't change unless insctructed by dev")]
    public ulong? ConfigVer { get; set; }
    
    [XmlDescription(
        "Allow silverbot to connect to lavalink and use audio commands, useful if you have lavalink installed or if you allowed silverbot to install and run lavalink (requires java)")]
    public bool UseLavaLink { get; set; } = true;

    [XmlDescription(
        "Allow silverbot to download and start Lavalink, if you disable this your responsible for launching lavalink")]
    public bool AutoDownloadAndStartLavalink { get; set; } = true;

    [XmlDescription(
        "Enable sponsorblock plugin, if https://github.com/TopiSenpai/Sponsorblock-Plugin is installed on lavalink server")]
    public bool SponsorBlock { get; set; } = false;

    [XmlDescription(
       "If AutoDownloadAndStartLavalink is set to true then you have to set this and LavalinkBuildsSourceGitHubRepo to a valid github repo with releases that contain jar files, Some examples are the official repo that is freyacodes/Lavalink (Official source) or Cog-Creators/Lavalink-Jars (Contains native libraries for ARM and some other platforms not nativly supported by lavalink officially)")]
    public string LavalinkBuildsSourceGitHubUser { get; set; } = "freyacodes";

    public string LavalinkBuildsSourceGitHubRepo { get; set; } = "Lavalink";

    [XmlDescription("The Lavalink Node restful HTTP api URI.")]
    public string LavalinkRestUri { get; set; } = "http://localhost:2333/";

    [XmlDescription("The Lavalink Node WebSocket URI.")]
    public string LavalinkWebSocketUri { get; set; } = "ws://localhost:2333/";

    [XmlDescription("The Lavalink Node Password.")]
    public string LavalinkPassword { get; set; } = "youshallnotpass";

    [XmlDescription("Friday text channel. leave 0 to disable")]
    public ulong FridayTextChannel { get; set; }

    [XmlDescription("Allow silverbot to load (or make) a colors config, useful if you plan on adding colors")]
    public bool ColorConfig { get; set; } = true;

    [XmlDescription("Allow silverbot to emulate bubot (enables some basic commands)")]
    public bool EmulateBubot { get; set; } = false;

    [XmlDescription("Allow silverbot to emulate bubot's bibi commands (also enables the bibi gallery)")]
    public bool EmulateBubotBibi { get; set; } = false;

    [XmlDescription("Location of bibi pictures (local -  pictures(png) -  folder)")]
    public string LocalBibiPictures { get; set; } = "SilverBotAssets/Bibi";

    [XmlDescription("Location of bibi pictures (BibiLibCutout - web - pictures)")]
    public string BibiLibCutOut { get; set; } = "https://github.com/thesilvercraft/SilverBotAssets/blob/main/BibiLibCutout/{0}.png?raw=true";

    [XmlDescription("Location of titles for BibiLibCutout pictures (BibiLibCutout - local - config)")]
    public string BibiLibCutOutConfig { get; set; } = "SilverBotAssets/BibiLibCutout/Titles.json";

    [XmlDescription("Location of bibi pictures (BibiLibFull - web - pictures)")]
    public string BibiLibFull { get; set; } = "https://github.com/thesilvercraft/SilverBotAssets/blob/main/BibiLibFull/{0}.png?raw=true";

    [XmlDescription("Location of titles for BibiLibFull pictures (BibiLibFull - local - config)")]
    public string BibiLibFullConfig { get; set; } = "SilverBotAssets/BibiLibFull/Titles.json";

    [XmlDescription("A temporary way to disable the interactivity tracker")]
    public bool SitInVc { get; set; } = false;
    
    [XmlDescription("Do we enable the server statistics")]
    public bool EnableServerStatistics { get; set; } = true;

    [XmlDescription("Used for the new Translator Mode thingy")]
    public ulong TranslatorRoleId { get; set; } = 0;

    public ulong TranslatorModeChannel { get; set; } = 0;

    public ulong LoginPageDiscordClientId { get; set; } = 702445582559739976;
    public string LoginPageDiscordClientSecret { get; set; } = "lol no";

    [XmlDescription("Do we check github for a newer commit")]
    public bool EnableUpdateChecking { get; set; } = false;

    public bool UseAnalytics { get; set; } = false;

    [XmlDescription("Webhooks for archiving")]
    public string[] ArchiveWebhooks { get; set; } = Array.Empty<string>();

    [XmlDescription("Where the hell do we get our data from")]
    public ulong[] ChannelsToArchivePicturesFrom { get; set; } = Array.Empty<ulong>();

    [XmlDescription(
        "Song aliases, It can be any kind of url or search term (It also supports SilverBotPlaylist files)")]
    public SerializableDictionary<string, string> SongAliases { get; set; } = new()
    {
        { "special for bub", "https://www.youtube.com/watch?v=y1TJBgpGrd8" },
        { "meme playlist", "https://www.youtube.com/playlist?list=PLiiTWcm0RsKj8toM1CoxDbjDZftLYDFo1" },
        { "evening mix", "https://www.youtube.com/playlist?list=PLiiTWcm0RsKhoGLQA84m1ag9QV5AH1usW" },
        { "kae chill beats", "https://www.youtube.com/playlist?list=PLB7khNwMQ_sPGMqBYhS3_3u57U33JK2jB" },
        { "spirit phone", "https://music.youtube.com/playlist?list=OLAK5uy_k-gjwrMLQJbpBbYgWuTv0FYiws5aXkoG0" },
        { "mouth moods", "https://www.youtube.com/playlist?list=PL4Nm4rhtI5e57ElPzTNKW0XHE1wisqr5H" },
        { "mouth silence", "https://www.youtube.com/playlist?list=PL4Nm4rhtI5e6F0to07duRPcEclXq_bYta" },
        { "mouth sounds", "https://www.youtube.com/playlist?list=PL4Nm4rhtI5e5p_f8YmH2jk23Zvq_h-UE-" },
        { "view monster", "https://www.youtube.com/playlist?list=PL621AB9949A35F306" },
        { "doug stream music", "https://www.youtube.com/playlist?list=PLzTxt5iYdhzifPXw_g0hWp0YgFetgazuv" },
        { "ninja tuna", "https://music.youtube.com/playlist?list=OLAK5uy_lYrDqWbPEMCqxcBSOdaMWonNJzP24mLhA" },
        { "antenna5", "http://antenna5stream.neotel.mk:8000/live128" }
    };

    public Splash[] Splashes { get; set; } =
    {
        new("D̶U̶K̶T̶  Silver Hosting", ActivityType.Watching),
        new("100% pure!", ActivityType.Playing),
        new("20 GOTO 10!", ActivityType.Playing),
        new("Déjà vu!", ActivityType.Playing),
        new("Déjà vu!", ActivityType.Playing),
        new("Does barrel rolls!", ActivityType.Playing),
        new("Internet enabled!", ActivityType.Playing),
        new("Now supports emoji 😉", ActivityType.Playing),
        new("if not OK return end", ActivityType.Playing),
        new("Now on {Platform}.", ActivityType.Playing),
        new("5 is best number!", ActivityType.Playing),
        new("C#!", ActivityType.ListeningTo),
        new("Plastic Guns!", ActivityType.Playing),
        new("Lines from 2022!", ActivityType.ListeningTo),
        new("Absolutely no sarcasm!", ActivityType.Playing),
        new("Thinking involved!", ActivityType.Playing),
        new("huehuehue", ActivityType.Playing),
        new("!gnilleps sdrawkcab", ActivityType.Playing),
        new("0% Milk!", ActivityType.Playing),
        new("Available in stores!", ActivityType.Playing),
        new("Gluten Free!", ActivityType.Playing),
        new("But wait there's more!", ActivityType.Playing),
        new("except in nebraska", ActivityType.Playing),
        new("Maximizing Profits!", ActivityType.Playing),
        new("Blue!", ActivityType.Playing),
        new("Green!", ActivityType.Playing),
        new("Exclamation Mark!", ActivityType.Playing),
        new("Strongly Dislikes Birch!", ActivityType.Playing),
        new("Bestest Grammar!", ActivityType.Playing),
        new("Relevant Text!", ActivityType.Playing),
        new("finally released", ActivityType.Playing),
        new("needs update", ActivityType.Playing),
        new("Woah!", ActivityType.Playing),
        new("fruit salad is best salad", ActivityType.Playing),
        new("10% less random!", ActivityType.Playing),
        new("Move now, think later!", ActivityType.Playing),
        new("*teleports behind you*", ActivityType.Playing),
        new("Less than Minimum!", ActivityType.Playing),
        new("The Exits are... Irrelevant!", ActivityType.Playing),
        new("The facts they dont want you to know", ActivityType.ListeningTo),
        new("requires additional threads", ActivityType.Playing),
        new("Really Dark Purples!", ActivityType.Playing),
        new("Two Ups Thumb!", ActivityType.Playing),
        new("Gilden Ultra Cotton Pocket Tee!", ActivityType.Playing),
        new("Crazy Cool Coincidence!", ActivityType.Playing),
        new("Over the mountains, into Terraria!", ActivityType.Playing),
        new("Fools' Gold!", ActivityType.Playing),
        new("Globular Resources!", ActivityType.Playing),
        new("Falcon Heavy!", ActivityType.Playing),
        new("shoutout to you", ActivityType.Playing),
        new("Acid Trips!", ActivityType.Playing),
        new("Wonder-filly-ful!", ActivityType.Playing),
        new("Nostalgic!", ActivityType.Playing),
        new("som splashes for u ^_^", ActivityType.Playing),
        new("20% more memory!", ActivityType.Playing),
        new("Kalorichen has invited you to play Minecraft", ActivityType.ListeningTo),
        new("ThePajamaSlime! ThePajamaSlime! ThePajamaSlime!", ActivityType.Playing),
        new("Xbox 360 Edition", ActivityType.Playing),
        new("sign in with your microsoft account", ActivityType.Playing),
        new("Now with 69% more ThePajamaSlime!", ActivityType.Playing),
        new("Teach me the stats!", ActivityType.Playing),
        new("Teach me the strats!", ActivityType.Playing),
        new("capped image of caps with captions in caps", ActivityType.Playing),
        new("subscribe to idots", ActivityType.Playing),
        new("Cinnamon oat cereal!", ActivityType.Playing),
        new("Silver chest unlocked!", ActivityType.Playing),
        new("Flame-Headed Boy!", ActivityType.Playing),
        new("Non-Newtonian Fluids!", ActivityType.Playing),
        new("Intensive and Extensive!", ActivityType.Playing),
        new("Aquatic!", ActivityType.Playing),
        new("4TH DOWN AND 9!", ActivityType.Playing),
        new("pls r8 5", ActivityType.Playing),
        new("dabbing on them endermans", ActivityType.Playing),
        new("GIMP FTW", ActivityType.Playing),
        new("Bush hid the facts", ActivityType.Playing),
        new("ugh, finally, no more sugar", ActivityType.Playing),
        new("uploading...41%", ActivityType.Playing),
        new("my stuff stuff", ActivityType.Playing),
        new("teleporting-porter-pearl", ActivityType.Playing),
        new("(it's a xml file with a list, not a method)", ActivityType.Playing),
        new("superb bus service", ActivityType.Playing),
        new("SELCET? AWESMOE!", ActivityType.Playing),
        new("pretty pink spigots", ActivityType.Playing),
        new("you're uh winner", ActivityType.Playing),
        new("wifi killed the dinosaurs", ActivityType.Playing),
        new("60 layers of wax", ActivityType.Playing),
        new("12 years later", ActivityType.Playing),
        new("it came in teal!", ActivityType.Playing),
        new("pretend the sun is real", ActivityType.Playing),
        new("people hate people", ActivityType.Playing),
        new("north of what?", ActivityType.Playing),
        new("pertrify yourself!", ActivityType.Playing),
        new("pay the poor", ActivityType.Playing),
        new("check your spam!", ActivityType.Playing),
        new("see it with your ears!", ActivityType.Playing),
        new("hear it with your eyes!", ActivityType.Playing),
        new("it'll change, soon!", ActivityType.Playing),
        new("space is key!", ActivityType.Playing),
        new("Smash Mouth - All Star", ActivityType.ListeningTo),
        new("indentation error", ActivityType.Watching),
        new("THE CAKE IS A LIE", ActivityType.Playing),
        new("ThE CaKe Is A lIe", ActivityType.Playing),
        new("0s and 1s", ActivityType.ListeningTo),
    };

    private static XmlDocument MakeDocumentWithComments(XmlDocument xmlDocument)
    {
        foreach (var i in typeof(Config).GetMembers())
        {
            foreach (var e in i.GetCustomAttributes(false))
            {
                if (e.GetType() == typeof(XmlDescriptionAttribute))
                {
                    xmlDocument = XmlUtils.CommentBeforeObject(xmlDocument, $"/Config/{i.Name}",
                    ((XmlDescriptionAttribute)e).Description);
                }
                else if (e.GetType() == typeof(XmlCommentInsideAttribute))
                {
                    xmlDocument = XmlUtils.CommentInObject(xmlDocument, $"/Config/{i.Name}",
                    ((XmlCommentInsideAttribute)e).Comment);
                }
            }
        }

        return xmlDocument;
    }

    public static async Task OutdatedConfigTask(Config readconfig)
    {
        using (var streamReader = new StreamReader("silverbot.xml"))
        {
            await using var streamWriter = new StreamWriter("silverbotold.xml", false);
            await streamWriter.WriteAsync(await streamReader.ReadToEndAsync());
        }

        await using (var streamWriter = new StreamWriter("silverbot.xml"))
        {
            if (readconfig.ConfigVer < 20 && File.Exists("splashes.json"))
            {
                using StreamReader reader = new("splashes.json");
                readconfig.Splashes = JsonSerializer.Deserialize<DiscordActivity[]>(await reader.ReadToEndAsync())
                    .Select(item => Splash.GetFromDiscordActivity(item)).ToArray();
            }

            readconfig.ConfigVer = CurrentConfVer;
            MakeDocumentWithComments(XmlUtils.SerializeToXmlDocument(readconfig)).Save(streamWriter);
        }
    }

    public static async Task<Config?> GetAsync()
    {
        var serializer = new XmlSerializer(typeof(Config));
        if (!File.Exists("silverbot.xml"))
        {
            var conf = new Config
            {
                ConfigVer = CurrentConfVer
            };
            if (Console.IsInputRedirected && Console.IsOutputRedirected && Environment.UserInteractive && Console.WindowHeight > 0)
            {
                Console.WriteLine("Would you like help creating the config? (y/n)");
                if (ConsoleInputHelper.GetBoolFromConsole())
                {
                    Console.WriteLine("Give me the Token of the discord bot.");
                    conf.Token = Console.ReadLine()??"DiscordTokenHere";
                    Console.WriteLine("Will you use lavalink?");
                    conf.UseLavaLink = ConsoleInputHelper.GetBoolFromConsole();
                    if (conf.UseLavaLink)
                    {
                        Console.WriteLine("Do you want silverbot to automatically download and start lavalink?");
                        conf.AutoDownloadAndStartLavalink = ConsoleInputHelper.GetBoolFromConsole();
                        if (conf.AutoDownloadAndStartLavalink)
                        {
                            Console.WriteLine("Where do you store your java executable? (type java or java.exe if in path) (E.g. C:\\Program Files\\Java\\jdk-11.0.2\\bin\\java.exe)");
                            conf.JavaLoc = Console.ReadLine() ?? "java";
                        }

                        conf.ServerId = ConsoleHelper.GetInput<ulong>("Enter main server id (long number)");
                    }
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Please manually review the generated config as this generator is not accurate");
                }
            }
            await using (var streamWriter = new StreamWriter("silverbot.xml"))
            {
                MakeDocumentWithComments(XmlUtils.SerializeToXmlDocument(conf)).Save(streamWriter);
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("silverbot.xml should exist in the CWD, edit it, save it and restart silverbot");
            Environment.Exit(420);
        }
        Config? cnf = null;
        using (var fs = new StreamReader("silverbot.xml"))
        {
            cnf = (Config?)serializer.Deserialize(fs);
            if (cnf is { ConfigVer: CurrentConfVer })
            {
                return cnf;
            }

            fs.Dispose();
            await OutdatedConfigTask(cnf);
            return await GetAsync();
        }
        return cnf;
    }
}