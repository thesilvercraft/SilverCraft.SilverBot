using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

namespace SilverBotDS.AnimeSlash;

[Category("Anime")]
public class AnimeSlash : ApplicationCommandModule
{
    private const string BaseUrl = "https://anime-api.hisoka17.repl.co/";
    public HttpClient Client { private get; set; }

    private async Task<string> GetAnimeUrl(string endpoint)
    {
        return JsonSerializer
            .Deserialize<RootObject>(await (await Client.GetAsync(BaseUrl + endpoint)).Content.ReadAsStringAsync())!.Url;
    }

    private async Task SendImage(InteractionContext ctx, string url)
    {
        await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
            .WithEmbed(new DiscordEmbedBuilder().WithImageUrl(url))
            .SendAsync(ctx.Channel);
    }

    [SlashCommand("hug", "i have no idea what this means")]
    public async Task Hug(InteractionContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/hug"));
    }

    [SlashCommand("kiss", "i have no idea what this means")]
    public async Task Kiss(InteractionContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/kiss"));
    }

    [SlashCommand("slap", "i have no idea what this means")]
    public async Task Slap(InteractionContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/slap"));
    }

    [SlashCommand("wink", "i have no idea what this means")]
    public async Task Wink(InteractionContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/wink"));
    }

    [SlashCommand("pat", "i have no idea what this means")]
    public async Task Pat(InteractionContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/pat"));
    }

    [SlashCommand("kill", "the thing im gonna do to bub in fartnite")]
    public async Task Kill(InteractionContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/kill"));
    }

    [SlashCommand("cuddle", "i have no idea what this means")]
    public async Task Cuddle(InteractionContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/cuddle"));
    }

    [SlashCommand("punch", "i have no idea what this means")]
    public async Task Punch(InteractionContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/punch"));
    }

    public class RootObject
    {
        [JsonPropertyName("url")] public string Url { get; set; }
    }
}