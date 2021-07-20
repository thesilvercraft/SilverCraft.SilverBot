using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SilverBotDS.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    [Category("Anime")]
    public class Anime : BaseCommandModule
    {
        public HttpClient client { private get; set; }
        private const string BaseURL = "https://anime-api.hisoka17.repl.co/";
        private async Task<string> GetAnimeUrl(string endpoint) => JsonSerializer.Deserialize<Rootobject>(await (await client.GetAsync(BaseURL + endpoint)).Content.ReadAsStringAsync()).Url;
        private async Task SendImage(CommandContext ctx,string url)
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                             .WithEmbed(new DiscordEmbedBuilder().WithImageUrl(url))
                                             .SendAsync(ctx.Channel);
        }
        [Command("hug")]
        [Description("i have no idea what this means")]
        public async Task Hug(CommandContext ctx) => await SendImage(ctx, await GetAnimeUrl("img/hug"));
        [Command("kiss")]
        [Description("i have no idea what this means")]
        public async Task Kiss(CommandContext ctx) => await SendImage(ctx, await GetAnimeUrl("img/kiss"));
        [Command("slap")]
        [Description("i have no idea what this means")]
        public async Task Slap(CommandContext ctx) => await SendImage(ctx, await GetAnimeUrl("img/slap"));
        [Command("wink")]
        [Description("i have no idea what this means")]
        public async Task Wink(CommandContext ctx) => await SendImage(ctx, await GetAnimeUrl("img/wink"));
        [Command("pat")]
        [Description("i have no idea what this means")]
        public async Task Pat(CommandContext ctx) => await SendImage(ctx, await GetAnimeUrl("img/pat"));
        [Command("kill")]
        [Description("the thing im gonna do to bub in fartnite")]
        public async Task Kill(CommandContext ctx) => await SendImage(ctx, await GetAnimeUrl("img/kill"));
        [Command("cuddle")]
        [Description("i have no idea what this means")]
        public async Task Cuddle(CommandContext ctx) => await SendImage(ctx, await GetAnimeUrl("img/cuddle"));
        [Command("punch")]
        [Description("i have no idea what this means")]
        public async Task Punch(CommandContext ctx) => await SendImage(ctx, await GetAnimeUrl("img/punch"));
        /*[Command("waifu")]
        [Description("i have no idea what this means")]
        public async Task Waifu(CommandContext ctx) => await SendImage(ctx, await GetAnimeUrl("img/waifu"));*/
        public class Rootobject
        {
            [JsonPropertyName("url")]
            public string Url { get; set; }
        }
    }
}
