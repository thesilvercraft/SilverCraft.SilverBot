using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using SteamStoreQuery;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SilverBotDS
{
    [Group("steam")]
    internal class SteamCommands : BaseCommandModule
    {
#pragma warning disable CA1822 // Mark members as static

        [Command("search"), Aliases("s")]
        [Description("Search about a game")]
        public async Task Search(CommandContext ctx, [RemainingText()][Description("The game")] string game)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild?.Id);
            try
            {
                List<Listing> listings = Steam.Search(game);
                InteractivityExtension interactivity = ctx.Client.GetInteractivity();
                List<Page> pages = new List<Page>();
                DiscordEmbedBuilder tempbuilder;
                for (int i = 0; i < listings.Count; i++)
                {
                    tempbuilder = new DiscordEmbedBuilder();
                    tempbuilder.WithTitle(listings[i].Name);
                    tempbuilder.WithUrl(listings[i].StoreLink);
                    if (listings[i].Price == null)
                    {
                        if (listings[i].SaleType == SteamStoreQuery.Enums.sType.FreeToPlay)
                        {
                            tempbuilder.WithAuthor("F2P");
                        }
                        else if (listings[i].SaleType == SteamStoreQuery.Enums.sType.NotAvailable)
                        {
                            tempbuilder.WithAuthor("Not Available");
                        }
                        else if (listings[i].SaleType == SteamStoreQuery.Enums.sType.CostsMoney)
                        {
                            tempbuilder.WithAuthor("It costs merica bucks but idk how much");
                        }
                    }
                    else
                    {
                        tempbuilder.WithAuthor(listings[i].Price + "merica bucks");
                    }

                    tempbuilder.WithFooter(lang.Requested_by + ctx.User.Username + $" Page: {i + 1}/{listings.Count}", ctx.User.GetAvatarUrl(ImageFormat.Png));
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
                    DiscordEmbedBuilder bob = new DiscordEmbedBuilder();
                    bob.WithTitle("Something went fucky wucky on my side");
                    bob.WithDescription("Try again a little later?\n 0 Games were returned");
                    await ctx.RespondAsync(embed: bob.Build());
                }
            }
            catch (Exception e)
            {
                DiscordEmbedBuilder bob = new DiscordEmbedBuilder();
                bob.WithTitle("Something went fucky wucky on my side");
                bob.WithDescription("Try again a little later?\n" + e.Message);
                await ctx.RespondAsync(embed: bob.Build());
                throw;
            }
        }
    }
}