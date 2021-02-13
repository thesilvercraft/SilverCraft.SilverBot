using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Fortnite_API;
using Fortnite_API.Objects;
using Fortnite_API.Objects.V1;
using Fortnite_API.Objects.V2;
using SilverBotDS.Utils;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    internal class Fortnite : BaseCommandModule
    {
#pragma warning disable CA1822 // Mark members as static
        private static FortniteApiClient api;
        private static bool Disabled = false;

        /// <summary>
        /// fortite mor lioke fartnite am i righe or am i righe
        /// </summary>
        /// <param name="apie"></param>
        public static void Setapi(string api_key)
        {
            if (string.IsNullOrEmpty(api_key) || api_key == "Fortnite_Token_Here" || api_key.ToLower() == "none")
            {
                Disabled = true;
            }
            api = new(api_key);
        }

        private async Task SendDisabledMessage(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromCtx(ctx);
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithEmbed(new DiscordEmbedBuilder().WithTitle(lang.Command_Is_Disabled).WithColor(color: await ColorUtils.GetSingleAsync())).SendAsync(ctx.Channel);
        }

        [Command("fortstats")]
        [Description("Get the stats of a person using their username")]
        public async Task Stats(CommandContext ctx, [Description("The username of the person")][RemainingText()] string name)
        {
            if (Disabled)
            {
                await SendDisabledMessage(ctx);
                return;
            }
            Language lang = Language.GetLanguageFromCtx(ctx);
            ApiResponse<BrStatsV2V1> statsV2V1 = await api.V1.Stats.GetBrV2Async(x =>
            {
                x.Name = name;
                x.ImagePlatform = BrStatsV2V1ImagePlatform.All;
            });
            if (statsV2V1.IsSuccess)
            {
                await ctx.RespondAsync(statsV2V1.Data.Image.ToString());
            }
            else
            {
                await ctx.RespondAsync(lang.Search_Fail);
            }
        }

        [Command("fortbrnews")]
        [Description("Get a gif of the latest battle royale news")]
        public async Task Brnews(CommandContext ctx)
        {
            if (Disabled)
            {
                await SendDisabledMessage(ctx);
                return;
            }
            ApiResponse<NewsV2Combined> newsV2 = await api.V2.News.GetAsync();
            await ctx.RespondAsync(newsV2.Data.Br.Image.ToString());
        }

        [Command("fortcrnews")]
        [Description("Get a gif of the latest creative news")]
        public async Task Crnews(CommandContext ctx)
        {
            if (Disabled)
            {
                await SendDisabledMessage(ctx);
                return;
            }
            ApiResponse<NewsV2Combined> newsV2 = await api.V2.News.GetAsync();
            await ctx.RespondAsync(newsV2.Data.Creative.Image.ToString());
        }

        [Command("fortstwnews")]
        [Description("Get a gif of the latest save-the-world news")]
        public async Task Stwnews(CommandContext ctx)
        {
            if (Disabled)
            {
                await SendDisabledMessage(ctx);
                return;
            }
            ApiResponse<NewsV2Combined> newsV2 = await api.V2.News.GetAsync();
            await ctx.RespondAsync(newsV2.Data.Stw.Image.ToString());
        }

        [Command("fortitms")]
        [Description("Try to get the item shop items")]
        public async Task Itm(CommandContext ctx)
        {
            if (Disabled)
            {
                await SendDisabledMessage(ctx);
                return;
            }
            ApiResponse<BrShopV2Combined> shop = await api.V2.Shop.GetBrCombinedAsync();
            StringBuilder sb = new StringBuilder();
            foreach (BrShopV2StoreFrontEntry thing in shop.Data.Daily.Entries)
            {
                sb.Append(thing.DevName + Environment.NewLine);
            }
            await ctx.RespondAsync(sb.ToString());
        }
    }
}