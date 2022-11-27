using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace SilverBotDS.Converters;

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