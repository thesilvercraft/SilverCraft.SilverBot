/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Humanizer;
#if NoAudio
#else
using Lavalink4NET;
using Lavalink4NET.Artwork;
using Lavalink4NET.DSharpPlus;
using Lavalink4NET.Integrations.SponsorBlock;
using Lavalink4NET.Lyrics;
using Lavalink4NET.Tracking;
#endif
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;
using SilverBotDS.Converters;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.VoiceNext;
using SilverBot.Shared;
using SilverBot.Shared.Objects;
using SilverBot.Shared.Objects.Classes;
using SilverBot.Shared.Objects.Database;
using SilverBot.Shared.Objects.Database.Classes;
using SilverBot.Shared.Objects.Language;
using SilverBot.Shared.Pagination;
using SilverBot.Shared.Utils;
using SilverBotDS.Commands;
using SilverBotDS.ProgramExtensions;
using SilverCraft.SnowdPlayer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace SilverBotDS
{
    public record class ProgramHelper;

    public static class Program
    {
        public static Config _config;

        private static DiscordClient _discord;
#if NoAudio
#else
        private static LavalinkNode _audioService;
        private static InactivityTrackingService _trackingService;
#endif
        private static Logger _log;
        private static readonly HttpClient HttpClient = NewHttpClientWithUserAgent();


        private static readonly string[] MessagesToRepeat =
        {
            "anime", "canada", "e",
            "https://media.discordapp.net/attachments/811583810264629252/824266450818695168/image0-1.gif", "h",
            "gaming", "<:kalorichan:839099093552332850>", "kalorichan", "silver face"
        };

        private static readonly Dictionary<ulong, DateTime> XpLevelling = new();
        private static readonly TimeSpan MessageLimit = TimeSpan.FromMinutes(2);
        public static ServiceProvider ServiceProvider { get; private set; }


        /// <summary>
        /// EFCore Scaring measure
        /// </summary>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return null;
        }

        private static void Main(string[] args)
        {
            switch (args)
            {
                case ["generatelang", _]:
                {
                    if (!Directory.Exists(Path.GetDirectoryName(args[1])))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(args[1]) ?? string.Empty);
                    }

                    LanguageService.SerialiseDefault(args[1]);
                    Console.WriteLine("Serialized en.json");
                    Environment.Exit(70);
                    return;
                }
                case ["generatelang"]:
                    Console.WriteLine(LanguageService.SerialiseDefault());
                    Environment.Exit(70);
                    return;
            }

            if (Debugger.IsAttached && !(Environment.CurrentDirectory.EndsWith("bin\\Debug\\net7.0") ||
                                         Environment.CurrentDirectory.EndsWith("bin/Debug/net7.0")))
            {
                Environment.CurrentDirectory += Environment.OSVersion.Platform == PlatformID.Win32NT
                    ? "\\bin\\Debug\\net7.0"
                    : "/bin/Debug/net7.0";
            }

            MainAsync().GetAwaiter().GetResult();
        }


        public static void SendLog(Exception exception)
        {
            _log.Error(exception, "An exception occurred");
        }


        private static HttpClient NewHttpClientWithUserAgent()
        {
            HttpClient e = new();
            e.DefaultRequestHeaders.UserAgent.TryParseAdd("SilverBot");
            return e;
        }

        private static bool IsNotNullAndIsNotB(object? a, object? b)
        {
            if (a is string ea && b is string ba)
            {
                return !(string.IsNullOrEmpty(ea) || ea == ba);
            }

            return !(a is null || a == b);
        }


        public static bool AssetPresent(string asset)
        {
            return AssetSchemeCheckers.FirstOrDefault(x => asset.StartsWith(x.Scheme))?.CheckForAsset(asset) == true;
        }

        public static List<IAssetSchemeChecker> AssetSchemeCheckers =
            new() { new FileAssetSchemeChecker(),  };

        public static ServiceCollection services = new();

        private static Task SetupLogging()
        {
            var loggerConfiguration = new LoggerConfiguration();
            switch (_config.MinimumLogLevel)
            {
                case LogLevel.Trace:
                    loggerConfiguration = loggerConfiguration.MinimumLevel.Verbose();
                    break;
                case LogLevel.Debug:
                    loggerConfiguration = loggerConfiguration.MinimumLevel.Debug();
                    break;
                case LogLevel.Information:
                    loggerConfiguration = loggerConfiguration.MinimumLevel.Information();
                    break;
                case LogLevel.Warning:
                    loggerConfiguration = loggerConfiguration.MinimumLevel.Warning();
                    break;
                case LogLevel.Error:
                    loggerConfiguration = loggerConfiguration.MinimumLevel.Error();
                    break;
                case LogLevel.Critical:
                    loggerConfiguration = loggerConfiguration.MinimumLevel.Fatal();
                    break;
                case LogLevel.None:
                    break;
                default:
                    throw new NotImplementedException(nameof(_config.MinimumLogLevel));
            }

            loggerConfiguration.WriteTo.Console(theme: AnsiConsoleTheme.Code);
            if (_config.UseTxtFilesAsLogs)
            {
                loggerConfiguration.WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, shared: true);
            }

            loggerConfiguration.Enrich.FromLogContext();
            _log = loggerConfiguration.CreateLogger();
            Log.Logger = _log;
            return Task.CompletedTask;
        }

        private static Task InitializeInteractivity(ILogger mainlog)
        {
            using (mainlog.BeginScope("Initializing interactivity"))
            {
                _discord.UseInteractivity(new InteractivityConfiguration
                {
                    PollBehaviour = PollBehaviour.KeepEmojis,
                    Timeout = TimeSpan.FromSeconds(60),
                    PaginationButtons = new PaginationButtons()
                    {
                        Stop = new DiscordButtonComponent(ButtonStyle.Danger, "stop", null, false,
                            new DiscordComponentEmoji("⏹️")),
                        Left = new DiscordButtonComponent(ButtonStyle.Secondary, "left", null, false,
                            new DiscordComponentEmoji("◀️")),
                        Right = new DiscordButtonComponent(ButtonStyle.Secondary, "right", null, false,
                            new DiscordComponentEmoji("▶️")),
                        SkipLeft = new DiscordButtonComponent(ButtonStyle.Primary, "skipl", null, false,
                            new DiscordComponentEmoji("⏮️")),
                        SkipRight = new DiscordButtonComponent(ButtonStyle.Primary, "skipr", null, false,
                            new DiscordComponentEmoji("⏭️"))
                    },
                    ButtonBehavior = ButtonPaginationBehavior.Disable,
                    PaginationBehaviour = PaginationBehaviour.WrapAround
                });
                //set up XP and repeating things
                _discord.MessageCreated += Discord_MessageCreated;
            }

            return Task.CompletedTask;
        }

        private static ICallBack taskService = new TaskSchedulerService();
        private static Task SetupCommandServices()
        {
            services.AddSingleton<IAnalyse>(new ConsoleAnalytics());
            services.AddSingleton(new LanguageService());
            services.AddSingleton<IPaginator>(new CoolerPaginatior(_discord));
            services.AddSingleton<ICallBack>(taskService);
            services.AddSingleton(new ColourService());
            services.AddDbContext<DatabaseContext>(
                options =>
                {
                    options.UseLazyLoadingProxies().UseSqlite("Filename=./silverbotdatabasev3.db",
                        b => b.MigrationsAssembly("SilverBotDS"));
                    if (!Debugger.IsAttached)
                    {
                        return;
                    }

                    Console.WriteLine("LOGGING SENSITIVE DATA");
                    options.EnableSensitiveDataLogging();
                }, ServiceLifetime.Transient);

            services.AddSingleton(_config);
            services.AddSingleton(HttpClient);
            return Task.CompletedTask;
        }

        private static async Task SetupCommandLavalink(ILogger mainlog)
        {
#if NoAudio
#else
            using (mainlog.BeginScope("AutoStart Lavalink"))
            {
                if (!File.Exists("Lavalink.jar"))
                {
                    using (mainlog.BeginScope("Downloading lavalink"))
                    {
                        await (await GitHubUtils.Release.GetLatestFromRepoAsync(
                                new GitHubUtils.Repo(_config.LavalinkBuildsSourceGitHubUser,
                                    _config.LavalinkBuildsSourceGitHubRepo),
                                HttpClient))
                            .DownloadLatestAsync(HttpClient);
                    }
                }

                if (!Process.GetProcesses().Any(x => x.ProcessName.Contains("java")))
                {
                    using (mainlog.BeginScope("Launching lavalink"))
                    {
                        new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = _config.JavaLoc,
                                Arguments = "-jar Lavalink.jar",
                                UseShellExecute = true
                            }
                        }.Start();
                    }
                }
            }
#endif
        }

        private static Task SetupCommandsLavalinkService(ILogger mainlog)
        {
#if NoAudio
#else
            using (mainlog.BeginScope("Lavalink"))
            {
                mainlog.LogInformation("Creating lavalink wrapper");
                var discordClientWrapper = new DiscordClientWrapper(_discord);
                _audioService = new LavalinkNode(new LavalinkNodeOptions
                {
                    RestUri = _config.LavalinkRestUri,
                    WebSocketUri = _config.LavalinkWebSocketUri,
                    Password = _config.LavalinkPassword
                }, discordClientWrapper);
                _trackingService = new InactivityTrackingService(_audioService, discordClientWrapper,
                    new InactivityTrackingOptions());
                mainlog.LogDebug("Activating audio service");
                services.AddSingleton(_audioService);
                mainlog.LogDebug("Activating artwork service");
                var artworkService = new ArtworkService();
                services.AddSingleton(artworkService);
                mainlog.LogDebug("Activating MusicController service");
                services.AddSingleton(new MusicControllerService(_discord, artworkService,
                    services.BuildServiceProvider().GetRequiredService<LanguageService>()));
                services.AddSingleton<ISnowdPlayerService>(new SnowdPlayerService(HttpClient));

                if (_config.EnableJellyFinLookupService)
                {
                    services.AddSingleton<ITrackOrAlbumLookupService>(new JellyFinLookupService(HttpClient, _config));
                }

                if (!_config.SitInVc)
                {
                    mainlog.LogDebug("Activating tracking service");
                    services.AddSingleton(_trackingService);
                }

                mainlog.LogDebug("Activating lyrics service");
                services.AddSingleton(new LyricsService(new LyricsOptions { UserAgent = "SilverBot" }));
            }
#endif
            return Task.CompletedTask;
        }

        private static async Task SetupCommandsExternalServices(ILogger mainlog,
            ModuleRegistrationService moduleRegistrationService)
        {
            using (mainlog.BeginScope("External services"))
            {
                foreach (var group in _config.ServicesToLoadExternal.GroupBy(x => x.Key))
                {
                    if (File.Exists(group.Key))
                    {
                        var assembly = Assembly.LoadFrom(group.Key);
                        foreach (var module in group.Select(module => module.Value))
                        {
                            try
                            {
                                var t = assembly.GetType(module);
                                await moduleRegistrationService.ProcessExternalServiceType(t, services);
                            }
                            catch (Exception ex)
                            {
                                mainlog.LogError(ex, "Failed to load service {Module} Exception occured",
                                    module);
                            }
                        }
                    }
                    else
                    {
                        mainlog.LogInformation("Services from the {File} won't be loaded as its file doesn't exist",
                            group.Key);
                    }
                }
            }
        }

        private static async Task SetupCommandsInternal(ILogger mainlog,
            ModuleRegistrationService moduleRegistrationService, CommandsNextExtension commands,
            SlashCommandsExtension slash)
        {
            using (mainlog.BeginScope("Registering internal commands"))
            {
                foreach (var module in _config.ModulesToLoad)
                {
                    try
                    {
                        await moduleRegistrationService.ProcessModuleType(Type.GetType(module), _config, commands,
                            slash);
                    }
                    catch (Exception ex)
                    {
                        mainlog.LogError(ex, "Failed to load internal module {Module} Exception occured",
                            module);
                    }
                }

                if (_config.ModulesToLoad.Any(x => x.Contains("BingCommandModule")))
                {
                    _discord.ComponentInteractionCreated += async (o, e) =>
                        await BingCommandModule.HandleButton(o, e, ServiceProvider);
                }
            }
        }

        private static async Task SetupCommandsExternal(ILogger mainlog,
            ModuleRegistrationService moduleRegistrationService, CommandsNextExtension commands,
            SlashCommandsExtension slash)
        {
            using (mainlog.BeginScope("Registering external commands"))
            {
                foreach (var group in _config.ModulesFilesToLoadExternal)
                {
                    if (File.Exists(group))
                    {
                        var assembly = Assembly.LoadFrom(group);
                        foreach (var module in assembly.ExportedTypes.Where(x =>
                                     x.IsSubclassOf(typeof(ApplicationCommandModule)) ||
                                     x.IsSubclassOf(typeof(BaseCommandModule))))
                        {
                            try
                            {
                                await moduleRegistrationService.ProcessModuleType(module, _config, commands, slash);
                            }
                            catch (Exception ex)
                            {
                                mainlog.LogError(ex,
                                    "Failed to load external module {Module} Exception occured",
                                    module);
                            }
                        }
                    }
                    else
                    {
                        mainlog.LogWarning("Modules from the {File} won't be loaded as its file doesn't exist",
                            group);
                    }
                }
            }
        }

        private static async Task SetupCommands(ILogger mainlog, CancellationToken cancellationToken)
        {
            using (mainlog.BeginScope("Initializing Commands"))
            {
                await SetupCommandServices();
                if (_config.AutoDownloadAndStartLavalink)
                {
                    await SetupCommandLavalink(mainlog);
                }

                await Task.Delay(2000, cancellationToken);
                if (_config.UseLavaLink)
                {
                    await SetupCommandsLavalinkService(mainlog);
                }

                services.AddSingleton(_log);
                services.AddSingleton(_discord);
                ModuleRegistrationService moduleRegistrationService = new();
                services.AddSingleton(moduleRegistrationService);
                var commandErrorHandler = new CommandErrorHandler();
                services.AddSingleton(commandErrorHandler);
                var slashErrorHandler = new SlashErrorHandler();
                services.AddSingleton(slashErrorHandler);
                var birthdayWatcher = new BirthdayWatcher();
                services.AddSingleton(birthdayWatcher);


                await SetupCommandsExternalServices(mainlog, moduleRegistrationService);
#if NoAudio
#else
                if (_config.UseLavaLink)
                {
                    services.AddSingleton(
                        (NeutralAudio)ModuleRegistrationService.CreateInstance(typeof(NeutralAudio),
                            services.BuildServiceProvider()));
                }
#endif
                
                ServiceProvider = services.BuildServiceProvider();
                using (mainlog.BeginScope("Database migration"))
                {
                    var context = ServiceProvider.GetService<DatabaseContext>();
                    //Do stuff with the database making sure its up to date.
                    await context!.Database!.MigrateAsync(cancellationToken)!;
                }

                using (mainlog.BeginScope("Registering commands and converters"))
                {
                    var commands = _discord.UseCommandsNext(new CommandsNextConfiguration
                    {
                        Services = ServiceProvider,
                        PrefixResolver = ResolvePrefixAsync
                    });
                    var slash = _discord.UseSlashCommands(new SlashCommandsConfiguration()
                        { Services = ServiceProvider });

                    #region Registering Commands

                    using (mainlog.BeginScope("Registering converters"))
                    {
                        commands.SetHelpFormatter<CoolerHelpFormatter>();
                        commands.RegisterConverter(new SdImageConverter());
                        commands.RegisterConverter(new SColorConverter());
#if NoAudio
#else
                        commands.RegisterConverter(new LoopSettingsConverter());
                        commands.RegisterConverter(new SongOrSongsConverter());
#endif
                        //commands.RegisterConverter(new ImageFormatConverter());
                    }

                    await SetupCommandsInternal(mainlog, moduleRegistrationService, commands, slash);
                    await SetupCommandsExternal(mainlog, moduleRegistrationService, commands, slash);


                    commands.CommandExecuted += Commands_CommandExecuted;
                    await slashErrorHandler.Register(ServiceProvider, _log, slash);
                    await commandErrorHandler.Register(ServiceProvider, _log, commands);
                    await birthdayWatcher.Register(ServiceProvider, _log, _discord);
                    //await llama.Register(ServiceProvider, _log, _discord);

                    #endregion Registering Commands
                }
            }
        }

        private static async Task ConnectToLavaLink()
        {
#if NoAudio
#else
            await _discord.UpdateStatusAsync(
                new DiscordActivity("console logs while connecting to lavalink", ActivityType.Watching));

            try
            {
                await _audioService.InitializeAsync();
                if (!_config.SitInVc)
                {
                    _trackingService.BeginTracking();
                }

                if (_config.SponsorBlock)
                {
                    _audioService.UseSponsorBlock();
                    var sponsorBlock = _audioService.Integrations.Get<ISponsorBlockIntegration>();
                    sponsorBlock.DefaultSkipCategories = ImmutableArray.Create(
                        SegmentCategory.SelfPromotion,
                        SegmentCategory.Sponsor,
                        SegmentCategory.Intro,
                        SegmentCategory.Outro,
                        SegmentCategory.Filler,
                        SegmentCategory.Interaction,
                        SegmentCategory.Preview,
                        SegmentCategory.OfftopicMusic
                    );
                }
            }
            catch (WebSocketException e)
            {
                _log.Error(e, "Error occured while trying to connect to lavalink");
            }
#endif
        }

        private static async Task SplashTask()
        {
            _log.Verbose("Updating the status to a random one");
            await _discord.UpdateStatusAsync(_config.Splashes.RandomFrom()
                .GetDiscordActivity(GetStringDictionary(_discord, ServiceProvider)));
        }

        public static async Task MainAsync(bool exitAfterStartup = false,
            CancellationToken cancellationToken = default)
        {
            _config = await Config.GetAsync() ?? throw new InvalidOperationException();
            await SetupLogging();
            VersionInfo.SetRepoURL();
            if (_config.EnableUpdateChecking)
            {
                _log.Information("Checking for updates");
                await VersionInfo.Checkforupdates(HttpClient, _log);
            }

            var logFactory = new LoggerFactory().AddSerilog(_log);
            var mainlog = logFactory.CreateLogger<ProgramHelper>();
            using (mainlog.BeginScope("Creating discord client"))
            {
                _discord = new DiscordClient(new DiscordConfiguration
                {
                    LoggerFactory = logFactory,
                    Token = _config.Token,
                    Intents = DiscordIntents.All.RemoveIntent(DiscordIntents.GuildPresences)
                });
            }

            _discord.UseVoiceNext(new()
            {
                
            });
            await InitializeInteractivity(mainlog);
         
            await SetupCommands(mainlog, cancellationToken);
            _log.Information("Connecting to discord");
            var wait = new ManualResetEvent(false);
            _discord.SessionCreated += (_, _) =>
            {
                wait.Set();
                return Task.CompletedTask;
            };
            await _discord.ConnectAsync(new DiscordActivity("console logs while booting up", ActivityType.Watching));
            _log.Information("Waiting for client to connect");
            wait.WaitOne();
            await Task.Delay(2000, cancellationToken);
            
            if (_config.UseLavaLink)
            {
                await ConnectToLavaLink();
            }

            if (IsNotNullAndIsNotB(_config.FridayTextChannel, 0))
            {
                CancellationTokenSource s = new();
                if (DateTime.Now.DayOfWeek != DayOfWeek.Friday)
                {
                    if (ShouldCleanUpFriday())
                    {
                        _ = Task.Run(async () =>
                        {
                            await ExecuteFridayCleanupAsync(s.Token);
                            File.Delete(FridayFileName);
                        }, cancellationToken);
                    }
                    //Wait for friday
                    taskService.Add(async () => await FridayTaskAsync(s), DateTimeUtils.CalculateNextFriday(), "FridayTask", s );
                }
                else if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                {
                    //It is already friday
                    if (ShouldDoFriday())
                    {
                        taskService.Add(async () => await FridayTaskAsync(s), DateTime.Now, "FridayTask", s );
                    }
                }
                taskService.Add(async () =>
                {
                    await ExecuteFridayCleanupAsync(s.Token);
                    File.Delete(FridayFileName);
                    return DateTimeUtils.CalculateNextSaturday();
                }, DateTimeUtils.CalculateNextSaturday(), "CleanUpFridayTask", s );
            }

            if (_config.ChannelsToArchivePicturesFrom.Length > 0 && (_config.ArchiveWebhooks.Length > 1 ||
                                                                     (_config.ArchiveWebhooks.Length == 1 &&
                                                                      _config.ArchiveWebhooks[0] !=
                                                                      "https://discordapp.com/api/webhooks/id/key")))
            {
                var archiverExtension = new Archiver();
                await archiverExtension.Register(ServiceProvider, _log, _discord);
            }

            if (_config.ReactionRolesEnabled)
            {
                _discord.AddReactionRolesHandlers();
            }

            if (_config.EnableServerStatistics)
            {
                CancellationTokenSource s = new();
                taskService.Add(async () => await StatisticsMainAsync(s.Token), 0, "StatisticsTask", s );
            }
            EventsRunner eventsRunner=new();
            await eventsRunner.Register(ServiceProvider, _log);
            

            if (_config.HostWebsite)
            {
                await _discord.UpdateStatusAsync(new DiscordActivity("console logs while launching the website module",
                    ActivityType.Watching));
                _log.Information("Creating host");
            }

            await SplashTask();
            _log.Information("Booted up");
            Console.CancelKeyPress += delegate
            {
                Console.WriteLine("bye");
            };
            while (!exitAfterStartup && !cancellationToken.IsCancellationRequested)
            {
                _log.Verbose("Waiting {V}", new TimeSpan(0, 0, 0, 0, _config.MsInterval).Humanize(5));
                await Task.Delay(_config.MsInterval, cancellationToken);
                await SplashTask();
            }
        }

        private static async Task<DateTime> FridayTaskAsync(CancellationTokenSource s)
        {
            if (!ShouldDoFriday())
            {
                return DateTimeUtils.CalculateNextFriday();
            }
            await File.WriteAllTextAsync(FridayFileName, DateTime.Now.DayOfYear.ToString(), Encoding.UTF8, s.Token);
            await ExecuteFridayAsync(ct: s.Token);
            return DateTimeUtils.CalculateNextFriday();
        }


        private static Task<int> ResolvePrefixAsync(DiscordMessage msg)
        {
            var db = ServiceProvider.GetService<DatabaseContext>();
            if (db?.IsBanned(msg.Author.Id) == true)
            {
                return Task.FromResult(-1);
            }

            if (msg.Channel.Type == ChannelType.Private)
            {
                return Task.FromResult(0);
            }

            var gld = msg.Channel.Guild;
            if (gld is null)
            {
                _log.Warning("Guild {GuildId} was null on channel {ChannelId}",
                    msg.Channel.GuildId, msg.Channel.Id);
                return Task.FromResult(-1);
            }

            var serverSettings = db?.serverSettings.SingleOrDefault(x => x.ServerId == gld.Id);
            if (serverSettings != null)
            {
                foreach (var prefix in serverSettings.Prefixes)
                {
                    var prefixLocation = msg.GetStringPrefixLength(prefix, StringComparison.OrdinalIgnoreCase);
                    if (prefixLocation != -1)
                    {
                        return Task.FromResult(prefixLocation);
                    }
                }
            }

            foreach (var prefix in _config.Prefix)
            {
                var prefixLength = msg.GetStringPrefixLength(prefix, StringComparison.OrdinalIgnoreCase);
                if (prefixLength != -1)
                {
                    return Task.FromResult(prefixLength);
                }
            }

            return Task.FromResult(-1);
        }

        private static async Task Commands_CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e)
        {
            var analytics = ServiceProvider.GetService<IAnalyse>();
            if (analytics is not null)
            {
                await analytics.EmitEvent(e.Context.User, "CommandExecuted", new Dictionary<string, object>()
                {
                    {
                        "commandname",
                        (e.Command.Parent is not null ? e.Command.Parent.Name : string.Empty) + e.Command.Name
                    }
                });
            }
        }

        public static Dictionary<string, string> GetStringDictionary(DiscordClient client, ServiceProvider provider)
        {
            return new Dictionary<string, string>
            {
                ["GuildCount"] = client.Guilds.Values.LongCount().ToString(),
                ["Platform"] = RuntimeInformation.ProcessArchitecture.ToString(),
                ["Threads"] = Process.GetCurrentProcess().Threads.Count.ToString(),
                ["TaskCount"] = provider.GetService<ICallBack>()?.RunningTaskCount.ToString() ?? "unknown"
            };
        }

        public static async Task<double> StatisticsMainAsync(CancellationToken ct = default)
        {
          
                try
                {
                    await using var dbctx = ServiceProvider.GetRequiredService<DatabaseContext>();
                    var things = dbctx.GetStatisticSettings();
                    foreach (var (item1, item2, item3) in things)
                    {
                        try
                        {
                            var server = await _discord.GetGuildAsync(item1);
                            var dict = await ServerStatString.GetStringDictionaryAsync(server);
                            try
                            {
                                if (item2 == null)
                                {
                                    continue;
                                }

                                var category = server.Channels[(ulong)item2];
                                if (category.Type is ChannelType.Category)
                                {
                                    var e = 0;
                                    foreach (var child in category.Children)
                                    {
                                        if (item3.Length > e)
                                        {
                                            await child.ModifyAsync(a => a.Name = item3[e].Serialize(dict));
                                        }

                                        e++;
                                    }
                                }
                                else
                                {
                                    _log.Error(
                                        "Category type is {CategoryType} and not Category for channel {ChannelId}",
                                        category.Type, item2);
                                }
                            }
                            catch (Exception ex) when (ex.GetType() == typeof(NotFoundException) ||
                                                       ex.GetType() == typeof(UnauthorizedException))
                            {
                                try
                                {
                                    var dmChannel = await server.Owner.CreateDmChannelAsync();
                                    await dmChannel.SendMessageAsync(
                                        $"You will have to set up statistics again for `{server.Name}` as something broke. This might happen if any channel used for it was deleted, or if necessary permissions were denied.");
                                    dbctx.SetServerStatsCategory(item1, null);
                                    await dbctx.SaveChangesAsync(CancellationToken.None);
                                }
                                catch (UnauthorizedException)
                                {
                                    dbctx.SetServerStatsCategory(item1, null);
                                    await dbctx.SaveChangesAsync(CancellationToken.None);
                                }
                            }
                        }
                        catch (Exception ex) when (ex.GetType() == typeof(NotFoundException) ||
                                                   ex.GetType() == typeof(UnauthorizedException))
                        {
                            var serverSettings = dbctx.serverSettings.FirstOrDefault(x => x.ServerId == item1);
                            if (serverSettings != null)
                            {
                                dbctx.serverSettings.Remove(serverSettings);
                                await dbctx.SaveChangesAsync(CancellationToken.None);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.Error(e, "exception happened in stats thread");
                }

                return 1800000;
        }

        private const string FridayFileName = "_LastFriday.txt";

        public static bool ShouldDoFriday()
        {
            if (!File.Exists(FridayFileName))
            {
                return true;
            }

            var lastFridayFileText = File.ReadAllText(FridayFileName, Encoding.UTF8);
            if (int.TryParse(lastFridayFileText, out var x))
            {
                return x != DateTime.Now.DayOfYear;
            }

            return true;
        }

        public static bool ShouldCleanUpFriday()
        {
            if (!File.Exists(FridayFileName))
            {
                return false;
            }

            var lastFridayFileText = File.ReadAllText(FridayFileName, Encoding.UTF8);
            if (int.TryParse(lastFridayFileText, out var x))
            {
                return x == DateTime.Now.DayOfYear - 1;
            }

            return false;
        }

        

        public static async Task ExecuteFridayAsync(CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var channel = await _discord.GetChannelAsync(_config.FridayTextChannel);
            await channel.SendMessageAsync("It is Friday");
            await channel.AddOverwriteAsync(channel.Guild.EveryoneRole, Permissions.SendMessages, reason: "friday");
        }

        public static async Task ExecuteFridayCleanupAsync(CancellationToken ct = default)
        {
            var channel = await _discord.GetChannelAsync(_config.FridayTextChannel);
            await channel.SendMessageAsync("Friday ended :c");
            await channel.AddOverwriteAsync(channel.Guild.EveryoneRole, deny: Permissions.SendMessages,
                reason: "not friday");
        }

        private static async Task IncreaseXp(ulong id, ulong count = 1)
        {
            await using var context = ServiceProvider.GetService<DatabaseContext>();

            if (context != null)
            {
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
        }

        private static async Task DoXpLogic(MessageCreateEventArgs e)
        {
            if (XpLevelling.TryGetValue(e.Author.Id, out var value))
            {
                if (DateTime.UtcNow - value > MessageLimit)
                {
                    await IncreaseXp(e.Author.Id);
                }
            }
            else
            {
                XpLevelling.Add(e.Author.Id, DateTime.UtcNow);
                await IncreaseXp(e.Author.Id);
            }
        }

        private static async Task DoRepeatingLogic(MessageCreateEventArgs e)
        {
            await using var context = ServiceProvider.GetService<DatabaseContext>();
            if (context != null)
            {
                var o = e.Channel.IsPrivate
                    ? default
                    : await context.serverSettings.FirstOrDefaultAsync(x => x.ServerId == e.Guild.Id);
                if (MessagesToRepeat.Contains(e.Message.Content.ToLowerInvariant()) && (e.Channel.IsPrivate
                        || (o is not default(ServerSettings) && o.RepeatThings && e.Channel
                            .PermissionsFor(e.Guild.CurrentMember)
                            .HasPermission(Permissions.SendMessages)))
                   )
                {
                    if (_config.EmulateBubot)
                    {
                        switch (e.Message.Content)
                        {
                            case "kalorichan":
                            case "<:kalorichan:839099093552332850>":
                                await e.Message.RespondAsync("<:kalorichan:839099093552332850>");
                                return;

                            case "silver face":
                                await e.Message.RespondAsync("<:silverface:853297508632756234>");
                                return;
                        }
                    }

                    await new DiscordMessageBuilder().WithReply(e.Message.Id)
                        .WithContent(e.Message.Content)
                        .SendAsync(e.Channel);
                }
            }
        }

        private static async Task Discord_MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Author.IsBot)
            {
                //ha nice try
                return;
            }

            if (!e.Channel.IsPrivate)
            {
                await DoXpLogic(e);
            }

            await DoRepeatingLogic(e);
        }
    }
}