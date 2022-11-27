/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using SilverBotDS.Objects;

namespace SilverBotDS.Commands;

public class CodeEnv
{
    public CodeEnv(CommandContext context, Config config, DatabaseContext dbctx)
    {
        Ctx = context;
        Member = Ctx.Member;
        User = Ctx.User;
        Guild = Ctx.Guild;
        VerString = VersionInfo.VNumber;
        Client = Ctx.Client;
        Config = config;
        ExConfig = new Config();
        DbContext = dbctx;
    }

    public CommandContext Ctx { get; init; }
    public DiscordMember Member { get; init; }
    public DiscordUser User { get; init; }
    public DiscordGuild Guild { get; init; }
    public DiscordClient Client { get; init; }
    public Config ExConfig { get; init; }
    public Config Config { get; init; }
    public string VerString { get; init; }
    public DatabaseContext DbContext { get; init; }
}