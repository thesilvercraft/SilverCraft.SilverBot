using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Linq;
using System.Threading.Tasks;

namespace SilverBotDS.Converters
{
    public class RequireDJAttribute : CheckBaseAttribute
    {
        public RequireDJAttribute()
        {
        }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => Task.FromResult(!ctx.Channel.IsPrivate && (ctx.Member.Roles.Any(e => e.CheckPermission(DSharpPlus.Permissions.ManageChannels) == DSharpPlus.PermissionLevel.Allowed || e.Name.ToLower().Contains("dj")) || ctx.Member.VoiceState.Channel.Users.Where(x => !x.IsBot).LongCount() == 1));
    }
}