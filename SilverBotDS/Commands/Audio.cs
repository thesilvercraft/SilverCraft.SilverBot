/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
#if NoAudio
#else
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using SilverBot.Shared.Attributes;
using SilverBot.Shared.Objects.Classes;
using SilverBot.Shared.Objects.Database.Classes;
using CategoryAttribute = SilverBot.Shared.Attributes.CategoryAttribute;
using SilverBot.Shared;

namespace SilverBotDS.Commands
{
    [RequireGuild]
    [Category("Audio")]
    [RequireModuleGuildEnabled(EnabledModules.Audio, false)]
    public class Audio : BaseCommandModule
    {
        public NeutralAudio NeutralAudio { private get; set; }

        [Command("playnext")]
        [Aliases("pn")]
        public Task PlayNext(CommandContext ctx, [RemainingText] SongORSongs song)
        {
            return NeutralAudio.PlayNext(new TextNeutralCommandContext(ctx), song);
        }


        [Command("play")]
        [Description("Tell me to play a song")]
        [Aliases("p")]
        public Task Play(CommandContext ctx)
        {
            return NeutralAudio.Play(new TextNeutralCommandContext(ctx));
        }

        [Command("musiccontroller")]
        [Description("Controls music playback")]
        public Task MusicController(CommandContext ctx)
        {
            return NeutralAudio.MusicController(new TextNeutralCommandContext(ctx));
        }

        [Command("play")]
        public Task Play(CommandContext ctx, [RemainingText] SongORSongs song)
        {
            return NeutralAudio.Play(new TextNeutralCommandContext(ctx), song);
        }

        [Command("volume")]
        [Aliases("vol")]
        [RequireDj]
        [Description("Change the volume 1-100%")]
        public Task Volume(CommandContext ctx, ushort volume)
        {
            return NeutralAudio.Volume(new TextNeutralCommandContext(ctx), volume);
        }

        [Command("seek")]
        [RequireDj]
        [Description("Seeks to the specified time")]
        public Task Seek(CommandContext ctx, TimeSpan time)
        {
            return NeutralAudio.Seek(new TextNeutralCommandContext(ctx), time);
        }


        [Command("clearqueue")]
        [RequireDj]
        [Description("Clears the queue")]
        public Task ClearQueue(CommandContext ctx)
        {
            return NeutralAudio.ClearQueue(new TextNeutralCommandContext(ctx));
        }

        [Command("shuffle")]
        [RequireDj]
        [Description("Shuffles the queue")]
        public Task Shuffle(CommandContext ctx)
        {
            return NeutralAudio.Shuffle(new TextNeutralCommandContext(ctx));
        }

        [Command("export")]
        [Description("Export the queue")]
        public Task ExportQueue(CommandContext ctx, string? playlistName = null)
        {
            return NeutralAudio.ExportQueue(new TextNeutralCommandContext(ctx), playlistName);
        }

        [Command("remove")]
        [Description("Remove song X from the queue. 0 < index > queue length")]
        public Task Remove(CommandContext ctx, int songindex)
        {
            return NeutralAudio.Remove(new TextNeutralCommandContext(ctx), songindex);
        }

        [Command("queuehistory")]
        [Description("check what was playing")]
        [Aliases("qh", "prevplaying", "pq")]
        public Task QueueHistory(CommandContext ctx)
        {
            return NeutralAudio.QueueHistory(new TextNeutralCommandContext(ctx));
        }

        [Command("queue")]
        [Description("check whats playing rn and whats next")]
        [Aliases("np", "nowplaying", "q")]
        public Task Queue(CommandContext ctx)
        {
            return NeutralAudio.Queue(new TextNeutralCommandContext(ctx));
        }

        [Command("loop")]
        [Aliases("repeat")]
        [Description("Loops nothing/the queue/the currently playing song")]
        public Task Loop(CommandContext ctx, [Description("Has to be none, song or queue")] LoopSettings settings)
        {
            return NeutralAudio.Loop(new TextNeutralCommandContext(ctx), settings);
        }


        [Command("pause")]
        [Description("pause the current song")]
        public Task Pause(CommandContext ctx)
        {
            return NeutralAudio.Pause(new TextNeutralCommandContext(ctx));
        }

        [Command("ovh")]
        [Description("get the lyrics from ovh")]
        public Task Ovh(CommandContext ctx, string name, string artist)
        {
            return NeutralAudio.Ovh(new TextNeutralCommandContext(ctx), name, artist);
        }

        [Command("songaliases")]
        [Description("Get the hardcoded aliases in silverbots code")]
        public Task Aliases(CommandContext ctx)
        {
            return NeutralAudio.Aliases(new TextNeutralCommandContext(ctx));
        }

        [Command("resume")]
        [Description("resume the current song")]
        public Task Resume(CommandContext ctx)
        {
            return NeutralAudio.Resume(new TextNeutralCommandContext(ctx));
        }

        [Command("join")]
        [Description("to join the voice chat you're in")]
        public Task Join(CommandContext ctx)
        {
            return NeutralAudio.Join(new TextNeutralCommandContext(ctx));
        }


        [Command("forceskip")]
        [Description("skip a song. dj only command")]
        [RequireDj]
        [Aliases("fs")]
        public Task Skip(CommandContext ctx)
        {
            return NeutralAudio.Skip(new TextNeutralCommandContext(ctx));
        }

        [Command("voteskip")]
        [Description("skip a song")]
        [Aliases("skip")]
        public Task VoteSkip(CommandContext ctx)
        {
            return NeutralAudio.VoteSkip(new TextNeutralCommandContext(ctx));
        }

        [Command("forcedisconnect")]
        [Description("Tell me to leave your channel of the voice type, without checking if its in a vc")]
        [Aliases("fuckoffisntworking")]
        [RequireDj]
        public Task ForceDisconnect(CommandContext ctx)
        {
            return NeutralAudio.VoteSkip(new TextNeutralCommandContext(ctx));
        }

        [Command("disconnect")]
        [Description("Tell me to leave your channel of the voice type")]
        [Aliases("fuckoff", "minecraftbedrockisbetter", "fockoff", "leave")]
        [RequireDj]
        public Task Disconnect(CommandContext ctx)
        {
            return NeutralAudio.Disconnect(new TextNeutralCommandContext(ctx));
        }
    }
}
#endif