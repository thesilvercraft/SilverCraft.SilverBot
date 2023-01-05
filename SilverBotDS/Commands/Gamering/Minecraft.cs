/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Net.Http;
using System.Threading.Tasks;
using SilverBot.Shared.Attributes;
using SilverBot.Shared.Objects.Database.Classes;
using SilverBot.Shared.Utils;
using CategoryAttribute = SilverBot.Shared.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands.Gamering;

using CategoryAttribute = CategoryAttribute;

[Category("Gaming")]
[Group("minecraft")]
[Aliases("mc")]
[RequireModuleGuildEnabled(EnabledModules.Minecraft, true)]

public class MinecraftModule : BaseCommandModule
{
    //HEY LOIS IM PLAYING MINECRAFT
    public HttpClient HttpClient { private get; set; }

    [Command("getfromusername")]
    [Description("Get a minecraft players UUID from their username")]
    [Aliases("username")]
    public async Task Calculate(CommandContext ctx, string input)
    {
        var player = await MinecraftUtils.GetPlayerAsync(input, HttpClient);
        var b = new DiscordEmbedBuilder();
        b.WithThumbnail(player.GetAvatarUrl())
            .WithImageUrl(player.GetBodyUrl())
            .AddField("uuid", "`" + player.Id + "`")
            .WithColor(await ColorUtils.GetSingleAsync());
        await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
            .WithEmbed(b.Build())
            .SendAsync(ctx.Channel);
    }
}