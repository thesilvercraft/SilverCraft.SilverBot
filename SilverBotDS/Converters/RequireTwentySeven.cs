using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Linq;
using System.Threading.Tasks;

namespace SilverBotDS.Converters
{
    public class RequireTwentySevenAttribute : CheckBaseAttribute
    {
        public RequireTwentySevenAttribute()
        {
        }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => Task.FromResult(Program.GetConfig().TwentyFourSevenGuilds.Contains(ctx.Guild.Id));
    }
}