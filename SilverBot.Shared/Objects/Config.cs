/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.ComponentModel;
using System.Text.Json;
using System.Xml.Serialization;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using SilverBot.Shared.Objects.Classes;
using SilverConfig;
using Towel;

namespace SilverBot.Shared.Objects
{
    [Serializable]
    public class Config : INotifyPropertyChanged, ICanBeToldThatAPartOfMeIsChanged
    {
        private const ulong CurrentConfVer = 51;

        [Comment("Array of prefixes the bot will respond to")]
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

        [Comment(
            "how much is the bot allowed to log, Values can be Trace, Debug, Information, Warning, Error or Critical")]
        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

        [Comment("Should silverbot keep its logs on a log.txt file in its directory")]
        public bool UseTxtFilesAsLogs { get; set; } = true;

        [Comment("The Discord token, can be had at https://discord.com/developers/")]
        public string Token { get; set; } = "Discord_Token_Here";

        [Comment("What modules should silverbot load")]
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
            "SilverBotDS.Commands.Slash.BubotSlash"
        };

        [Comment("What services should silverbot load from external dlls")]
        public Classes.SerializableDictionary<string, string> ServicesToLoadExternal { get; set; } = new()
        {
            { "SilverBot.SysAdminModule.dll", "SilverBot.SysAdminModule.SysAdminModule" }
        };

        [Comment("Extra string paramaters you might want to give to any code that can read this config")]
        public Classes.SerializableDictionary<string, string> ExtraParams { get; set; } = new()
        {
            { "StableDiff.SafeModel", "sd-v1-4, App_Icons_V1_PublicPrompts, Pixel_Art_V1_PublicPrompts" },
            { "StableDiff.NotSafeModel", "hd-17, wd-v1-3-float16" },
            { "StableDiff.Trusted", "687387957296103541, 1024769162419126362" },
            { "StableDiff.BaseUrl", "http://localhost:9000" },

            { "JellyFinLookupService.BaseUrlNoSlash", "http://localhost:8096" },
            { "JellyFinLookupService.ApiKey", "68dc268e4d264dcd9a625ac37e3b4f7b" },
            { "JellyFinLookupService.ParentLibId", "7e64e319657a9516ec78490da03edccb" },
            { "JellyFinLookupService.ParentUserId", "b981a5d9970c44c184ecd4dfd1202439" },
            {"MiscMail.MailUser", ""},
            {"MiscMail.MailPass", ""},
            {"MiscMail.MailServer",""}
        };

        [Comment("What modules should silverbot load from external dlls")]
        public string[] ModulesFilesToLoadExternal { get; set; } = new[]
        {
            "SilverBotDS.AnimeModule.dll"
        };

        [Comment("The Giphy token, can be got from https://developers.giphy.com/")]
        public string Gtoken { get; set; } = "Giphy_Token_Here";

        [Comment("The FortniteAPI token, can be got from https://dash.fortnite-api.com/")]
        public string FApiToken { get; set; } = "Fortnite_Token_Here";

        [Comment("Location of java.exe to be able to launch lavalink (type java if in path")]
        public string JavaLoc { get; set; } = "C:\\Program Files\\Java\\jdk-13\\bin\\java.exe";

        [Comment("(ulong)Id of main server")] public ulong ServerId { get; set; } = 679353407667961877;

        [Comment("Call the garbage collector when the splash changes (the gc is just snake oil)")]
        public bool CallGcOnSplashChange { get; set; } = false;

        [Comment(
            "Enable reaction role related functions, essential if you load SilverBotDS.Commands.ReactionRoleCommands")]
        public bool ReactionRolesEnabled { get; set; } = true;

        [Comment("Should this instance of silverbot host a webserver?")]
        public bool HostWebsite { get; set; } = true;

        [Comment(
            "Should this instance of silverbot clear dead tasks? (answer is usually yes unless you are debugging)")]
        public bool ClearTasks { get; set; } = true;

        [Comment(
            "Interval to use so discord dont ban us, in ms, is int32 so use -1 if you want no splash changes, defaults to 30m (1800000ms)")]
        public int MsInterval { get; set; } = 1800000;

        [Comment("The current config version, don't change unless insctructed by dev")]
        public ulong? ConfigVer { get; set; }

        [Comment(
            "Allow silverbot to connect to lavalink and use audio commands, useful if you have lavalink installed or if you allowed silverbot to install and run lavalink (requires java)")]
        public bool UseLavaLink { get; set; } = true;

        [Comment(
            "Allow silverbot to download and start Lavalink, if you disable this your responsible for launching lavalink")]
        public bool AutoDownloadAndStartLavalink { get; set; } = true;

        [Comment(
            "Enable sponsorblock plugin, if https://github.com/TopiSenpai/Sponsorblock-Plugin is installed on lavalink server")]
        public bool SponsorBlock { get; set; } = false;

        [Comment(
            "If AutoDownloadAndStartLavalink is set to true then you have to set this and LavalinkBuildsSourceGitHubRepo to a valid github repo with releases that contain jar files, Some examples are the official repo that is freyacodes/Lavalink (Official source) or Cog-Creators/Lavalink-Jars (Contains native libraries for ARM and some other platforms not nativly supported by lavalink officially)")]
        public string LavalinkBuildsSourceGitHubUser { get; set; } = "freyacodes";

        public string LavalinkBuildsSourceGitHubRepo { get; set; } = "Lavalink";

        [Comment("The Lavalink Node restful HTTP api URI.")]
        public string LavalinkRestUri { get; set; } = "http://localhost:2333/";

        [Comment("The Lavalink Node WebSocket URI.")]
        public string LavalinkWebSocketUri { get; set; } = "ws://localhost:2333/";

        [Comment("The Lavalink Node Password.")]
        public string LavalinkPassword { get; set; } = "youshallnotpass";

        [Comment("Enable the JellyFin lookup service, uses jellyfin, must be configured")]
        public bool EnableJellyFinLookupService { get; set; } = true;


        [Comment("Friday text channel. leave 0 to disable")]
        public ulong FridayTextChannel { get; set; }

        [Comment("Allow silverbot to load (or make) a colors config, useful if you plan on adding colors")]
        public bool ColorConfig { get; set; } = true;

        [Comment("Allow silverbot to emulate bubot (enables some basic commands)")]
        public bool EmulateBubot { get; set; } = false;

        [Comment("Allow silverbot to emulate bubot's bibi commands (also enables the bibi gallery)")]
        public bool EmulateBubotBibi { get; set; } = false;

        [Comment("Location of bibi pictures (local -  pictures(png) -  folder)")]
        public string LocalBibiPictures { get; set; } = "SilverBotAssets/Bibi";

        [Comment("Location of bibi pictures (BibiLibCutout - web - pictures)")]
        public string BibiLibCutOut { get; set; } =
            "https://github.com/thesilvercraft/SilverBotAssets/blob/main/BibiLibCutout/{0}.png?raw=true";

        [Comment("Location of titles for BibiLibCutout pictures (BibiLibCutout - local - config)")]
        public string BibiLibCutOutConfig { get; set; } = "SilverBotAssets/BibiLibCutout/Titles.json";

        [Comment("Location of bibi pictures (BibiLibFull - web - pictures)")]
        public string BibiLibFull { get; set; } =
            "https://github.com/thesilvercraft/SilverBotAssets/blob/main/BibiLibFull/{0}.png?raw=true";

        [Comment("Location of titles for BibiLibFull pictures (BibiLibFull - local - config)")]
        public string BibiLibFullConfig { get; set; } = "SilverBotAssets/BibiLibFull/Titles.json";

        [Comment("A temporary way to disable the interactivity tracker")]
        public bool SitInVc { get; set; } = false;

        [Comment("Do we enable the server statistics")]
        public bool EnableServerStatistics { get; set; } = true;

        [Comment("Used for the new Translator Mode thingy")]
        public ulong TranslatorRoleId { get; set; } = 0;

        public ulong TranslatorModeChannel { get; set; } = 0;

        public ulong LoginPageDiscordClientId { get; set; } = 702445582559739976;
        public string LoginPageDiscordClientSecret { get; set; } = "lol no";

        [Comment("Do we check github for a newer commit")]
        public bool EnableUpdateChecking { get; set; } = false;

        public bool UseAnalytics { get; set; } = false;

        [Comment("Webhooks for archiving")] public string[] ArchiveWebhooks { get; set; } = Array.Empty<string>();

        [Comment("Where the hell do we get our data from")]
        public ulong[] ChannelsToArchivePicturesFrom { get; set; } = Array.Empty<ulong>();

        [Comment(
            "Song aliases, It can be any kind of url or search term (It also supports SilverBotPlaylist files)")]
        public Classes.SerializableDictionary<string, string> SongAliases { get; set; } = new()
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
            new("100% pure!", ActivityType.Custom),
            new("20 GOTO 10!", ActivityType.Custom),
            new("Déjà vu!", ActivityType.Custom),
            new("Déjà vu!", ActivityType.Custom),
            new("Does barrel rolls!", ActivityType.Custom),
            new("Internet enabled!", ActivityType.Custom),
            new("Now supports emoji 😉", ActivityType.Custom),
            new("if not OK return end", ActivityType.Custom),
            new("Now on {Platform}.", ActivityType.Custom),
            new("5 is best number!", ActivityType.Custom),
            new("C#!", ActivityType.ListeningTo),
            new("Lines from 2024!", ActivityType.Custom),
            new("Absolutely no sarcasm!", ActivityType.Custom),
            new("Thinking involved!", ActivityType.Custom),
            new("huehuehue", ActivityType.Custom),
            new("!gnilleps sdrawkcab", ActivityType.Custom),
            new("0% Milk!", ActivityType.Custom),
            new("Available in stores!", ActivityType.Custom),
            new("Gluten Free!", ActivityType.Custom),
            new("But wait there's more!", ActivityType.Custom),
            new("except in nebraska", ActivityType.Custom),
            new("Maximizing Profits!", ActivityType.Custom),
            new("Blue!", ActivityType.Custom),
            new("Green!", ActivityType.Custom),
            new("Exclamation Mark!", ActivityType.Custom),
            new("Strongly Dislikes Birch!", ActivityType.Custom),
            new("Bestest Grammar!", ActivityType.Custom),
            new("Relevant Text!", ActivityType.Custom),
            new("finally released", ActivityType.Custom),
            new("needs update", ActivityType.Custom),
            new("Woah!", ActivityType.Custom),
            new("fruit salad is best salad", ActivityType.Custom),
            new("10% less random!", ActivityType.Custom),
            new("Move now, think later!", ActivityType.Custom),
            new("*teleports behind you*", ActivityType.Custom),
            new("Less than Minimum!", ActivityType.Custom),
            new("The Exits are... Irrelevant!", ActivityType.Custom),
            new("The facts they dont want you to know", ActivityType.ListeningTo),
            new("requires additional threads", ActivityType.Custom),
            new("Really Dark Purples!", ActivityType.Custom),
            new("Two Ups Thumb!", ActivityType.Custom),
            new("Gilden Ultra Cotton Pocket Tee!", ActivityType.Custom),
            new("Crazy Cool Coincidence!", ActivityType.Custom),
            new("Over the mountains, into Terraria!", ActivityType.Custom),
            new("Fools' Gold!", ActivityType.Custom),
            new("Globular Resources!", ActivityType.Custom),
            new("Falcon Heavy!", ActivityType.Custom),
            new("shoutout to you", ActivityType.Custom),
            new("Acid Trips!", ActivityType.Custom),
            new("Wonder-filly-ful!", ActivityType.Custom),
            new("Nostalgic!", ActivityType.Custom),
            new("som splashes for u ^_^", ActivityType.Custom),
            new("20% more memory!", ActivityType.Custom),
            new("Kalorichen has invited you to play Minecraft", ActivityType.ListeningTo),
            new("ThePajamaSlime! ThePajamaSlime! ThePajamaSlime!", ActivityType.Custom),
            new("Minecraft Xbox 360 Edition", ActivityType.Playing),
            new("sign in with your microsoft account", ActivityType.Custom),
            new("Now with 69% more ThePajamaSlime!", ActivityType.Custom),
            new("Teach me the stats!", ActivityType.Custom),
            new("Teach me the strats!", ActivityType.Custom),
            new("capped image of caps with captions in caps", ActivityType.Custom),
            new("subscribe to idots", ActivityType.Custom),
            new("Cinnamon oat cereal!", ActivityType.Custom),
            new("Silver chest unlocked!", ActivityType.Custom),
            new("Flame-Headed Boy!", ActivityType.Custom),
            new("Non-Newtonian Fluids!", ActivityType.Custom),
            new("Intensive and Extensive!", ActivityType.Custom),
            new("Aquatic!", ActivityType.Custom),
            new("4TH DOWN AND 9!", ActivityType.Custom),
            new("pls r8 5", ActivityType.Custom),
            new("dabbing on them endermans", ActivityType.Custom),
            new("GIMP FTW", ActivityType.Custom),
            new("Bush hid the facts", ActivityType.Custom),
            new("ugh, finally, no more sugar", ActivityType.Custom),
            new("uploading...41%", ActivityType.Custom),
            new("my stuff stuff", ActivityType.Custom),
            new("teleporting-porter-pearl", ActivityType.Custom),
            new("(it's a xml file with a list, not a method)", ActivityType.Custom),
            new("superb bus service", ActivityType.Custom),
            new("SELCET? AWESMOE!", ActivityType.Custom),
            new("pretty pink spigots", ActivityType.Custom),
            new("you're uh winner", ActivityType.Custom),
            new("wifi killed the dinosaurs", ActivityType.Custom),
            new("60 layers of wax", ActivityType.Custom),
            new("12 years later", ActivityType.Custom),
            new("it came in teal!", ActivityType.Custom),
            new("pretend the sun is real", ActivityType.Custom),
            new("people hate people", ActivityType.Custom),
            new("north of what?", ActivityType.Custom),
            new("pertrify yourself!", ActivityType.Custom),
            new("pay the poor", ActivityType.Custom),
            new("check your spam!", ActivityType.Custom),
            new("see it with your ears!", ActivityType.Custom),
            new("hear it with your eyes!", ActivityType.Custom),
            new("it'll change, soon!", ActivityType.Custom),
            new("space is key!", ActivityType.Custom),
            new("Smash Mouth - All Star", ActivityType.ListeningTo),
            new("indentation error", ActivityType.Watching),
            new("THE CAKE IS A LIE", ActivityType.Custom),
            new("ThE CaKe Is A lIe", ActivityType.Custom),
            new("0s and 1s", ActivityType.ListeningTo)
        };


        public static async Task OutdatedConfigTask(Config readConfig,
            CommentXmlConfigReaderNotifyWhenChanged<Config> configReader)
        {
            using (var streamReader = new StreamReader("silverbot.xml"))
            {
                await using var streamWriter = new StreamWriter("silverbotold.xml", false);
                await streamWriter.WriteAsync(await streamReader.ReadToEndAsync());
            }

            if (readConfig.ConfigVer < 20 && File.Exists("splashes.json"))
            {
                using StreamReader reader = new("splashes.json");
                readConfig.Splashes = (JsonSerializer.Deserialize<DiscordActivity[]>(await reader.ReadToEndAsync()) ??
                                       Array.Empty<DiscordActivity>())
                    .Select(Splash.GetFromDiscordActivity).ToArray();
            }

            readConfig.ConfigVer = CurrentConfVer;
            readConfig.AllowedToRead = false;
            configReader.Write(readConfig, "silverbot.xml");
            readConfig.AllowedToRead = true;
        }

        public static async Task<Config?> GetAsync()
        {
            CommentXmlConfigReaderNotifyWhenChanged<Config> reader = new();
            if (!File.Exists("silverbot.xml"))
            {
                var conf = new Config
                {
                    ConfigVer = CurrentConfVer
                };
                if (Console.IsInputRedirected && Console.IsOutputRedirected && Environment.UserInteractive &&
                    Console.WindowHeight > 0)
                {
                    Console.WriteLine("Would you like help creating the config? (y/n)");
                    if (ConsoleInputHelper.GetBoolFromConsole())
                    {
                        Console.WriteLine("Give me the Token of the discord bot.");
                        conf.Token = Console.ReadLine() ?? "DiscordTokenHere";
                        Console.WriteLine("Will you use lavalink?");
                        conf.UseLavaLink = ConsoleInputHelper.GetBoolFromConsole();
                        if (conf.UseLavaLink)
                        {
                            Console.WriteLine("Do you want silverbot to automatically download and start lavalink?");
                            conf.AutoDownloadAndStartLavalink = ConsoleInputHelper.GetBoolFromConsole();
                            if (conf.AutoDownloadAndStartLavalink)
                            {
                                Console.WriteLine(
                                    "Where do you store your java executable? (type java or java.exe if in path) (E.g. C:\\Program Files\\Java\\jdk-11.0.2\\bin\\java.exe)");
                                conf.JavaLoc = Console.ReadLine() ?? "java";
                            }

                            conf.ServerId = ConsoleHelper.GetInput<ulong>("Enter main server id (long number)");
                        }

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(
                            "Please manually review the generated config as this generator is not accurate");
                    }
                }

                reader.Write(conf, "silverbot.xml");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("silverbot.xml should exist in the CWD, edit it, save it and restart silverbot");
                Environment.Exit(420);
            }

            var cnf = reader.Read("silverbot.xml");
            if (cnf is not { ConfigVer: CurrentConfVer })
            {
                await OutdatedConfigTask(cnf, reader);
            }

            return cnf;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        void ICanBeToldThatAPartOfMeIsChanged.PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        [XmlIgnore] public bool AllowedToRead { get; set; } = true;
    }
}