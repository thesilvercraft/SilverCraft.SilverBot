/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Database.Classes;
using SilverBotDS.Utils;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilverBotDS
{
    public static class EventsRunner
    {
        private static ServiceProvider ServiceProvider { get; set; }
        private static Logger Log { get; set; }

        public static void InjectEvents(ServiceProvider sp, Logger log)
        {
            Log = log;
            ServiceProvider = sp;
        }

        public static Task RunEmojiEvent(PlannedEvent @event, DatabaseContext dbctx)
        {
            if (@event.Type != PlannedEventType.EmojiPoll)
            {
                throw new ArgumentException("The parameter @event needs to be an EmojiPoll", nameof(@event));
            }
            if (dbctx == null)
            {
                throw new ArgumentNullException(nameof(dbctx));
            }
            return RunEmojiEventAsync(@event, dbctx);
        }

        public static async Task RunEmojiEventAsync(PlannedEvent @event, DatabaseContext dbctx)
        {
            var _discordClient = (DiscordClient)ServiceProvider.GetService(typeof(DiscordClient));
            var channel = await _discordClient.GetChannelAsync(@event.ChannelID);
            if (@event.ResponseMessageID != null)
            {
                var lang = await Language.GetLanguageFromGuildIdAsync((ulong)channel.GuildId, dbctx);
                var msg = await channel.GetMessageAsync((ulong)@event.ResponseMessageID);
                var bob = new DiscordEmbedBuilder(msg.Embeds[0]);
                var yesVotes =
                    (await msg.GetReactionsAsync(DiscordEmoji.FromName(_discordClient, ":everybodyvotes:"))).Count(x =>
                        x.Id != _discordClient.CurrentUser.Id && !x.IsBot);
                var noVotes =
                    (await msg.GetReactionsAsync(DiscordEmoji.FromName(_discordClient, ":nobodyvotes:"))).Count(x =>
                        x.Id != _discordClient.CurrentUser.Id && !x.IsBot);
                var pollResultText = new StringBuilder();
                pollResultText.Append(lang.PollResultsStart).Append(" **");
                if (yesVotes > noVotes)
                {
                    pollResultText.Append(lang.PollResultsResultYes);
                }
                else if (yesVotes == noVotes)
                {
                    pollResultText.Append(lang.PollResultsResultUndecided);
                }
                else
                {
                    pollResultText.Append(lang.PollResultsResultNo);
                }

                pollResultText.Append("**\n").Append(lang.PollResultsResultYes).Append(':').Append(yesVotes).Append(' ').Append(lang.PollResultsResultNo).Append(':').Append(noVotes).Append(' ').Append(lang.PollResultsResultUndecided).Append(": ")
                    .Append(channel.Guild.Members.Count(x => !x.Value.IsBot) - (yesVotes + noVotes));
                bob.WithDescription(pollResultText.ToString());
                await msg.ModifyAsync(bob.Build());
            }
            @event.Handled = true;
        }

        public static Task RunReminderEvent(PlannedEvent @event, DatabaseContext dbctx)
        {
            if (@event.Type != PlannedEventType.Reminder)
            {
                throw new ArgumentException("The parameter @event needs to be an Reminder", nameof(@event));
            }
            if (dbctx == null)
            {
                throw new ArgumentNullException(nameof(dbctx));
            }
            return RunReminderEventAsync(@event, dbctx);
        }

        public static async Task RunReminderEventAsync(PlannedEvent @event, DatabaseContext dbctx)
        {
            var _discordClient = (DiscordClient)ServiceProvider.GetService(typeof(DiscordClient));
            var channel = await _discordClient.GetChannelAsync(@event.ChannelID);
            var lang = await Language.GetLanguageFromGuildIdAsync((ulong)channel.GuildId, dbctx);
            if (!@event.Handled)
            {
                DiscordMessageBuilder a = new();
                a.WithReply(@event.MessageID, true);
                a.WithContent($"<@!{@event.UserID.ToString(CultureInfo.InvariantCulture)}>");
                a.AddEmbed(new DiscordEmbedBuilder().WithTitle(lang.ReminderContent).WithDescription(@event.Data).Build());
                await a.SendAsync(channel);
            }
            @event.Handled = true;
        }

        public static Task RunGiveAwayEvent(PlannedEvent @event, DatabaseContext dbctx)
        {
            if (@event.Type != PlannedEventType.GiveAway)
            {
                throw new ArgumentException("The parameter @event needs to be an GiveAway", nameof(@event));
            }
            if (dbctx == null)
            {
                throw new ArgumentNullException(nameof(dbctx));
            }
            return RunGiveAwayEventAsync(@event, dbctx);
        }

        public static async Task RunGiveAwayEventAsync(PlannedEvent @event, DatabaseContext dbctx)
        {
            var _discordClient = (DiscordClient)ServiceProvider.GetService(typeof(DiscordClient));

            var channel = await _discordClient.GetChannelAsync(@event.ChannelID);
            if (@event.ResponseMessageID != null)
            {
                var lang = await Language.GetLanguageFromGuildIdAsync((ulong)channel.GuildId, dbctx);

                var msg = await channel.GetMessageAsync((ulong)@event.ResponseMessageID);
                var people =
                    (await msg.GetReactionsAsync(DiscordEmoji.FromName(_discordClient, ":everybodyvotes:"))).Where(x =>
                        x.Id != _discordClient.CurrentUser.Id && !x.IsBot);
                var discordUsers = people as DiscordUser[] ?? people.ToArray();
                if (discordUsers.Length == 0)
                {
                    await channel.SendMessageAsync(lang.GiveawayResultsNoReactions);
                }
                else
                {
                    await channel.SendMessageAsync(string.Format(lang.GiveawayResultsWon, discordUsers[RandomGenerator.Next(0, discordUsers.Length)].Mention, msg.Embeds[0].Title));
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
                                CancellationTokenSource cts = new(15 * 1007);
                                switch (evnt.Type)
                                {
                                    case PlannedEventType.EmojiPoll:

                                        await Program.RunningTasksOfSecondRowAdd(Guid.NewGuid(), new(Task.Run(() => RunEmojiEvent(evnt, dbctx), cts.Token), cts));
                                        break;

                                    case PlannedEventType.GiveAway:
                                        await Program.RunningTasksOfSecondRowAdd(Guid.NewGuid(), new(Task.Run(() => RunGiveAwayEvent(evnt, dbctx), cts.Token), cts));
                                        break;

                                    case PlannedEventType.Reminder:
                                        await Program.RunningTasksOfSecondRowAdd(Guid.NewGuid(), new(Task.Run(() => RunReminderEvent(evnt, dbctx), cts.Token), cts));
                                        break;

                                    default:
                                        Log.Warning("[SUS] DB event with unknown type was ignored, Type: {Type}", evnt.Type);
                                        break;
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "exception happened in events thread in the switch case");
                            }
                        }
                        else
                        {
                            Log.Verbose("removed an {EventType}", evnt.Type);
                            dbctx.plannedEvents.Remove(evnt);
                            await dbctx.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "exception happened in events thread");
                }

                await Task.Delay(3000);
            }
        }
    }
}