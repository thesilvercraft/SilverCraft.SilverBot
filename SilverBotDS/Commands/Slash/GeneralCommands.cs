/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilverBot.Shared.Objects;
using SilverBot.Shared.Objects.Database;
using SilverBot.Shared.Objects.Language;
using SilverBot.Shared.Utils;

namespace SilverBotDS.Commands.Slash;

public class GeneralCommands : ApplicationCommandModule
{
    public DatabaseContext Dbctx { private get; set; }
    public Config Cnf { private get; set; }
    public LanguageService LanguageService { private get; set; }
    public ColourService ColourService {private get; set;}
    [SlashCommand("hello", "A simple hello command")]
    public async Task TestCommand(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent($"hello {ctx.User.Mention}"));
    }

    public async Task WhoIsTask(BaseContext ctx, DiscordUser user)
    {
        var lang = await LanguageService.GetLanguageFromGuildIdAsync(ctx.Guild.Id, Dbctx);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder()
                .WithTitle(lang.User + user.Username)
                .WithDescription(lang.InformationAbout + user.Mention)
                .AddField(lang.Userid, Formatter.InlineCode(user.Id.ToString()), true)
                .AddField(lang.JoinedSilverCraft,
                    StringUtils.BoolToEmoteString(await Genericcommands.IsAtSilverCraftAsync(ctx.Client, user, Cnf)),
                    true)
                .AddField(lang.IsAnOwner,
                    StringUtils.BoolToEmoteString(ctx.Client.CurrentApplication.Owners.Contains(user)), true)
                .AddField(lang.IsABot, StringUtils.BoolToEmoteString(user.IsBot), true)
                .AddField(lang.AccountCreationDate, user.CreationTimestamp.UtcDateTime.DateTimeToTimeStamp(TimestampFormat.LongDateTime), true)
                .AddField(lang.AccountJoinDate,
                    (ctx.Guild?.Members.ContainsKey(user.Id) == true
                        ? ctx.Guild?.Members[user.Id].JoinedAt.UtcDateTime.DateTimeToTimeStamp(TimestampFormat.LongDateTime)
                        : "NA") ?? "NA", true)
                .WithColor(ColourService.GetSingle())
                .WithThumbnail(user.GetAvatarUrl(ImageFormat.Png))
                .AddRequestedByFooter(ctx,lang)
                .Build()).AsEphemeral(true));
    }

    [SlashCommand("whois", "Find out the info silverbot knows about someone.")]
    public Task WhoIsCommand(InteractionContext ctx, [Option("user", "User to view info on")] DiscordUser user)
    {
        return WhoIsTask(ctx, user);
    }

    [ContextMenu(ApplicationCommandType.UserContextMenu, "Whois-like search")]
    public Task UserMenu(ContextMenuContext ctx)
    {
        return WhoIsTask(ctx, ctx.TargetMember);
    }

    [SlashCommand("version", "Find out the version info for this instance of silverbot")]
    public async Task VersionInfoCommand(InteractionContext ctx)
    {
        var lang = await LanguageService.FromCtxAsync(ctx);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(MiscCommands.VersionInfoEmbed(lang,ctx)).AsEphemeral(true));
    }

    [SlashCommand("dukthosting", "SilverHosting:tm: best")]
    public async Task DuktHostingCommand(InteractionContext ctx)
    {
        var lang = await LanguageService.FromCtxAsync(ctx);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder()
                .WithTitle(lang.SilverhostingJokeTitle)
                .WithDescription(lang.SilverhostingJokeDescription)
                .WithAuthor($"{ctx.Client.CurrentUser.Username}#{ctx.Client.CurrentUser.Discriminator}",
                    iconUrl: ctx.Client.CurrentUser.GetAvatarUrl(ImageFormat.Auto))
                .WithColor(ColourService.GetSingle())
                .Build()));
    }

    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Dump message")]
    public async Task DumpCommand(ContextMenuContext ctx)
    {
        await using var outStream = new MemoryStream(Encoding.UTF8.GetBytes(ctx.TargetMessage.Content))
        {
            Position = 0
        };
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddFile("message.txt", outStream));
    }
}