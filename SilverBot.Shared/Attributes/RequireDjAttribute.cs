/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace SilverBotDS.Converters;

public class RequireDjAttribute : CheckBaseAttribute
{
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        return Task.FromResult(help && ctx.Channel.IsPrivate ||
               ctx.Member!=null && ctx.Member.Roles.Any(e =>
                   e.CheckPermission(Permissions.ManageChannels) == PermissionLevel.Allowed ||
                   e.Name.Contains("dj", StringComparison.OrdinalIgnoreCase)) ||
               ctx.Member.VoiceState?.Channel?.Users?.LongCount(x => !x.IsBot) == 1);
    }
}
