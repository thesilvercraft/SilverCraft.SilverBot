using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using SilverBotDS.Config;
namespace SilverBotDS
{
    class Program
    {
       static Config.Config config = Config.Config.Get();
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }
      public  static Config.Config GetConfig()
        {
            return config;
        }
        static async Task MainAsync()
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = config.Token,
                TokenType = TokenType.Bot,
                Intents= DiscordIntents.All
            });
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = config.Prefix
                
            });

            commands.RegisterCommands<genericcommands>();

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
