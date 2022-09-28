using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SilverBotDS.Attributes;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SilverBotDS.Objects.Database.Classes;
using SilverBotDS.Utils;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands;

[RequireGuild]
[Category("Reminder")]
[RequireUserPermissions(Permissions.SendMessages)]
[RequireGuildDatabaseValue("Reminders", true, true)]
public class ReminderCommands : SilverBotCommandModule
{
    public DatabaseContext DbCtx { private get; set; }

    [Command("remindme")]
    [Description("simple reminder")]
    public async Task RemindCommand(CommandContext ctx,
        [Description("delay of reminder (e.g. 1m = 1 minute)")] TimeSpan duration,
        [Description("content")][RemainingText] string item)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        if (!string.IsNullOrEmpty(item))
        {
            var s = RandomGenerator.RandomString(20);
            var t = DateTime.Now + duration;
            await DbCtx.plannedEvents.AddAsync(new PlannedEvent
            {
                ChannelID = ctx.Channel.Id,
                EventID = s,
                Handled = false,
                MessageID = ctx.Message.Id,
                Time = t,
                Type = PlannedEventType.Reminder,
                UserID = ctx.User.Id,
                Data = item
            });
            await new DiscordMessageBuilder().WithContent(string.Format(lang.ReminderSuccess, Formatter.Timestamp(t, TimestampFormat.RelativeTime), Formatter.Timestamp(t, TimestampFormat.LongDateTime))).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
            await DbCtx.SaveChangesAsync();
        }
        else
        {
            await ctx.RespondAsync(lang.ReminderErrorNoContent);
        }
    }

    [Command("listreminders")]
    [Description("lists all reminders")]
    public async Task ListReminders(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);

        var events = DbCtx.plannedEvents.Where(a => a.UserID == ctx.User.Id && a.Type == PlannedEventType.Reminder && !a.Handled).ToList();
        if (events.Count > 0)
        {
            StringBuilder bob = new(lang.ListReminderStart);
            bob.AppendLine();
            foreach (var even in events)
            {
                if (bob.Length > 1888)
                {
                    bob.AppendFormat(lang.ListReminderListMore, events.Count - (events.IndexOf(even) + 1));
                    break;
                }
                bob.Append(even.EventID).Append("   ").Append(Formatter.Timestamp(even.Time, TimestampFormat.RelativeTime)).Append("    ").Append(Formatter.Timestamp(even.Time, TimestampFormat.LongDateTime)).AppendLine("```").Append(even.Data).AppendLine("```");
            }
            await new DiscordMessageBuilder().WithContent(bob.ToString()).WithAllowedMentions(Mentions.None).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
        }
        else
        {
            await new DiscordMessageBuilder().WithContent(lang.ListReminderNone).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
        }
    }

    [Command("listremindersguild")]
    [Description("lists all reminders here")]
    [RequireUserPermissions(Permissions.Administrator)]
    public async Task ListRemindersG(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        var channels = (await ctx.Guild.GetChannelsAsync()).Select(xa => xa.Id).ToArray();
        var events = DbCtx.plannedEvents.Where(a => channels.Contains(a.ChannelID) && a.Type == PlannedEventType.Reminder && !a.Handled).ToList();
        if (events.Count > 0)
        {
            StringBuilder bob = new(lang.ListReminderStart);
            bob.AppendLine();
            foreach (var even in events)
            {
                if (bob.Length > 1888)
                {
                    bob.AppendFormat(lang.ListReminderListMore, events.Count - (events.IndexOf(even) + 1));
                    break;
                }
                bob.Append(even.EventID).Append("   ").Append(Formatter.Timestamp(even.Time, TimestampFormat.RelativeTime)).Append("    ").Append(Formatter.Timestamp(even.Time, TimestampFormat.LongDateTime)).AppendLine("```").Append(even.Data).AppendLine("```");
            }
            await new DiscordMessageBuilder().WithContent(bob.ToString()).WithAllowedMentions(Mentions.None).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
        }
        else
        {
            await new DiscordMessageBuilder().WithContent(lang.ListReminderNone).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
        }
    }

    [Command("cancelreminder")]
    [Description("deletes a specific reminder")]
    public async Task DeleteReminder(CommandContext ctx, [RemainingText] string id)
    {
        if (id.Length < 10)
        {
            await ctx.RespondAsync("Invalid ID");
            return;
        }
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        var a = DbCtx.plannedEvents.Where(a => a.UserID == ctx.User.Id && a.Type == PlannedEventType.Reminder && a.EventID == id).ToList();
        if (a.Count == 0)
        {
            await ctx.RespondAsync(lang.CancelReminderErrorNoEvent);
        }
        else if (a.Count == 1)
        {
            if (a[0].Handled)
            {
                await ctx.RespondAsync(lang.CancelReminderErrorAlreadyHandled);
                return;
            }
            DbCtx.plannedEvents.Remove(a[0]);
            await DbCtx.SaveChangesAsync();
            await ctx.RespondAsync(lang.CancelReminderSuccess);
        }
        else
        {
            await ctx.RespondAsync(lang.CancelReminderErrorMultiple);
        }
    }

    [Command("cancelreminderf")]
    [Description("deletes a reminder with force")]
    [RequireUserPermissions(Permissions.Administrator)]
    public async Task DeleteReminderF(CommandContext ctx, [RemainingText] string id)
    {
        if (id.Length < 10)
        {
            await ctx.RespondAsync("Invalid ID");
            return;
        }
        var channels = (await ctx.Guild.GetChannelsAsync()).Select(xa => xa.Id).ToArray();

        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        var a = DbCtx.plannedEvents.Where(a => channels.Contains(a.ChannelID) && a.Type == PlannedEventType.Reminder && a.EventID == id).ToList();
        if (a.Count == 0)
        {
            await ctx.RespondAsync(lang.CancelReminderErrorNoEvent);
        }
        else if (a.Count == 1)
        {
            if (a[0].Handled)
            {
                await ctx.RespondAsync(lang.CancelReminderErrorAlreadyHandled);
                return;
            }
            DbCtx.plannedEvents.Remove(a[0]);
            await DbCtx.SaveChangesAsync();
            await ctx.RespondAsync(lang.CancelReminderSuccess);
        }
        else
        {
            await ctx.RespondAsync(lang.CancelReminderErrorMultiple);
        }
    }
}