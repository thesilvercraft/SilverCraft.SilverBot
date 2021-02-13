using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using GiphyDotNet.Model.Parameters;
using System;
using System.Threading.Tasks;
using GiphyDotNet.Manager;
using SilverBotDS.Utils;

namespace SilverBotDS.Commands
{
    [Group("gif")]
    internal class Giphy : BaseCommandModule
    {
#pragma warning disable CA1822 // Mark members as static
        private static GiphyDotNet.Manager.Giphy giphy = new GiphyDotNet.Manager.Giphy();

        public static void Set(string token)
        {
            if (string.IsNullOrEmpty(token) || token == "Giphy_Token_Here" || token.ToLower() == "none")
            {
                giphy = new();
            }
            else
            {
                giphy = new(token);
            }
        }

        [Command("random")]
        public async Task Kindsffeefergergrgfdfdsgfdfg(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromCtx(ctx);
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            GiphyDotNet.Model.Results.GiphyRandomResult gifresult = await giphy.RandomGif(new RandomParameter
            {
                Rating = Rating.Pg
            });
            b.WithDescription(lang.Random_GIF + gifresult.Data.Url).WithAuthor(lang.Powered_by_GIPHY, "https://developers.giphy.com/", "https://cdn.discordapp.com/attachments/728360861483401240/747894851814817863/Poweredby_640px_Badge.gif").WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).WithImageUrl(gifresult.Data.ImageUrl).WithColor(color: await ColorUtils.GetSingleAsync());

            await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithEmbed(b.Build()).SendAsync(ctx.Channel);
        }

        [Command("search"), Aliases("s")]
        public async Task Kindsffeefergergrgfdfdsgfdgfdsfgdfgfdfdghdfg(CommandContext ctx, [RemainingText] string term)
        {
            Language lang = Language.GetLanguageFromCtx(ctx);
            GiphyDotNet.Model.Results.GiphySearchResult gifResult;
            int page = 0;
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            SearchParameter searchParameter = new SearchParameter
            {
                Query = term,
                Rating = Rating.Pg,
            };
            gifResult = await giphy.GifSearch(searchParameter);
            string formated = string.Format(lang.Searched_for, term);
            b.WithDescription(formated + " : " + gifResult.Data[0].Url + string.Format(lang.Page_Gif, 1, gifResult.Data.Length)).WithAuthor(lang.Powered_by_GIPHY, "https://developers.giphy.com/", "https://cdn.discordapp.com/attachments/728360861483401240/747894851814817863/Poweredby_640px_Badge.gif").WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).WithImageUrl(gifResult.Data[0].Images.Original.Url).WithColor(color: await ColorUtils.GetSingleAsync());
            DiscordMessage AmIthisDumb = await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithEmbed(b.Build()).SendAsync(ctx.Channel);

        Wait:
            var interactivity = ctx.Client.GetInteractivity();
            var msg = await interactivity.WaitForMessageAsync(xm => xm.Content.Contains("next"), TimeSpan.FromSeconds(60));
            if (msg.Result != null)
            {
                _ = msg.Result.DeleteAsync();
                page++;
                if (page > gifResult.Data.Length)
                {
                    page = 0;
                }

                b.WithDescription(formated + " : " + gifResult.Data[page].Url + string.Format(lang.Page_Gif, page + 1, gifResult.Data.Length)).WithAuthor(lang.Powered_by_GIPHY, "https://developers.giphy.com/", "https://cdn.discordapp.com/attachments/728360861483401240/747894851814817863/Poweredby_640px_Badge.gif").WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).WithImageUrl(gifResult.Data[page].Images.Original.Url).WithColor(color: await ColorUtils.GetSingleAsync());
                await AmIthisDumb.ModifyAsync(embed: b.Build());
                goto Wait;
            }
            else
            {
                await AmIthisDumb.ModifyAsync(lang.Period_Expired);
            }
        }
    }
}