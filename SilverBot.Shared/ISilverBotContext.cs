using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace SilverBot.Shared
{
    public interface ISilverBotContext
    {
        object GetOriginal();
        public DiscordGuild? Guild { get; }
        public DiscordClient Client { get; }
        public DiscordUser User { get; }
        public DiscordChannel Channel { get; }
        public IServiceProvider Services { get; }
        public DiscordMember? Member { get; }
        public CommandsNextExtension? CommandsNext { get; }
        public Task<DiscordMessage> RespondAsync(string content);
        public Task<DiscordMessage> RespondAsync(DiscordEmbed embed);
        public Task<DiscordMessage> RespondAsync(string content, DiscordEmbed embed);
        public Task<DiscordMessage> RespondAsync(DiscordMessageBuilder builder);
        public Task<DiscordMessage> RespondAsync(Action<DiscordMessageBuilder> action);
    }
}