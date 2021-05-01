using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Markdig;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using SilverBotDS.Objects;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS.Commands.SBCalendar
{
    [Group("Calendar"), Aliases("cal"), Description("SilverBot Calendar commands, these aren't a replacement for a calendar app but they could be useful when wanting to schedule on one")]
    internal class CalendarCommands : BaseCommandModule
    {
        public Config Config { private get; set; }
        private readonly ConcurrentDictionary<ulong, UserCalendar> calendars = new();

        [Command("login")]
        public async Task LoginAsync(CommandContext ctx)
        {
            var calendar = new UserCalendar();
            async Task deviceCodeReadyCallback(DeviceCodeResult dcr) => await ctx.RespondAsync(dcr.Message);

            calendar = await calendar.GetCalendar(Config, deviceCodeReadyCallback);
            calendars.TryAdd(ctx.User.Id, calendar);
            await new DiscordMessageBuilder()
                                               .WithReply(ctx.Message.Id)
                                              .WithContent($"Signed into {calendar.UserName()} ({calendar.Email()})")
                                              .WithAllowedMentions(Mentions.None)
                                               .SendAsync(ctx.Channel);
        }

        [Command("add")]
        public async Task AddAsync(CommandContext ctx, DateTime start, DateTime end, [RemainingText] string timeZone = "Central European Time")
        {
            calendars.TryGetValue(ctx.User.Id, out var calendar);
            var interactivity = ctx.Client.GetInteractivity();
            var oldmessage = await new DiscordMessageBuilder()
                                               .WithReply(ctx.Message.Id)
                                              .WithContent($"Send the Subject (plain text):")
                                              .WithAllowedMentions(Mentions.None)
                                               .SendAsync(ctx.Channel);
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            var msg = await interactivity.WaitForMessageAsync(xm => xm.Channel.Id == ctx.Channel.Id, TimeSpan.FromSeconds(60));
            var @event = new Event
            {
                Start = new DateTimeTimeZone
                {
                    DateTime = start.ToString("o"),
                    TimeZone = timeZone
                },
                End = new DateTimeTimeZone
                {
                    DateTime = end.ToString("o"),
                    TimeZone = timeZone
                },
            };
            if (msg.Result is not null)
            {
                _ = msg.Result.DeleteAsync();
                @event.Subject = msg.Result.Content;
            }
            else
            {
                await oldmessage.ModifyAsync(lang.PeriodExpired);
            }
            await oldmessage.ModifyAsync("Would you like to add a body? (***Y***es or ***N***o)");

            msg = await interactivity.WaitForMessageAsync(xm => xm.Channel.Id == ctx.Channel.Id && (xm.Content.ToLowerInvariant().Contains("y") || xm.Content.ToLowerInvariant().Contains("n")), TimeSpan.FromSeconds(60));

            if (msg.Result != null)
            {
                _ = msg.Result.DeleteAsync();

                if (msg.Result.Content.ToLowerInvariant().Contains("y"))
                {
                    await oldmessage.ModifyAsync("Please send the text you wish to be added (as CommmonMark markdown):");
                    msg = await interactivity.WaitForMessageAsync(xm => xm.Channel.Id == ctx.Channel.Id, TimeSpan.FromSeconds(60));
                    if (msg.Result is not null)
                    {
                        @event.Body = new ItemBody { Content = Markdown.ToHtml(msg.Result.Content), ContentType = BodyType.Html };
                    }
                }
            }
            else
            {
                await oldmessage.ModifyAsync(lang.PeriodExpired);
            }

            await calendar.Add(@event);
        }

        [Command("view")]
        public async Task EAsync(CommandContext ctx)
        {
            calendars.TryGetValue(ctx.User.Id, out var calendar);
            StringBuilder bob = new();
            foreach (var evnt in await calendar.GetCurrentPageAsync())
            {
                bob.AppendLine($"{evnt.Subject} { DateTime.Parse(evnt.Start.DateTime)}");
            }
            var interactivity = ctx.Client.GetInteractivity();
            var pages = interactivity.GeneratePagesInEmbed(bob.ToString());

            await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages);
            bob.Clear();
        }
    }
}