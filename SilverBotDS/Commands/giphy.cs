using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;

namespace SilverBotDS
{
    [Group("gif")]
    internal class giphy : BaseCommandModule
    {
        [Command("random")]
        public async Task Kindsffeefergergrgfdfdsgfdfg(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild.Id);
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            GiphyDotNet.Model.Results.GiphyRandomResult gifresult = await GiphyO.Get().RandomGif(new RandomParameter
            {
                Rating = Rating.Pg
            });
            b.WithDescription(lang.Random_GIF + gifresult.Data.Url);
            b.WithAuthor(lang.Powered_by_GIPHY, "https://developers.giphy.com/", "https://cdn.discordapp.com/attachments/728360861483401240/747894851814817863/Poweredby_640px_Badge.gif");
            b.WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            b.WithImageUrl(gifresult.Data.ImageUrl);
            await ctx.RespondAsync(embed: b.Build());
        }

        [Command("search"), Aliases("s")]
        public async Task Kindsffeefergergrgfdfdsgfdgfdsfgdfgfdfdghdfg(CommandContext ctx, [RemainingText] string term)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild.Id);
            GiphyDotNet.Model.Results.GiphySearchResult gifResult;
            int page = 0;
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            SearchParameter searchParameter = new SearchParameter
            {
                Query = term,
                Rating = Rating.Pg,
            };
            gifResult = await GiphyO.Get().GifSearch(searchParameter);
            string formated = string.Format(lang.Searched_for, term);
            b.WithDescription(formated + " : " + gifResult.Data[0].Url + $"\n Page 1/{gifResult.Data.Length} Use `next` in the next 5 min to see the next page");
            b.WithAuthor(lang.Powered_by_GIPHY, "https://developers.giphy.com/", "https://cdn.discordapp.com/attachments/728360861483401240/747894851814817863/Poweredby_640px_Badge.gif");
            b.WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            b.WithImageUrl(gifResult.Data[0].Images.Original.Url);
            DiscordMessage AmIthisDumb = await ctx.RespondAsync(embed: b.Build());

        Wait:
            var interactivity = ctx.Client.GetInteractivity();
            var msg = await interactivity.WaitForMessageAsync(xm => xm.Content.Contains("next"), TimeSpan.FromSeconds(60));
            if (msg.Result != null)
            {
                page++;
                if (page > gifResult.Data.Length)
                {
                    page = 0;
                }

                b.WithDescription(formated + " : " + gifResult.Data[page].Url + $"\n Page {(page + 1)}/{gifResult.Data.Length} Use `next` in the next 5 min to see the next page");
                b.WithAuthor(lang.Powered_by_GIPHY, "https://developers.giphy.com/", "https://cdn.discordapp.com/attachments/728360861483401240/747894851814817863/Poweredby_640px_Badge.gif");
                b.WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
                b.WithImageUrl(gifResult.Data[page].Images.Original.Url);
                await AmIthisDumb.ModifyAsync(embed: b.Build());
                goto Wait;
            }
            else
            {
                await AmIthisDumb.ModifyAsync("You may not send `next` as the 5 minutes expired");
            }
        }
    }
}