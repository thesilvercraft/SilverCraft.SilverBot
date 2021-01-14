using CefSharp;
using CefSharp.OffScreen;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SilverBotDS
{
    internal class Program
    {
        private static readonly Config.Config config = Config.Config.Get();
        private static readonly bool shitlog = false;

        private static void Main()
        {
            //Add the resolver so cefsharp works
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            //Do cefsharp things
            LoadApp();
            //Start the main async task
            MainAsync().GetAwaiter().GetResult();
        }

        public static Config.Config GetConfig()
        {
            return config;
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
                                                   "CefSharp.BrowserSubprocess.exe")
            };

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

        private static async Task MainAsync()
        {
            //Make us a little cute client
            DiscordClient discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = config.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });
            //Tell our client to initialize interactivity
            discord.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30)
            });
            //set up logging?
            discord.MessageCreated += Discord_MessageCreated;
            //Tell our cute client to use commands or in other words become a working class member of society
            CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = config.Prefix
            });
            //Register our commands
            commands.RegisterCommands<Genericcommands>();
            commands.RegisterCommands<Emotes>();
            commands.RegisterCommands<Moderation>();
            commands.RegisterCommands<giphy>();
            commands.RegisterCommands<Imagemodule>();
            commands.RegisterCommands<AdminCommands>();
            commands.RegisterCommands<OwnerOnly>();
            commands.RegisterCommands<SteamCommands>();
            //🥁🥁🥁 drumroll please
            await discord.ConnectAsync();
            //We have achived pog
            await Task.Delay(3000);
            while (true)
            {
                //update the status to some random one
                await discord.UpdateStatusAsync(splashes.GetSingle());
                //wait 10s
                await Task.Delay(10000);
                //repeat🔁
            }
        }

        private static async Task Discord_MessageCreated(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            if (shitlog)
            {
                Logging.Log(e.Message);
            }
        }
    }
}