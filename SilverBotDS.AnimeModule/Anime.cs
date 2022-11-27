/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

namespace SilverBotDS.Anime;

[Category("Anime")]
public class Anime : BaseCommandModule
{
    private const string BaseUrl = "https://anime-api.hisoka17.repl.co/";
    public HttpClient Client { private get; set; }

    private async Task<string> GetAnimeUrl(string endpoint)
    {
        return JsonSerializer
            .Deserialize<RootObject>(await (await Client.GetAsync(BaseUrl + endpoint)).Content.ReadAsStringAsync())!.Url;
    }

    private async Task SendImage(CommandContext ctx, string url)
    {
        await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
            .WithEmbed(new DiscordEmbedBuilder().WithImageUrl(url))
            .SendAsync(ctx.Channel);
    }

    [Command("hug")]
    [Description("i have no idea what this means")]
    public async Task Hug(CommandContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/hug"));
    }

    [Command("kiss")]
    [Description("i have no idea what this means")]
    public async Task Kiss(CommandContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/kiss"));
    }

    [Command("slap")]
    [Description("i have no idea what this means")]
    public async Task Slap(CommandContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/slap"));
    }

    [Command("wink")]
    [Description("i have no idea what this means")]
    public async Task Wink(CommandContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/wink"));
    }

    [Command("pat")]
    [Description("i have no idea what this means")]
    public async Task Pat(CommandContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/pat"));
    }

    [Command("kill")]
    [Description("the thing im gonna do to bub in fartnite")]
    public async Task Kill(CommandContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/kill"));
    }

    [Command("cuddle")]
    [Description("i have no idea what this means")]
    public async Task Cuddle(CommandContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/cuddle"));
    }

    [Command("punch")]
    [Description("i have no idea what this means")]
    public async Task Punch(CommandContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/punch"));
    }


}
public class RootObject
{
    [JsonPropertyName("url")] public string Url { get; set; }
}