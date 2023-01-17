using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Serilog;
using SilverBot.Shared.Objects.Language;

namespace SilverBot.Shared.Utils
{
    public static class EmbedBuilderUtils
    {
        public static DiscordEmbedBuilder AddRequestedByFooter(this DiscordEmbedBuilder builder, CommandContext ctx, Language language) => 
            AddRequestedByFooter(builder, ctx.User, language);
        public static DiscordEmbedBuilder AddRequestedByFooter(this DiscordEmbedBuilder builder, BaseContext ctx, Language language) => 
            AddRequestedByFooter(builder, ctx.User, language);
        public static DiscordEmbedBuilder AddRequestedByFooter(this DiscordEmbedBuilder builder, DiscordUser user, Language language) =>
            builder.WithFooter(string.Format(language.RequestedBy, user.Username),
                user.GetAvatarUrl(ImageFormat.Png));
    }
}