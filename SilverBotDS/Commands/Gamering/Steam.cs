using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using SilverBotDS.Attributes;
using SilverBotDS.Objects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SilverBotDS.Commands.Gamering
{
    [Category("Gaming")]
    [Group("steam")]
    internal class SteamCommands : BaseCommandModule
    {
#pragma warning disable CA1822 // Mark members as static

        [Command("search"), Aliases("s")]
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
                            case SteamStoreQuery.Enums.sType.FreeToPlay:
                                tempbuilder.WithAuthor(lang.SteamCommand.FreeToPlayGameType);
                                break;

                            case SteamStoreQuery.Enums.sType.NotAvailable:
                                tempbuilder.WithAuthor(lang.SteamCommand.NotAvailableGameType);
                                break;

                            case SteamStoreQuery.Enums.sType.CostsMoney:
                                tempbuilder.WithAuthor(lang.SteamCommand.CostsMoneyGameTypeBug);
                                break;

                            default:
                                throw new InvalidOperationException($"Unexpected value listings[i].SaleType = {listings[i].SaleType}");
                        }
                    }
                    else
                    {
                        tempbuilder.WithAuthor(string.Format(lang.SteamCommand.AmericanMoney, listings[i].Price));
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
                    var interactivity = ctx.Client.GetInteractivity();
                    await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages, token: new System.Threading.CancellationToken());
                }
                else
                {
                    var bob = new DiscordEmbedBuilder();
                    bob.WithTitle(lang.SteamCommand.NoGamesWereReturned);
                    bob.WithDescription(lang.SteamCommand.NoGamesWereReturnedDescription);
                    await ctx.RespondAsync(embed: bob.Build());
                }
            }
            catch (Exception e)
            {
                var bob = new DiscordEmbedBuilder();
                bob.WithTitle(lang.SearchFailTitle);
                bob.WithDescription($"{lang.SearchFailDescription}\n{e.Message}");
                await ctx.RespondAsync(embed: bob.Build());
                throw;
            }
        }
    }
}