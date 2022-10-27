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
using Lavalink4NET.Lyrics;
using Lavalink4NET.Tracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetVips;
using SDiscordSink;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;
using SilverBotDS.Converters;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SilverBotDS.Objects.Database.Classes;
using SilverBotDS.Utils;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilverBotDS
{
    internal static class Program
    {
        private static Config _config;

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
            if (args.Length == 2 && args[0] == "generatelang")
            {
                if (!Directory.Exists(Path.GetDirectoryName(args[1])))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(args[1]) ?? string.Empty);
                }

                Language.SerialiseDefault(args[1]);
                Console.WriteLine("Serialized en.json");
                Environment.Exit(70);
                return;
            }

            if (Debugger.IsAttached && !Environment.CurrentDirectory.EndsWith("bin\\Debug\\net6.0"))
            {
                Environment.CurrentDirectory += Environment.OSVersion.Platform == PlatformID.Win32NT
                    ? "\\bin\\Debug\\net6.0"
                    : "/bin/Debug/net6.0";
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

        public static Config GetConfig()
        {
            return ServiceProvider.GetService<Config>();
        }

        private static HttpClient NewHttpClientWithUserAgent()
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

        public static IEnumerable<string> GetMissingAssets(string[] required)
        {
            return required.Where(x => !AssetPresent(x));
        }
        public static bool AssetPresent(string asset)
        {
            if(asset.StartsWith("font://"))
            {
                var font = asset.RemoveStringFromStart("font://").Replace(' ','-');
                return MagickNET.FontNames.Contains(font);
            }
            else if(asset.StartsWith("file://"))
            {
                var file = asset.RemoveStringFromStart("file://");
                return File.Exists(file);
            }
            return false;
        }

        private static async Task MainAsync(string[] args)
        {
            _config = await Config.GetAsync();
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            WebHookUtils.ParseWebhookUrlNullable(_config.LogWebhook, out ulong? id, out string token);
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

            if (!(id == null || string.IsNullOrEmpty(token) || Debugger.IsAttached))
            {
                loggerConfiguration.WriteTo.DiscordSink((ulong)id, token);
            }

            _log = loggerConfiguration.CreateLogger();
            if (_config.EnableUpdateChecking)
            {
                _log.Information("Checking for updates");
                await VersionInfo.Checkforupdates(HttpClient, _log);
            }

            var logFactory = new LoggerFactory().AddSerilog(logger: _log);
            _log.Verbose("Creating the discord client");
            _discord = new DiscordClient(new DiscordConfiguration
            {
                LoggerFactory = logFactory,
                Token = _config.Token,
                Intents = DiscordIntents.All.RemoveIntent(DiscordIntents.GuildPresences)
            });
            _log.Verbose("Initializing interactivity");
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
            _log.Verbose("Initializing Commands");
            ServiceCollection services = new();
            services.AddSingleton<IAnalyse>(new ConsoleAnalytics());

            services.AddDbContext<DatabaseContext>(
                              options =>
                              {
                                  options.UseLazyLoadingProxies().UseSqlite("Filename=./silverbotdatabasev3.db", b => b.MigrationsAssembly("SilverBotDS"));
                                  if (Debugger.IsAttached) { options.EnableSensitiveDataLogging(); }
                              }, ServiceLifetime.Transient);

            services.AddSingleton(_config);
            services.AddSingleton(HttpClient);
            if (_config.AutoDownloadAndStartLavalink)
            {
                //Launch lavalink
                if (!File.Exists("Lavalink.jar"))
                {
                    //file not found time to download lavalink
                    _log.Information("Downloading lavalink");
                    await (await GitHubUtils.Release.GetLatestFromRepoAsync(new(_config.LavalinkBuildsSourceGitHubUser, _config.LavalinkBuildsSourceGitHubRepo), HttpClient))
                        .DownloadLatestAsync(HttpClient);
                }

                if (!Process.GetProcesses().Any(x=>x.ProcessName.Contains("java")))
                {
                    _log.Information("Launching lavalink");

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
            _log.Verbose("Waiting 2s");
            await Task.Delay(2000);
            if (_config.UseLavaLink)
            {
                _log.Information("Making a lavalinknode");
                var discordClientWrapper = new DiscordClientWrapper(_discord);
                _audioService = new LavalinkNode(new LavalinkNodeOptions
                {
                    RestUri = _config.LavalinkRestUri,
                    WebSocketUri = _config.LavalinkWebSocketUri,
                    Password = _config.LavalinkPassword
                }, discordClientWrapper);
                _trackingService = new InactivityTrackingService(_audioService, discordClientWrapper,
                    new InactivityTrackingOptions());
                services.AddSingleton(_audioService);
                services.AddSingleton(new ArtworkService());
                if (!_config.SitInVc)
                {
                    services.AddSingleton(_trackingService);
                }

                services.AddSingleton(new LyricsService(new LyricsOptions { UserAgent = "SilverBot" }));
            }

            if (IsNotNullAndIsNotB(_config.SpotifyClientId, "Spotify_CLIENT_ID") &&
                IsNotNullAndIsNotB(_config.SpotifyClientSecret, "Spotify_CLIENT_SECRET"))
            {
                var spotifyClientConfig = SpotifyClientConfig.CreateDefault();
                services.AddSingleton(new SpotifyClient(spotifyClientConfig.WithToken(
                    (await new OAuthClient(spotifyClientConfig).RequestToken(
                        new ClientCredentialsRequest(_config.SpotifyClientId, _config.SpotifyClientSecret)))
                    .AccessToken)));
            }
            services.AddSingleton(_log);
            object CreateInstance(Type t, IServiceProvider services)
            {
                var ti = t.GetTypeInfo();
                var constructors = ti.DeclaredConstructors
                    .Where(xci => xci.IsPublic)
                    .ToArray();

                if (constructors.Length != 1)
                    throw new ArgumentException("Specified type does not contain a public constructor or contains more than one public constructor.");

                var constructor = constructors[0];
                var constructorArgs = constructor.GetParameters();
                var args = new object[constructorArgs.Length];

                if (constructorArgs.Length != 0 && services == null)
                    throw new InvalidOperationException("Dependency collection needs to be specified for parameterized constructors.");

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
                var props = t.GetRuntimeProperties().Where(xp => xp.CanWrite && xp.SetMethod?.IsStatic == false && xp.SetMethod.IsPublic);
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
                                if(assets!=null)
                                {
                                    var missingassets = GetMissingAssets(assets);
                                    if(missingassets.Any())
                                    {
                                        _log.Information(
                                                                                "Module {Module} might not work properly as its requirements weren't met, the asset/s {MissingAssets} is/are missing",
                                                                                module, string.Join(',', missingassets));
                                        var autofix = t.GetMethod("AutoFixRequiredAssets");
                                        if (autofix!=null)
                                        {
                                            try
                                            {
                                                _log.Information("Trying to autofix {Module}'s dependancies", module.Value);
                                                autofix.Invoke(null, null);
                                            }
                                            catch (Exception ex)
                                            {
                                                _log.Error(ex, "Failed to autofix {Module}'s dependancies Exception occured", module.Value);
                                            }
                                        }
                                    }
                                }
                            }

                            if (t.GetInterfaces().Contains(typeof(IService)))
                            {
                                var thing = CreateInstance(t, services.BuildServiceProvider());
                                await ((IService)thing).Start();
                                services.AddSingleton(thing);
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex, "Failed to load service {Module} Exception occured", module.Value);
                        }
                    }
                }
                else
                {
                    _log.Information("Services from the {File} won't be loaded as its file doesn't exist", group.Key);
                }
            }
            services.AddSingleton(_discord);
            ServiceProvider = services.BuildServiceProvider();
            var context = ServiceProvider.GetService<DatabaseContext>();
            //Do stuff with the database making sure its up to date.
            await context!.Database!.MigrateAsync()!;
            var commands = _discord.UseCommandsNext(new CommandsNextConfiguration
            {
                Services = ServiceProvider,
                PrefixResolver = ResolvePrefixAsync
            });
            var slash = _discord.UseSlashCommands(new SlashCommandsConfiguration() { Services = ServiceProvider });

            #region Registering Commands

            commands.SetHelpFormatter<CustomHelpFormatter>();
            _log.Verbose("Registering Commands&Converters");
            commands.RegisterConverter(new SdImageConverter());
            commands.RegisterConverter(new SColorConverter());
            commands.RegisterConverter(new LoopSettingsConverter());
            commands.RegisterConverter(new SongOrSongsConverter());
            commands.RegisterConverter(new ImageFormatConverter());
            async Task ProcessModuleType(Type? type)
            {
                if(type == null)
                {
                    return;
                }
                if (type.GetInterfaces().Contains(typeof(IRequireAssets)))
                {
                    var assets = (string[]?)type.GetProperty("RequiredAssets").GetValue(null);
                    if (assets != null)
                    {
                        var missingassets = GetMissingAssets(assets);
                        if (missingassets.Any())
                        {
                            _log.Information(
                                                                    "Module {Module} might not work properly as its requirements weren't met, the asset/s {MissingAssets} is/are missing",
                                                                    type.Name, string.Join(',', missingassets));
                            var autofix = type.GetMethod("AutoFixRequiredAssets");
                            if (autofix != null)
                            {
                                try
                                {
                                    _log.Information("Trying to autofix {Module}'s dependancies", type.Name);
                                    autofix.Invoke(null, new object[] { missingassets });
                                }
                                catch (Exception ex)
                                {
                                    _log.Error(ex, "Failed to autofix {Module}'s dependancies Exception occured", type.Name);
                                }
                            }
                        }
                    }
                }

                if (type.IsSubclassOf(typeof(SilverBotCommandModule)))
                {
                    var n = (SilverBotCommandModule)Activator.CreateInstance(type);
                    if (await n.ExecuteRequirements(_config))
                    {
                        commands.RegisterCommands(type);
                    }
                    else
                    {
                        _log.Information("Module {Module} won't be loaded as its requirements weren't met", type.Name);
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
           
            foreach (var module in _config.ModulesToLoad)
            {
                try
                {
                    await ProcessModuleType(Type.GetType(module));
                }
                catch (Exception ex)
                {
                    _log.Error(ex, "Failed to load internal module {Module} Exception occured", module);
                }
            }
            foreach (var group in _config.ModulesToLoadExternal.GroupBy(x => x.Key))
            {
                if (File.Exists(group.Key))
                {
                    var assembly = Assembly.LoadFrom(group.Key);
                    foreach (var module in group)
                    {
                        try
                        {
                            await ProcessModuleType(assembly.GetType(module.Value));
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex, "Failed to load external module {Module} Exception occured", module.Value);
                        }
                    }
                }
                else
                {
                    _log.Information("Modules from the {File} won't be loaded as its file doesn't exist", group.Key);
                }
            }
            commands.CommandExecuted += Commands_CommandExecuted;
            await SlashErrorHandler.RegisterErrorHandler(ServiceProvider, _log, slash);
            await CommandErrorHandler.RegisterErrorHandler(ServiceProvider, _log, commands);

            #endregion Registering Commands

            //🥁🥁🥁 drum-roll
            _log.Information("Connecting to discord");
            var isConnected = false;
            _discord.Ready += (_, _) =>
            {
                isConnected = true;
                return Task.CompletedTask;
            };
            await _discord.ConnectAsync(new("console logs while booting up", ActivityType.Watching));
            _log.Verbose("Waiting for client to connect");
            while (isConnected)
            {
                //intentional empty statement
            }
            await Task.Delay(2000);
            await _discord.UpdateStatusAsync(new("console logs while connecting to lavalink", ActivityType.Watching));
            if (IsNotNullAndIsNotB(_config.FridayTextChannel, 0))
            {
                CancellationTokenSource s = new();
                RunningTasks.Add("WaitForFridayTask", new(Task.Run(() => WaitForFridayAsync(s.Token), s.Token), s));
            }
            if (_config.UseLavaLink)
            {
                await _audioService.InitializeAsync();
                if (!_config.SitInVc)
                {
                    _trackingService.BeginTracking();
                }
            }

            await _discord.UpdateStatusAsync(new("console logs while configuring server statistics",
                ActivityType.Watching));

            if (_config.ChannelsToArchivePicturesFrom.Length > 0 && (_config.ArchiveWebhooks.Length > 1 || (_config.ArchiveWebhooks.Length == 1 && _config.ArchiveWebhooks[0] != "https://discordapp.com/api/webhooks/id/key")))
            {
                DiscordWebhookClient webhookClient = new();
                foreach (var aa in _config.ArchiveWebhooks)
                {
                    WebHookUtils.ParseWebhookUrlNullable(aa, out ulong? ida, out string tokena);
                    if (ida is not null && !string.IsNullOrWhiteSpace(tokena))
                    {
                        await webhookClient.AddWebhookAsync(ida.Value, tokena);
                    }
                }
                _log.Information("Archive webhooks configured");
                async Task OnDiscordOnMessagewhatever(DiscordClient e, DiscordMessage a)
                {
                    if (_config.ChannelsToArchivePicturesFrom.Contains(a.Channel.Id))
                    {
                        Dictionary<string, Stream> files = new();
                        foreach (var attachment in a.Attachments)
                        {
                            await HttpClient.GetAsync(attachment.Url)
                                .ContinueWith(async (t) =>
                                {
                                    if (t.IsCompletedSuccessfully)
                                    {
                                        files.Add(attachment.FileName, await t.Result.Content.ReadAsStreamAsync());
                                    }
                                });
                        }

                        if (a.Embeds.Count > 0)
                        {
                            foreach (var embed in a.Embeds)
                            {
                                if (embed.Image != null)
                                {
                                    await HttpClient.GetAsync(embed.Image.Url.ToUri())
                                        .ContinueWith(async (t) =>
                                        {
                                            if (t.IsCompletedSuccessfully)
                                            {
                                                files.Add(Path.GetFileName(embed.Image.Url.ToUri().AbsolutePath), await t.Result.Content.ReadAsStreamAsync());
                                            }
                                        });
                                }
                                if (embed.Thumbnail != null)
                                {
                                    await HttpClient.GetAsync(embed.Thumbnail.Url.ToUri())
                                        .ContinueWith(async (t) =>
                                        {
                                            if (t.IsCompletedSuccessfully)
                                            {
                                                files.Add(Path.GetFileName(embed.Thumbnail.Url.ToUri().AbsolutePath), await t.Result.Content.ReadAsStreamAsync());
                                            }
                                        });
                                }
                            }
                        }
                        if (a.Content.Length > 0)
                        {
                            files.Add("message.txt", new MemoryStream(Encoding.UTF8.GetBytes(a.Content)));
                        }
                        files.Add("message.json", new MemoryStream(Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(a))));
                        var zipFile = new MemoryStream();
                        var zip = new ZipArchive(zipFile, ZipArchiveMode.Create, true);
                        foreach (var (key, value) in files)
                        {
                            var entry = zip.CreateEntry(key);
                            var stream = entry.Open();
                            value.Position = 0;
                            await value.CopyToAsync(stream);
                            stream.Close();
                        }
                        var dwb = new DiscordWebhookBuilder();
                        foreach (var file in files)
                        {
                            file.Value.Position = 0;
                            dwb.AddFile(file.Key, file.Value);
                        }
                        zipFile.Position = 0;
                        dwb.AddFile("message.zip", zipFile);
                        dwb.AddEmbeds(a.Embeds);
                        dwb.WithContent(a.Content);
                        await webhookClient.BroadcastMessageAsync(dwb);
                        await zipFile.DisposeAsync();
                        foreach (var file in files)
                        {
                            await file.Value.DisposeAsync();
                        }
                    }
                }
                _discord.MessageCreated += (e, a) => OnDiscordOnMessagewhatever(e, a.Message);
                _discord.MessageUpdated += (e, a) => OnDiscordOnMessagewhatever(e, a.Message);
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
            while (true)
            {
                _log.Verbose("Updating the status to a random one");
                //update the status to some random one
                await _discord.UpdateStatusAsync(_config.Splashes.RandomFrom()
                    .GetDiscordActivity(GetStringDictionary(_discord)));
                if (_config.CallGCOnSplashChange)
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
                await Task.Delay(_config.MsInterval);
                //repeat🔁
            }
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
                    _log.Debug("Entered statistics method");
                    await using var dbctx = ServiceProvider.GetRequiredService<DatabaseContext>();
                    _log.Debug("Getting the settings about statistics");
                    var things = dbctx.GetStatisticSettings();
                    _log.Debug("Got the settings about statistics");
                    foreach (var (item1, item2, item3) in things)
                    {
                        _log.Debug("Getting the guild with the id {Id}", item1);
                        try
                        {
                            var server = await _discord.GetGuildAsync(item1);
                            var dict = await ServerStatString.GetStringDictionaryAsync(server);
                            _log.Debug("Got the guild with the id {Id}", item1);
                            _log.Debug("Getting the channel with the id {Id}", item2);
                            try
                            {
                                var category = server.Channels[(ulong)item2];
                                _log.Debug("Got the channel with the id {Id}", item2);
                                if (category.Type is ChannelType.Category)
                                {
                                    var e = 0;
                                    _log.Debug("Getting the children of the channel {Id}", item2);
                                    foreach (var child in category.Children)
                                    {
                                        if (item3.Length > e)
                                        {
                                            _log.Debug("Updating {Id}", child.Id);
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
                ct.ThrowIfCancellationRequested();
                var channel = await _discord.GetChannelAsync(_config.FridayTextChannel);
                ct.ThrowIfCancellationRequested();
                await channel.SendMessageAsync("It is Friday");
                ct.ThrowIfCancellationRequested();
                await channel.AddOverwriteAsync(channel.Guild.EveryoneRole, Permissions.SendMessages, reason: "friday");
                ct.ThrowIfCancellationRequested();
            }
            else
            {
                ct.ThrowIfCancellationRequested();
                var channel = await _discord.GetChannelAsync(_config.FridayTextChannel);
                ct.ThrowIfCancellationRequested();
                await channel.SendMessageAsync("Friday ended :c");
                ct.ThrowIfCancellationRequested();
                await channel.AddOverwriteAsync(channel.Guild.EveryoneRole, deny: Permissions.SendMessages,
                    reason: "not friday");
                ct.ThrowIfCancellationRequested();
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
                if (XpLevelling.ContainsKey(e.Author.Id))
                {
                    var prev = XpLevelling[e.Author.Id];
                    if ((DateTime.UtcNow - prev) > MessageLimit)
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
                if ((e.Channel.IsPrivate
                     || (o is not default(ServerSettings) && o.RepeatThings && e.Channel
                         .PermissionsFor(await e.Guild.GetMemberAsync(sender.CurrentUser.Id))
                         .HasPermission(Permissions.SendMessages))) &&
                    MessagesToRepeat.Contains(e.Message.Content.ToLowerInvariant()))
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