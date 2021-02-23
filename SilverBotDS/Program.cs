using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using SDBrowser;
using SilverBotDS.Commands;
using SilverBotDS.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using SilverBotDS.Commands.Gamering;
using SilverBotDS.Converters;
using SilverBotDS.Objects;
using Lavalink4NET;
using Lavalink4NET.DSharpPlus;
using System.Threading;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;

namespace SilverBotDS
{
    internal static class Program
    {
        private static Config config;
        private static readonly bool Shitlog = false;

        private static Config GetNewConfig()
        {
            MainLogLine("Loading config");
            var task = Config.GetAsync();
            task.Wait();
            var res = task.Result;
            MainLogLine("GREAT SUCCESS LOADING CONFIG");
            return res;
        }

        private static void MainLogLine(string line)
        {
            Console.WriteLine($"[Main]: {line}");
        }

        private static async Task MainLogLineAsync(string line)
        {
            await Console.Out.WriteLineAsync($"[MainAsync]: {line}");
        }

        private static void Main()
        {
            MainLogLine("Load config");
            config = GetNewConfig();
            MainLogLine("Connect to selinum??");
            switch (config.BrowserType)
            {
                case 1:
                    {
                        Browser.SetBrowser(new SeleniumBrowser(Browsertype.Chrome, config.DriverLocation));
                        break;
                    }
                case 2:
                    {
                        Browser.SetBrowser(new SeleniumBrowser(Browsertype.Firefox, config.DriverLocation));
                        break;
                    }
            }

            MainLogLine("Checking for updates");
            //Check for updates
            VersionInfo.Checkforupdates();
            MainLogLine("Starting MainAsync");
            //Start the main async task
            MainAsync().GetAwaiter().GetResult();
        }

        public static Config GetConfig()
        {
            return config;
        }

        private static readonly DiscordWebhookClient Wc = new();

        public static async Task SendLogAsync(string text, List<DiscordEmbed> embeds)
        {
            var whb = new DiscordWebhookBuilder();
            whb.AddEmbeds(embeds);
            whb.WithContent(text);
            await Wc.BroadcastMessageAsync(whb);
        }

        private static DiscordClient discord;
        private static LavalinkNode audioService;

        private static async Task MainAsync()
        {
            //add log at first
            _ = MainLogLineAsync("Adding the logging webhook");
            var uri = new UriBuilder(config.LogWebhook);
            await Wc.AddWebhookAsync(uri.Uri);
            //Make us a little cute client
            _ = MainLogLineAsync("Creating the discord client");
            discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = config.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });
            //Tell our client to initialize interactivity
            _ = MainLogLineAsync("Initialising interactivity");
            discord.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30)
            });
            //set up logging?
            if (Shitlog)
            {
                _ = MainLogLineAsync("Setup unimplemented message tracking:tm:");
                discord.MessageCreated += Discord_MessageCreated;
            }
            //Tell our cute client to use commands or in other words become a working class member of society
            _ = MainLogLineAsync("Initialising Commands");
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = config.Prefix
            });
            //Register our commands
            _ = MainLogLineAsync("Regisitring Commands&Converters");
            commands.RegisterConverter(new SdImageConverter());
            commands.RegisterCommands<Genericcommands>();
            commands.RegisterCommands<Emotes>();
            commands.RegisterCommands<Moderation>();
            commands.RegisterCommands<Giphy>();
            commands.RegisterCommands<Imagemodule>();
            commands.RegisterCommands<AdminCommands>();
            commands.RegisterCommands<OwnerOnly>();
            commands.RegisterCommands<SteamCommands>();
            commands.RegisterCommands<Fortnite>();
            commands.RegisterCommands<Audio>();

            //Launch lavalink
            if (!File.Exists("Lavalink.jar"))
            {
                _ = MainLogLineAsync("Downloading lavalink");
                GitHubUtils.Repo repo = new("Frederikam", "Lavalink");
                var release = await GitHubUtils.Release.GetLatestFromRepoAsync(repo);
                await release.DownloadLatestAsync();
            }
            _ = MainLogLineAsync("Launching lavalink");

            var proStart = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = config.JavaLoc,
                    Arguments = " -jar Lavalink.jar",
                    UseShellExecute = true
                }
            };

            proStart.Start();
            _ = MainLogLineAsync("Waiting 6s");
            await Task.Delay(6000);
            _ = MainLogLineAsync("Making a node");
            audioService = new LavalinkNode(new LavalinkNodeOptions
            {
                RestUri = config.LavalinkRestUri,
                WebSocketUri = config.LavalinkWebSocketUri,
                Password = config.LavalinkPassword
            }, new DiscordClientWrapper(discord));
            //🥁🥁🥁 drumroll please

            _ = MainLogLineAsync("Connecting to discord");
            discord.Ready += Discord_Ready;
            await discord.ConnectAsync(new("console logs while booting up", ActivityType.Watching));
            if (!(config.FridayTextChannel == 0 || config.FridayVoiceChannel == 0))
            {
                waitforfriday = new Thread(new ThreadStart(WaitForFridayAsync));
            }
            //We have achived pog
            _ = MainLogLineAsync("Waiting 3s");
            await Task.Delay(3000);
            while (true)
            {
                _ = MainLogLineAsync("Updating the status to a random one");
                //update the status to some random one
                await discord.UpdateStatusAsync(await Splashes.GetSingleAsync(useinternal: config.UseSplashConfig));
                //wait the specified time
                _ = MainLogLineAsync($"Waiting {config.MsInterval}ms");
                await Task.Delay(config.MsInterval);
                //repeat🔁
            }
        }

        private static Thread waitforfriday;
        private const string FridayUrl = "https://youtu.be/akT0wxv9ON8";
        private static int last_friday = 0;

        public static async void WaitForFridayAsync()
        {
            while (true)
            {
                if (DayOfWeek.Friday == DateTime.Now.DayOfWeek && (last_friday == 0 || last_friday != DateTime.Now.DayOfYear))
                {
                    last_friday = DateTime.Now.DayOfYear;
                    await ExecuteFridayAsync();
                }
                await Task.Delay(1000);
            }
        }

        public static async Task ExecuteFridayAsync()
        {
            var channel = await discord.GetChannelAsync(config.FridayTextChannel);
            var vchannel = await discord.GetChannelAsync(config.FridayVoiceChannel);
            if (audioService.HasPlayer(vchannel.GuildId))
            {
                VoteLavalinkPlayer player = audioService.GetPlayer<VoteLavalinkPlayer>(vchannel.GuildId);
                await player.DisconnectAsync();
                await player.ConnectAsync(config.FridayVoiceChannel);
                var track = await audioService.GetTrackAsync(FridayUrl, SearchMode.YouTube);
                await player.StopAsync();
                await player.PlayAsync(track);
            }
            else
            {
                VoteLavalinkPlayer player = await audioService.JoinAsync<VoteLavalinkPlayer>(vchannel.GuildId, vchannel.Id, true);
                var track = await audioService.GetTrackAsync(FridayUrl, SearchMode.YouTube);
                await player.PlayAsync(track);
            }
            await channel.SendMessageAsync("its friday");
            return;
        }

        private static async Task Discord_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            await audioService.InitializeAsync();
            Audio.SetLavaLinkNode(audioService);
            waitforfriday.Start();
        }

        private static async Task Discord_MessageCreated(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            await Logging.Log(e.Message).ConfigureAwait(false);
        }
    }
}