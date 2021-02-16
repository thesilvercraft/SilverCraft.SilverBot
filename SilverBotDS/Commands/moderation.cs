using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using SilverBotDS.Objects;

namespace SilverBotDS.Commands
{
    internal class Moderation : BaseCommandModule
    {
#pragma warning disable CA1822 // Mark members as static

        [Command("kick")]
        [Description("Kick a specified user")]
        [RequireBotPermissions(Permissions.KickMembers)]
        [RequireUserPermissions(Permissions.KickMembers)]
        public async Task Kick(CommandContext ctx, [Description("the user like duh")] DiscordMember a, [Description("the reason")][RemainingText] string reason = "The kick boot has spoken")
        {
            var lang = Language.GetLanguageFromCtx(ctx);
            var b = new DiscordEmbedBuilder();
            var thing = "";
            int bp = (await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id)).Hierarchy, up = ctx.Member.Hierarchy, ap = a.Hierarchy;
            if (up < ap)
            {
                thing += lang.UserHasLowerRole + lang.Kick;
            }
            if (up == ap)
            {
                thing += lang.UserHasLowerRole + lang.Kick;
            }
            if (ap > bp)
            {
                thing += lang.BotHasLowerRole;
            }
            if (up > bp && bp > ap)
            {
                await a.RemoveAsync(reason);
                thing += "The bot has attempted to kick the user";
            }
            b.WithDescription(thing);
            b.WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            await ctx.RespondAsync(embed: b.Build());
        }

        [Command("ban")]
        [Description("bans a specified user")]
        [RequireBotPermissions(Permissions.BanMembers)]
        [RequireUserPermissions(Permissions.BanMembers)]
        public async Task Ban(CommandContext ctx, [Description("the user like duh")] DiscordMember a, [Description("the reason")][RemainingText] string reason = "The ban hammer has spoken")
        {
            var lang = Language.GetLanguageFromCtx(ctx);
            var b = new DiscordEmbedBuilder();
            var thing = "";
            int bp = (await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id)).Hierarchy, up = ctx.Member.Hierarchy, ap = a.Hierarchy;
            if (up < ap)
            {
                thing += lang.UserHasLowerRole + lang.Ban;
            }
            if (up == ap)
            {
                thing += lang.UserHasLowerRole + lang.Ban;
            }
            if (ap > bp)
            {
                thing += lang.BotHasLowerRole;
            }
            if (up > bp && bp > ap)
            {
                await a.BanAsync(reason: reason);
                thing += "The bot has attempted to ban the user";
            }
            b.WithDescription(thing);
            b.WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            await ctx.RespondAsync(embed: b.Build());
        }
    }
}