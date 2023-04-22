using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace SilverBot.Shared
{
    public class TextNeutralCommandContext : ISilverBotContext
    {
        public static implicit operator TextNeutralCommandContext(CommandContext d)
        {
            return new TextNeutralCommandContext(d);
        }

        public static implicit operator CommandContext(TextNeutralCommandContext d)
        {
            return d.Org;
        }

        private CommandContext Org { get; }
        public DiscordGuild? Guild => Org.Guild;
        public DiscordChannel Channel => Org.Channel;
        public IServiceProvider Services => Org.Services;
        public DiscordClient Client => Org.Client;
        public DiscordUser User => Org.User;
        public DiscordMessage Message => Org.Message;
        public DiscordMember? Member => Org.Member;
        public CommandsNextExtension? CommandsNext => Org.CommandsNext;

        public Task<DiscordMessage> RespondAsync(string content)
        {
            return Message.RespondAsync(content);
        }

        public Task<DiscordMessage> RespondAsync(DiscordEmbed embed)
        {
            return Message.RespondAsync(embed);
        }

        public Task<DiscordMessage> RespondAsync(string content, DiscordEmbed embed)
        {
            return Message.RespondAsync(content, embed);
        }

        public Task<DiscordMessage> RespondAsync(DiscordMessageBuilder builder)
        {
            return Message.RespondAsync(builder);
        }

        public Task<DiscordMessage> RespondAsync(Action<DiscordMessageBuilder> action)
        {
            return Message.RespondAsync(action);
        }

        public TextNeutralCommandContext(CommandContext ctx)
        {
            Org = ctx;
        }

        public object GetOriginal()
        {
            return Org;
        }
    }
}