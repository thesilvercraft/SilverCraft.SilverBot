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
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using DSharpPlus.VoiceNext;
using Humanizer;
using Lavalink4NET;
using Lavalink4NET.DSharpPlus;
using Lavalink4NET.Lyrics;
using Lavalink4NET.Tracking;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SDBrowser;
using SDiscordSink;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using SilverBotDS.Attributes;
using SilverBotDS.Commands.Slash;
using SilverBotDS.Converters;
using SilverBotDS.Exceptions;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SilverBotDS.Objects.Database.Classes;
using SilverBotDS.Utils;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SnowdPlayer;
using SpotifyAPI.Web;

namespace SilverBotDS
{
    internal static class Program
    {
        public static readonly char DirSlash = Environment.OSVersion.Platform == PlatformID.Win32NT ? '\\' : '/';
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
            "gaming", "<:kalorichan:839099093552332850>", "kalorichan",
        };

        private static readonly Dictionary<ulong, DateTime> XpLevelling = new();
        private static readonly TimeSpan MessageLimit = TimeSpan.FromMinutes(2);
        public static ServiceProvider ServiceProvider { get; private set; }
        private static readonly Dictionary<string, Tuple<Task, CancellationTokenSource>> RunningTasks = new();
        private static readonly Dictionary<Guid, Tuple<Task, CancellationTokenSource>> RunningTasksOfSecondRow = new();

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

        public static bool CheckIfAllFontsAreHere(string[] requiredFonts)
        {
            if (requiredFonts == null)
            {
                throw new ArgumentNullException(nameof(requiredFonts));
            }

            var familyNames = SystemFonts.Families.Select(x => x.Name).ToList();
            return requiredFonts.All(font => familyNames.Contains(font));
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

            loggerConfiguration
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) //So uh asp.net LOVES spamming logs
                .WriteTo.Console(theme: AnsiConsoleTheme.Code);
            if (_config.UseTxtFilesAsLogs)
            {
                loggerConfiguration.WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, shared: true);
            }

            if (!(id == null || string.IsNullOrEmpty(token)))
            {
                loggerConfiguration.WriteTo.DiscordSink(new Tuple<ulong, string>((ulong)id, token));
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
                Intents = DiscordIntents.All
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

            #region Browser fun stuff

            switch (_config.BrowserType)
            {
                case 0:
                    {
                        _log.Verbose("Not using a browser");
                        break;
                    }
                case 1:
                    {
                        _log.Verbose("Launching chrome");
                        services.AddSingleton<IBrowser>(new SeleniumBrowser(Browsertype.Chrome,
                            string.IsNullOrEmpty(_config.DriverLocation)
                                ? Environment.CurrentDirectory
                                : _config.DriverLocation));
                        break;
                    }
                case 2:
                    {
                        _log.Verbose("Launching firefox");
                        services.AddSingleton<IBrowser>(new SeleniumBrowser(Browsertype.Firefox,
                            string.IsNullOrEmpty(_config.DriverLocation)
                                ? Environment.CurrentDirectory
                                : _config.DriverLocation));
                        break;
                    }
                case 3:
                    {
                        _log.Verbose("Using the remoteBrowser");
                        services.AddSingleton<IBrowser>(new RemoteBrowser(HttpClient));
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException();
                    }
            }

            #endregion Browser fun stuff

            if (IsNotNullAndIsNotB(_config.SegmentPrivateSource, "Segment_Key"))
            {
                services.AddSingleton<IAnalyse>(new SegmentIo(_config.SegmentPrivateSource));
            }

            #region Database fun stuff

            switch (_config.DatabaseType)
            {
                case 1:
                    {
                        if (_config != null && !string.IsNullOrEmpty(_config.ConnString))
                        {
                            services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(_config.ConnString),
                                ServiceLifetime.Transient);
                        }
                        else
                        {
                            Uri tmp = new(Environment.GetEnvironmentVariable("DATABASE_URL") ??
                                          throw new InvalidOperationException());
                            string[] usernameandpass = tmp.UserInfo.Split(":");
                            services.AddDbContext<DatabaseContext>(
                                options => options.UseNpgsql(
                                    $"Host={tmp.Host};Username={usernameandpass[0]};Password={usernameandpass[1]};Database={HttpUtility.UrlDecode(tmp.AbsolutePath).Remove(0, 1)}"),
                                ServiceLifetime.Transient);
                        }

                        break;
                    }
                case 2:
                    {
                        services.AddDbContext<DatabaseContext>(
                            options => options.UseSqlite("Filename=./silverbotdatabasev2.db"), ServiceLifetime.Transient);
                        break;
                    }
                case 3:
                    {
                        var conn = configurationBuilder.GetConnectionString("Kestrel:Certificates:Development:Password");
                        conn ??= _config.ConnString;
                        services.AddDbContext<DatabaseContext>(options =>
                            options.UseSqlServer(conn));
                        break;
                    }
            }

            #endregion Database fun stuff

            services.AddSingleton(_config);
            services.AddSingleton(HttpClient);
            if (_config.AutoDownloadAndStartLavalink)
            {
                //Launch lavalink
                if (!File.Exists("Lavalink.jar"))
                {
                    //file not found time to download lavalink
                    _log.Information("Downloading lavalink");
                    await (await GitHubUtils.Release.GetLatestFromRepoAsync(new("freyacodes", "Lavalink"), HttpClient))
                        .DownloadLatestAsync(HttpClient);
                }

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

            if (_config.UseNewAudio)
            {
                services.AddSingleton(new SnowService(_discord.UseVoiceNext(), false, true));
            }

            _log.Verbose("Waiting 6s");
            await Task.Delay(6000);
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
                            if (t.GetInterfaces().Contains(typeof(IRequireFonts)))
                            {
                                var fonts = (string[])t.GetProperty("RequiredFontFamilies").GetValue(null);
                                if (!CheckIfAllFontsAreHere(fonts))
                                {
                                    _log.Information(
                                        "Module {Module} won't be loaded as its requirements weren't met, the font/fonts {Fonts} is/are missing",
                                        module, string.Join(',', fonts));
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
            ServiceProvider = services.BuildServiceProvider();
            var context = ServiceProvider.GetService<DatabaseContext>();
            //Do stuff with the database making sure its up to date.
            await context?.Database.MigrateAsync()!;
            var commands = _discord.UseCommandsNext(new CommandsNextConfiguration
            {
                Services = ServiceProvider,
                PrefixResolver = ResolvePrefixAsync
            });
            var slash = _discord.UseSlashCommands(new SlashCommandsConfiguration() { Services = ServiceProvider });
            slash.SlashCommandErrored += Slash_SlashCommandErrored;

            #region Registering Commands

            commands.SetHelpFormatter<CustomHelpFormatter>();
            _log.Verbose("Registering Commands&Converters");
            commands.RegisterConverter(new SdImageConverter());
            commands.RegisterConverter(new SColorConverter());
            commands.RegisterConverter(new LoopSettingsConverter());
            commands.RegisterConverter(new SongOrSongsConverter());
            commands.RegisterConverter(new TimeSpanConverter());
            commands.RegisterConverter(new ImageFormatConverter());
            foreach (var module in _config.ModulesToLoad)
            {
                try
                {
                    var type = Type.GetType(module);
                    if (type.GetInterfaces().Contains(typeof(IRequireFonts)))
                    {
                        var fonts = (string[])type.GetProperty("RequiredFontFamilies").GetValue(null);
                        if (!CheckIfAllFontsAreHere(fonts))
                        {
                            _log.Information(
                                "Module {Module} won't be loaded as its requirements weren't met: the font/fonts {Fonts} is/are missing",
                                module, string.Join(',', fonts));
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
                            _log.Information("Module {Module} won't be loaded as its requirements weren't met", module);
                        }
                    }
                    else
                    {
                        commands.RegisterCommands(type);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex, "Failed to load module {Module} Exception occured", module);
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
                            var type = assembly.GetType(module.Value);
                            if (type.GetInterfaces().Contains(typeof(IRequireFonts)))
                            {
                                var fonts = (string[])type.GetProperty("RequiredFontFamilies").GetValue(null);
                                if (!CheckIfAllFontsAreHere(fonts))
                                {
                                    _log.Information(
                                        "Module {Module} won't be loaded as its requirements weren't met, the font/fonts {Fonts} is/are missing",
                                        module, string.Join(',', fonts));
                                }
                            }

                            if (type.IsSubclassOf(typeof(SilverBotCommandModule)))
                            {
                                SilverBotCommandModule n = (SilverBotCommandModule)Activator.CreateInstance(type);
                                if (await n.ExecuteRequirements(_config))
                                {
                                    commands.RegisterCommands(type);
                                }
                                else
                                {
                                    _log.Information("Module {Module} won't be loaded as its requirements weren't met",
                                        module.Value);
                                }
                            }
                            else
                            {
                                commands.RegisterCommands(type);
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex, "Failed to load module {Module} Exception occured", module.Value);
                        }
                    }
                }
                else
                {
                    _log.Information("Modules from the {File} won't be loaded as its file doesn't exist", group.Key);
                }
            }

            commands.CommandErrored += Commands_CommandErrored;
            commands.CommandExecuted += Commands_CommandExecuted;

            #endregion Registering Commands

            if (_config.UseSlashCommands)
            {
                if (Debugger.IsAttached)
                {
                    slash.RegisterCommands<GeneralCommands>(_config.ServerId);
                    if (_config.AllowPublicWebshot)
                    {
                        if (_config.BrowserType == 0)
                        {
                            _log.Information(
                                "You have not set-up a browser for silverbot to use. As such the public webshot slash-command will not be registered");
                        }
                        else
                        {
                            slash.RegisterCommands<WebshotSlash>(_config.ServerId);
                        }
                    }
                }
                else
                {
                    slash.RegisterCommands<GeneralCommands>();
                    if (_config.AllowPublicWebshot)
                    {
                        if (_config.BrowserType == 0)
                        {
                            _log.Information(
                                "You have not set-up a browser for silverbot to use. As such the public webshot slash-command will not be registered");
                        }
                        else
                        {
                            slash.RegisterCommands<WebshotSlash>();
                        }
                    }
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
            _log.Verbose("Waiting for client to connect");
            while (isConnected)
            {
                //intentional empty statement
            }
            await Task.Delay(2000);
            await _discord.UpdateStatusAsync(new("console logs while connecting to lavalink", ActivityType.Watching));
            if (_config.UseLavaLink)
            {
                await _audioService.InitializeAsync();
                if (!_config.SitInVc)
                {
                    _trackingService.BeginTracking();
                }
                if (IsNotNullAndIsNotB(_config.FridayTextChannel, 0) &&
                    IsNotNullAndIsNotB(_config.FridayVoiceChannel, 0))
                {
                    CancellationTokenSource s = new();
                    RunningTasks.Add("WaitForFridayTask", new(Task.Run(() => WaitForFridayAsync(s.Token), s.Token), s));
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
            if (_config.EnableServerStatistics)
            {
                CancellationTokenSource s = new();
                RunningTasks.Add("StatisticsTask", new(Task.Run(() => StatisticsMainAsync(s.Token), s.Token), s));
            }
            CancellationTokenSource ets = new();
            RunningTasks.Add("EventsTask", new(Task.Run(RunEventsAsync, ets.Token), ets));

            if (_config.HostWebsite)
            {
                await _discord.UpdateStatusAsync(new("console logs while launching the website module",
               ActivityType.Watching));
                _log.Information("Creating host");

                #region Website Fun Time

                var _host = Host.CreateDefaultBuilder(args).ConfigureServices(s =>
                {
                    foreach (var e in services)
                    {
                        _log.Verbose("Giving an instance of {svcname} to the host", e.ServiceType.Name);
                        s.Add(e);
                    }
                    _log.Verbose("Giving an instance of the discord client to the host");

                    s.AddSingleton(_discord);
                    if (_config.AzureSignalR)
                    {
                        _log.Verbose("Adding azure signalr");
                        services.AddSignalR().AddAzureSignalR();
                    }
                })
                    .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<WebpageStartup>()).UseSerilog(_log)
                    .Build();

                #endregion Website Fun Time

                CancellationTokenSource wts = new();
                RunningTasks.Add("WebsiteTask", new(Task.Run(async () => await _host.RunAsync(wts.Token), wts.Token), wts));
            }
            _log.Information("Booted up");
            while (true)
            {
                _log.Verbose("Updating the status to a random one");
                //update the status to some random one
                await _discord.UpdateStatusAsync(ArrayUtils.RandomFromArray(_config.Splashes)
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

        private static async Task Slash_SlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e)
        {
            async Task RespondWithContent(string content, bool ephermal = true)
            {
                await e.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().AsEphemeral(ephermal).WithContent(content));
            }

            var config = ServiceProvider.GetService<Config>();
            if (config is { SendErrorsThroughSegment: true })
            {
                var analytics = ServiceProvider.GetService<IAnalyse>();
                if (analytics is not null)
                {
                    await analytics.EmitEvent(e.Context.User, "SlashCommandErrored", new Dictionary<string, object>()
                    {
                        {"commandname", e.Context.CommandName},
                        {"error", e.Exception}
                    });
                }
            }

            if (e.Context.Channel.IsPrivate || e.Context.Channel
                    .PermissionsFor(await e.Context.Guild.GetMemberAsync(sender.Client.CurrentUser.Id))
                    .HasPermission(Permissions.SendMessages))
            {
                if (e.Exception is CommandNotFoundException)
                {
                    //we do not do anything if it is a nonexistent command, i would have liked it to be a user only visible message but discord is shit
                    return;
                }
                else
                {
                    var lang = await Language.GetLanguageFromCtxAsync(e.Context);
                    switch (e.Exception)
                    {
                        case ChecksFailedException cfe when cfe.FailedChecks.Count is 1:
                            await RespondWithContent(RenderErrorMessageForAttribute(cfe.FailedChecks[0], lang,
                                e.Context.Guild != null, e));
                            break;

                        case ChecksFailedException cfe:
                            {
                                var builder = new DiscordEmbedBuilder().WithTitle(lang.ChecksFailed);
                                var pages = cfe.FailedChecks.Select((t, i) => new Page(embed: builder
                                        .WithFooter($"{i + 1} / {cfe.FailedChecks.Count}")
                                        .WithDescription(
                                            RenderErrorMessageForAttribute(t, lang, e.Context.Guild != null, e))))
                                    .ToList();
                                var interactivity = e.Context.Client.GetInteractivity();
                                await interactivity.SendPaginatedMessageAsync(e.Context.Channel, e.Context.User, pages,
                                    token: new CancellationToken());
                                break;
                            }
                        case InvalidOverloadException:
                        case ArgumentException { Message: "Could not find a suitable overload for the command." }:
                            await RespondWithContent(string.Format(lang.InvalidOverload, e.Context.CommandName));
                            break;

                        case InvalidOperationException { Message: "No matching subcommands were found, and this group is not executable." }:
                            await RespondWithContent(lang.NoMatchingSubcommandsAndGroupNotExecutable);
                            break;

                        case UnknownImageFormatException:
                            await RespondWithContent(lang.UnknownImageFormat);
                            break;

                        case AttachmentCountIncorrectException aa when aa.AttachmentCount == AttachmentCountIncorrect.TooManyAttachments:
                            await RespondWithContent(lang.WrongImageCount);
                            break;

                        case AttachmentCountIncorrectException:
                            await RespondWithContent(lang.NoImageGeneric);
                            break;

                        default:
                            await RespondWithContent(lang.GeneralException);
                            break;
                    }
                }
            }

            _log.Error(e.Exception,
                "Error `{ExceptionName}` encountered.\nGuild `{GuildId}`, channel `{ChannelId}`, user `{UserID}`\n```\nNA\n```", e.Exception.GetType().FullName, e.Context.Guild?.Id.ToString() ?? "None", e.Context.Channel?.Id.ToString(), e.Context.User?.Id.ToString() ?? "None");
        }

        public static Dictionary<string, string> GetStringDictionary(DiscordClient client)
        {
            return new Dictionary<string, string>
            {
                ["GuildCount"] = client.Guilds.Values.LongCount().ToString(),
                ["Platform"] = RuntimeInformation.ProcessArchitecture.ToString()
            };
        }

        private static string RemoveStringFromEnd(string a, string sub)
        {
            if (a.EndsWith(sub))
            {
                a = a[..a.LastIndexOf(sub, StringComparison.Ordinal)];
            }

            return a;
        }

        /// <summary>
        ///     Render the error message for an Attribute
        /// </summary>
        /// <param name="checkBase">The attribute</param>
        /// <param name="lang">The language</param>
        /// <param name="isinguild">Was the command executed in a guild or in direct messages</param>
        /// <param name="e">Gives the raw command error arguments</param>
        /// <returns>A <see cref="string" /> containing the error message</returns>
        private static string RenderErrorMessageForAttribute(CheckBaseAttribute checkBase, Language lang,
            bool isinguild, CommandErrorEventArgs e)
        {
            var type = checkBase.GetType();
            if (type == typeof(RequireDjAttribute))
            {
                return lang.RequireDJCheckFailed;
            }
            if (type == typeof(RequireGuildAttribute))
            {
                return lang.RequireGuildCheckFailed;
            }
            if (type == typeof(RequireNsfwAttribute))
            {
                return lang.RequireNsfwCheckFailed;
            }
            if (type == typeof(RequireOwnerAttribute))
            {
                return lang.RequireOwnerCheckFailed;
            }
            return checkBase switch
            {
                RequireRolesAttribute requireRolesAttribute when requireRolesAttribute.RoleNames.Count == 1 => string.Format(lang.RequireRolesCheckFailedSG, requireRolesAttribute.RoleNames[0]),
                RequireRolesAttribute requireRolesAttribute => string.Format(lang.RequireRolesCheckFailedPL, requireRolesAttribute.RoleNames.Humanize()),
                RequireBotPermissionsAttribute requireBotPermissions when !(requireBotPermissions.IgnoreDms && isinguild) => lang.RequireGuildCheckFailed,
                RequireBotPermissionsAttribute requireBotPermissions when Enum.IsDefined(requireBotPermissions.Permissions) &&
requireBotPermissions.Permissions != Permissions.All => string.Format(lang.RequireBotPermisionsCheckFailedSG,
requireBotPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequireBotPermissionsAttribute requireBotPermissions => string.Format(lang.RequireBotPermisionsCheckFailedPL,
requireBotPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequireUserPermissionsAttribute userPermissions when !(userPermissions.IgnoreDms && isinguild) => lang.RequireGuildCheckFailed,
                RequireUserPermissionsAttribute userPermissions when Enum.IsDefined(userPermissions.Permissions) && userPermissions.Permissions != Permissions.All => string.Format(lang.RequireUserPermisionsCheckFailedSG,
userPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequireUserPermissionsAttribute userPermissions => string.Format(lang.RequireUserPermisionsCheckFailedPL,
userPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequirePermissionsAttribute userAndBotPermissions when !(userAndBotPermissions.IgnoreDms && isinguild) => lang.RequireGuildCheckFailed,
                RequirePermissionsAttribute userAndBotPermissions when Enum.IsDefined(userAndBotPermissions.Permissions) &&
userAndBotPermissions.Permissions != Permissions.All => string.Format(lang.RequireBotAndUserPermisionsCheckFailedSG,
userAndBotPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequirePermissionsAttribute userAndBotPermissions => string.Format(lang.RequireBotAndUserPermisionsCheckFailedPL,
userAndBotPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequireAttachmentAttribute attachmentAttribute when e.Context.Message.Attachments.Count > attachmentAttribute.AttachmentCount => (string)typeof(Language).GetProperty(attachmentAttribute.MoreThenLang)?.GetValue(lang),
                RequireAttachmentAttribute attachmentAttribute => (string)typeof(Language).GetProperty(attachmentAttribute.LessThenLang)?.GetValue(lang),
                _ => string.Format(lang.CheckFailed, RemoveStringFromEnd(type.Name, "Attribute").Humanize()),
            };
        }

        private static string RenderErrorMessageForAttribute(CheckBaseAttribute checkBase, Language lang,
            bool isinguild, SlashCommandErrorEventArgs e)
        {
            var type = checkBase.GetType();
            if (type == typeof(RequireDjAttribute))
            {
                return lang.RequireDJCheckFailed;
            }
            if (type == typeof(RequireGuildAttribute))
            {
                return lang.RequireGuildCheckFailed;
            }
            if (type == typeof(RequireNsfwAttribute))
            {
                return lang.RequireNsfwCheckFailed;
            }
            if (type == typeof(RequireOwnerAttribute))
            {
                return lang.RequireOwnerCheckFailed;
            }
            return checkBase switch
            {
                RequireRolesAttribute requireRolesAttribute => requireRolesAttribute.RoleNames.Count == 1 ? string.Format(lang.RequireRolesCheckFailedSG, requireRolesAttribute.RoleNames[0]) : string.Format(lang.RequireRolesCheckFailedPL, requireRolesAttribute.RoleNames.Humanize()),
                RequireBotPermissionsAttribute requireBotPermissions when !(requireBotPermissions.IgnoreDms && isinguild) => lang.RequireGuildCheckFailed,
                RequireBotPermissionsAttribute requireBotPermissions when Enum.IsDefined(requireBotPermissions.Permissions) &&
requireBotPermissions.Permissions != Permissions.All => string.Format(lang.RequireBotPermisionsCheckFailedSG,
requireBotPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequireBotPermissionsAttribute requireBotPermissions => string.Format(lang.RequireBotPermisionsCheckFailedPL,
requireBotPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequireUserPermissionsAttribute userPermissions when !(userPermissions.IgnoreDms && isinguild) => lang.RequireGuildCheckFailed,
                RequireUserPermissionsAttribute userPermissions when Enum.IsDefined(userPermissions.Permissions) && userPermissions.Permissions != Permissions.All => string.Format(lang.RequireUserPermisionsCheckFailedSG,
userPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequireUserPermissionsAttribute userPermissions => string.Format(lang.RequireUserPermisionsCheckFailedPL,
userPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequirePermissionsAttribute userAndBotPermissions when !(userAndBotPermissions.IgnoreDms && isinguild) => lang.RequireGuildCheckFailed,
                RequirePermissionsAttribute userAndBotPermissions when Enum.IsDefined(userAndBotPermissions.Permissions) &&
userAndBotPermissions.Permissions != Permissions.All => string.Format(lang.RequireBotAndUserPermisionsCheckFailedSG,
userAndBotPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequirePermissionsAttribute userAndBotPermissions => string.Format(lang.RequireBotAndUserPermisionsCheckFailedPL,
userAndBotPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequireAttachmentAttribute => throw new NotSupportedException("Attachment checks are not supported for slash commands."),
                _ => string.Format(lang.CheckFailed, RemoveStringFromEnd(type.Name, "Attribute").Humanize()),
            };
        }

        private static async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            async Task RespondWithContent(string content)
            {
                await new DiscordMessageBuilder()
                    .WithReply(e.Context.Message.Id)
                    .WithContent(content)
                    .SendAsync(e.Context.Channel);
            }

            if (_config.SendErrorsThroughSegment)
            {
                var analytics = ServiceProvider.GetService<IAnalyse>();
                if (analytics is not null)
                {
                    await analytics.EmitEvent(e.Context.User, "SlashCommandErrored", new Dictionary<string, object>()
                    {
                        {"commandname", e.Context.Command.Name},
                        {"error", e.Exception}
                    });
                }
            }

            if (e.Context.Channel.IsPrivate || e.Context.Channel
                    .PermissionsFor(await e.Context.Guild.GetMemberAsync(sender.Client.CurrentUser.Id))
                    .HasPermission(Permissions.SendMessages))
            {
                if (e.Exception is CommandNotFoundException)
                {
                    //we do not do anything if it is a nonexistent command, i would have liked it to be a user only visible message but discord is shit
                    return;
                }
                else
                {
                    var lang = await Language.GetLanguageFromCtxAsync(e.Context);
                    switch (e.Exception)
                    {
                        case ChecksFailedException cfe when cfe.FailedChecks.Count is 1:
                            await RespondWithContent(RenderErrorMessageForAttribute(cfe.FailedChecks[0], lang,
                                e.Context.Guild != null, e));
                            break;

                        case ChecksFailedException cfe:
                            {
                                var embedBuilder = new DiscordEmbedBuilder().WithTitle(lang.ChecksFailed);
                                var pages = cfe.FailedChecks.Select((t, i) => new Page(embed: embedBuilder.WithFooter($"{i + 1} / {cfe.FailedChecks.Count}")
                                        .WithDescription(RenderErrorMessageForAttribute(t, lang, e.Context.Guild != null, e))))
                                    .ToList();

                                var interactivity = e.Context.Client.GetInteractivity();
                                await interactivity.SendPaginatedMessageAsync(e.Context.Channel, e.Context.User, pages,
                                    token: new CancellationToken());
                                break;
                            }
                        case InvalidOverloadException:
                        case ArgumentException { Message: "Could not find a suitable overload for the command." }:
                            await RespondWithContent(string.Format(lang.InvalidOverload, e.Context.Command.Name));
                            break;

                        case InvalidOperationException { Message: "No matching subcommands were found, and this group is not executable." }:
                            await RespondWithContent(lang.NoMatchingSubcommandsAndGroupNotExecutable);
                            break;

                        case UnknownImageFormatException:
                            await RespondWithContent(lang.UnknownImageFormat);
                            break;

                        case AttachmentCountIncorrectException { AttachmentCount: AttachmentCountIncorrect.TooManyAttachments }:
                            await RespondWithContent(lang.WrongImageCount);
                            break;

                        case AttachmentCountIncorrectException aa:
                            await RespondWithContent(lang.NoImageGeneric);
                            break;

                        default:
                            await RespondWithContent(lang.GeneralException);
                            break;
                    }
                }
            }

            _log.Error(e.Exception,
                "Error `{ExceptionName}` encountered.\nGuild `{GuildId}`, channel `{ChannelId}`, user `{UserId}`\n```\n{MessageContent}\n```", e.Exception.GetType().FullName, e.Context.Guild?.Id.ToString() ?? "None", e.Context.Channel?.Id.ToString(), e.Context.User?.Id.ToString() ?? "None", e.Context.Message.Content);
        }

        public static Task RunEmojiEvent(PlannedEvent @event)
        {
            if (@event.Type != PlannedEventType.EmojiPoll)
            {
                throw new ArgumentException("The parameter @event needs to be an EmojiPoll", nameof(@event));
            }
            return RunEmojiEventAsync(@event);
        }

        public static async Task RunEmojiEventAsync(PlannedEvent @event)
        {
            var channel = await _discord.GetChannelAsync(@event.ChannelID);
            if (@event.ResponseMessageID != null)
            {
                var msg = await channel.GetMessageAsync((ulong)@event.ResponseMessageID);
                var bob = new DiscordEmbedBuilder(msg.Embeds[0]);
                var yesVotes =
                    (await msg.GetReactionsAsync(DiscordEmoji.FromName(_discord, ":everybodyvotes:"))).Count(x =>
                        x.Id != _discord.CurrentUser.Id && !x.IsBot);
                var noVotes =
                    (await msg.GetReactionsAsync(DiscordEmoji.FromName(_discord, ":nobodyvotes:"))).Count(x =>
                        x.Id != _discord.CurrentUser.Id && !x.IsBot);
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

                pollResultText.Append("**\nYes:").Append(yesVotes).Append(" No:").Append(noVotes).Append(" Undecided: ")
                    .Append(channel.Guild.Members.Count(x => !x.Value.IsBot) - (yesVotes + noVotes));
                bob.WithDescription(pollResultText.ToString());
                await msg.ModifyAsync(bob.Build());
            }
            @event.Handled = true;
        }

        public static Task RunGiveAwayEvent(PlannedEvent @event)
        {
            if (@event.Type != PlannedEventType.EmojiPoll)
            {
                throw new ArgumentException("The parameter evnt needs to be an GiveAway", nameof(@event));
            }
            return RunGiveAwayEventAsync(@event);
        }

        public static async Task RunGiveAwayEventAsync(PlannedEvent @event)
        {
            var channel = await _discord.GetChannelAsync(@event.ChannelID);
            if (@event.ResponseMessageID != null)
            {
                var msg = await channel.GetMessageAsync((ulong)@event.ResponseMessageID);
                var people =
                    (await msg.GetReactionsAsync(DiscordEmoji.FromName(_discord, ":everybodyvotes:"))).Where(x =>
                        x.Id != _discord.CurrentUser.Id && !x.IsBot);
                var discordUsers = people as DiscordUser[] ?? people.ToArray();
                if (discordUsers.Length == 0)
                {
                    await channel.SendMessageAsync("Nobody reacted in time :(");
                }
                else
                {
                    await channel.SendMessageAsync(
                        $"{discordUsers[RandomGenerator.Next(0, discordUsers.Length)].Mention} won {msg.Embeds[0].Title}");
                }
            }
            @event.Handled = true;
        }

        public static async Task RunEventsAsync()
        {
            var dbctx = ServiceProvider.GetRequiredService<DatabaseContext>();
            while (true)
            {
                try
                {
                    var arr = dbctx.plannedEvents.ToArray();
                    foreach (var evnt in arr)
                    {
                        if (!evnt.Handled)
                        {
                            if (evnt.Time > DateTime.Now)
                            {
                                continue;
                            }

                            try
                            {
                                switch (evnt.Type)
                                {
                                    case PlannedEventType.EmojiPoll:
                                        CancellationTokenSource cts = new(15 * 1000);
                                        RunningTasksOfSecondRow.Add(Guid.NewGuid(), new(Task.Run(() => RunEmojiEvent(evnt), cts.Token), cts));
                                        break;

                                    case PlannedEventType.GiveAway:
                                        CancellationTokenSource cts2 = new(15 * 1000);
                                        RunningTasksOfSecondRow.Add(Guid.NewGuid(), new(Task.Run(() => RunGiveAwayEvent(evnt), cts2.Token), cts2));
                                        break;

                                    case PlannedEventType.Reminder:
                                        throw new NotImplementedException();
                                    default:
                                        throw new ArgumentOutOfRangeException(nameof(evnt.Type));
                                }
                            }
                            catch (Exception e)
                            {
                                _log.Error(e, "exception happened in events thread in the switch case");
                            }
                        }
                        else
                        {
                            _log.Verbose("removed an {EventType}", evnt.Type);
                            dbctx.plannedEvents.Remove(evnt);
                            await dbctx.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.Error(e, "exception happened in events thread");
                }

                await Task.Delay(3000);
            }
        }

        public static async Task StatisticsMainAsync(CancellationToken ct = default)
        {
            while (true)
            {
                ct.ThrowIfCancellationRequested();
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
                                            await child.ModifyAsync(a => a.Name = item3[e].Serialize(server));
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
                    ct.ThrowIfCancellationRequested();
                }
                ct.ThrowIfCancellationRequested();
                await Task.Delay(1800000, ct);
                ct.ThrowIfCancellationRequested();
            }
        }

        public static async Task WaitForFridayAsync(CancellationToken ct = default)
        {
            while (true)
            {
                ct.ThrowIfCancellationRequested();
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
                ct.ThrowIfCancellationRequested();
                await Task.Delay(1000, ct);
                ct.ThrowIfCancellationRequested();
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