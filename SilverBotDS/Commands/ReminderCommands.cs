using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SilverBotDS.Attributes;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SilverBotDS.Objects.Database.Classes;
using SilverBotDS.Utils;

namespace SilverBotDS.Commands;

[RequireGuild]
[Category("Reminder")]
[RequireUserPermissions(Permissions.SendMessages)]
public class ReminderCommands : SilverBotCommandModule
{
    public DatabaseContext dbCtx { private get; set; }

    
    [Command("remindme")]
    [Description("simple reminder")]
    public async Task RemindCommand(CommandContext ctx,
        [Description("delay of reminder (e.g. 1m = 1 minute)")] TimeSpan duration,
        [Description("content")][RemainingText] string item)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        if (!string.IsNullOrEmpty(item))
        {
            var s = RandomGenerator.RandomAbcString(20);
            var t = DateTime.Now + duration;
            await dbCtx.plannedEvents.AddAsync(new PlannedEvent
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
            await dbCtx.SaveChangesAsync();
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

        var events = dbCtx.plannedEvents.Where(a=>a.UserID== ctx.User.Id && a.Type==PlannedEventType.Reminder && !a.Handled).ToList();
        if(events.Count>0)
        {
            StringBuilder bob = new(lang.ListReminderStart);
            bob.AppendLine();
            foreach(var even in events)
            {
                if(bob.Length>1888)
                {
                    bob.AppendFormat(lang.ListReminderListMore, events.Count-(events.IndexOf(even)+1));
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


}