/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using SilverBotDS.Objects;
using SteamStoreQuery.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SilverBotDS.Attributes;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands.Gamering;

[Category("Gaming")]
[Group("steam")]
[RequireModuleGuildEnabled(EnabledModules.Steam, true)]

public class SteamCommands : BaseCommandModule
{
    public LanguageService LanguageService { private get; set; }

    [Command("search")]
    [Aliases("s")]
    [Description("Search about a game")]
    public async Task Search(CommandContext ctx, [RemainingText][Description("The game")] string game)
    {
        var lang = await LanguageService.FromCtxAsync(ctx);
        try
        {
            var listings = SteamStoreQuery.Query.Search(game);
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
                    $"{string.Format(lang.RequestedBy, ctx.User.Username)} {string.Format(lang.PageNuget, i + 1, listings.Count)}",
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