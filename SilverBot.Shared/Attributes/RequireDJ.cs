using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;

namespace SilverBotDS.Converters;

public class RequireDjAttribute : CheckBaseAttribute
{
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        return Task.FromResult(help && ctx.Channel.IsPrivate ||
               ctx.Member.Roles.Any(e =>
                   e.CheckPermission(Permissions.ManageChannels) == PermissionLevel.Allowed ||
                   e.Name.Contains("dj", StringComparison.OrdinalIgnoreCase)) ||
               ctx.Member.VoiceState?.Channel?.Users?.LongCount(x => !x.IsBot) == 1);
    }
}
public class RequireDjSlashAttribute : SlashCheckBaseAttribute
{

  
    public override Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        return Task.FromResult(
              ctx.Member.Roles.Any(e =>
                  e.CheckPermission(Permissions.ManageChannels) == PermissionLevel.Allowed ||
                  e.Name.Contains("dj", StringComparison.OrdinalIgnoreCase)) ||
              ctx.Member.VoiceState?.Channel?.Users?.LongCount(x => !x.IsBot) == 1);
    }
}