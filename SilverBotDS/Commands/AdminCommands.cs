using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using SilverBotDS.Attributes;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                var interactivity = client.GetInteractivity();
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
                var pollResult = await interactivity.DoPollAsync(pollStartMessage, _pollEmojiCache, PollBehaviour.KeepEmojis, duration);
                var yesVotes = pollResult.First(x => x.Emoji.Name == "everybodyvotes").Total;
                var noVotes = pollResult.First(x => x.Emoji.Name == "nobodyvotes").Total;
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
                pollResultText.Append("**\nYes:").Append(yesVotes).Append(" No:").Append(noVotes).Append(" Undecided: ").Append(commandContext.Guild.MemberCount - (yesVotes + noVotes)).Append(" (server total-people that voted)");
                bob.WithDescription(pollResultText.ToString());
                await pollStartMessage.ModifyAsync(embed: bob.Build());
            }
            else
            {
                await commandContext.RespondAsync("Error: the question can't be empty");
            }
        }

        [Command("GiveAway"), Description("Start a simple giveaway")]
        public async Task GiveAway(CommandContext commandContext, [Description("How long should the giveaway last. (e.g. 1m = 1 minute)")] TimeSpan duration, [Description("Giveaway content"), RemainingText] string question)
        {
            if (!string.IsNullOrEmpty(question))
            {
                var client = commandContext.Client;
                var interactivity = client.GetInteractivity();
                if (_pollEmojiCache == null)
                {
                    _pollEmojiCache = new[] {
                        DiscordEmoji.FromName(client, ":everybodyvotes:"),
                        DiscordEmoji.FromName(client, ":nobodyvotes:")
                    };
                }
                var bob = new DiscordEmbedBuilder();
                bob.WithTitle(question).WithAuthor(commandContext.Member.Nickname ?? commandContext.User.Username, iconUrl: commandContext.User.GetAvatarUrl(ImageFormat.Png));
                var pollStartMessage = await commandContext.RespondAsync(bob.Build());

                var pollResult = await interactivity.DoPollAsync(pollStartMessage, _pollEmojiCache.SkipLast(1), PollBehaviour.KeepEmojis, duration);
                var yesVotes = pollResult.First(x => x.Emoji.Name == "everybodyvotes").Voted;
                if (yesVotes.Count == 0)
                {
                    await commandContext.RespondAsync("Error: noone reacted in time");
                    return;
                }
                RandomGenerator generator = new();
                await commandContext.RespondAsync($"{yesVotes.ElementAt(generator.Next(0, yesVotes.Count)).Mention} won {question}");
            }
            else
            {
                await commandContext.RespondAsync("Error: the question can't be empty");
            }
        }
    }
}