using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SilverBotDS.Objects;

namespace SilverBotDS.Utils
{
    public static class EmbedBuilderUtils
    {
        public static DiscordEmbedBuilder AddRequestedByFooter(this DiscordEmbedBuilder builder, CommandContext ctx, Language lang) => 
            AddRequestedByFooter(builder,ctx.User,lang);
        public static DiscordEmbedBuilder AddRequestedByFooter(this DiscordEmbedBuilder builder, BaseContext ctx, Language lang) => 
            AddRequestedByFooter(builder,ctx.User,lang);
        public static DiscordEmbedBuilder AddRequestedByFooter(this DiscordEmbedBuilder builder, DiscordUser user, Language lang) => 
            builder.WithFooter(string.Format(lang.RequestedBy, user.Username), user.GetAvatarUrl(ImageFormat.Png));
    }
}