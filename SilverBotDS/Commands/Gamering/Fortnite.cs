using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Fortnite_API;
using Fortnite_API.Objects.V1;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS.Commands.Gamering
{
    internal class Fortnite : BaseCommandModule
    {
        public static Fortnite CreateInstance()
        {
            return new Fortnite();
        }

#pragma warning disable CA1822 // Mark members as static
        private static FortniteApiClient api;
        private static bool disabled;

        /// <summary>
        /// fortite mor lioke fartnite am i righe or am i righe
        /// </summary>
        /// <param name="apiKey">the key to use</param>
        public static void Setapi(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey) || apiKey == "Fortnite_Token_Here" || apiKey.ToLowerInvariant() == "none")
            {
                disabled = true;
            }
            api = new FortniteApiClient(apiKey);
        }

        private static async Task SendDisabledMessage(CommandContext ctx)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithEmbed(new DiscordEmbedBuilder().WithTitle(lang.CommandIsDisabled).WithColor(color: await ColorUtils.GetSingleAsync())).SendAsync(ctx.Channel);
        }

        [Command("fortstats")]
        [Description("Get the stats of a person using their username")]
        public async Task Stats(CommandContext ctx, [Description("The username of the person")][RemainingText] string name)
        {
            if (disabled)
            {
                await SendDisabledMessage(ctx);
                return;
            }
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            var statsV2V1 = await api.V1.Stats.GetBrV2Async(x =>
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
                await ctx.RespondAsync(lang.SearchFail);
            }
        }

        [Command("fortbrnews")]
        [Description("Get a gif of the latest battle royale news")]
        public async Task Brnews(CommandContext ctx)
        {
            if (disabled)
            {
                await SendDisabledMessage(ctx);
                return;
            }
            var newsV2 = await api.V2.News.GetAsync();
            await ctx.RespondAsync(newsV2.Data.Br.Image.ToString());
        }

        [Command("fortcrnews")]
        [Description("Get a gif of the latest creative news")]
        public async Task Crnews(CommandContext ctx)
        {
            if (disabled)
            {
                await SendDisabledMessage(ctx);
                return;
            }
            var newsV2 = await api.V2.News.GetAsync();
            await ctx.RespondAsync(newsV2.Data.Creative.Image.ToString());
        }

        [Command("fortstwnews")]
        [Description("Get a gif of the latest save-the-world news")]
        public async Task Stwnews(CommandContext ctx)
        {
            if (disabled)
            {
                await SendDisabledMessage(ctx);
                return;
            }
            var newsV2 = await api.V2.News.GetAsync();
            await ctx.RespondAsync(newsV2.Data.Stw.Image.ToString());
        }

        [Command("fortitms")]
        [Description("Try to get the item shop items")]
        public async Task Itm(CommandContext ctx)
        {
            if (disabled)
            {
                await SendDisabledMessage(ctx);
                return;
            }
            var shop = await api.V2.Shop.GetBrCombinedAsync();
            var sb = new StringBuilder();
            foreach (var thing in shop.Data.Daily.Entries)
            {
                sb.Append(thing.DevName + Environment.NewLine);
            }
            await ctx.RespondAsync(sb.ToString());
        }
    }
}