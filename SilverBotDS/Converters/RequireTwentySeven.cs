using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Linq;
using System.Threading.Tasks;

namespace SilverBotDS.Converters
{
    public class RequireTwentySeven : CheckBaseAttribute
    {
        public RequireTwentySeven()
        {
        }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => Task.FromResult(Program.GetConfig().TwentyFourSevenGuilds.Contains(ctx.Guild.Id));
    }
}