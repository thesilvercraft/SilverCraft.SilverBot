using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using GiphyDotNet.Model.Parameters;
using GiphyDotNet.Model.Results;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using System;
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    [Group("gif")]
    internal class Giphy : BaseCommandModule
    {
        public static Giphy CreateInstance()
        {
            return new Giphy();
        }

#pragma warning disable CA1822 // Mark members as static
        private static GiphyDotNet.Manager.Giphy giphy = new();

        public static void Set(string token)
        {
            if (string.IsNullOrEmpty(token) || token == "Giphy_Token_Here" || token.ToLowerInvariant() == "none")
            {
                giphy = new GiphyDotNet.Manager.Giphy();
            }
            else
            {
                giphy = new(token);
            }
        }

        [Command("random")]
        public async Task Kindsffeefergergrgfdfdsgfdfg(CommandContext ctx)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            var gifresult = await giphy.RandomGif(new RandomParameter
            {
                Rating = Rating.Pg
            });
#pragma warning disable S1075 // URIs should not be hardcoded
            var b = new DiscordEmbedBuilder().WithDescription(lang.RandomGif + gifresult.Data.Url).WithAuthor(lang.PoweredByGiphy, "https://developers.giphy.com/", "https://cdn.discordapp.com/attachments/728360861483401240/747894851814817863/Poweredby_640px_Badge.gif").WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).WithImageUrl(gifresult.Data.ImageUrl).WithColor(color: await ColorUtils.GetSingleAsync());
#pragma warning restore S1075 // URIs should not be hardcoded
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithEmbed(b.Build()).SendAsync(ctx.Channel);
        }

        [Command("search"), Aliases("s")]
        public async Task Kindsffeefergergrgfdfdsgfdgfdsfgdfgfdfdghdfg(CommandContext ctx, [RemainingText] string term)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            var page = 0;
            var b = new DiscordEmbedBuilder();
            var searchParameter = new SearchParameter
            {
                Query = term,
                Rating = Rating.Pg,
            };
            var gifResult = await giphy.GifSearch(searchParameter);
            var formated = string.Format(lang.SearchedFor, term);
            b.WithDescription($"{formated} : {gifResult.Data[0].Url} {string.Format(lang.PageGif, 1, gifResult.Data.Length)}").WithAuthor(
#pragma warning disable S1075 // URIs should not be hardcoded
                lang.PoweredByGiphy, "https://developers.giphy.com/",
                "https://cdn.discordapp.com/attachments/728360861483401240/747894851814817863/Poweredby_640px_Badge.gif").WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).WithImageUrl(gifResult.Data[0].Images.Original.Url).WithColor(color: await ColorUtils.GetSingleAsync());
#pragma warning restore S1075 // URIs should not be hardcoded
            await WaitForNextMessage(ctx, await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithEmbed(b.Build()).AddComponents(new DiscordButtonComponent(ButtonStyle.Primary, "nextgif", lang.PageGifButtonText)).SendAsync(ctx.Channel), ctx.Client.GetInteractivity(), lang, page, formated, gifResult, b);
        }

        private async Task WaitForNextMessage(CommandContext ctx, DiscordMessage oldmessage, InteractivityExtension interactivity, Language lang, int page, string formated, GiphySearchResult gifResult, DiscordEmbedBuilder b = null)
        {
            b ??= new DiscordEmbedBuilder();
            var msg = await oldmessage.WaitForButtonAsync(ctx.User, TimeSpan.FromSeconds(300));
            if (msg.Result != null)
            {
                page++;
                if (page >= gifResult.Data.Length)
                {
                    page = 0;
                } 
                b.WithDescription($"{formated} : {gifResult.Data[page].Url} {string.Format(lang.PageGif, page + 1, gifResult.Data.Length)}").WithImageUrl(gifResult.Data[page].Images.Original.Url).WithColor(color: await ColorUtils.GetSingleAsync());
                await msg.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithEmbed(b).AddComponents(new DiscordButtonComponent(ButtonStyle.Primary, "nextgif", lang.PageGifButtonText))));
                await WaitForNextMessage(ctx, oldmessage, interactivity, lang, page, formated, gifResult, b);
            }
            else
            {
                await oldmessage.ModifyAsync(new DiscordMessageBuilder().WithEmbed(b).WithContent(lang.PeriodExpired).AddComponents(new DiscordButtonComponent(ButtonStyle.Primary, "nextgif", lang.PageGifButtonText, true)));
            }
        }
    }
}