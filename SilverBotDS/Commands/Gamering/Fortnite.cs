using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Fortnite_API;
using Fortnite_API.Objects.V1;
using SilverBotDS.Attributes;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SilverBotDS.Utils;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS.Commands.Gamering
{
    [Category("Gaming")]
    [Group("fortnite")]
    public class Fortnite : SilverBotCommandModule
    {
        public Config config {private get; set; }
        public override Task<bool> ExecuteRequirements(Config conf)
        {
            return Task.FromResult(!(string.IsNullOrEmpty(conf.FApiToken) || conf.FApiToken == "Fortnite_Token_Here" || string.Equals(conf.FApiToken, "none", StringComparison.InvariantCultureIgnoreCase)));
        }
        public static Fortnite CreateInstance()
        {
            return new Fortnite();
        }
        private FortniteApiClient api;
        /// <summary>
        /// fortite mor lioke fartnite am i righe or am i righe
        /// </summary>
        /// <param name="apiKey">the key to use</param>
        public void MakeSureApiIsSet()
        {
            api = new FortniteApiClient(config.FApiToken);
        }
        [Command("fortstats")]
        [Description("Get the stats of a person using their username")]
        public async Task Stats(CommandContext ctx, [Description("The username of the person")][RemainingText] string name)
        {
            MakeSureApiIsSet();
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
            MakeSureApiIsSet();
            var newsV2 = await api.V2.News.GetAsync();
            await ctx.RespondAsync(newsV2.Data.Br.Image.ToString());
        }

        [Command("fortcrnews")]
        [Description("Get a gif of the latest creative news")]
        public async Task Crnews(CommandContext ctx)
        {
            MakeSureApiIsSet();
            var newsV2 = await api.V2.News.GetAsync();
            await ctx.RespondAsync(newsV2.Data.Creative.Image.ToString());
        }

        [Command("fortstwnews")]
        [Description("Get a gif of the latest save-the-world news")]
        public async Task Stwnews(CommandContext ctx)
        {
            MakeSureApiIsSet();
            var newsV2 = await api.V2.News.GetAsync();
            await ctx.RespondAsync(newsV2.Data.Stw.Image.ToString());
        }

        [Command("fortitms")]
        [Description("Try to get the item shop items")]
        public async Task Itm(CommandContext ctx)
        {
            MakeSureApiIsSet();
            var shop = await api.V2.Shop.GetBrCombinedAsync();
            var sb = new StringBuilder();
            foreach (var thing in shop.Data.Daily.Entries)
            {
                sb.Append(thing.DevName).Append(Environment.NewLine);
            }
            await ctx.RespondAsync(sb.ToString());
        }
    }
}