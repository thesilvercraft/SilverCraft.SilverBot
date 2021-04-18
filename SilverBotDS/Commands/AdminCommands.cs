using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    [RequireUserPermissions(Permissions.Administrator)]
    public class AdminCommands : BaseCommandModule
    {
        private DiscordEmoji[] _pollEmojiCache;

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
                var pollStartText = new StringBuilder();
                bob.WithTitle(question);
                bob.WithAuthor(commandContext.Member.Nickname ?? commandContext.User.Username, iconUrl: commandContext.User.GetAvatarUrl(ImageFormat.Png));
                var pollStartMessage = await commandContext.RespondAsync(bob.Build());
                var pollResult = await interactivity.DoPollAsync(pollStartMessage, _pollEmojiCache, PollBehaviour.KeepEmojis, duration);
                var yesVotes = pollResult.First(x => x.Emoji.Name == "everybodyvotes").Total;
                var noVotes = pollResult.First(x => x.Emoji.Name == "nobodyvotes").Total;
                var pollResultText = new StringBuilder();
                pollResultText.Append("Poll result: ");
                pollResultText.Append("**");
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
                pollResultText.Append("**");
                pollResultText.Append($"\nYes:{yesVotes} No:{noVotes} Undecided: {commandContext.Guild.MemberCount - (yesVotes + noVotes)} (server total-people that voted)");
                bob.WithDescription(pollResultText.ToString());
                await pollStartMessage.ModifyAsync(embed: bob.Build());
            }
            else
            {
                await commandContext.RespondAsync("Error: the question can't be empty");
            }
        }
    }
}