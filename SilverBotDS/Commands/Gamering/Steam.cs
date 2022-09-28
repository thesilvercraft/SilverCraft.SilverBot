using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using SilverBotDS.Attributes;
using SilverBotDS.Objects;
using SteamStoreQuery.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands.Gamering;

[Category("Gaming")]
[Group("steam")]
public class SteamCommands : BaseCommandModule
{
    [Command("search")]
    [Aliases("s")]
    [Description("Search about a game")]
    public async Task Search(CommandContext ctx, [RemainingText][Description("The game")] string game)
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
                        case sType.FreeToPlay:
                            tempbuilder.WithAuthor(lang.FreeToPlayGameType);
                            break;

                        case sType.NotAvailable:
                            tempbuilder.WithAuthor(lang.NotAvailableGameType);
                            break;

                        case sType.CostsMoney:
                            tempbuilder.WithAuthor(lang.CostsMoneyGameTypeBug);
                            break;

                        default:
                            throw new InvalidOperationException(
                                $"Unexpected value listings[i].SaleType = {listings[i].SaleType}");
                    }
                }
                else
                {
                    tempbuilder.WithAuthor(string.Format(lang.AmericanMoney, listings[i].Price));
                }

                tempbuilder.WithFooter(
                    $"{lang.RequestedBy}{ctx.User.Username} {string.Format(lang.PageNuget, i + 1, listings.Count)}",
                    ctx.User.GetAvatarUrl(ImageFormat.Png));
                if (!string.IsNullOrEmpty(listings[i].ImageLink))
                {
                    tempbuilder.WithThumbnail(listings[i].ImageLink);
                }

                pages.Add(new Page(embed: tempbuilder));
            }

            if (pages.Count > 0)
            {
                var interactivity = ctx.Client.GetInteractivity();
                await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages,
                    token: new CancellationToken());
            }
            else
            {
                var bob = new DiscordEmbedBuilder();
                bob.WithTitle(lang.NoGamesWereReturned);
                bob.WithDescription(lang.NoGamesWereReturnedDescription);
                await ctx.RespondAsync(bob.Build());
            }
        }
        catch (Exception e)
        {
            var bob = new DiscordEmbedBuilder();
            bob.WithTitle(lang.SearchFailTitle);
            bob.WithDescription($"{lang.SearchFailDescription}\n{e.Message}");
            await ctx.RespondAsync(bob.Build());
            throw;
        }
    }
}