using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SilverBotDS
{
    internal class Program
    {
        private static readonly Config.Config config = Config.Config.Get();
        private static readonly bool shitlog = false;

        private static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        public static Config.Config GetConfig()
        {
            return config;
        }

        private static async Task MainAsync()
        {
            DiscordClient discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = config.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });
            discord.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30)
            });
            discord.MessageCreated += Discord_MessageCreated;
            //var dsconfig = new DiscordSlashConfiguration
            //  {
            //      Client = discord,
            //     Token = config.Token
            //  };
            //  var client = new DiscordSlashClient(dsconfig);
            CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = config.Prefix
            });

            commands.RegisterCommands<genericcommands>();

            await discord.ConnectAsync();
            //await client.StartAsync();
            NuGetUtils.Datum[] data = await NuGetUtils.SearchAsync("DSharpPlus");
            foreach (NuGetUtils.Datum thingy in data)
            {
                Console.WriteLine($"{thingy.title} {thingy.description} {thingy.iconUrl}");
            }
            await Task.Delay(-1);
        }

        private static async Task Discord_MessageCreated(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            if (shitlog)
            {
                logging.log(e.Message);
            }
        }
    }
}