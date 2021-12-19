using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace SilverBotDS.Converters;

public class RequireDJAttribute : CheckBaseAttribute
{
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        return help && ctx.Channel.IsPrivate ||
               ctx.Member.Roles.Any(e =>
                   e.CheckPermission(Permissions.ManageChannels) == PermissionLevel.Allowed ||
                   e.Name.Contains("dj", StringComparison.OrdinalIgnoreCase)) ||
               ctx.Member.VoiceState?.Channel?.Users?.LongCount(x => !x.IsBot) == 1
            ? Task.FromResult(true)
            : Task.FromResult(false);
    }
}