/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;
using SilverBotDS.Attributes;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands;

[Category("Moderation")]
[RequireModuleGuildEnabled(EnabledModules.Moderator, true)]
public class ModCommands : BaseCommandModule
{
    
    public LanguageService LanguageService { private get; set; }
    [Command("kick")]
    [Description("Kick a specified user")]
    [RequirePermissions(Permissions.KickMembers)]
    public async Task Kick(CommandContext ctx, [Description("the user like duh")] DiscordMember a,
        [Description("the reason")][RemainingText] string reason = "The kick boot has spoken")
    {
        var lang = await LanguageService.FromCtxAsync(ctx);
        var b = new DiscordEmbedBuilder();
        var thing = "";
        int bp = (await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id)).Hierarchy,
            up = ctx.Member.Hierarchy,
            ap = a.Hierarchy;
        if (up < ap)
        {
            thing = lang.UserHasLowerRole + lang.Kick;
        }
        else if (up == ap)
        {
            thing = lang.UserHasLowerRole + lang.Kick;
        }
        else if (ap > bp)
        {
            thing = lang.BotHasLowerRole + lang.Kick;
        }
        else if (up > bp && bp > ap)
        {
            await a.RemoveAsync(reason);
            thing = lang.BotKickedUser;
        }

        b.WithDescription(thing).WithColor(await ColorUtils.GetSingleAsync())
            .AddRequestedByFooter(ctx,lang);
        await ctx.RespondAsync(b.Build());
    }

    [Command("ban")]
    [Description("bans a specified user")]
    [RequirePermissions(Permissions.BanMembers)]
    public async Task Ban(CommandContext ctx, [Description("the user like duh")] DiscordUser a,
        [Description("the reason")][RemainingText] string reason = "The ban hammer has spoken")
    {
        var lang = await LanguageService.FromCtxAsync(ctx);
        var b = new DiscordEmbedBuilder();
        var thing = "";
        if (ctx.Guild.Members.ContainsKey(a.Id))
        {
            int bp = (await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id)).Hierarchy,
                up = ctx.Member.Hierarchy,
                ap = (await ctx.Guild.GetMemberAsync(a.Id)).Hierarchy;
            if (up < ap)
            {
                thing = lang.UserHasLowerRole + lang.Ban;
            }
            else if (up == ap)
            {
                thing = lang.UserHasLowerRole + lang.Ban;
            }
            else if (ap > bp)
            {
                thing = lang.BotHasLowerRole + lang.Ban;
            }
        }

        if (string.IsNullOrEmpty(thing))
        {
            await ctx.Guild.BanMemberAsync(a.Id, reason: reason);
            thing = lang.BotBannedUser;
        }

        b.WithDescription(thing).WithColor(await ColorUtils.GetSingleAsync())
            .AddRequestedByFooter(ctx,lang);
        await ctx.RespondAsync(b.Build());
    }

    [Command("kickme")]
    [Description("Kicks yourself lmao (or ban if you're super daring)")]
    [RequireBotPermissions(Permissions.KickMembers)]
    public async Task Kms(CommandContext ctx, [Description("hardcore")] bool ban = false)
    {
        if (ban)
        {
            await ctx.Member.BanAsync(reason: "asked lol");
        }
        else
        {
            await ctx.Member.RemoveAsync("asked lol");
        }

        await ctx.RespondAsync($"'{ctx.Member.Mention} took the easy way out' - michael de santa gta five");
    }

    [Command("purge")]
    [Aliases("clean", "clear")]
    [Description("Downloads and removes X messages from the current channel.")]
    [RequirePermissions(Permissions.ManageMessages)]
    public async Task Purge(CommandContext ctx, [Description("number of messages")] int amount)
    {
        var lang = await LanguageService.FromCtxAsync(ctx);
        if (amount is < 0 or 0)
        {
            await new DiscordMessageBuilder()
                .WithReply(ctx.Message.Id)
                .WithEmbed(new DiscordEmbedBuilder()
                    .WithTitle(lang.PurgeNumberNegative)
                    .WithColor(await ColorUtils.GetSingleAsync()))
                .SendAsync(ctx.Channel);
            return;
        }

        var messages = (await ctx.Channel.GetMessagesBeforeAsync(ctx.Message.Id, amount))
            .Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14).ToList();
        if (messages.Count == 0)
        {
            await new DiscordMessageBuilder()
                .WithReply(ctx.Message.Id)
                .WithEmbed(new DiscordEmbedBuilder()
                    .WithTitle(lang.PurgeNothingToDelete)
                    .WithColor(await ColorUtils.GetSingleAsync()))
                .SendAsync(ctx.Channel);
            return;
        }

        await ctx.Channel.DeleteMessagesAsync(messages,
            string.Format(lang.PurgedBySilverBotReason, ctx.Member.Username));
        await new DiscordMessageBuilder()
            .WithReply(ctx.Message.Id)
            .WithEmbed(new DiscordEmbedBuilder()
                .WithTitle(lang.PurgeRemovedFront + messages.Count +
                           (messages.Count > 1 ? lang.PurgeRemovedPlural : lang.PurgeRemovedSingle))
                .WithColor(await ColorUtils.GetSingleAsync()))
            .SendAsync(ctx.Channel);
    }
}