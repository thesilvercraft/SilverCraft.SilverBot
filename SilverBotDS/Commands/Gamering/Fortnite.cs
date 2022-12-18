/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Fortnite_API;
using Fortnite_API.Objects.V1;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using System;
using System.Text;
using System.Threading.Tasks;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands.Gamering;

[Category("Gaming")]
[Group("fortnite")]
public class Fortnite : BaseCommandModule, IHaveExecutableRequirements
{
    private FortniteApiClient _api;
    public Config Config { private get; set; }
    public LanguageService LanguageService { private get; set; }
    public Task<bool> ExecuteRequirements(Config conf)
    {
        return Task.FromResult(!(string.IsNullOrEmpty(conf.FApiToken) || conf.FApiToken == "Fortnite_Token_Here" ||
                                 string.Equals(conf.FApiToken, "none", StringComparison.InvariantCultureIgnoreCase)));
    }

    public static Fortnite CreateInstance()
    {
        return new Fortnite();
    }

    /// <summary>
    ///     fortite mor lioke fartnite am i righe or am i righe
    /// </summary>
    /// <param name="apiKey">the key to use</param>
    public void MakeSureApiIsSet()
    {
        _api = new FortniteApiClient(Config.FApiToken);
    }

    [Command("fortstats")]
    [Description("Get the stats of a person using their username")]
    public async Task Stats(CommandContext ctx, [Description("The username of the person")][RemainingText] string name)
    {
        MakeSureApiIsSet();
        var lang = await LanguageService.FromCtxAsync(ctx);
        var statsV2V1 = await _api.V1.Stats.GetBrV2Async(x =>
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
        var newsV2 = await _api.V2.News.GetAsync();
        await ctx.RespondAsync(newsV2.Data.Br.Image.ToString());
    }

    [Command("fortcrnews")]
    [Description("Get a gif of the latest creative news")]
    public async Task Crnews(CommandContext ctx)
    {
        MakeSureApiIsSet();
        var newsV2 = await _api.V2.News.GetAsync();
        await ctx.RespondAsync(newsV2.Data.Creative.Image.ToString());
    }

    [Command("fortstwnews")]
    [Description("Get a gif of the latest save-the-world news")]
    public async Task Stwnews(CommandContext ctx)
    {
        MakeSureApiIsSet();
        var newsV2 = await _api.V2.News.GetAsync();
        await ctx.RespondAsync(newsV2.Data.Stw.Image.ToString());
    }

    [Command("fortitms")]
    [Description("Try to get the item shop items")]
    public async Task Itm(CommandContext ctx)
    {
        MakeSureApiIsSet();
        var shop = await _api.V2.Shop.GetBrCombinedAsync();
        var sb = new StringBuilder();
        foreach (var thing in shop.Data.Daily.Entries)
        {
            sb.Append(thing.DevName).Append(Environment.NewLine);
        }

        await ctx.RespondAsync(sb.ToString());
    }
}