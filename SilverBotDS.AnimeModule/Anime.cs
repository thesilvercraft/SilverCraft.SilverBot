using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SilverBotDS.Attributes;
using SilverBotDS.Utils;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    [Category("Anime")]
    public class Anime : BaseCommandModule
    {
        public HttpClient Client { private get; set; }
        private const string BaseURL = "https://anime-api.hisoka17.repl.co/";
        private string[] quotes = { "Playing a game by yourself is boring. You need a loser to crush so badly they can't recover.", "In our society, letting others find out that you're a nice person is a very risky move. It's extremely likely that someone would take advantage of that.", "Don't believe in the you that believes in me and don't believe in the me that believes in you. Believe in the you that believes in yourself!", "It is a poor knight indeed who slays one who defies him. The key is to show yourself to be superior that they will not defy you in the first place.", "There's something wrong with people who seek reality in fiction.", "There are things that cannot be taken back. But the world will keep on spinning whether you laugh or you cry. High school life will eventually end too.", "People don't realize they aren't able to choose whether or not to believe something. If they subconsciously believe something, therein lies the potential for a curse.", "Even if he hates me, being able to die with the knowledge that the one I love will never forget me… there is no greater happiness a woman could desire.", "There's something wrong with people who seek reality in fiction.", "People who don't work hard don't have the right to be envious of the people with talent. People fail because they don't understand the hard work necessary to be successful.", "You may be unfortunate, but that doesn't mean you have to suffer. You may not be blessed, but that doesn't mean you have to throw a fit over it. Even if bad things happen to you, just be strong!", "Those with talent who aren't aware of themselves cause pain for those who have none.", "If I'm getting in trouble no matter what I do, I may as well make a grand frontal assault and fight to my last breath.", "I have someone I like. I never had the confidence and I’ve doubted my feelings countless of times. But, when she stays by my side and smiles with me, I feel like I can do anything.", "Well, it's an anime, so you shouldn't think so hard about it.", "A faint clap of thunder, clouded skies; perhaps rain comes. If so, will you stay here with me?", "The fake is of far greater value. In its deliberate attempt to be real, it's more real than the real thing." };

        private async Task<string> GetAnimeUrl(string endpoint)
        {
            return JsonSerializer.Deserialize<Rootobject>(await (await Client.GetAsync(BaseURL + endpoint)).Content.ReadAsStringAsync()).Url;
        }

        private async Task SendImage(CommandContext ctx, string url)
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                             .WithEmbed(new DiscordEmbedBuilder().WithImageUrl(url))
                                             .SendAsync(ctx.Channel);
        }

        [Command("randomquote")]
        [Description("Gives you a random anime quote")]
        public async Task RandomQuote(CommandContext ctx) => await ctx.Message.RespondAsync(quotes[RandomGenerator.Next(0, quotes.Length)]);

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