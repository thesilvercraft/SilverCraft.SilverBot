using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using SilverBotDS.Objects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SilverBotDS.Commands.Gamering
{
    [Group("steam")]
    internal class SteamCommands : BaseCommandModule
    {
#pragma warning disable CA1822 // Mark members as static

        [Command("search"), Aliases("s")]
        [Description("Search about a game")]
        public async Task Search(CommandContext ctx, [RemainingText()][Description("The game")] string game)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            try
            {
                var listings = Steam.Search(game);
                var pages = new List<Page>();
                for (var i = 0; i < listings.Count; i++)
                {
                    var tempbuilder = new DiscordEmbedBuilder();
                    tempbuilder.WithTitle(listings[i].Name);
                    tempbuilder.WithUrl(listings[i].StoreLink);
                    if (listings[i].Price == null)
                    {
                        switch (listings[i].SaleType)
                        {
                            case SteamStoreQuery.Enums.sType.FreeToPlay:
                                tempbuilder.WithAuthor("F2P");
                                break;

                            case SteamStoreQuery.Enums.sType.NotAvailable:
                                tempbuilder.WithAuthor("Not Available");
                                break;

                            case SteamStoreQuery.Enums.sType.CostsMoney:
                                tempbuilder.WithAuthor("It costs merica bucks but idk how much");
                                break;
                        }
                    }
                    else
                    {
                        tempbuilder.WithAuthor(listings[i].Price + "merica bucks");
                    }
                    tempbuilder.WithFooter($"{lang.RequestedBy}{ctx.User.Username} {string.Format(lang.PageNuget, i + 1, listings.Count)}", ctx.User.GetAvatarUrl(ImageFormat.Png));
                    if (!string.IsNullOrEmpty(listings[i].ImageLink))
                    {
                        tempbuilder.WithThumbnail(listings[i].ImageLink);
                    }
                    pages.Add(new Page(embed: tempbuilder));
                }
                if (pages.Count > 0)
                {
                    await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages, timeoutoverride: new TimeSpan(0, 2, 0));
                }
                else
                {
                    var bob = new DiscordEmbedBuilder();
                    bob.WithTitle("0 Games were returned");
                    bob.WithDescription("Try again later, or try changing your search term");
                    await ctx.RespondAsync(embed: bob.Build());
                }
            }
            catch (Exception e)
            {
                var bob = new DiscordEmbedBuilder();
                bob.WithTitle(lang.SearchFailTitle);
                bob.WithDescription(lang.SearchFailDescription + "\n" + e.Message);
                await ctx.RespondAsync(embed: bob.Build());
                throw;
            }
        }
    }
}