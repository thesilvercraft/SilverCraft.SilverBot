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
using SilverBot.Shared;
using SilverBot.Shared.Objects.Database;
using SilverBot.Shared.Objects.Database.Classes;
using SilverBot.Shared.Objects.Language;
using SilverBot.Shared.Utils;
using SilverBotDS.Commands;
using SilverBotDS.ProgramExtensions;
using System.Collections.Generic;

namespace SilverBotDS.ProgramExtensions
{
    public  class EventsRunner : IProgramExtension
    {
        private  ServiceProvider serviceProvider { get; set; }
        private  Logger _log { get; set; }
        private bool _isLoaded;
        public bool IsLoaded => _isLoaded;

        public Task RunEmojiEvent(PlannedEvent @event)
        {
            if (@event.Type != PlannedEventType.EmojiPoll)
            {
                throw new ArgumentException("The parameter @event needs to be an EmojiPoll", nameof(@event));
            }

            return RunEmojiEventAsync(@event);
        }

        public Task RunEmojiEventAsync(PlannedEvent @event) =>
        RunEventCommon(@event, async (discordClient, databaseCtx, channel, languageService, lang) =>
            {
                if (@event.ResponseMessageID != null)
                {
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
            });

        public  Task RunReminderEvent(PlannedEvent @event)
        {
            if (@event.Type != PlannedEventType.Reminder)
            {
                throw new ArgumentException("The parameter @event needs to be an Reminder", nameof(@event));
            }

            return RunReminderEventAsync(@event);
        }
        public async Task RunEventCommon(PlannedEvent @event, Func<DiscordClient,DatabaseContext,DiscordChannel,LanguageService,Language, Task> func)
        {
            var discordClient = serviceProvider.GetRequiredService<DiscordClient>();
            var databaseContext = serviceProvider.GetRequiredService<DatabaseContext>();
            var channel = await discordClient.GetChannelAsync(@event.ChannelID);
            var languageService = serviceProvider.GetRequiredService<LanguageService>();
            var lang = await languageService.GetLanguageFromGuildIdAsync((ulong)channel.GuildId, databaseContext);
            await func(discordClient,databaseContext,channel,languageService,lang);

        }
        public Task RunReminderEventAsync(PlannedEvent @event) =>
            RunEventCommon(@event, async (discordClient, databaseCtx, channel, languageService, lang) =>
            {
                if (!@event.Handled)
                {
                    DiscordMessageBuilder a = new();
                    a.WithReply(@event.MessageID, true);
                    a.WithContent($"<@!{@event.UserID.ToString(CultureInfo.InvariantCulture)}>");
                    a.AddEmbed(new DiscordEmbedBuilder().WithTitle(lang.ReminderContent).WithDescription(@event.Data)
                        .Build());
                    await a.SendAsync(channel);
                }
            });
  

        public Task RunGiveAwayEvent(PlannedEvent @event)
        {
            if (@event.Type != PlannedEventType.GiveAway)
            {
                throw new ArgumentException("The parameter @event needs to be an GiveAway", nameof(@event));
            }

            return RunGiveAwayEventAsync(@event);
        }
        public Task RunGiveAwayEventAsync(PlannedEvent @event) =>
             RunEventCommon(@event, async (discordClient, databaseCtx, channel, languageService, lang) =>
             {
                 if (@event is { Handled: false, ResponseMessageID: not null })
                 {
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
             });
       
        static List<PlannedEvent> Queued=new();
        public async Task<double> RunEventsAsync(CancellationToken token)
        {
            var dbctx = serviceProvider.GetRequiredService<DatabaseContext>();
            var taskService = serviceProvider.GetRequiredService<ICallBack>();
            try
            {
                var plannedEvents = dbctx.plannedEvents.Where(x => x.Time < DateTime.Now.AddSeconds(20) && !x.Handled).ToArray();
                foreach (var @event in plannedEvents)
                {
                    try
                    {
                        CancellationTokenSource cts = new();
                        Func<PlannedEvent, Task>? task = null;
                        string? name = null;
                        switch (@event.Type)
                        {
                            case PlannedEventType.EmojiPoll:
                                task = (ev) => RunEmojiEvent(ev);
                                name = "EmojiEvent";
                                break;
                            case PlannedEventType.GiveAway:
                                task = (ev) => RunGiveAwayEvent(ev);
                                name = "GiveAwayEvent";
                                break;
                            case PlannedEventType.Reminder:
                                task = (ev) => RunReminderEvent(ev);
                                name = "ReminderEvent";
                                break;
                            default:
                                _log.Warning("DB event with unknown type was ignored, Type: {Type}", @event.Type);
                                break;
                        }
                        if (name != null && task != null)
                        {
                            Queued.Add(@event);
                            taskService.AddOnce(async () =>
                            {
                                if(Queued.Contains(@event))
                                {
                                    Queued.Remove(@event);
                                    await task(@event);
                                }
                                else
                                {
                                    Console.WriteLine("Something's not right, {0} was removed from the queue before", @event);
                                }
                            }, @event.Time, name, cts);
                        }
                        dbctx.plannedEvents.Remove(@event);
                        await dbctx.SaveChangesAsync(cts.Token);
                    }
                    catch (Exception e)
                    {
                        _log.Error(e, "exception happened in events thread in the switch case");
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "exception happened in events thread");
            }
            if(token.IsCancellationRequested)
            {
                return double.PositiveInfinity;
            }
            return 4000;

        }

        public Task Register(ServiceProvider sp, Logger log, params object[] additionalContext)
        {
            serviceProvider=sp;
            _log=log;
            var taskService = sp.GetRequiredService<ICallBack>();
            ets=new();
            taskService.Add(async () => await RunEventsAsync(ets.Token), 0, "EventsTask", ets );
            return Task.CompletedTask;
        }
        CancellationTokenSource ets;

        public Task Reload()
        {
            return Task.CompletedTask;
        }

        public Task Unregister(ServiceProvider sp, Logger log, params object[] additionalContext)
        {
            ets.Cancel();
            return Task.CompletedTask;
        }
    }
}