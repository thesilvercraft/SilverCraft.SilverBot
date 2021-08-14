using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using SilverBotDS.Objects;
using SilverBotDS.Utils;

namespace SilverBotDS.Commands.Slash
{
    public class GeneralCommands : ApplicationCommandModule
    {
        public DatabaseContext DBCTX { private get; set; }
        public Config CNF { private get; set; }
        [SlashCommand("hello", "A simple hello command")]
        public async Task TestCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"hey {ctx.User.Mention} what are you doing here, go back to using normal commands"));
        }
        [ContextMenu(ApplicationCommandType.UserContextMenu, "Whois-like search")]
        public async Task UserMenu(ContextMenuContext ctx)
        {
            var a = ctx.TargetMember;
            var lang = await Language.GetLanguageFromGuildIdAsync(ctx.Guild.Id, DBCTX);
            var cultureinfo = lang.GetCultureInfo();
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder()
                .WithTitle(lang.User + a.Username)
                .WithDescription(lang.InformationAbout + a.Mention)
                .AddField(lang.Userid, a.Id.ToString(), true)
                .AddField(lang.JoinedSilverCraft, StringUtils.BoolToEmoteString(await Genericcommands.IsAtSilverCraftAsync(ctx.Client, a, CNF)), true)
                .AddField(lang.IsAnOwner, StringUtils.BoolToEmoteString(ctx.Client.CurrentApplication.Owners.Contains(a)), true)
                .AddField(lang.IsABot, StringUtils.BoolToEmoteString(a.IsBot), true)
                .AddField(lang.AccountCreationDate, a.CreationTimestamp.ToString(cultureinfo), true)
                .AddField(lang.AccountJoinDate, ctx.Guild?.Members.ContainsKey(a.Id) == true ? ctx.Guild?.Members[a.Id].JoinedAt.ToString(cultureinfo) : "NA", true)
                .WithColor(await ColorUtils.GetSingleAsync())
                .WithThumbnail(a.GetAvatarUrl(ImageFormat.Png))
                .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
                .Build()).AsEphemeral(true));
        }
    }
}
