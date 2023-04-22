using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using SilverBot.Shared.Objects.Language;
using SilverBot.Shared.Utils;

namespace SilverBot.Shared
{
    public static class ContextExtensions
    {
        public static async Task<Language?> GetLanguageAsync(this BaseContext ctx, Language? language = null)
        {
            var languageService = ctx.Services.GetService<LanguageService>();
            if (languageService == null)
            {
                return language;
            }

            language ??= await languageService.FromCtxAsync(ctx);
            language ??= await languageService.GetDefaultAsync();
            return language;
        }

        public static async Task<Language?> GetLanguageAsync(this CommandContext ctx, Language? language = null)
        {
            var languageService = ctx.Services.GetService<LanguageService>();
            if (languageService == null)
            {
                return language;
            }

            language ??= await languageService.FromCtxAsync(ctx);
            language ??= await languageService.GetDefaultAsync();
            return language;
        }

        public static async Task<Language?> GetLanguageAsync(this ISilverBotContext ctx, Language? language = null)
        {
            var languageService = ctx.Services.GetService<LanguageService>();
            if (languageService == null)
            {
                return language;
            }

            language ??= await languageService.FromCtxAsync(ctx);
            language ??= await languageService.GetDefaultAsync();
            return language;
        }

        public static Language? GetLanguage(this ISilverBotContext ctx, Language? language = null)
        {
            var languageService = ctx.Services.GetService<LanguageService>();
            if (languageService == null)
            {
                return language;
            }

            language ??= languageService.FromCtx(ctx);
            language ??= languageService.GetDefault();
            return language;
        }

        public static Language? GetLanguage(this CommandContext ctx, Language? language = null)
        {
            var languageService = ctx.Services.GetService<LanguageService>();
            if (languageService == null)
            {
                return language;
            }

            language ??= languageService.FromCtx(ctx);
            language ??= languageService.GetDefault();
            return language;
        }

        public static async Task SendStringFileWithContent(this CommandContext ctx, string title, string file,
            string filename = "message.txt")
        {
            await new DiscordMessageBuilder().WithContent(title)
                .AddFile(filename, new MemoryStream(Encoding.UTF8.GetBytes(file))).WithAllowedMentions(Mentions.None)
                .SendAsync(ctx.Channel);
        }

        public static async Task SendStringFileWithContent(this BaseContext ctx, string title, string file,
            string filename = "message.txt")
        {
            await new DiscordMessageBuilder().WithContent(title)
                .AddFile(filename, new MemoryStream(Encoding.UTF8.GetBytes(file))).WithAllowedMentions(Mentions.None)
                .SendAsync(ctx.Channel);
        }

        public static async Task SendStringFileWithContent(this ISilverBotContext ctx, string title, string file,
            string filename = "message.txt")
        {
            await new DiscordMessageBuilder().WithContent(title)
                .AddFile(filename, new MemoryStream(Encoding.UTF8.GetBytes(file))).WithAllowedMentions(Mentions.None)
                .SendAsync(ctx.Channel);
        }

        private static void CommonSendMessageLogic(DiscordEmbedBuilder embedBuilder, string title = "",
            string message = "", string imageUrl = "", string url = "")
        {
            if (!string.IsNullOrEmpty(message))
            {
                embedBuilder.WithDescription(message);
            }

            if (!string.IsNullOrEmpty(title))
            {
                embedBuilder.WithTitle(title);
            }

            if (!string.IsNullOrEmpty(imageUrl))
            {
                embedBuilder.WithThumbnail(imageUrl);
            }

            if (!string.IsNullOrEmpty(url))
            {
                embedBuilder.WithUrl(url);
            }
        }

        public static async Task SendMessageAsync(this CommandContext ctx, string title = "", string message = "",
            string imageUrl = "", string url = "", Language? language = null)
        {
            language ??= await ctx.GetLanguageAsync(language);
            var embedBuilder = ctx.GetNewBuilder(language);
            var messageBuilder = new DiscordMessageBuilder();
            CommonSendMessageLogic(embedBuilder, title, message, imageUrl, url);
            await messageBuilder
                .WithReply(ctx.Message.Id)
                .WithEmbed(embedBuilder.Build())
                .SendAsync(ctx.Channel);
        }

        public static async Task SendMessageAsync(this ISilverBotContext ctx, string title = "", string message = "",
            string imageUrl = "", string url = "", Language? language = null)
        {
            language ??= await ctx.GetLanguageAsync(language);
            var embedBuilder = ctx.GetNewBuilder(language);
            var messageBuilder = new DiscordMessageBuilder();
            CommonSendMessageLogic(embedBuilder, title, message, imageUrl, url);
            ctx.RespondAsync(embedBuilder);
        }

        public static async Task SendMessageAsync(this BaseContext ctx, string title = "", string message = "",
            string imageUrl = "", string url = "", Language? language = null)
        {
            language ??= await ctx.GetLanguageAsync(language);
            var embedBuilder = ctx.GetNewBuilder(language);
            CommonSendMessageLogic(embedBuilder, title, message, imageUrl, url);
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedBuilder.Build()));
        }

        public static DiscordEmbedBuilder GetNewBuilder(this BaseContext ctx, Language language)
        {
            var colourService = ctx.Services.GetService<ColourService>();
            if (colourService is not null)
            {
                return new DiscordEmbedBuilder()
                    .AddRequestedByFooter(ctx, language)
                    .WithColor(colourService.GetSingle());
            }

            return new DiscordEmbedBuilder()
                .AddRequestedByFooter(ctx, language);
        }

        public static DiscordEmbedBuilder GetNewBuilder(this ISilverBotContext ctx, Language language)
        {
            var colourService = ctx.Services.GetService<ColourService>();
            if (colourService is not null)
            {
                return new DiscordEmbedBuilder()
                    .AddRequestedByFooter(ctx, language)
                    .WithColor(colourService.GetSingle());
            }

            return new DiscordEmbedBuilder()
                .AddRequestedByFooter(ctx, language);
        }

        public static DiscordEmbedBuilder GetNewBuilder(this CommandContext ctx, Language language)
        {
            var colourService = ctx.Services.GetService<ColourService>();
            if (colourService is not null)
            {
                return new DiscordEmbedBuilder()
                    .AddRequestedByFooter(ctx, language)
                    .WithColor(colourService.GetSingle());
            }

            return new DiscordEmbedBuilder()
                .AddRequestedByFooter(ctx, language);
        }
    }
}