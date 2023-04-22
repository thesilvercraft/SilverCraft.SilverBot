/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Text.Json;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using CategoryAttribute = SilverBot.Shared.Attributes.CategoryAttribute;

namespace SilverBotDS.AnimeModule
{
    [Category("Anime")]
    public class AnimeSlash : ApplicationCommandModule
    {
        private const string BaseUrl = "https://anime-api.hisoka17.repl.co/";
        public HttpClient Client { private get; set; }

        private async Task<string> GetAnimeUrl(string endpoint)
        {
            return JsonSerializer
                .Deserialize<RootObject>(await (await Client.GetAsync(BaseUrl + endpoint)).Content.ReadAsStringAsync())!
                .Url;
        }

        private async Task SendImage(InteractionContext ctx, string url)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder().WithImageUrl(url)));
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
    }
}