/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SilverBot.Shared.Objects.Database;
using SilverBot.Shared.Objects.Database.Classes;
using SilverBot.Shared.Objects.Language;
using SilverBot.Shared.Utils;
using SilverBotDS.Commands;
using SilverBotDS.ProgramExtensions;

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
            var discordClient = ServiceProvider.GetRequiredService<DiscordClient>();
            var databaseContext = ServiceProvider.GetRequiredService<DatabaseContext>();
            var channel = await discordClient.GetChannelAsync(@event.ChannelID);
            if (@event.ResponseMessageID != null)
            {
                var languageService = (LanguageService?)ServiceProvider.GetService(typeof(LanguageService));
                var lang = await languageService.GetLanguageFromGuildIdAsync((ulong)channel.GuildId, databaseContext);
                var msg = await channel.GetMessageAsync((ulong)@event.ResponseMessageID);
                var bob = new DiscordEmbedBuilder(msg.Embeds[0]);
                var yesVotes =
                    (await msg.GetReactionsAsync(AdminCommands.YesEmoji)).Count(x =>
                        x.Id != discordClient.CurrentUser.Id && !x.IsBot);
                var noVotes =
                    (await msg.GetReactionsAsync(AdminCommands.NoEmoji)).Count(x =>
                        x.Id != discordClient.CurrentUser.Id && !x.IsBot);
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

                pollResultText.Append("**\n").Append(lang.PollResultsResultYes).Append(':').Append(yesVotes).Append(' ')
                    .Append(lang.PollResultsResultNo).Append(':').Append(noVotes).Append(' ')
                    .Append(lang.PollResultsResultUndecided).Append(": ")
                    .Append(channel.Guild.Members.Count(x => !x.Value.IsBot) - (yesVotes + noVotes));
                bob.WithDescription(pollResultText.ToString());
                await msg.ModifyAsync(bob.Build());
            }
        }

        public static Task RunReminderEvent(PlannedEvent @event)
        {
            if (@event.Type != PlannedEventType.Reminder)
            {
                throw new ArgumentException("The parameter @event needs to be an Reminder", nameof(@event));
            }

            return RunReminderEventAsync(@event);
        }

        public static async Task RunReminderEventAsync(PlannedEvent @event)
        {
            var discordClient = ServiceProvider.GetRequiredService<DiscordClient>();
            var databaseContext = ServiceProvider.GetRequiredService<DatabaseContext>();
            var channel = await discordClient.GetChannelAsync(@event.ChannelID);
            var languageService = (LanguageService?)ServiceProvider.GetService(typeof(LanguageService));
            var lang = await languageService.GetLanguageFromGuildIdAsync((ulong)channel.GuildId, databaseContext);
            if (!@event.Handled)
            {
                DiscordMessageBuilder a = new();
                a.WithReply(@event.MessageID, true);
                a.WithContent($"<@!{@event.UserID.ToString(CultureInfo.InvariantCulture)}>");
                a.AddEmbed(new DiscordEmbedBuilder().WithTitle(lang.ReminderContent).WithDescription(@event.Data)
                    .Build());
                await a.SendAsync(channel);
            }
        }

        public static Task RunGiveAwayEvent(PlannedEvent @event)
        {
            if (@event.Type != PlannedEventType.GiveAway)
            {
                throw new ArgumentException("The parameter @event needs to be an GiveAway", nameof(@event));
            }

            return RunGiveAwayEventAsync(@event);
        }

        public static async Task RunGiveAwayEventAsync(PlannedEvent @event)
        {
            var discordClient = (DiscordClient)ServiceProvider.GetService(typeof(DiscordClient));
            var databaseContext = ServiceProvider.GetRequiredService<DatabaseContext>();
            var channel = await discordClient.GetChannelAsync(@event.ChannelID);
            if (@event.ResponseMessageID != null)
            {
                var languageService = (LanguageService?)ServiceProvider.GetService(typeof(LanguageService));
                var lang = await languageService.GetLanguageFromGuildIdAsync((ulong)channel.GuildId, databaseContext);
                var msg = await channel.GetMessageAsync((ulong)@event.ResponseMessageID);
                var people =
                    (await msg.GetReactionsAsync(AdminCommands.EntryEmoji)).Where(x =>
                        x.Id != discordClient.CurrentUser.Id && !x.IsBot);
                var discordUsers = people as DiscordUser[] ?? people.ToArray();
                if (discordUsers.Length == 0)
                {
                    await channel.SendMessageAsync(lang.GiveawayResultsNoReactions);
                }
                else
                {
                    await channel.SendMessageAsync(string.Format(lang.GiveawayResultsWon,
                        discordUsers[RandomGenerator.Next(0, discordUsers.Length)].Mention, msg.Embeds[0].Title));
                }
            }
        }

        public static async Task RunEventsAsync()
        {
            var dbctx = ServiceProvider.GetRequiredService<DatabaseContext>();
            var taskService = ServiceProvider.GetRequiredService<TaskService>();
            while (true)
            {
                try
                {
                    var plannedEvents = dbctx.plannedEvents.Where(x => x.Time < DateTime.Now && !x.Handled).ToArray();
                    foreach (var @event in plannedEvents)
                    {
                        try
                        {
                            CancellationTokenSource cts = new(15 * 1007);
                            switch (@event.Type)
                            {
                                case PlannedEventType.EmojiPoll:
                                    taskService.AddSecondaryTask(Guid.NewGuid(),
                                        new Tuple<Task, CancellationTokenSource>(
                                            Task.Run(() => RunEmojiEvent(@event), cts.Token), cts));
                                    break;

                                case PlannedEventType.GiveAway:
                                    taskService.AddSecondaryTask(Guid.NewGuid(),
                                        new Tuple<Task, CancellationTokenSource>(
                                            Task.Run(() => RunGiveAwayEvent(@event), cts.Token), cts));
                                    break;

                                case PlannedEventType.Reminder:
                                    taskService.AddSecondaryTask(Guid.NewGuid(),
                                        new Tuple<Task, CancellationTokenSource>(
                                            Task.Run(() => RunReminderEvent(@event), cts.Token), cts));
                                    break;

                                default:
                                    Log.Warning("DB event with unknown type was ignored, Type: {Type}", @event.Type);
                                    break;
                            }

                            dbctx.plannedEvents.Remove(@event);
                            await dbctx.SaveChangesAsync(cts.Token);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "exception happened in events thread in the switch case");
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