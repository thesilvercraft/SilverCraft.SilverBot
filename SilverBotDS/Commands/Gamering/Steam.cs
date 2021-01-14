using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using SteamStoreQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS
{
    [Group("steam")]
    internal class SteamCommands : BaseCommandModule
    {
        [Command("search"), Aliases("s")]
        [Description("Search about a game")]
        public async Task Search(CommandContext ctx, [RemainingText()][Description("The game")] string game)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild.Id);
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
                    tempbuilder.WithAuthor(listings[i].Price + "merica bucks");
                    tempbuilder.WithFooter(lang.Requested_by + ctx.User.Username + $" Page: {i}/{listings.Count}", ctx.User.GetAvatarUrl(ImageFormat.Png));
                    if (!string.IsNullOrEmpty(listings[i].ImageLink))
                    {
                        tempbuilder.WithThumbnail(listings[i].ImageLink);
                    }
                    pages.Add(new Page(embed: tempbuilder));
                }

                await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages, timeoutoverride: new TimeSpan(0, 2, 0));
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