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
using Lavalink4NET.Lyrics;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using Lavalink4NET.Tracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SDBrowser;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using SilverBotDS.Commands;
using SilverBotDS.Commands.Gamering;
using SilverBotDS.Commands.SBCalendar;
using SilverBotDS.Converters;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SilverBotDS
{
    internal static class Program
    {
        public static readonly char DirSlash = Environment.OSVersion.Platform == PlatformID.Win32NT ? '\\' : '/';
        private static Config config;

        private static void Main()
        {
            //Start the main async task
            MainAsync().GetAwaiter().GetResult();
        }

        public static void SendLog(string text)
        {
            log.Error(text);
        }

        public static void SendLog(Exception exception)
        {
            log.Error(exception: exception, "An exception occurred");
        }

        public static Config GetConfig()
        {
            return serviceProvider.GetService<Config>();
        }

        private static DiscordClient discord;
        private static LavalinkNode audioService;
        private static InactivityTrackingService trackingService;
        private static Serilog.Core.Logger log;
        private static ServiceProvider serviceProvider;
        private static readonly HttpClient httpClient = NewhttpClientWithUserAgent();

        private static HttpClient NewhttpClientWithUserAgent()
        {
            HttpClient e = new();
            e.DefaultRequestHeaders.UserAgent.TryParseAdd("SilverBot");
            return e;
        }

        private static bool IsNotNullAndIsNotB(string a, string b)
        {
            return !(string.IsNullOrEmpty(a) || a == b);
        }

        private static async Task MainAsync()
        {
            config = await Config.GetAsync();
            WebHookUtilz.ParseWebhookUrl(config.LogWebhook, out ulong id, out string token);
            log = new LoggerConfiguration()
                         .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                         .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, shared: true)
                         .WriteTo.Discord(new DiscordWebhookMessenger(id, token))
                         .CreateLogger();

            log.Information("Checking for updates");
            //Check for updates
            await VersionInfo.Checkforupdates(httpClient, log);
            ILoggerFactory logFactory = new LoggerFactory().AddSerilog();
            //Make us a little cute client
            log.Verbose("Creating the discord client");
            discord = new DiscordClient(new DiscordConfiguration()
            {
                LoggerFactory = logFactory,
                Token = config.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });
            //Tell our client to initialize interactivity
            log.Verbose("Initializing interactivity");
            discord.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30)
            });
            //set up logging?
            discord.MessageCreated += Discord_MessageCreated;
            //Tell our cute client to use commands or in other words become a working class member of society
            log.Verbose("Initializing Commands");
            ServiceCollection services = new();
            //add a browser
            switch (config.BrowserType)
            {
                case 0:
                    {
                        log.Verbose("Not using a browser");
                        break;
                    }
                case 1:
                    {
                        log.Verbose("Launching chrome");
                        services.AddSingleton<IBrowser>(new SeleniumBrowser(Browsertype.Chrome, config.DriverLocation));
                        break;
                    }
                case 2:
                    {
                        log.Verbose("Launching firefox");
                        services.AddSingleton<IBrowser>(new SeleniumBrowser(Browsertype.Firefox, config.DriverLocation));
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
            switch (config.DatabaseType)
            {
                case 1:
                    {
                        PostgreDatabase postgre;
                        if (config != null && !string.IsNullOrEmpty(config.ConnString))
                        {
                            postgre = new(config.ConnString);
                        }
                        else
                        {
                            Uri tmp = new(Environment.GetEnvironmentVariable("DATABASE_URL") ?? throw new InvalidOperationException());
                            string[] usernameandpass = tmp.UserInfo.Split(":");
                            string connString = $"Host={tmp.Host};Username={usernameandpass[0]};Password={usernameandpass[1]};Database={HttpUtility.UrlDecode(tmp.AbsolutePath).Remove(0, 1)}";
                            postgre = new(connString);
                        }
                        log.Verbose("Using Postgre");
                        services.AddSingleton<ISBDatabase>(postgre);
                        break;
                    }
                case 2:
                    {
                        log.Verbose("Using LiteDB");
                        services.AddSingleton<ISBDatabase>(new LiteDBDatabase());
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
            services.AddSingleton(config);
            services.AddSingleton(httpClient);

            if (config.AutoDownloadAndStartLavalink)
            {
                //Launch lavalink
                if (!File.Exists("Lavalink.jar"))
                {
                    //file not found time to download lavalink
                    log.Information("Downloading lavalink");
                    GitHubUtils.Repo repo = new("freyacodes", "Lavalink");
                    GitHubUtils.Release release = await GitHubUtils.Release.GetLatestFromRepoAsync(repo, httpClient);
                    await release.DownloadLatestAsync(httpClient);
                }
                log.Information("Launching lavalink");
                new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = config.JavaLoc,
                        Arguments = " -jar Lavalink.jar",
                        UseShellExecute = true
                    }
                }.Start();
            }
            log.Verbose("Waiting 6s");
            await Task.Delay(6000);
            if (config.UseLavaLink)
            {
                log.Information("Making a lavalinknode");
                var discordclientwrapper = new DiscordClientWrapper(discord);
                audioService = new LavalinkNode(new LavalinkNodeOptions
                {
                    RestUri = config.LavalinkRestUri,
                    WebSocketUri = config.LavalinkWebSocketUri,
                    Password = config.LavalinkPassword
                }, discordclientwrapper);
                trackingService = new InactivityTrackingService(audioService, discordclientwrapper,
                                                                new InactivityTrackingOptions());
                services.AddSingleton(audioService);
                if (!config.SitInVc)
                {
                    services.AddSingleton(trackingService);
                }

                services.AddSingleton(new LyricsService(new LyricsOptions { UserAgent = "SilverBot" }));
            }
            if (IsNotNullAndIsNotB(config.SpotifyClientId, "Spotify_CLIENT_ID") && IsNotNullAndIsNotB(config.SpotifyClientSecret, "Spotify_CLIENT_SECRET"))
            {
                var sconfig = SpotifyClientConfig.CreateDefault();
                services.AddSingleton(new SpotifyClient(sconfig.WithToken((await new OAuthClient(sconfig).RequestToken(new ClientCredentialsRequest(config.SpotifyClientId, config.SpotifyClientSecret))).AccessToken)));
            }

            serviceProvider = services.BuildServiceProvider();
            CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = config.Prefix,
                Services = serviceProvider
            });
            //Register our commands
            log.Verbose("Registering Commands&Converters");
            commands.RegisterConverter(new SdImageConverter());
            commands.RegisterCommands<Genericcommands>();
            commands.RegisterCommands<Emotes>();
            commands.RegisterCommands<ModCommands>();
            commands.RegisterCommands<Giphy>();
            commands.RegisterCommands<ImageModule>();
            commands.RegisterCommands<AdminCommands>();
            if (config.AllowOwnerOnlyCommands)
            {
                commands.RegisterCommands<OwnerOnly>();
            }
            commands.RegisterCommands<SteamCommands>();
            if (IsNotNullAndIsNotB(config.FApiToken, "Fortnite_Token_Here"))
            {
                commands.RegisterCommands<Fortnite>();
            }
            if (config.EmulateBubot)
            {
                commands.RegisterCommands<Bubot>();
            }
            if (config.UseLavaLink)
            {
                commands.RegisterCommands<Audio>();
            }
            commands.RegisterCommands<MiscCommands>();
            commands.RegisterCommands<MinecraftModule>();
            if (IsNotNullAndIsNotB(config.MicrosoftGraphClientId, "Graph-Client-Id-Here"))
            {
                commands.RegisterCommands<CalendarCommands>();
            }
            commands.CommandErrored += Commands_CommandErrored;
            if (config.UseNodeJs)
            {
                commands.RegisterCommands<CalculatorCommands>();
            }
            if (config.AllowTrollCommands)
            {
                commands.RegisterCommands<NsfwCommands>();
            }
            //🥁🥁🥁 drum-roll
            log.Information("Connecting to discord");
            if (config.UseLavaLink)
            {
                discord.Ready += async (DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e) =>
                {
                    await audioService.InitializeAsync();
                    if (!config.SitInVc)
                    {
                        trackingService.BeginTracking();
                    }
                    if (!(config.FridayTextChannel == 0 || config.FridayVoiceChannel == 0) && config.UseLavaLink)
                    {
                        waitforfriday.Start();
                    }
                };
            }
            await discord.ConnectAsync(new("console logs while booting up", ActivityType.Watching));
            if (!(config.FridayTextChannel == 0 || config.FridayVoiceChannel == 0) && config.UseLavaLink)
            {
                waitforfriday = new Thread(new ThreadStart(WaitForFriday));
            }
            log.Verbose("Waiting 3s");
            await Task.Delay(3000);
            while (true)
            {
                log.Verbose("Updating the status to a random one");
                //update the status to some random one
                await discord.UpdateStatusAsync(await Splashes.GetSingleAsync(useinternal: !config.UseSplashConfig));
                //wait the specified time
                log.Verbose("Waiting {V}", new TimeSpan(0, 0, 0, 0, config.MsInterval).Humanize(precision: 5));
                await Task.Delay(config.MsInterval);
                //repeat🔁
            }
        }

        private static async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            if (e.Context.Channel.PermissionsFor(await e.Context.Guild.GetMemberAsync(sender.Client.CurrentUser.Id)).HasPermission(Permissions.SendMessages))
            {
                //inform user
                if (e.Exception is DSharpPlus.CommandsNext.Exceptions.CommandNotFoundException)
                {
                    return;
                }
                else if (e.Exception is DSharpPlus.CommandsNext.Exceptions.ChecksFailedException cfe)
                {
                    if (cfe.FailedChecks.Count is 1)
                    {
                        await new DiscordMessageBuilder().WithReply(e.Context.Message.Id)
                                 .WithContent($"Check `{cfe.FailedChecks[0].GetType()}` Failed")
                                 .SendAsync(e.Context.Channel);
                    }
                    else
                    {
                        var pages = new List<Page>();
                        var tempbuilder = new DiscordEmbedBuilder();
                        tempbuilder.WithTitle("Checks failed");
                        for (var i = 0; i < cfe.FailedChecks.Count; i++)
                        {
                            tempbuilder.WithFooter($"{ i + 1} / {cfe.FailedChecks.Count}");
                            tempbuilder.WithDescription(cfe.FailedChecks[i].GetType().Name);
                            pages.Add(new Page(embed: tempbuilder));
                        }

                        await e.Context.Channel.SendPaginatedMessageAsync(e.Context.Member, pages, timeoutoverride: new TimeSpan(0, 2, 0));
                    }
                }
                if (e.Exception is DSharpPlus.CommandsNext.Exceptions.InvalidOverloadException || e.Exception is ArgumentException a && a.Message == "Could not find a suitable overload for the command.")
                {
                    await new DiscordMessageBuilder().WithReply(e.Context.Message.Id)
                                .WithContent($"Invalid overload for `{e.Command.Name}`")
                                .SendAsync(e.Context.Channel);
                }
                if (e.Exception is InvalidOperationException ioe && ioe.Message == "No matching subcommands were found, and this group is not executable.")
                {
                    await new DiscordMessageBuilder().WithReply(e.Context.Message.Id)
                                .WithContent(ioe.Message)
                                .SendAsync(e.Context.Channel);
                }
            }

            SendLog(e.Exception);
        }

        private static Thread waitforfriday;
#pragma warning disable S1075 // URIs should not be hardcoded
        private const string FridayUrl = "https://youtu.be/akT0wxv9ON8";
#pragma warning restore S1075 // URIs should not be hardcoded
        private static int last_friday;

        public static void WaitForFriday()
        {
            Task.Run(WaitForFridayAsync);
        }

        public static async Task WaitForFridayAsync()
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
            var vchannel = await discord.GetChannelAsync(config.FridayVoiceChannel);
            if (audioService.HasPlayer(vchannel.GuildId))
            {
                VoteLavalinkPlayer player = audioService.GetPlayer<VoteLavalinkPlayer>(vchannel.GuildId);
                await player.DisconnectAsync();
            }
            VoteLavalinkPlayer playere = await audioService.JoinAsync<VoteLavalinkPlayer>(vchannel.GuildId, vchannel.Id, true);
            var track = await audioService.GetTrackAsync(FridayUrl, SearchMode.YouTube);
            await playere.PlayAsync(track);
            var channel = await discord.GetChannelAsync(config.FridayTextChannel);
            await channel.SendMessageAsync("It is Friday");
        }

        private static readonly string[] repeatstrings = { "anime", "canada", "fuck", "fock", "e", "https://media.discordapp.net/attachments/811583810264629252/824266450818695168/image0-1.gif", "h", "gaming", "quality fock", "fock you", "we will fock you" };

        private static async Task Discord_MessageCreated(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            if (e.Author.IsBot)
            {
                //haha nice try
                return;
            }
            if ((e.Channel.IsPrivate || e.Channel.PermissionsFor(await e.Guild.GetMemberAsync(sender.CurrentUser.Id)).HasPermission(Permissions.SendMessages)) && repeatstrings.Contains(e.Message.Content.ToLowerInvariant()))
            {
                if (config.EmulateBubot)
                {
                    switch (e.Message.Content)
                    {
                        case "fock":
                            await new DiscordMessageBuilder().WithReply(e.Message.Id)
                                     .WithContent(e.Message.Content)
                                     .WithFile("fock.mp3", Assembly.GetExecutingAssembly().GetManifestResourceStream($"SilverBotDS.Templates.fock.mp3") ?? throw new InvalidOperationException())
                                     .SendAsync(e.Channel);
                            return;

                        case "quality fock":
                            await new DiscordMessageBuilder().WithReply(e.Message.Id)
                             .WithContent(e.Message.Content)
                             .WithFile("quality_fock.mp3", Assembly.GetExecutingAssembly().GetManifestResourceStream($"SilverBotDS.Templates.quality_fock.mp3") ?? throw new InvalidOperationException())
                             .SendAsync(e.Channel);
                            return;

                        case "fock you":
                            await new DiscordMessageBuilder().WithReply(e.Message.Id)
                             .WithContent("fock you too")
                             .WithFile("fock.mp3", Assembly.GetExecutingAssembly().GetManifestResourceStream($"SilverBotDS.Templates.fock.mp3") ?? throw new InvalidOperationException())
                             .SendAsync(e.Channel);
                            return;

                        case "we will fock you":
                            await new DiscordMessageBuilder().WithReply(e.Message.Id)
                             .WithContent("https://www.youtube.com/watch?v=lLN3caSQI1w")
                             .SendAsync(e.Channel);
                            return;

                        default:
                            break;
                    }
                }
                await new DiscordMessageBuilder().WithReply(e.Message.Id)
                                             .WithContent(e.Message.Content)
                                             .SendAsync(e.Channel);
            }
        }
    }
}