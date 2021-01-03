using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Threading.Tasks;

namespace SilverBotDS
{
    internal class Program
    {
        private static readonly Config.Config config = Config.Config.Get();
        private static readonly bool shitlog = false;

        private static void Main()
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
            CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = config.Prefix
            });

            commands.RegisterCommands<Genericcommands>();

            await discord.ConnectAsync();
            NuGetUtils.Datum[] data = await NuGetUtils.SearchAsync("DSharpPlus");
            foreach (NuGetUtils.Datum thingy in data)
            {
                Console.WriteLine($"{thingy.Title} {thingy.Description} {thingy.IconUrl}");
            }
            await Task.Delay(-1);
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