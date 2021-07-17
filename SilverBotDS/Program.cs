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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SDBrowser;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using SilverBotDS.Commands;
using SilverBotDS.Commands.Gamering;
using SilverBotDS.Commands.SBCalendar;
using SilverBotDS.Converters;
using SilverBotDS.Exceptions;
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
using SDiscordSink;
using SilverBotDS.Objects.Database.Classes;
using SilverBotDS.Objects.Classes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using DSharpPlus.CommandsNext.Exceptions;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Serilog.Events;
using SixLabors.Fonts;

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
                Console.WriteLine("Serialised en.json");
                Environment.Exit(70);
                return;
            }
            if (Debugger.IsAttached && !Environment.CurrentDirectory.EndsWith("bin\\Debug\\net5.0"))
            {
                Environment.CurrentDirectory += "\\bin\\Debug\\net5.0";
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
            var logfactory = new LoggerConfiguration()
                         .MinimumLevel.Information()
                         .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) //So uh asp.net LOVES spamming logs
                         .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                         .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, shared: true);
            if (!(id == null || string.IsNullOrEmpty(token)))
            {
                logfactory.WriteTo.DiscordSink(new Tuple<ulong, string>((ulong)id, token));
            }
            log = logfactory.CreateLogger();
            if (!CheckIfAllFontsAreHere())
            {
                log.Fatal("You do not have all reqired fonts to run silverbot, on windows you have to install Diavlo Light and Futura Extra Black Condensed while on linux you have to install the base windows fonts (using \"sudo apt-get install ttf-mscorefonts-installer\"), Diavlo Light and Futura Extra Black Condensed. You might have to find all of the fonts in a TTF format.");
            }
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
                Timeout = TimeSpan.FromSeconds(30)
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
            #endregion
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
            #endregion
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
            #region Registering Commands
            commands.SetHelpFormatter<CustomHelpFormatter>();
            log.Verbose("Registering Commands&Converters");
            commands.RegisterConverter(new SdImageConverter());
            commands.RegisterConverter(new SColorConverter());
            commands.RegisterConverter(new LoopSettingsConverter());
            commands.RegisterConverter(new SongOrSongsConverter());
            commands.RegisterConverter(new TimeSpanConverter());
            commands.RegisterCommands<Anime>();
            commands.RegisterCommands<Genericcommands>();
            commands.RegisterCommands<Emotes>();
            commands.RegisterCommands<ModCommands>();
            commands.RegisterCommands<Giphy>();
            commands.RegisterCommands<ImageModule>();
            commands.RegisterCommands<AdminCommands>();
            commands.RegisterCommands<Experience>();
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
                commands.RegisterCommands<BibiLib>();
            }
            if (config.UseLavaLink)
            {
                commands.RegisterCommands<Audio>();
            }
            commands.RegisterCommands<MiscCommands>();
            commands.RegisterCommands<MinecraftModule>();
            commands.RegisterCommands<UserQuotesModule>();
            if (IsNotNullAndIsNotB(config.MicrosoftGraphClientId, "Graph-Client-Id-Here"))
            {
                commands.RegisterCommands<CalendarCommands>();
            }
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
            await discord.UpdateStatusAsync(new("console logs while connnecting to lavalink", ActivityType.Watching));
            if (config.UseLavaLink)
            {
                await audioService.InitializeAsync();
                if (!config.SitInVc)
                {
                    trackingService.BeginTracking();
                }
                if (IsNotNullAndIsNotB(config.FridayTextChannel, 0) && IsNotNullAndIsNotB(config.FridayVoiceChannel, 0) && config.UseLavaLink)
                {
                    _ = Task.Run(WaitForFridayAsync);
                }
            }
            await discord.UpdateStatusAsync(new("console logs while configuring server statistics", ActivityType.Watching));
            if (config.EnableServerStatistics)
            {
                _ = Task.Run(() => StatisticsMainAsync());
            }
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
            #endregion
            _ = Task.Run(async () => await host.RunAsync());
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
        public static Dictionary<string, string> GetStringDictionary(DiscordClient client)
        {
            return new Dictionary<string, string> { ["GuildCount"] = client.Guilds.Values.LongCount().ToString(), ["Platform"] = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString() };
        }
        private static string RemoveStringFromEnd(string a, string sub)
        {
            if(a.EndsWith(sub))
            {
                a = a.Substring(0, a.LastIndexOf(sub));
            }
            return a;
        }
        private static async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            if (e.Context.Channel.IsPrivate || e.Context.Channel.PermissionsFor(await e.Context.Guild.GetMemberAsync(sender.Client.CurrentUser.Id)).HasPermission(Permissions.SendMessages) )
            {
                if (e.Exception is CommandNotFoundException)
                {
                    //we do not do anything if it is a nonexistant command, i would have liked it to be a user only visible message but discord is shit
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
                                                             .WithContent(string.Format(lang.CheckFailed, RemoveStringFromEnd(cfe.FailedChecks[0].GetType().Name, "Attribute").Humanize()))
                                                             .SendAsync(e.Context.Channel);
                        }
                        else
                        {
                            var pages = new List<Page>();
                            var tempbuilder = new DiscordEmbedBuilder().WithTitle(lang.ChecksFailed);
                            for (var i = 0; i < cfe.FailedChecks.Count; i++)
                            {
                                pages.Add(new Page(embed: tempbuilder.WithFooter($"{i + 1} / {cfe.FailedChecks.Count}").WithDescription(RemoveStringFromEnd(cfe.FailedChecks[i].GetType().Name, "Attribute").Humanize())));
                            }
                            await e.Context.Channel.SendPaginatedMessageAsync(e.Context.Member, pages, timeoutoverride: new TimeSpan(0, 2, 0));
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
                    else
                    {
                        await new DiscordMessageBuilder()
                                                         .WithReply(e.Context.Message.Id)
                                                         .WithContent(lang.GeneralException)
                                                         .SendAsync(e.Context.Channel);
                    }
                }
            }
            log.Error(exception: e.Exception, "An exception occurred while trying to run {Command} with the raw arguments as {Raw}", e.Command.Name, e.Context.RawArgumentString);
        }

        private const string FridayUrl = "https://youtu.be/akT0wxv9ON8";
        private static int last_friday;

        public static async Task StatisticsMainAsync()
        {
            try
            {
                log.Verbose("Entered statistics method");
                while (true)
                {
                    var dbctx = serviceProvider.GetRequiredService<DatabaseContext>();
                    log.Verbose("Getting the settings about statistics");
                    var things = dbctx.GetStatisticSettings();
                    log.Verbose("Got the settings about statistics");
                    await foreach (var thing in things)
                    {
                        log.Verbose("Getting the guild with the id {Id}", thing.Item1);
                        try
                        {
                            var server = await discord.GetGuildAsync(thing.Item1);
                            log.Verbose("Got the guild with the id {Id}", thing.Item1);
                            log.Verbose("Getting the channel with the id {Id}", thing.Item2);
                            try
                            {
                                var category = server.Channels[(ulong)thing.Item2];
                                log.Verbose("Got the channel with the id {Id}", thing.Item2);
                                if (category.Type is ChannelType.Category)
                                {
                                    int e = 0;
                                    log.Verbose("Getting the children of the channel {Id}", thing.Item2);
                                    foreach (var child in category.Children)
                                    {
                                        if (thing.Item3.Count > e)
                                        {
                                            log.Verbose("Updating {Id}", child.Id);
                                            await child.ModifyAsync(a => a.Name = thing.Item3[e].Serialize(server));
                                        }
                                        e++;
                                    }
                                }
                                else
                                {
                                    log.Error("uhhhhhh awkward category.type is {CategoryType} and not Category for channel {ChannelId}", category.Type, thing.Item2);
                                }
                            }
                            catch (DSharpPlus.Exceptions.NotFoundException)
                            {
                                var dmchannel = await server.Owner.CreateDmChannelAsync();
                                await dmchannel.SendMessageAsync($"Hello silverbot here,\n it appears that you own `{server.Name}` and i just wanted to let you know that you will have to set the stats category again for stats to work as something broke.");
                                dbctx.SetServerStatsCategory(thing.Item1, null);
                            }
                        }
                        catch (DSharpPlus.Exceptions.NotFoundException)
                        {
                            // TODO: ADD TRACKING OF STUFF AND DELETE SERVER SETTINGS IF THIS FAILS 3 TIMES
                        }
                    }
                    await Task.Delay(1800000);
                }
            }
            catch (Exception e)
            {
                log.Error(e, "FOCK exception happened in stats thread");
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
                await Task.Delay(1000);
            }
        }

        public static async Task ExecuteFridayAsync()
        {
            var vchannel = await discord.GetChannelAsync(config.FridayVoiceChannel);
            if (audioService.HasPlayer((ulong)vchannel.GuildId))
            {
                BetterVoteLavalinkPlayer player = audioService.GetPlayer<BetterVoteLavalinkPlayer>((ulong)vchannel.GuildId);
                await player.DisconnectAsync();
            }
            BetterVoteLavalinkPlayer playere = await audioService.JoinAsync<BetterVoteLavalinkPlayer>((ulong)vchannel.GuildId, vchannel.Id, true);
            var track = await audioService.GetTrackAsync(FridayUrl, SearchMode.YouTube);
            await playere.PlayAsync(track);
            var channel = await discord.GetChannelAsync(config.FridayTextChannel);
            await channel.SendMessageAsync("It is Friday");
        }

        private static readonly string[] repeatstrings = { "anime", "canada", "fuck", "fock", "e", "https://media.discordapp.net/attachments/811583810264629252/824266450818695168/image0-1.gif", "h", "gaming", "quality fock", "fock you", "we will fock you", "<:kalorichan:839099093552332850>","kalorichan" };
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
        private static async Task SendRepeatedMessage(ulong msgid, string msgcontent, DiscordChannel chan, string filename = null, Stream stream = null)
        {
            if(string.IsNullOrEmpty(filename) || stream is null)
            {
                await new DiscordMessageBuilder().WithReply(msgid)
                                                 .WithContent(msgcontent)
                                                 .SendAsync(chan);
            }
            else
            {
                await new DiscordMessageBuilder().WithReply(msgid)
                                                 .WithContent(msgcontent)
                                                 .WithFile(filename, stream)
                                                 .SendAsync(chan);
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
                        case "fock":
                            await SendRepeatedMessage(
                                e.Message.Id, e.Message.Content, e.Channel, "fock.mp3",
                                Assembly.GetExecutingAssembly().GetManifestResourceStream("SilverBotDS.Templates.fock.mp3") ?? throw new TemplateReturningNullException("SilverBotDS.Templates.fock.mp3"));
                            return;
                        case "quality fock":
                            await SendRepeatedMessage(
                                e.Message.Id, e.Message.Content, e.Channel, "quality_fock.mp3",
                                Assembly.GetExecutingAssembly().GetManifestResourceStream("SilverBotDS.Templates.quality_fock.mp3") ?? throw new TemplateReturningNullException("SilverBotDS.Templates.quality_fock.mp3"));
                            return;
                        case "fock you":
                            await SendRepeatedMessage(
                                e.Message.Id, "fock you too", e.Channel, "fock.mp3",
                                Assembly.GetExecutingAssembly().GetManifestResourceStream("SilverBotDS.Templates.fock.mp3") ?? throw new TemplateReturningNullException("SilverBotDS.Templates.fock.mp3"));
                            return;
                        case "kalorichan":
                        case "<:kalorichan:839099093552332850>":
                            await e.Message.RespondAsync("<:kalorichan:839099093552332850>");
                            return;
                        case "we will fock you":
                            await new DiscordMessageBuilder().WithReply(e.Message.Id)
                             .WithContent("https://www.youtube.com/watch?v=lLN3caSQI1w")
                             .SendAsync(e.Channel);
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