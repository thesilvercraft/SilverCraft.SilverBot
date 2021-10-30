using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SilverBotDS.Attributes;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using System;
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    [RequireUserPermissions(Permissions.Administrator)]
    [Category("Utility")]
    public class AdminCommands : BaseCommandModule
    {
        public DatabaseContext Database { private get; set; }
        private DiscordEmoji[] _pollEmojiCache;

        [Command("setprefix")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task SetPrefix(CommandContext ctx, params string[] cake)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            Database.SetServerPrefixes(ctx.Guild.Id, cake);
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                      .WithContent(lang.StatisticCommand.SetToProvidedStrings)
                                      .WithAllowedMentions(Mentions.None)
                                      .SendAsync(ctx.Channel);
        }

        /// <summary>
        /// A simple emoji based yes/no poll.
        /// </summary>
        /// <param name="commandContext">CommandContext of the message that has executed this command</param>
        /// <param name="duration">Amount of time how long the poll should last.</param>
        /// <param name="question">Polls question</param>
        /// <returns></returns>
        [Command("emojipoll"), Description("Start a simple emoji poll for a simple yes/no question"), Cooldown(2, 30, CooldownBucketType.Guild)]
        public async Task EmojiPollAsync(CommandContext commandContext, [Description("How long should the poll last. (e.g. 1m = 1 minute)")] TimeSpan duration, [Description("Poll question"), RemainingText] string question)
        {
            if (!string.IsNullOrEmpty(question))
            {
                var client = commandContext.Client;
                if (_pollEmojiCache == null)
                {
                    _pollEmojiCache = new[] {
                        DiscordEmoji.FromName(client, ":everybodyvotes:"),
                        DiscordEmoji.FromName(client, ":nobodyvotes:")
                    };
                }
                var bob = new DiscordEmbedBuilder();
                bob.WithTitle(question).WithAuthor(commandContext.Member.Nickname ?? commandContext.User.Username, iconUrl: commandContext.User.GetAvatarUrl(ImageFormat.Png)).WithColor(await ColorUtils.GetSingleAsync());
                var pollStartMessage = await commandContext.RespondAsync(bob.Build());
                foreach (var emote in _pollEmojiCache)
                {
                    await pollStartMessage.CreateReactionAsync(emote);
                }
                using var rng = new RandomGenerator();
                await Database.plannedEvents.AddAsync(new() { ChannelID = commandContext.Channel.Id, EventID = rng.RandomAbcString(20), Handled = false, MessageID = commandContext.Message.Id, Time = DateTime.Now + duration, Type = Objects.Database.Classes.PlannedEventType.EmojiPoll, UserID = commandContext.User.Id, ResponseMessageID = pollStartMessage.Id });
                await Database.SaveChangesAsync();
            }
            else
            {
                await commandContext.RespondAsync("Error: the question can't be empty");
            }
        }

        [Command("GiveAway"), Description("Start a simple giveaway")]
        public async Task GiveAway(CommandContext commandContext, [Description("How long should the giveaway last. (e.g. 1m = 1 minute)")] TimeSpan duration, [Description("Giveaway content"), RemainingText] string item)
        {
            if (!string.IsNullOrEmpty(item))
            {
                var client = commandContext.Client;
                if (_pollEmojiCache == null)
                {
                    _pollEmojiCache = new[] {
                        DiscordEmoji.FromName(client, ":everybodyvotes:"),
                        DiscordEmoji.FromName(client, ":nobodyvotes:")
                    };
                }
                var bob = new DiscordEmbedBuilder();
                bob.WithTitle(item).WithAuthor(commandContext.Member.Nickname ?? commandContext.User.Username, iconUrl: commandContext.User.GetAvatarUrl(ImageFormat.Png));
                var pollStartMessage = await commandContext.RespondAsync(bob.Build());
                await pollStartMessage.CreateReactionAsync(_pollEmojiCache[0]);
                using var rng = new RandomGenerator();
                await Database.plannedEvents.AddAsync(new() { ChannelID = commandContext.Channel.Id, EventID = rng.RandomAbcString(20), Handled = false, MessageID = commandContext.Message.Id, Time = DateTime.Now + duration, Type = Objects.Database.Classes.PlannedEventType.GiveAway, UserID = commandContext.User.Id, ResponseMessageID = pollStartMessage.Id });
                await Database.SaveChangesAsync();
            }
            else
            {
                await commandContext.RespondAsync("Error: the item can't be empty");
            }
        }
    }
}