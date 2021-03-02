using ELIZA.NET;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SilverBotDS.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    internal class AICommands : BaseCommandModule
    {
        private static readonly ELIZALib eliza = new(Environment.CurrentDirectory + "eliza.json");

        [Command("ai")]
        [Description("Talk to the ai")]
        public async Task GreetCommand(CommandContext ctx, [RemainingText] string input)
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                             .WithContent("```\n" + eliza.GetResponse(input) + "```")
                                             .SendAsync(ctx.Channel);
        }
    }
}