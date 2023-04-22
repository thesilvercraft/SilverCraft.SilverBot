using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace SilverBot.Shared
{
    public class SlashNeutralCommandContext : ISilverBotContext
    {
        public static implicit operator SlashNeutralCommandContext(InteractionContext d)
        {
            return new SlashNeutralCommandContext(d);
        }

        public static implicit operator InteractionContext(SlashNeutralCommandContext d)
        {
            return d.Org;
        }

        private InteractionContext Org { get; }
        public DiscordGuild? Guild => Org.Guild;
        public DiscordChannel Channel => Org.Channel;
        public IServiceProvider Services => Org.Services;
        public DiscordClient Client => Org.Client;
        public DiscordUser User => Org.User;
        public DiscordMember? Member => Org.Member;
        public CommandsNextExtension? CommandsNext => null;

        public Task<DiscordMessage> RespondAsync(string content)
        {
            return Org.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(content));
        }

        public Task<DiscordMessage> RespondAsync(DiscordEmbed embed)
        {
            return Org.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed));
        }

        public Task<DiscordMessage> RespondAsync(string content, DiscordEmbed embed)
        {
            return Org.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(content).AddEmbed(embed));
        }

        public Task<DiscordMessage> RespondAsync(DiscordMessageBuilder builder)
        {
            var followup = new DiscordFollowupMessageBuilder();
            if (!string.IsNullOrEmpty(builder.Content))
            {
                followup.WithContent(builder.Content);
            }

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

        public SlashNeutralCommandContext(InteractionContext ctx)
        {
            Org = ctx;
        }

        public object GetOriginal()
        {
            return Org;
        }
    }
}