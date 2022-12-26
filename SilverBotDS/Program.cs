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
using ImageMagick;
using Lavalink4NET;
using Lavalink4NET.Artwork;
using Lavalink4NET.DSharpPlus;
using Lavalink4NET.Integrations.SponsorBlock;
using Lavalink4NET.Lyrics;
using Lavalink4NET.Tracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;
using SilverBotDS.Converters;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SilverBotDS.Objects.Database.Classes;
using SilverBotDS.Utils;
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
using SilverBot.Shared;

namespace SilverBotDS
{
    public record class ProgramHelper;

    public static class Program
    {
        public static Config _config;

        private static DiscordClient _discord;
        private static LavalinkNode _audioService;
        private static InactivityTrackingService _trackingService;
        private static Logger _log;
        private static readonly HttpClient HttpClient = NewHttpClientWithUserAgent();

        private static int _lastFriday;

        private static readonly string[] MessagesToRepeat =
        {
            "anime", "canada", "e",
            "https://media.discordapp.net/attachments/811583810264629252/824266450818695168/image0-1.gif", "h",
            "gaming", "<:kalorichan:839099093552332850>", "kalorichan", "silver face"
        };

        private static readonly Dictionary<ulong, DateTime> XpLevelling = new();
        private static readonly TimeSpan MessageLimit = TimeSpan.FromMinutes(2);
        public static ServiceProvider ServiceProvider { get; private set; }
        private static readonly Dictionary<string, Tuple<Task, CancellationTokenSource>> RunningTasks = new();
        private static readonly Dictionary<Guid, Tuple<Task, CancellationTokenSource>> RunningTasksOfSecondRow = new();

        public static Task RunningTasksOfSecondRowAdd(Guid a, Tuple<Task, CancellationTokenSource> b)
        {
            RunningTasksOfSecondRow.Add(a, b);
            return Task.CompletedTask;
        }

        public static Task RunningTasksAdd(string a, Tuple<Task, CancellationTokenSource> b)
        {
            RunningTasks.Add(a, b);
            return Task.CompletedTask;
        }

        /// <summary>
        /// EFCore Scaring mesure
        /// </summary>
        public static IHostBuilder CreateHostBuilder(string[] args)
            => null;

        private static void Main(string[] args)
        {
            if (args is ["generatelang", _])
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

            if (Debugger.IsAttached && !(Environment.CurrentDirectory.EndsWith("bin\\Debug\\net7.0") ||
                                         Environment.CurrentDirectory.EndsWith("bin/Debug/net7.0")))
            {
                Environment.CurrentDirectory += Environment.OSVersion.Platform == PlatformID.Win32NT
                    ? "\\bin\\Debug\\net7.0"
                    : "/bin/Debug/net7.0";
            }

            MainAsync(args).GetAwaiter().GetResult();
        }

        public static void CancelTasks()
        {
            foreach (var task in RunningTasksOfSecondRow)
            {
                task.Value.Item2.Cancel();
            }

            foreach (var task in RunningTasks)
            {
                task.Value.Item2.Cancel();
            }
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

        public static IEnumerable<string> GetMissingAssets(string[] required)
        {
            return required.Where(x => !AssetPresent(x));
        }

        public static bool AssetPresent(string asset) =>
            AssetSchemeCheckers.FirstOrDefault(x => asset.StartsWith(x.Scheme))?.CheckForAsset(asset) == true;

        public static List<IAssetSchemeChecker> AssetSchemeCheckers =
            new() { new FileAssetSchemeChecker(), new FontAssetSchemeChecker() };

        public static ServiceCollection services = new();

        public static async Task MainAsync(string[] args, bool ExitAfterbootup = false,
            CancellationToken cancellationToken = default)
        {
            _config = await Config.GetAsync() ?? throw new InvalidOperationException();
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
            if (_config.EnableUpdateChecking)
            {
                _log.Information("Checking for updates");
                await VersionInfo.Checkforupdates(HttpClient, _log);
            }

            var logFactory = new LoggerFactory().AddSerilog(logger: _log);
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
                    AckPaginationButtons = true,
                    ButtonBehavior = ButtonPaginationBehavior.Disable,
                    PaginationBehaviour = PaginationBehaviour.WrapAround
                });
                //set up XP and repeating things
                _discord.MessageCreated += Discord_MessageCreated;
            }


            using (mainlog.BeginScope("Initializing Commands"))
            {
                services.AddSingleton<IAnalyse>(new ConsoleAnalytics());
                services.AddSingleton(new LanguageService());
                services.AddDbContext<DatabaseContext>(
                    options =>
                    {
                        options.UseLazyLoadingProxies().UseSqlite("Filename=./silverbotdatabasev3.db",
                            b => b.MigrationsAssembly("SilverBotDS"));
                        if (Debugger.IsAttached)
                        {
                            options.EnableSensitiveDataLogging();
                        }
                    }, ServiceLifetime.Transient);

                services.AddSingleton(_config);
                services.AddSingleton(HttpClient);
                if (_config.AutoDownloadAndStartLavalink)
                {
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
                }


                await Task.Delay(2000);
                if (_config.UseLavaLink)
                {
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
                        services.AddSingleton(new ArtworkService());
                        if (!_config.SitInVc)
                        {
                            mainlog.LogDebug("Activating tracking service");
                            services.AddSingleton(_trackingService);
                        }

                        mainlog.LogDebug("Activating lyrics service");
                        services.AddSingleton(new LyricsService(new LyricsOptions { UserAgent = "SilverBot" }));
                    }
                }

                services.AddSingleton(_log);
                services.AddSingleton(_discord);

                using (mainlog.BeginScope("External services"))
                {
                    foreach (var group in _config.ServicesToLoadExternal.GroupBy(x => x.Key))
                    {
                        if (File.Exists(group.Key))
                        {
                            var assembly = Assembly.LoadFrom(group.Key);
                            foreach (var module in group)
                            {
                                try
                                {
                                    var t = assembly.GetType(module.Value);
                                    if (t.GetInterfaces().Contains(typeof(IRequireAssets)))
                                    {
                                        var assets = (string[]?)t.GetProperty("RequiredAssets").GetValue(null);
                                        if (assets != null)
                                        {
                                            var missingassets = GetMissingAssets(assets);
                                            if (missingassets.Any())
                                            {
                                                mainlog.LogInformation(
                                                    "Module {Module} might not work properly as its requirements weren't met, the asset/s {MissingAssets} is/are missing",
                                                    module, string.Join(',', missingassets));
                                                var autofix = t.GetMethod("AutoFixRequiredAssets");
                                                if (autofix != null)
                                                {
                                                    try
                                                    {
                                                        mainlog.LogInformation(
                                                            "Trying to autofix {Module}'s dependancies",
                                                            module.Value);
                                                        autofix.Invoke(null, null);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        mainlog.LogInformation(ex,
                                                            "Failed to autofix {Module}'s dependancies Exception occured",
                                                            module.Value);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (!t.GetInterfaces().Contains(typeof(IService)))
                                    {
                                        continue;
                                    }

                                    var thing = CreateInstance(t, services.BuildServiceProvider());
                                    await ((IService)thing).Start();
                                    services.AddSingleton(thing);
                                }
                                catch (Exception ex)
                                {
                                    mainlog.LogError(ex, "Failed to load service {Module} Exception occured",
                                        module.Value);
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

                ServiceProvider = services.BuildServiceProvider();
                using (mainlog.BeginScope("Database migration"))
                {
                    var context = ServiceProvider.GetService<DatabaseContext>();
                    //Do stuff with the database making sure its up to date.
                    await context!.Database!.MigrateAsync()!;
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
                        commands.SetHelpFormatter<CustomHelpFormatter>();
                        commands.RegisterConverter(new SdImageConverter());
                        commands.RegisterConverter(new SColorConverter());
                        commands.RegisterConverter(new LoopSettingsConverter());
                        commands.RegisterConverter(new SongOrSongsConverter());
                        commands.RegisterConverter(new ImageFormatConverter());
                    }

                    async Task ProcessModuleType(Type? type)
                    {
                        if (type == null)
                        {
                            return;
                        }

                        if (type.GetInterfaces().Contains(typeof(IRequireAssets)))
                        {
                            var assets = (string[]?)type.GetProperty("RequiredAssets")?.GetValue(null);
                            if (assets != null)
                            {
                                var missingAssets = GetMissingAssets(assets).ToArray();
                                if (missingAssets.Length!=0)
                                {
                                    _log.Information(
                                        "Module {Module} might not work properly as its requirements weren't met, the asset/s {MissingAssets} is/are missing",
                                        type.Name, string.Join(',', missingAssets));
                                    var autofix = type.GetMethod("AutoFixRequiredAssets");
                                    if (autofix != null)
                                    {
                                        try
                                        {
                                            _log.Information("Trying to autofix {Module}'s dependencies", type.Name);
                                            autofix.Invoke(null, new object[] { missingAssets });
                                        }
                                        catch (Exception ex)
                                        {
                                            _log.Error(ex,
                                                "Failed to autofix {Module}'s dependencies Exception occured",
                                                type.Name);
                                        }
                                    }
                                }
                            }
                        }

                        if (type.IsSubclassOf(typeof(IHaveExecutableRequirements)))
                        {
                            var n = (IHaveExecutableRequirements)Activator.CreateInstance(type)!;
                            if (await n.ExecuteRequirements(_config))
                            {
                                commands.RegisterCommands(type);
                            }
                            else
                            {
                                _log.Information("Module {Module} won't be loaded as its requirements weren't met",
                                    type.Name);
                            }
                        }
                        else if (type.IsSubclassOf(typeof(ApplicationCommandModule)))
                        {
                            slash.RegisterCommands(type);
                        }
                        else if (type.IsSubclassOf(typeof(BaseCommandModule)))
                        {
                            commands.RegisterCommands(type);
                        }
                    }

                    using (mainlog.BeginScope("Registering internal commands"))
                    {
                        foreach (var module in _config.ModulesToLoad)
                        {
                            try
                            {
                                await ProcessModuleType(Type.GetType(module));
                            }
                            catch (Exception ex)
                            {
                                mainlog.LogError(ex, "Failed to load internal module {Module} Exception occured",
                                    module);
                            }
                        }
                    }

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
                                        await ProcessModuleType(module);
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

                    commands.CommandExecuted += Commands_CommandExecuted;
                    await SlashErrorHandler.RegisterErrorHandler(ServiceProvider, _log, slash);
                    await CommandErrorHandler.RegisterErrorHandler(ServiceProvider, _log, commands);

                    #endregion Registering Commands
                }
            }

            //🥁🥁🥁 drum-roll
            _log.Information("Connecting to discord");
            var isConnected = false;
            _discord.Ready += (_, _) =>
            {
                isConnected = true;
                return Task.CompletedTask;
            };
            await _discord.ConnectAsync(new("console logs while booting up", ActivityType.Watching));
            _log.Information("Waiting for client to connect");
            while (isConnected)
            {
                //intentional empty statement
                _log.Information("_");
                await Task.Delay(100, cancellationToken);
            }

            await Task.Delay(2000, cancellationToken);
            if (_config.UseLavaLink)
            {
                await _discord.UpdateStatusAsync(
                    new("console logs while connecting to lavalink", ActivityType.Watching));

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
            }

            if (IsNotNullAndIsNotB(_config.FridayTextChannel, 0))
            {
                CancellationTokenSource s = new();
                RunningTasks.Add("WaitForFridayTask", new(Task.Run(() => WaitForFridayAsync(s.Token), s.Token), s));
            }

            await _discord.UpdateStatusAsync(new("console logs while configuring server statistics",
                ActivityType.Watching));
            ProgramHelper helper = new();
            if (_config.ChannelsToArchivePicturesFrom.Length > 0 && (_config.ArchiveWebhooks.Length > 1 ||
                                                                     (_config.ArchiveWebhooks.Length == 1 &&
                                                                      _config.ArchiveWebhooks[0] !=
                                                                      "https://discordapp.com/api/webhooks/id/key")))
            {
                await helper.AddArchiverAsync(_config, _log, HttpClient, _discord);
            }
            if (_config.ReactionRolesEnabled)
            {
                _discord.AddReactionRolesHandlers();
            }

            if (_config.EnableServerStatistics)
            {
                CancellationTokenSource s = new();
                RunningTasks.Add("StatisticsTask", new(Task.Run(() => StatisticsMainAsync(s.Token), s.Token), s));
            }

            CancellationTokenSource ets = new();
            EventsRunner.InjectEvents(ServiceProvider, _log);
            RunningTasks.Add("EventsTask", new(Task.Run(EventsRunner.RunEventsAsync, ets.Token), ets));

            if (_config.HostWebsite)
            {
                await _discord.UpdateStatusAsync(new("console logs while launching the website module",
                    ActivityType.Watching));
                _log.Information("Creating host");
            }

            _log.Information("Booted up");
            do
            {
                _log.Verbose("Updating the status to a random one");
                //update the status to some random one
                await _discord.UpdateStatusAsync(_config.Splashes.RandomFrom()
                    .GetDiscordActivity(GetStringDictionary(_discord)));
                if (_config.CallGcOnSplashChange)
                {
                    _log.Verbose("Calling the garbage collector");
                    GC.Collect();
                }

                if (_config.ClearTasks)
                {
                    _log.Verbose("Going through the task lists");
                    foreach (var task in RunningTasks.Where(task => task.Value.Item1.IsCompleted))
                    {
                        RunningTasks.Remove(task.Key);
                    }

                    foreach (var task in RunningTasksOfSecondRow.Where(task => task.Value.Item1.IsCompleted))
                    {
                        RunningTasksOfSecondRow.Remove(task.Key);
                    }
                }

                //wait the specified time
                _log.Verbose("Waiting {V}", new TimeSpan(0, 0, 0, 0, _config.MsInterval).Humanize(precision: 5));
                await Task.Delay(_config.MsInterval, cancellationToken);
                //repeat🔁
            } while (!ExitAfterbootup && !cancellationToken.IsCancellationRequested);
        }

        public static object CreateInstance(Type t, IServiceProvider services)
        {
            var ti = t.GetTypeInfo();
            var constructors = ti.DeclaredConstructors
                .Where(xci => xci.IsPublic)
                .ToArray();

            if (constructors.Length != 1)
                throw new ArgumentException(
                    "Specified type does not contain a public constructor or contains more than one public constructor.");

            var constructor = constructors[0];
            var constructorArgs = constructor.GetParameters();
            var args = new object[constructorArgs.Length];

            if (constructorArgs.Length != 0 && services == null)
                throw new InvalidOperationException(
                    "Dependency collection needs to be specified for parameterized constructors.");

            // inject via constructor
            if (constructorArgs.Length != 0)
            {
                for (var i = 0; i < args.Length; i++)
                {
                    args[i] = services.GetRequiredService(constructorArgs[i].ParameterType);
                }
            }

            var moduleInstance = Activator.CreateInstance(t, args);

            // inject into properties
            var props = t.GetRuntimeProperties().Where(xp =>
                xp.CanWrite && xp.SetMethod?.IsStatic == false && xp.SetMethod.IsPublic);
            foreach (var prop in props)
            {
                if (prop.GetCustomAttribute<DSharpPlus.CommandsNext.Attributes.DontInjectAttribute>() != null)
                    continue;

                var service = services.GetService(prop.PropertyType);
                if (service == null)
                    continue;

                prop.SetValue(moduleInstance, service);
            }

            // inject into fields
            var fields = t.GetRuntimeFields().Where(xf => !xf.IsInitOnly && !xf.IsStatic && xf.IsPublic);
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<DSharpPlus.CommandsNext.Attributes.DontInjectAttribute>() != null)
                    continue;

                var service = services.GetService(field.FieldType);
                if (service == null)
                    continue;

                field.SetValue(moduleInstance, service);
            }

            return moduleInstance;
        }

        private static Task<int> ResolvePrefixAsync(DiscordMessage msg)
        {
            if (msg.Channel.Type == ChannelType.Private)
            {
                return Task.FromResult(0);
            }

            var db = ServiceProvider.GetService<DatabaseContext>();
            if (db?.IsBanned(msg.Author.Id) == true)
            {
                return Task.FromResult(-1);
            }

            var gld = msg.Channel.Guild;
            if (gld == null)
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

        public static Dictionary<string, string> GetStringDictionary(DiscordClient client)
        {
            return new Dictionary<string, string>
            {
                ["GuildCount"] = client.Guilds.Values.LongCount().ToString(),
                ["Platform"] = RuntimeInformation.ProcessArchitecture.ToString()
            };
        }

        public static async Task StatisticsMainAsync(CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
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
                                        $"Hello SilverBot here,\n it appears that you own `{server.Name}` and i just wanted to let you know that you will have to set the stats category again for stats to work as something broke.");
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
                            dbctx.serverSettings.Remove(serverSettings);
                            await dbctx.SaveChangesAsync(CancellationToken.None);
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.Error(e, "exception happened in stats thread");
                }

                await Task.Delay(1800000, ct);
            }
        }

        public static async Task WaitForFridayAsync(CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                if (DayOfWeek.Friday == DateTime.Now.DayOfWeek &&
                    (_lastFriday == 0 || _lastFriday != DateTime.Now.DayOfYear))
                {
                    _lastFriday = DateTime.Now.DayOfYear;
                    await ExecuteFridayAsync(ct: ct);
                }
                else
                {
                    if (_lastFriday == DateTime.Now.DayOfYear - 1)
                    {
                        await ExecuteFridayAsync(false, ct);
                        _lastFriday = 0;
                    }
                }

                await Task.Delay(1000, ct);
            }
        }

        public static async Task ExecuteFridayAsync(bool friday = true, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            if (friday)
            {
                var channel = await _discord.GetChannelAsync(_config.FridayTextChannel);
                await channel.SendMessageAsync("It is Friday");
                await channel.AddOverwriteAsync(channel.Guild.EveryoneRole, Permissions.SendMessages, reason: "friday");
            }
            else
            {
                var channel = await _discord.GetChannelAsync(_config.FridayTextChannel);
                await channel.SendMessageAsync("Friday ended :c");
                await channel.AddOverwriteAsync(channel.Guild.EveryoneRole, deny: Permissions.SendMessages,
                    reason: "not friday");
            }
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

        private static async Task Discord_MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Author.IsBot)
            {
                //ha nice try
                return;
            }

            if (!e.Channel.IsPrivate)
            {
                if (XpLevelling.TryGetValue(e.Author.Id, out var value))
                {
                    if ((DateTime.UtcNow - value) > MessageLimit)
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
    }
}