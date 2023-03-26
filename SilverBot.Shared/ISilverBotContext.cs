using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace SilverBot.Shared
{
    public interface ISilverBotContext
    {
        object GetOriginal();
        public DiscordGuild? Guild { get; }
        public  DiscordClient Client { get; }
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

    public class UnBasedCommandContext :  ISilverBotContext
    {
        public static implicit operator UnBasedCommandContext(InteractionContext d) => new(d);
        public static implicit operator InteractionContext(UnBasedCommandContext d) => d.Org;
        private InteractionContext Org { get; }
        public DiscordGuild? Guild => Org.Guild;
        public DiscordChannel Channel => Org.Channel;
        public IServiceProvider Services => Org.Services;
        public DiscordClient Client => Org.Client;
        public DiscordUser User => Org.User;
        public DiscordMember? Member => Org.Member;
        public CommandsNextExtension? CommandsNext => null;
        public Task<DiscordMessage> RespondAsync(string content) => Org.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(content));
        public Task<DiscordMessage> RespondAsync(DiscordEmbed embed) => Org.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed));
        public Task<DiscordMessage> RespondAsync(string content, DiscordEmbed embed) => Org.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(content).AddEmbed(embed));

        public Task<DiscordMessage> RespondAsync(DiscordMessageBuilder builder)
        {
            var followup = new DiscordFollowupMessageBuilder();
            if (!string.IsNullOrEmpty(builder.Content))
                followup.WithContent(builder.Content);

            if (builder.Embeds.Count != 0)
            {
                followup.AddEmbeds(builder.Embeds);
            }

            if (builder.Files.Count != 0)
            {
                followup.AddFiles(builder.Files);
            }

            if (builder.Components.Count != 0)
            {
                followup.AddComponents(builder.Components);
            }
            followup.IsTTS = builder.IsTTS;
            return Org.FollowUpAsync(followup);
        }

        public Task<DiscordMessage> RespondAsync(Action<DiscordMessageBuilder> action)
        {
            var res = new DiscordMessageBuilder();
                action(res);
                return RespondAsync(res);
        }
        public  UnBasedCommandContext(InteractionContext ctx)
        {
            Org = ctx;
        }
        public object GetOriginal()
        {
            return Org;
        }
    }
    public class BasedCommandContext :  ISilverBotContext
    {
        public static implicit operator BasedCommandContext(CommandContext d) => new(d);
        public static implicit operator CommandContext(BasedCommandContext d) => d.Org;
        private CommandContext Org { get; }
        public DiscordGuild? Guild => Org.Guild;
        public DiscordChannel Channel => Org.Channel;
        public IServiceProvider Services => Org.Services;
        public DiscordClient Client => Org.Client;
        public DiscordUser User => Org.User;
        public DiscordMessage Message => Org.Message;
        public DiscordMember? Member => Org.Member;
        public CommandsNextExtension? CommandsNext => Org.CommandsNext;
        public Task<DiscordMessage> RespondAsync(string content) => Message.RespondAsync(content);
        public Task<DiscordMessage> RespondAsync(DiscordEmbed embed) => Message.RespondAsync(embed);
        public Task<DiscordMessage> RespondAsync(string content, DiscordEmbed embed) => Message.RespondAsync(content, embed);
        public Task<DiscordMessage> RespondAsync(DiscordMessageBuilder builder) => Message.RespondAsync(builder);
        public Task<DiscordMessage> RespondAsync(Action<DiscordMessageBuilder> action) => Message.RespondAsync(action);
        public  BasedCommandContext(CommandContext ctx)
        {
            Org = ctx;
        }
        public object GetOriginal()
        {
            return Org;
        }
    }
}