using CXuesong.Uel.Serilog.Sinks.Discord;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Humanizer;
using Lavalink4NET;
using Lavalink4NET.DSharpPlus;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using Microsoft.Extensions.Logging;
using SDBrowser;
using Serilog;
using SilverBotDS.Commands;
using SilverBotDS.Commands.Gamering;
using SilverBotDS.Converters;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SilverBotDS
{
    internal static class Program
    {
        private static Config config;

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
            Console.WriteLine($"(OLD)[Main]: {line}");
        }

        private static void Main()
        {
            MainLogLine("Load config");
            config = GetNewConfig();
            MainLogLine("Making new logger");
            WebHookUtilz.ParseWebhookUrl(config.LogWebhook, out ulong id, out string token);
            log = new LoggerConfiguration()
  .WriteTo.Console()
  .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, shared: true)
 .WriteTo.Discord(new DiscordWebhookMessenger(id, token))
 .CreateLogger();
            log.Information("Connect to selinum??");
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
            log.Information("NOT Checking for updates (silver has a new idea that will be implemented soon)");

            //Check for updates
            //VersionInfo.Checkforupdates();
            log.Information("Starting MainAsync");

            //Start the main async task
            MainAsync().GetAwaiter().GetResult();
        }

        public static Config GetConfig()
        {
            return config;
        }

        public static void SendLog(string text)
        {
            log.Error(text);
        }

        public static void SetDatabase(ISBDatabase newdb)
        {
            Database = newdb;
        }

        public static ISBDatabase GetDatabase()
        {
            return Database;
        }

        public static void SendLog(string text, bool info)
        {
            if (info)
            {
                log.Information(text);
            }
            else
            {
                log.Error(text);
            }
        }

        public static void SendLog(Exception exception)
        {
            log.Error(exception: exception, "An exception occured");
        }

        private static DiscordClient discord;
        private static LavalinkNode audioService;
        private static Serilog.Core.Logger log;
        private static ISBDatabase Database;

        private static async Task MainAsync()
        {
            var logFactory = new LoggerFactory().AddSerilog();
            //Make us a little cute client
            log.Information("Creating the discord client");
            discord = new DiscordClient(new DiscordConfiguration()
            {
                LoggerFactory = logFactory,
                Token = config.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });
            //Tell our client to initialize interactivity
            log.Information("Initialising interactivity");
            discord.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30)
            });
            //set up logging?

            discord.MessageCreated += Discord_MessageCreated;

            //Tell our cute client to use commands or in other words become a working class member of society
            log.Information("Initialising Commands");
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = config.Prefix
            });
            //Register our commands
            log.Information("Regisitring Commands&Converters");
            commands.RegisterConverter(new SdImageConverter());
            commands.RegisterCommands<Genericcommands>();
            commands.RegisterCommands<Emotes>();
            commands.RegisterCommands<ModCommands>();
            commands.RegisterCommands<Giphy>();
            commands.RegisterCommands<ImageModule>();
            commands.RegisterCommands<AdminCommands>();
            commands.RegisterCommands<OwnerOnly>();
            commands.RegisterCommands<SteamCommands>();
            commands.RegisterCommands<Fortnite>();
            commands.RegisterCommands<Audio>();
            commands.RegisterCommands<MiscCommands>();
            commands.RegisterCommands<MinecraftModule>();
            commands.CommandErrored += Commands_CommandErrored;
            if (config.UseNodeJs)
            {
                commands.RegisterCommands<CalculatorCommands>();
            }
            if (config.UseAimlBot)
            {
                commands.RegisterCommands<AICommands>();
            }

            //Launch lavalink
            if (!File.Exists("Lavalink.jar"))
            {
                log.Information("Downloading lavalink");
                GitHubUtils.Repo repo = new("Frederikam", "Lavalink");
                var release = await GitHubUtils.Release.GetLatestFromRepoAsync(repo);
                await release.DownloadLatestAsync();
            }
            log.Information("Launching lavalink");
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
            log.Information("Waiting 6s");
            await Task.Delay(6000);
            log.Information("Making a node");
            audioService = new LavalinkNode(new LavalinkNodeOptions
            {
                RestUri = config.LavalinkRestUri,
                WebSocketUri = config.LavalinkWebSocketUri,
                Password = config.LavalinkPassword
            }, new DiscordClientWrapper(discord));
            //🥁🥁🥁 drumroll please
            log.Information("Connecting to discord");
            discord.Ready += Discord_Ready;
            await discord.ConnectAsync(new("console logs while booting up", ActivityType.Watching));
            if (!(config.FridayTextChannel == 0 || config.FridayVoiceChannel == 0))
            {
                waitforfriday = new Thread(new ThreadStart(WaitForFridayAsync));
            }
            //We have achived pog
            log.Information("Waiting 3s");
            await Task.Delay(3000);
            while (true)
            {
                log.Information("Updating the status to a random one");
                //update the status to some random one
                await discord.UpdateStatusAsync(await Splashes.GetSingleAsync(useinternal: config.UseSplashConfig));
                //wait the specified time
                log.Information($"Waiting {new TimeSpan(0, 0, 0, 0, config.MsInterval).Humanize(precision: 5)}");
                await Task.Delay(config.MsInterval);
                //repeat🔁
            }
        }

        private static Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            if (e.Exception.GetType() == typeof(DSharpPlus.CommandsNext.Exceptions.CommandNotFoundException))
            {
                return Task.CompletedTask;
            }
            SendLog(e.Exception);
            return Task.CompletedTask;
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
            var track = await audioService.GetTrackAsync(FridayUrl, SearchMode.YouTube);
            if (audioService.HasPlayer(vchannel.GuildId))
            {
                VoteLavalinkPlayer player = audioService.GetPlayer<VoteLavalinkPlayer>(vchannel.GuildId);
                await player.DisconnectAsync();
            }

            VoteLavalinkPlayer playere = await audioService.JoinAsync<VoteLavalinkPlayer>(vchannel.GuildId, vchannel.Id, true);

            await playere.PlayAsync(track);

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
            if (e.Author.IsBot)
            {
                return;
            }
            if (e.Message.Content == "anime")
            {
                await new DiscordMessageBuilder().WithReply(e.Message.Id)
                                             .WithContent("anime")
                                             .SendAsync(e.Channel);
            }
            if (e.Message.Content == "canada")
            {
                await new DiscordMessageBuilder().WithReply(e.Message.Id)
                                             .WithContent("canada")
                                             .SendAsync(e.Channel);
            }
        }
    }
}