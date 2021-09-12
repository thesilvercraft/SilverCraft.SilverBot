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
using Lavalink4NET.Rest;
using Lavalink4NET.Tracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SDBrowser;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using SilverBotDS.Commands;
using SilverBotDS.Commands.Gamering;
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
using System.Threading.Tasks;
using System.Web;
using SDiscordSink;
using SilverBotDS.Objects.Database.Classes;
using SilverBotDS.Objects.Classes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using DSharpPlus.CommandsNext.Exceptions;
using Serilog.Events;
using SixLabors.Fonts;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.VoiceNext;
using SnowdPlayer;
using Xabe.FFmpeg.Downloader;
using DSharpPlus.Exceptions;
using System.Text;
using CodenameGenerator;
using DSharpPlus.SlashCommands;
using SilverBotDS.Commands.Slash;
using DSharpPlus.CommandsNext.Attributes;
using SixLabors.ImageSharp;

namespace SilverBotDS
{
    internal static class Program
    {
        public static readonly char DirSlash = Environment.OSVersion.Platform == PlatformID.Win32NT ? '\\' : '/';
        private static Config config;
        private static IHost host;

        private static void Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "generatelang")
            {
                if (!Directory.Exists(Path.GetDirectoryName(args[1])))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(args[1]));
                }
                Language.SerialiseDefault(args[1]);
                Console.WriteLine("Serialized en.json");
                Environment.Exit(70);
                return;
            }
            if (Debugger.IsAttached && !Environment.CurrentDirectory.EndsWith("bin\\Debug\\net5.0"))
            {
                Environment.CurrentDirectory += Environment.OSVersion.Platform == PlatformID.Win32NT ? "\\bin\\Debug\\net5.0" : "/bin/Debug/net5.0";
            }
            MainAsync(args).GetAwaiter().GetResult();
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

        private static bool IsNotNullAndIsNotB(object a, object b)
        {
            if (a is string ea && b is string ba)
            {
                return !(string.IsNullOrEmpty(ea) || ea == ba);
            }
            return !(a is null || a == b);
        }

        private static readonly string[] requiredFonts = new[] { "Diavlo Light", "Arial", "Impact", "Trebuchet MS", "Times New Roman", "Futura Extra Black Condensed" };

        private static bool CheckIfAllFontsAreHere()
        {
            var familynames = SystemFonts.Families.Select(x => x.Name);
            foreach (var font in requiredFonts)
            {
                if (!familynames.Contains(font))
                {
                    return false;
                }
            }
            return true;
        }
        private static async Task MainAsync(string[] args)
        {
            config = await Config.GetAsync();
            WebHookUtils.ParseWebhookUrlNullable(config.LogWebhook, out ulong? id, out string token);
            var logfactory = new LoggerConfiguration();
            switch (config.MinimumLogLevel)
            {
                case LogLevel.Trace:
                    logfactory = logfactory.MinimumLevel.Verbose();
                    break;
                case LogLevel.Debug:
                    logfactory = logfactory.MinimumLevel.Debug();
                    break;
                case LogLevel.Information:
                    logfactory = logfactory.MinimumLevel.Information();
                    break;
                case LogLevel.Warning:
                    logfactory = logfactory.MinimumLevel.Warning();
                    break;
                case LogLevel.Error:
                    logfactory = logfactory.MinimumLevel.Error();
                    break;
                case LogLevel.Critical:
                    logfactory = logfactory.MinimumLevel.Fatal();
                    break;
            }
            logfactory
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) //So uh asp.net LOVES spamming logs
            .WriteTo.Console(theme: AnsiConsoleTheme.Code);
            if(config.UseTXTFilesAsLogs)
            {
                logfactory.WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, shared: true);
            }
            
            if (!(id == null || string.IsNullOrEmpty(token)))
            {
                logfactory.WriteTo.DiscordSink(new Tuple<ulong, string>((ulong)id, token));
            }
            log = logfactory.CreateLogger();
            if (config.EnableUpdateChecking)
            {
                log.Information("Checking for updates");
                await VersionInfo.Checkforupdates(httpClient, log);
            }
            ILoggerFactory logFactory = new LoggerFactory().AddSerilog(logger: log);
            log.Verbose("Creating the discord client");
            discord = new DiscordClient(new DiscordConfiguration
            {
                LoggerFactory = logFactory,
                Token = config.Token,
                Intents = DiscordIntents.All
            });
            log.Verbose("Initializing interactivity");
            discord.UseInteractivity(new InteractivityConfiguration
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(60),
                PaginationButtons = new PaginationButtons()
                {
                    Stop = new DiscordButtonComponent(ButtonStyle.Danger, "stop", null, false, new DiscordComponentEmoji("⏹️")),
                    Left = new DiscordButtonComponent(ButtonStyle.Secondary, "left", null, false, new DiscordComponentEmoji("◀️")),
                    Right = new DiscordButtonComponent(ButtonStyle.Secondary, "right", null, false, new DiscordComponentEmoji("▶️")),
                    SkipLeft = new DiscordButtonComponent(ButtonStyle.Primary, "skipl", null, false, new DiscordComponentEmoji("⏮️")),
                    SkipRight = new DiscordButtonComponent(ButtonStyle.Primary, "skipr", null, false, new DiscordComponentEmoji("⏭️"))
                },
                AckPaginationButtons = true,
                ButtonBehavior = ButtonPaginationBehavior.Disable
            });
            //set up XP and repeating things
            discord.MessageCreated += Discord_MessageCreated;
            log.Verbose("Initializing Commands");
            ServiceCollection services = new();
            #region Browser fun stuff
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
                        services.AddSingleton<IBrowser>(new SeleniumBrowser(Browsertype.Chrome, string.IsNullOrEmpty(config.DriverLocation) ? Environment.CurrentDirectory : config.DriverLocation));
                        break;
                    }
                case 2:
                    {
                        log.Verbose("Launching firefox");
                        services.AddSingleton<IBrowser>(new SeleniumBrowser(Browsertype.Firefox, string.IsNullOrEmpty(config.DriverLocation) ? Environment.CurrentDirectory : config.DriverLocation));
                        break;
                    }
                case 3:
                    {
                        log.Verbose("Using the remoteBrowser");
                        services.AddSingleton<IBrowser>(new RemoteBrowser(httpClient));
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException();
                    }
            }

            #endregion Browser fun stuff

            if (IsNotNullAndIsNotB(config.SegmentPrivateSource, "Segment_Key"))
            {
                services.AddSingleton<IAnalyse>(new SegmentIo(config.SegmentPrivateSource));
            }
            #region Database fun stuff
            switch (config.DatabaseType)
            {
                case 1:
                    {
                        if (config != null && !string.IsNullOrEmpty(config.ConnString))
                        {
                            services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(config.ConnString), ServiceLifetime.Transient);
                        }
                        else
                        {
                            Uri tmp = new(Environment.GetEnvironmentVariable("DATABASE_URL") ?? throw new InvalidOperationException());
                            string[] usernameandpass = tmp.UserInfo.Split(":");
                            services.AddDbContext<DatabaseContext>(options => options.UseNpgsql($"Host={tmp.Host};Username={usernameandpass[0]};Password={usernameandpass[1]};Database={HttpUtility.UrlDecode(tmp.AbsolutePath).Remove(0, 1)}"), ServiceLifetime.Transient);
                        }
                        break;
                    }
                case 2:
                    {
                        services.AddDbContext<DatabaseContext>(options => options.UseSqlite("Filename=./silverbotdatabasev2.db"), ServiceLifetime.Transient);
                        break;
                    }

                default:
                    break;
            }
            #endregion Database fun stuff
            services.AddSingleton(config);
            services.AddSingleton(httpClient);
            if (config.AutoDownloadAndStartLavalink)
            {
                //Launch lavalink
                if (!File.Exists("Lavalink.jar"))
                {
                    //file not found time to download lavalink
                    log.Information("Downloading lavalink");
                    await (await GitHubUtils.Release.GetLatestFromRepoAsync(new("freyacodes", "Lavalink"), httpClient)).DownloadLatestAsync(httpClient);
                }
                log.Information("Launching lavalink");
                new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = config.JavaLoc,
                        Arguments = "-jar Lavalink.jar",
                        UseShellExecute = true
                    }
                }.Start();
            }
            if (config.UseNewAudio)
            {
                services.AddSingleton(new SnowService(discord.UseVoiceNext(), false, true));
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
            services.AddSingleton(log);
            serviceProvider = services.BuildServiceProvider();
            var context = serviceProvider.GetService<DatabaseContext>();
            context.Database.Migrate();
            CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                Services = serviceProvider,
                PrefixResolver = ResolvePrefixAsync
            });
            var slash = discord.UseSlashCommands(new SlashCommandsConfiguration() { Services = serviceProvider });
            slash.SlashCommandErrored += Slash_SlashCommandErrored;
            #region Registering Commands
            commands.SetHelpFormatter<CustomHelpFormatter>();
            log.Verbose("Registering Commands&Converters");
            commands.RegisterConverter(new SdImageConverter());
            commands.RegisterConverter(new SColorConverter());
            commands.RegisterConverter(new LoopSettingsConverter());
            commands.RegisterConverter(new SongOrSongsConverter());
            commands.RegisterConverter(new TimeSpanConverter());
            commands.RegisterConverter(new IImageFormatConverter());
            if (!CheckIfAllFontsAreHere())
            {
                log.Fatal("You do not have all required fonts to run silverbot, on windows you have to install Diavlo Light and Futura Extra Black Condensed while on linux you have to install the base windows fonts (using \"sudo apt-get install ttf-mscorefonts-installer\"), Diavlo Light and Futura Extra Black Condensed. You might have to find all of the fonts in a TTF format. SilverBot will be running in a reduced feature mode where experience and Image related commands will not be enabled.");
            }
            else
            {
                commands.RegisterCommands<Experience>();
                commands.RegisterCommands<ImageModule>();
            }
            commands.RegisterCommands<Anime>();
            commands.RegisterCommands<Genericcommands>();
            commands.RegisterCommands<Emotes>();
            commands.RegisterCommands<ModCommands>();
            if (IsNotNullAndIsNotB(config.Gtoken, "Giphy_Token_Here"))
            {
                commands.RegisterCommands<Giphy>();
            }
            else
            {
                log.Information("You do not have a giphy token in the config, giphy related commands will be disabled.");
            }
            commands.RegisterCommands<AdminCommands>();
            if (config.UseNewAudio)
            {
                commands.RegisterCommands<NewAudio>();
            }
            if (config.AllowOwnerOnlyCommands)
            {
                commands.RegisterCommands<OwnerOnly>();
            }
            commands.RegisterCommands<SteamCommands>();
            if (IsNotNullAndIsNotB(config.FApiToken, "Fortnite_Token_Here"))
            {
                commands.RegisterCommands<Fortnite>();
            }
            else
            {
                log.Information("You do not have a fortnite api token in the config, fortnite related commands will be disabled.");
            }
            if (config.EmulateBubot)
            {
                commands.RegisterCommands<Bubot>();
            }
            if (config.EmulateBubotBibi)
            {
                commands.RegisterCommands<BibiLib>();
            }
            if (config.UseLavaLink)
            {
                commands.RegisterCommands<Audio>();
            }
            commands.RegisterCommands<MiscCommands>();
            commands.RegisterCommands<MinecraftModule>();
            commands.RegisterCommands<UserQuotesModule>();
            commands.CommandErrored += Commands_CommandErrored;
            commands.CommandExecuted += Commands_CommandExecuted;
            if (config.UseNodeJs)
            {
                commands.RegisterCommands<CalculatorCommands>();
            }
            if (config.EnableServerStatistics)
            {
                commands.RegisterCommands<ServerStatsCommands>();
            }
            #endregion
            if (config.UseSlashCommands)
            {
                slash.RegisterCommands<GeneralCommands>();
            }
            //🥁🥁🥁 drum-roll
            log.Information("Connecting to discord");
            bool isconnected = false;
            discord.Ready += (DiscordClient _, DSharpPlus.EventArgs.ReadyEventArgs __) =>
            {
                isconnected = true;
                return Task.CompletedTask;
            };
            await discord.ConnectAsync(new("console logs while booting up", ActivityType.Watching));
            log.Verbose("Waiting for client to connect");
            while (isconnected)
            {
                //intentional empty statement
            }
            await Task.Delay(2000);
            await discord.UpdateStatusAsync(new("console logs while connecting to lavalink", ActivityType.Watching));
            if (config.UseLavaLink)
            {
                await audioService.InitializeAsync();
                if (!config.SitInVc)
                {
                    trackingService.BeginTracking();
                }
                if (IsNotNullAndIsNotB(config.FridayTextChannel, 0) && IsNotNullAndIsNotB(config.FridayVoiceChannel, 0) && config.UseLavaLink)
                {
                    var waitforfriday = Task.Run(WaitForFridayAsync);
                }
            }
            await discord.UpdateStatusAsync(new("console logs while configuring server statistics", ActivityType.Watching));
            if (config.EnableServerStatistics)
            {
                var statisticsinstance = Task.Run(() => StatisticsMainAsync());
            }
            var eventsinstance = Task.Run(() => RunEventsAsync());
            await discord.UpdateStatusAsync(new("console logs while launching the website module", ActivityType.Watching));
            #region Website Fun Time
            host = Host.CreateDefaultBuilder(args).ConfigureServices(s =>
            {
                s.AddSingleton(config);
                s.AddSingleton(discord);
                if (config.UseLavaLink)
                {
                    s.AddSingleton(audioService);
                }
                switch (config.DatabaseType)
                {
                    case 1:
                        {
                            if (config != null && !string.IsNullOrEmpty(config.ConnString))
                            {
                                s.AddDbContext<DatabaseContext>(options => options.UseNpgsql(config.ConnString), ServiceLifetime.Transient);
                            }
                            else
                            {
                                Uri tmp = new(Environment.GetEnvironmentVariable("DATABASE_URL") ?? throw new InvalidOperationException());
                                string[] usernameandpass = tmp.UserInfo.Split(":");
                                s.AddDbContext<DatabaseContext>(options => options.UseNpgsql($"Host={tmp.Host};Username={usernameandpass[0]};Password={usernameandpass[1]};Database={HttpUtility.UrlDecode(tmp.AbsolutePath).Remove(0, 1)}"), ServiceLifetime.Transient);
                            }
                            break;
                        }
                    case 2:
                        {
                            s.AddDbContext<DatabaseContext>(options => options.UseSqlite("Filename=./silverbotdatabasev2.db"), ServiceLifetime.Transient);
                            break;
                        }

                    default:
                        break;
                }
            })
             .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<WebpageStartup>()).UseSerilog(log).Build();
            #endregion Website Fun Time
            var hostwebsite = Task.Run(async () => await host.RunAsync());
            while (true)
            {
                log.Verbose("Updating the status to a random one");
                //update the status to some random one
                await discord.UpdateStatusAsync(ArrayUtils.RandomFromArray(config.Splashes).GetDiscordActivity(GetStringDictionary(discord)));
                //wait the specified time
                log.Verbose("Waiting {V}", new TimeSpan(0, 0, 0, 0, config.MsInterval).Humanize(precision: 5));
                await Task.Delay(config.MsInterval);
                //repeat🔁
            }
        }

        
        private static Task<int> ResolvePrefixAsync(DiscordMessage msg)
        {
            if (msg.Channel.Type == ChannelType.Private)
            {
                return Task.FromResult(0);
            }
            var db = serviceProvider.GetService<DatabaseContext>();
            if (db.IsBanned(msg.Author.Id))
            {
                return Task.FromResult(-1);
            }
            var gld = msg.Channel.Guild;
            if (gld == null)
            {
                log.Warning("Guild {GuildId} was null on channel {ChannelId}.",
                    msg.Channel.GuildId, msg.Channel.Id);
                return Task.FromResult(-1);
            }
            var srvrsttings = db.serverSettings.SingleOrDefault(x => x.ServerId == gld.Id);
            if (srvrsttings != null)
            {
                foreach (var pfix in srvrsttings.Prefixes)
                {
                    var pfixLocation = msg.GetStringPrefixLength(pfix, StringComparison.OrdinalIgnoreCase);
                    if (pfixLocation != -1)
                    {
                        return Task.FromResult(pfixLocation);
                    }
                }
            }
            foreach (var pfix in config.Prefix)
            {
                var pfixLocation = msg.GetStringPrefixLength(pfix, StringComparison.OrdinalIgnoreCase);
                if (pfixLocation != -1)
                    return Task.FromResult(pfixLocation);
            }
            return Task.FromResult(-1);
        }

        private static async Task Commands_CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e)
        {
            var analytics = serviceProvider.GetService<IAnalyse>();
            if (analytics is not null)
            {
                await analytics.EmitEvent(e.Context.User, "CommandExecuted", new Dictionary<string, object>()
               {
                   { "commandname", (e.Command.Parent is not null ? e.Command.Parent.Name : string.Empty) + e.Command.Name },
               });
            }
        }
        private static async Task Slash_SlashCommandErrored(SlashCommandsExtension sender, DSharpPlus.SlashCommands.EventArgs.SlashCommandErrorEventArgs e)
        {

        }

        public static Dictionary<string, string> GetStringDictionary(DiscordClient client)
        {
            return new Dictionary<string, string> { ["GuildCount"] = client.Guilds.Values.LongCount().ToString(), ["Platform"] = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString() };
        }
        private static string RemoveStringFromEnd(string a, string sub)
        {
            if (a.EndsWith(sub))
            {
                a = a.Substring(0, a.LastIndexOf(sub));
            }
            return a;
        }
        /// <summary>
        /// Render the error message for an Attribute
        /// </summary>
        /// <param name="checkBase">The attribute</param>
        /// <param name="lang">The language</param>
        /// <param name="isinguild">Was the command executed in a guild or in dm's</param>
        /// <returns>A <see cref="string"/> containing the error message</returns>
        static string RenderErrorMessageForAttribute(CheckBaseAttribute checkBase, Language lang, bool isinguild)
        {
            var chkbstype = checkBase.GetType();
            if(chkbstype == typeof(RequireDJAttribute))
            {
                return lang.RequireDJCheckFailed;
            }
            else if(chkbstype == typeof(RequireGuildAttribute))
            {
                return lang.RequireGuildCheckFailed;
            }
            else if(chkbstype == typeof(RequireNsfwAttribute))
            {
                return lang.RequireNsfwCheckFailed;
            }
            else if(chkbstype == typeof(RequireOwnerAttribute))
            {
                return lang.RequireOwnerCheckFailed;
            }
            else if(checkBase is RequireRolesAttribute rolesatt)
            {
                if(rolesatt.RoleNames.Count==1)
                {
                    return string.Format(lang.RequireRolesCheckFailedSG, rolesatt.RoleNames[0]);
                }
                else
                {
                    return string.Format(lang.RequireRolesCheckFailedPL, rolesatt.RoleNames.Humanize());
                }
            }
            else if(checkBase is RequireBotPermissionsAttribute requireBotPermissions)
            {
               if(!(requireBotPermissions.IgnoreDms && isinguild))
                {
                    return lang.RequireGuildCheckFailed;
                }
                return string.Format(lang.RequireBotPermisionsCheckFailedPL, requireBotPermissions.Permissions.Humanize());
            }
            else if(checkBase is RequireUserPermissionsAttribute userPermissions)
            {
                if(!(userPermissions.IgnoreDms && isinguild))
                {
                    return lang.RequireGuildCheckFailed;
                }
                return string.Format(lang.RequireUserPermisionsCheckFailedPL, userPermissions.Permissions.Humanize());
            }
            else if (checkBase is RequirePermissionsAttribute userAndBotPermissions)
            {
                if (!(userAndBotPermissions.IgnoreDms && isinguild))
                {
                    return lang.RequireGuildCheckFailed;
                }
                return string.Format(lang.RequireUserPermisionsCheckFailedPL, userAndBotPermissions.Permissions.Humanize());
            }
            return string.Format(lang.CheckFailed, RemoveStringFromEnd(chkbstype.Name, "Attribute").Humanize());
        }
        private static async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            if (e.Context.Channel.IsPrivate || e.Context.Channel.PermissionsFor(await e.Context.Guild.GetMemberAsync(sender.Client.CurrentUser.Id)).HasPermission(Permissions.SendMessages))
            {
                if (e.Exception is CommandNotFoundException)
                {
                    //we do not do anything if it is a nonexistent command, i would have liked it to be a user only visible message but discord is shit
                    return;
                }
                else
                {
                    var lang = await Language.GetLanguageFromCtxAsync(e.Context);
                    if (e.Exception is ChecksFailedException cfe)
                    {
                        if (cfe.FailedChecks.Count is 1)
                        {
                            
                            await new DiscordMessageBuilder()
                                                             .WithReply(e.Context.Message.Id)
                                                             .WithContent(RenderErrorMessageForAttribute(cfe.FailedChecks[0], lang, e.Context.Guild != null))
                                                             .SendAsync(e.Context.Channel);
                        }
                        else
                        {
                            var pages = new List<Page>();
                            var tempbuilder = new DiscordEmbedBuilder().WithTitle(lang.ChecksFailed);
                            for (var i = 0; i < cfe.FailedChecks.Count; i++)
                            {
                                pages.Add(new Page(embed: tempbuilder.WithFooter($"{i + 1} / {cfe.FailedChecks.Count}").WithDescription(RenderErrorMessageForAttribute(cfe.FailedChecks[i], lang, e.Context.Guild != null))));
                            }
                            var interactivity = e.Context.Client.GetInteractivity();
                            await interactivity.SendPaginatedMessageAsync(e.Context.Channel, e.Context.User, pages, token: new System.Threading.CancellationToken());
                        }
                    }
                    else if (e.Exception is InvalidOverloadException || (e.Exception is ArgumentException a && a.Message == "Could not find a suitable overload for the command."))
                    {
                        await new DiscordMessageBuilder()
                                                         .WithReply(e.Context.Message.Id)
                                                         .WithContent(string.Format(lang.InvalidOverload, e.Command.Name))
                                                         .SendAsync(e.Context.Channel);
                    }
                    else if (e.Exception is InvalidOperationException ioe && ioe.Message == "No matching subcommands were found, and this group is not executable.")
                    {
                        await new DiscordMessageBuilder()
                                                         .WithReply(e.Context.Message.Id)
                                                         .WithContent(lang.NoMatchingSubcommandsAndGroupNotExecutable)
                                                         .SendAsync(e.Context.Channel);
                    }
                    else if(e.Exception is UnknownImageFormatException)
                    {
                        await new DiscordMessageBuilder()
                                                         .WithReply(e.Context.Message.Id)
                                                         .WithContent(lang.UnknownImageFormat)
                                                         .SendAsync(e.Context.Channel);
                    }
                    else
                    {
                        await new DiscordMessageBuilder()
                                                         .WithReply(e.Context.Message.Id)
                                                         .WithContent(lang.GeneralException)
                                                         .SendAsync(e.Context.Channel);
                    }
                }
            }
            log.Error(exception: e.Exception, $"Error `{e.Exception.GetType().FullName}` encountered.\nGuild `{e.Context.Guild?.Id.ToString() ?? "None"}`, channel `{e.Context.Channel?.Id.ToString() ?? "None"}`, user `{ e.Context.User?.Id.ToString() ?? "None"}`\n```\n{e.Context.Message.Content}\n```");
        }

        private static int last_friday;
        public static async Task RunEmojiEvent(PlannedEvent @event)
        {
            if (@event.Type != PlannedEventType.EmojiPoll)
            {
                throw new ArgumentException("The parameter @event needs to be an EmojiPoll", nameof(@event));
            }
            var channel = await discord.GetChannelAsync(@event.ChannelID);
            var msg = await channel.GetMessageAsync((ulong)@event.ResponseMessageID);
            var bob = new DiscordEmbedBuilder(msg.Embeds[0]);
            var yesVotes = (await msg.GetReactionsAsync(DiscordEmoji.FromName(discord, ":everybodyvotes:"))).Count(x => x.Id != discord.CurrentUser.Id && !x.IsBot);
            var noVotes = (await msg.GetReactionsAsync(DiscordEmoji.FromName(discord, ":nobodyvotes:"))).Count(x => x.Id != discord.CurrentUser.Id && !x.IsBot);
            var pollResultText = new StringBuilder();
            pollResultText.Append("Poll result: **");
            if (yesVotes > noVotes)
            {
                pollResultText.Append("Yes");
            }
            else if (yesVotes == noVotes)
            {
                pollResultText.Append("Undecided");
            }
            else
            {
                pollResultText.Append("No");
            }
            pollResultText.Append("**\nYes:").Append(yesVotes).Append(" No:").Append(noVotes).Append(" Undecided: ").Append(channel.Guild.Members.Count(x => !x.Value.IsBot) - (yesVotes + noVotes));
            bob.WithDescription(pollResultText.ToString());
            await msg.ModifyAsync(bob.Build());
            @event.Handled = true;
        }
        public static async Task RunGiveAwayEvent(PlannedEvent @event)
        {
            if (@event.Type != PlannedEventType.GiveAway)
            {
                throw new ArgumentException("The parameter evnt needs to be an GiveAway", nameof(@event));
            }
            var channel = await discord.GetChannelAsync(@event.ChannelID);
            var msg = await channel.GetMessageAsync((ulong)@event.ResponseMessageID);
            var bob = new DiscordEmbedBuilder(msg.Embeds[0]);
            var people = (await msg.GetReactionsAsync(DiscordEmoji.FromName(discord, ":everybodyvotes:"))).Where(x => x.Id != discord.CurrentUser.Id && !x.IsBot);
            using var random = new RandomGenerator();
            if (!people.Any())
            {
                await channel.SendMessageAsync("Nobody reacted in time :(");
            }
            else
            {
                await channel.SendMessageAsync($"{people.ElementAt(random.Next(0, people.Count())).Mention} won {msg.Embeds[0].Title}");
            }
            @event.Handled = true;
        }
        public static async Task RunEventsAsync()
        {
            var dbctx = serviceProvider.GetRequiredService<DatabaseContext>();
            while (true)
            {
                try
                {
                    var arr = dbctx.plannedEvents.ToArray();
                    for (int a = 0; a < arr.Length; a++)
                    {
                        var evnt = arr[a];
                        if (evnt != null)
                        {
                            if (!evnt.Handled)
                            {
                                if (evnt.Time <= DateTime.Now)
                                {
                                    try
                                    {
                                        switch (evnt.Type)
                                        {
                                            case PlannedEventType.EmojiPoll:
                                                var emjiev = Task.Run(() => RunEmojiEvent(evnt));
                                                break;
                                            case PlannedEventType.GiveAway:
                                                var gvev = Task.Run(() => RunGiveAwayEvent(evnt));
                                                break;
                                            case PlannedEventType.Reminder:
                                                throw new NotImplementedException();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        log.Error(e, "exception happened in events thread in the switch case");
                                    }
                                }
                            }
                            else
                            {
                                log.Verbose($"removed an {evnt.Type}");
                                dbctx.plannedEvents.Remove(evnt);
                                await dbctx.SaveChangesAsync();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error(e, "exception happened in events thread");
                }
                await Task.Delay(3000);
            }
        }
        public static async Task StatisticsMainAsync()
        {
            while (true)
            {
                try
                {
                    log.Debug("Entered statistics method");
                    var dbctx = serviceProvider.GetRequiredService<DatabaseContext>();
                    log.Debug("Getting the settings about statistics");
                    var things = dbctx.GetStatisticSettings();
                    log.Debug("Got the settings about statistics");
                    foreach (var thing in things)
                    {
                        log.Debug("Getting the guild with the id {Id}", thing.Item1);
                        try
                        {
                            var server = await discord.GetGuildAsync(thing.Item1);
                            log.Debug("Got the guild with the id {Id}", thing.Item1);
                            log.Debug("Getting the channel with the id {Id}", thing.Item2);
                            try
                            {
                                var category = server.Channels[(ulong)thing.Item2];
                                log.Debug("Got the channel with the id {Id}", thing.Item2);
                                if (category.Type is ChannelType.Category)
                                {
                                    int e = 0;
                                    log.Debug("Getting the children of the channel {Id}", thing.Item2);
                                    foreach (var child in category.Children)
                                    {
                                        if (thing.Item3.Length > e)
                                        {
                                            log.Debug("Updating {Id}", child.Id);
                                            await child.ModifyAsync(a => a.Name = thing.Item3[e].Serialize(server));
                                        }
                                        e++;
                                    }
                                }
                                else
                                {
                                    log.Error("Category type is {CategoryType} and not Category for channel {ChannelId}", category.Type, thing.Item2);
                                }
                            }
                            catch (Exception ex) when (ex.GetType() == typeof(NotFoundException) || ex.GetType() == typeof(UnauthorizedException))
                            {
                                try
                                {
                                    var dmchannel = await server.Owner.CreateDmChannelAsync();
                                    await dmchannel.SendMessageAsync($"Hello SilverBot here,\n it appears that you own `{server.Name}` and i just wanted to let you know that you will have to set the stats category again for stats to work as something broke.");
                                    dbctx.SetServerStatsCategory(thing.Item1, null);
                                    await dbctx.SaveChangesAsync();
                                }
                                catch (UnauthorizedException)
                                {
                                    dbctx.SetServerStatsCategory(thing.Item1, null);
                                    await server.LeaveAsync();
                                    await dbctx.SaveChangesAsync();
                                }
                            }
                        }
                        catch (Exception ex) when (ex.GetType() == typeof(NotFoundException) || ex.GetType() == typeof(UnauthorizedException))
                        {
                            var serversettings = dbctx.serverSettings.FirstOrDefault(x => x.ServerId == thing.Item1);
                            dbctx.serverSettings.Remove(serversettings);
                            await dbctx.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error(e, "exception happened in stats thread");
                }
                await Task.Delay(1800000);
            }
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
                else
                {
                    if (last_friday == DateTime.Now.DayOfYear - 1)
                    {
                        await ExecuteFridayAsync(false);
                        last_friday = 0;
                    }
                }
                await Task.Delay(1000);
            }
        }

        public static async Task ExecuteFridayAsync(bool friday = true)
        {
            if (friday)
            {
                var channel = await discord.GetChannelAsync(config.FridayTextChannel);
                await channel.SendMessageAsync("It is Friday");
                await channel.AddOverwriteAsync(channel.Guild.EveryoneRole, Permissions.SendMessages, reason: "friday");
            }
            else
            {
                var channel = await discord.GetChannelAsync(config.FridayTextChannel);
                await channel.SendMessageAsync("Friday ended :c");
                await channel.AddOverwriteAsync(channel.Guild.EveryoneRole, deny: Permissions.SendMessages, reason: "not friday");
            }
        }

        private static readonly string[] repeatstrings = { "anime", "canada", "fuck", "e", "https://media.discordapp.net/attachments/811583810264629252/824266450818695168/image0-1.gif", "h", "gaming", "<:kalorichan:839099093552332850>", "kalorichan" };
        private static readonly Dictionary<ulong, DateTime> levellimit = new();
        private static readonly TimeSpan MessageLimit = TimeSpan.FromMinutes(2);

        private static async Task IncreaseXP(ulong id, ulong count = 1)
        {
            var context = serviceProvider.GetService<DatabaseContext>();

            var o = await context.userExperiences.FirstOrDefaultAsync(x => x.Id == id);
            if (o is not null)
            {
                o.Increase(count);
                context.userExperiences.Update(o);
                await context.SaveChangesAsync();
            }
            else
            {
                o = new UserExperience { Id = id, XP = 1 };
                context.userExperiences.Add(o);
                await context.SaveChangesAsync();
            }
        }
        private static async Task Discord_MessageCreated(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            if (e.Author.IsBot)
            {
                //ha nice try
                return;
            }
            if (!e.Channel.IsPrivate)
            {
                if (levellimit.ContainsKey(e.Author.Id))
                {
                    var prev = levellimit[e.Author.Id];
                    if ((DateTime.UtcNow - prev) > MessageLimit)
                    {
                        await IncreaseXP(e.Author.Id);
                    }
                }
                else
                {
                    levellimit.Add(e.Author.Id, DateTime.UtcNow);
                    await IncreaseXP(e.Author.Id);
                }
            }
            var context = serviceProvider.GetService<DatabaseContext>();

            var o = e.Channel.IsPrivate ? default : await context.serverSettings.FirstOrDefaultAsync(x => x.ServerId == e.Guild.Id);
            if ((e.Channel.IsPrivate
                || (o is not default(ServerSettings) && o.RepeatThings && e.Channel.PermissionsFor(await e.Guild.GetMemberAsync(sender.CurrentUser.Id)).HasPermission(Permissions.SendMessages))) && repeatstrings.Contains(e.Message.Content.ToLowerInvariant()))
            {
                if (config.EmulateBubot)
                {
                    switch (e.Message.Content)
                    {
                        case "kalorichan":
                        case "<:kalorichan:839099093552332850>":
                            await e.Message.RespondAsync("<:kalorichan:839099093552332850>");
                            return;
                    }
                }
                await new DiscordMessageBuilder().WithReply(e.Message.Id)
                                             .WithContent(e.Message.Content)
                                             .SendAsync(e.Channel);
            }
        }
    }
}