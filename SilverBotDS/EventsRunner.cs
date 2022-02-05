using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Database.Classes;
using SilverBotDS.Utils;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilverBotDS
{
    public static class EventsRunner
    {
        
        private static ServiceProvider ServiceProvider { get; set; }
        private static Logger _log { get; set; }
        public static void InjectEvents(ServiceProvider sp, Logger log)
        {
            _log = log;
            ServiceProvider = sp;
        }
        public static Task RunEmojiEvent(PlannedEvent @event)
        {
            if (@event.Type != PlannedEventType.EmojiPoll)
            {
                throw new ArgumentException("The parameter @event needs to be an EmojiPoll", nameof(@event));
            }
            return RunEmojiEventAsync(@event);
        }

        public static async Task RunEmojiEventAsync(PlannedEvent @event)
        {
            var _discordClient=ServiceProvider.GetRequiredService<DiscordClient>();
            var channel = await _discordClient.GetChannelAsync(@event.ChannelID);
            if (@event.ResponseMessageID != null)
            {
                var msg = await channel.GetMessageAsync((ulong)@event.ResponseMessageID);
                var bob = new DiscordEmbedBuilder(msg.Embeds[0]);
                var yesVotes =
                    (await msg.GetReactionsAsync(DiscordEmoji.FromName(_discordClient, ":everybodyvotes:"))).Count(x =>
                        x.Id != _discordClient.CurrentUser.Id && !x.IsBot);
                var noVotes =
                    (await msg.GetReactionsAsync(DiscordEmoji.FromName(_discordClient, ":nobodyvotes:"))).Count(x =>
                        x.Id != _discordClient.CurrentUser.Id && !x.IsBot);
                var pollResultText = new StringBuilder();
                pollResultText.Append("Poll result: **");
                if (yesVotes > noVotes)
                {
                    pollResultText.Append("Yes");
                }
                else if (yesVotes == noVotes)
                {
                    pollResultText.Append("Undecided");
                }
                else
                {
                    pollResultText.Append("No");
                }

                pollResultText.Append("**\nYes:").Append(yesVotes).Append(" No:").Append(noVotes).Append(" Undecided: ")
                    .Append(channel.Guild.Members.Count(x => !x.Value.IsBot) - (yesVotes + noVotes));
                bob.WithDescription(pollResultText.ToString());
                await msg.ModifyAsync(bob.Build());
            }
            @event.Handled = true;
        }

        public static Task RunGiveAwayEvent(PlannedEvent @event)
        {
            if (@event.Type != PlannedEventType.EmojiPoll)
            {
                throw new ArgumentException("The parameter evnt needs to be an GiveAway", nameof(@event));
            }
            return RunGiveAwayEventAsync(@event);
        }

        public static async Task RunGiveAwayEventAsync(PlannedEvent @event)
        {
            var _discordClient = ServiceProvider.GetRequiredService<DiscordClient>();

            var channel = await _discordClient.GetChannelAsync(@event.ChannelID);
            if (@event.ResponseMessageID != null)
            {
                var msg = await channel.GetMessageAsync((ulong)@event.ResponseMessageID);
                var people =
                    (await msg.GetReactionsAsync(DiscordEmoji.FromName(_discordClient, ":everybodyvotes:"))).Where(x =>
                        x.Id != _discordClient.CurrentUser.Id && !x.IsBot);
                var discordUsers = people as DiscordUser[] ?? people.ToArray();
                if (discordUsers.Length == 0)
                {
                    await channel.SendMessageAsync("Nobody reacted in time :(");
                }
                else
                {
                    await channel.SendMessageAsync(
                        $"{discordUsers[RandomGenerator.Next(0, discordUsers.Length)].Mention} won {msg.Embeds[0].Title}");
                }
            }
            @event.Handled = true;
        }

        public static async Task RunEventsAsync()
        {
            var dbctx = ServiceProvider.GetRequiredService<DatabaseContext>();
            while (true)
            {
                try
                {
                    var arr = dbctx.plannedEvents.ToArray();
                    foreach (var evnt in arr)
                    {
                        if (!evnt.Handled)
                        {
                            if (evnt.Time > DateTime.Now)
                            {
                                continue;
                            }

                            try
                            {
                                switch (evnt.Type)
                                {
                                    case PlannedEventType.EmojiPoll:
                                        CancellationTokenSource cts = new(15 * 1000);
                                        await Program.RunningTasksOfSecondRowAdd(Guid.NewGuid(), new(Task.Run(() => RunEmojiEvent(evnt), cts.Token), cts));
                                        break;

                                    case PlannedEventType.GiveAway:
                                        CancellationTokenSource cts2 = new(15 * 1000);
                                        await Program.RunningTasksOfSecondRowAdd(Guid.NewGuid(), new(Task.Run(() => RunGiveAwayEvent(evnt), cts2.Token), cts2));
                                        break;

                                    case PlannedEventType.Reminder:
                                        throw new NotImplementedException();
                                    default:
                                        throw new ArgumentOutOfRangeException(nameof(evnt.Type));
                                }
                            }
                            catch (Exception e)
                            {
                                _log.Error(e, "exception happened in events thread in the switch case");
                            }
                        }
                        else
                        {
                            _log.Verbose("removed an {EventType}", evnt.Type);
                            dbctx.plannedEvents.Remove(evnt);
                            await dbctx.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.Error(e, "exception happened in events thread");
                }

                await Task.Delay(3000);
            }
        }
    }
}
