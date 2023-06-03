/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using SilverBot.Shared;
using SilverBot.Shared.Attributes;
using SilverBot.Shared.Objects.Classes;
using SilverBot.Shared.Objects.Language;
using SilverBotDS.Converters;
#if NoAudio
#else
namespace SilverBotDS.Commands.Slash
{
    public class AudioSlash : ApplicationCommandModule
    {
        public NeutralAudio NeutralAudio { private get; set; }
        public LanguageService LanguageService { private get; set; }


        [SlashCommand("playnext", "Tell me to play a song next in the queue")]
        public async Task PlayNext(InteractionContext ctx,
            [Option("songname", "the song to add next in the queue")]
            string sg)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await ctx.DeferAsync();
            var song = await SongOrSongsConverter.ConvertToSong(ctx, sg, lang);
            await NeutralAudio.PlayNext(new SlashNeutralCommandContext(ctx), song);
        }

        [SlashCommand("play", "Tell me to play a song")]
        public async Task Play(InteractionContext ctx,
            [Option("songname", "the song to add next in the queue")]
            string sg)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await ctx.DeferAsync();
            var song = await SongOrSongsConverter.ConvertToSong(ctx, sg, lang);
            await NeutralAudio.Play(new SlashNeutralCommandContext(ctx), song);
        }

        [RequireDjSlash]
        [SlashCommand("volume", "Change the volume 1-100%")]
        public async Task Volume(InteractionContext ctx, [Option("volume", "1-100%")] long volume)
        {
            await ctx.DeferAsync();
            await NeutralAudio.Volume(new SlashNeutralCommandContext(ctx), (ushort)volume);
        }

        [RequireDjSlash]
        [SlashCommand("seek", "Seeks to the specified time")]
        public async Task Seek(InteractionContext ctx, [Option("time", "time to seek to")] TimeSpan? time)
        {
            if (time is not null)
            {
                await ctx.DeferAsync();
                await NeutralAudio.Seek(new SlashNeutralCommandContext(ctx), (TimeSpan)time);
            }
        }

        [RequireDjSlash]
        [SlashCommand("clearqueue", "Clears the queue")]
        public async Task ClearQueue(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            await NeutralAudio.ClearQueue(new SlashNeutralCommandContext(ctx));
        }

        [RequireDjSlash]
        [SlashCommand("shuffle", "Shuffles the queue")]
        public async Task Shuffle(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            await NeutralAudio.Shuffle(new SlashNeutralCommandContext(ctx));
        }


        [SlashCommand("export", "Export the queue")]
        public async Task ExportQueue(InteractionContext ctx,
            [Option("playlistName", "playlist name")]
            string? playlistName = null)
        {
            await ctx.DeferAsync();
            await NeutralAudio.ExportQueue(new SlashNeutralCommandContext(ctx), playlistName);
        }


        [SlashCommand("remove", "Remove song X from the queue. 0 < index > queue length")]
        public async Task Remove(InteractionContext ctx, [Option("songindex", "song index")] long songindex)
        {
            await ctx.DeferAsync();
            await NeutralAudio.Remove(new SlashNeutralCommandContext(ctx), (int)songindex);
        }

        [SlashCommand("loop", "Loops nothing/the queue/the currently playing song\"")]
        public async Task Loop(InteractionContext ctx,
            [Option("looptype", "What do you want looped?")]
            LoopSettings settings)
        {
            await ctx.DeferAsync();
            await NeutralAudio.Loop(new SlashNeutralCommandContext(ctx), settings);
        }


        [SlashCommand("pause", "pause the current song")]
        public async Task Pause(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            await NeutralAudio.Pause(new SlashNeutralCommandContext(ctx));
        }

        [SlashCommand("resume", "resume the current song")]
        public async Task Resume(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            await NeutralAudio.Resume(new SlashNeutralCommandContext(ctx));
        }

        [SlashCommand("join", "join the voice chat you're in")]
        public async Task Join(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            await NeutralAudio.Join(new SlashNeutralCommandContext(ctx));
        }


        [RequireDjSlash]
        [SlashCommand("forceskip", "skip a song. dj only command")]
        public async Task Skip(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            await NeutralAudio.Skip(new SlashNeutralCommandContext(ctx));
        }

        [SlashCommand("leave", "disconnect from voice channel")]
        [RequireDjSlash]
        public async Task Disconnect(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            await NeutralAudio.Disconnect(new SlashNeutralCommandContext(ctx));
        }

        [SlashCommand("skip", "vote skip")]
        public async Task VoteSkip(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            await NeutralAudio.Skip(new SlashNeutralCommandContext(ctx));
        }
    }
}
#endif