using CefSharp;
using CefSharp.OffScreen;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.VoiceNext;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SilverBotDS
{
    internal class Program
    {
        private static readonly Config config = GetNewConfig();
        private static readonly bool shitlog = false;

        private static Config GetNewConfig()
        {
            MainLogLine("Loading config for some reason");
            var task = Config.GetAsync();
            task.Wait();
            var res = task.Result;
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
            MainLogLine("Adding resolver so cefsharp works");
            //Add the resolver so cefsharp works
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            MainLogLine("Initialising cefsharp");
            //Do cefsharp things
            LoadApp();
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

        private static readonly DiscordWebhookClient wc = new();

        public static async Task SendLogAsync(string text, List<DiscordEmbed> embeds)
        {
            var whb = new DiscordWebhookBuilder();
            whb.AddEmbeds(embeds);
            whb.WithContent(text);
            await wc.BroadcastMessageAsync(whb);
        }

        /// <summary>
        /// Do cefsharp initialisation
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void LoadApp()
        {
            CefSettings settings = new CefSettings
            {
                // Set BrowserSubProcessPath based on app bitness at runtime
                BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                   Environment.Is64BitProcess ? "x64" : "x86",
                                                  "CefSharp.BrowserSubprocess.exe"),
                UserAgent = $"Mozilla/5.0+ (compatible; SilverBotCEF/{VersionInfo.VNumber}; {ThisAssembly.Git.RepositoryUrl})"
            };
            //Enable audio if you want
            //settings.EnableAudio();
            // Make sure you set performDependencyCheck false
            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
        }

        // Will attempt to load missing assembly from either x86 or x64 subdir
        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                       Environment.Is64BitProcess ? "x64" : "x86",
                                                       assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : null;
            }

            return null;
        }

        private static DiscordClient discord;

        private static async Task MainAsync()
        {
            //add log at first
            _ = MainLogLineAsync("Adding the logging webhook");
            UriBuilder uri = new UriBuilder(config.LogWebhook);
            await wc.AddWebhookAsync(uri.Uri);
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
            if (shitlog)
            {
                _ = MainLogLineAsync("Setup unimplemented message tracking:tm:");
                discord.MessageCreated += Discord_MessageCreated;
            }
            //Tell our cute client to use commands or in other words become a working class member of society
            _ = MainLogLineAsync("Initialising Commands");
            CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = config.Prefix
            });
            //Register our commands
            _ = MainLogLineAsync("Regisitring Commands&Converters");
            commands.RegisterConverter(new SDImageConverter());
            commands.RegisterCommands<Genericcommands>();
            commands.RegisterCommands<Emotes>();
            commands.RegisterCommands<Moderation>();
            commands.RegisterCommands<Giphy>();
            commands.RegisterCommands<Imagemodule>();
            commands.RegisterCommands<AdminCommands>();
            commands.RegisterCommands<OwnerOnly>();
            commands.RegisterCommands<SteamCommands>();
            commands.RegisterCommands<Fortnite>();
            commands.RegisterCommands<AudioNLN>();
            //Also enable voice
            _ = MainLogLineAsync("Enabling voice with incoming turned on");
            var voice = discord.UseVoiceNext(new VoiceNextConfiguration()
            {
                EnableIncoming = true
            });
            _ = MainLogLineAsync("Launching lavalink");
            //Setting an instance of ProcessStartInfo class

            // under System.Diagnostic Assembly Reference

            ProcessStartInfo pro = new ProcessStartInfo();

            //Setting the FileName to be Started like in our

            //Project we are just going to start a CMD Window.

            pro.FileName = config.JavaLoc;
            pro.Arguments = " -jar Lavalink.jar";
            pro.UseShellExecute = true;
            //Instead of using the above two line of codes, You

            // can just use the code below:

            // ProcessStartInfo pro = new ProcessStartInfo("cmd.exe");

            //Creating an Instance of the Process Class

            // which will help to execute our Process

            Process proStart = new Process();

            //Setting up the Process Name here which we are

            // going to start from ProcessStartInfo

            proStart.StartInfo = pro;

            //Calling the Start Method of Process class to

            // Invoke our Process viz 'cmd.exe'

            proStart.Start();

            //🥁🥁🥁 drumroll please
            var endpoint = new ConnectionEndpoint
            {
                Hostname = "127.0.0.1", // From your server configuration.
                Port = 2333 // From your server configuration
            };
            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "youshallnotpass", // From your server configuration.
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };
            var lavalink = discord.UseLavalink();

            _ = MainLogLineAsync("Connecting to discord");
            discord.Ready += Discord_Ready;
            await discord.ConnectAsync();
            //We have achived pog
            _ = MainLogLineAsync("Waiting 3s");
            await Task.Delay(3000);
            await lavalink.ConnectAsync(lavalinkConfig);
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

        private static async Task Discord_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e)
        {
        }

        private static async Task Discord_MessageCreated(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            await Logging.Log(e.Message).ConfigureAwait(false);
        }
    }
}