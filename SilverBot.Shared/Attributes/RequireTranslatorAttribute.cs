/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SilverBotDS.Objects;

namespace SilverBotDS.Attributes;

public class RequireTranslatorAttribute : CheckBaseAttribute
{
    public RequireTranslatorAttribute(bool inchannel = false)
    {
        InChannel = inchannel;
    }

    private bool InChannel { get; }

    public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        var conf = (Config)ctx.CommandsNext.Services.GetService(typeof(Config));
        return await IsTranslator(conf, ctx.Client, ctx.User.Id, InChannel ? ctx.Channel.Id : null);
    }

    public static async Task<bool> IsTranslator(Config cnf, DiscordClient client, ulong userid, ulong? channelid = null)
    {
        if (!(!channelid.HasValue || channelid == cnf.TranslatorModeChannel))
        {
            return false;
        }
        var gld = await client.GetGuildAsync(cnf.ServerId);
        return gld.Members.ContainsKey(userid)
               && gld.Members[userid].Roles
               .Any(x => x.Id == cnf.TranslatorRoleId);
    }
}