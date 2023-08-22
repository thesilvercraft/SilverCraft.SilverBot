/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using ReverseMarkdown.Converters;
using SilverAudioPlayer.Shared;
using SilverBot.Shared.Attributes;
using SilverBot.Shared.Objects.Database.Classes;
using SilverCraft.SnowdPlayer;
using CategoryAttribute = SilverBot.Shared.Attributes.CategoryAttribute;
using HttpClient = System.Net.Http.HttpClient;

namespace SilverBotDS.Commands
{
    [RequireGuild]
    [Category("Weird Audio")]
    [RequireModuleGuildEnabled(EnabledModules.Audio, false)]
    public class WeirdAudio : BaseCommandModule
    {
        public ISnowdPlayerService PlayerService { private get; set; }
        [Command("wjoin")]
        public async Task Join(CommandContext ctx, DiscordChannel channel = null)
        {
            var weirdaudioplayer = await PlayerService.GetPlayer(ctx, channel);
        }
        public HttpClient client { private get; set; }
        private Dictionary<ulong, MixedStream> cDictionary = new();
        [Command("wplay")]
        public async Task PlayCommand(CommandContext ctx, string path)
        {
            var weirdaudioplayer = await PlayerService.GetPlayer(ctx);
            if (weirdaudioplayer.GetPlaybackState() == PlaybackState.Stopped)
            {
                if (weirdaudioplayer is AudioPlayer a)
                {
                    a.LoadMedia(path);
                    a.Play();
                }
            }
            else
            {
                await ctx.RespondAsync("the current version of snowdplayer does not have queues :(");
            }
        }
        [Command("wkill")]
        public Task KillCommand(CommandContext ctx)
        {
            if (PlayerService is SnowdPlayerService s)
            {
                s.Players.Remove(ctx.Guild.Id);
            }

            return Task.CompletedTask;
        }
        [Command("wresume")]
        public async Task UnPauseCommand(CommandContext ctx)
        {
            var weirdaudioplayer = await PlayerService.GetPlayer(ctx);
            if (weirdaudioplayer.GetPlaybackState() == PlaybackState.Paused)
            {
                weirdaudioplayer.Resume();
            }
            else
            {
                await ctx.RespondAsync("cannot resume something that isnt paused");
            }
        }
        [Command("wpause")]
        public async Task PauseCommand(CommandContext ctx)
        {
            var weirdaudioplayer = await PlayerService.GetPlayer(ctx);
            if (weirdaudioplayer.GetPlaybackState() == PlaybackState.Playing)
            {
                weirdaudioplayer.Pause();
            }
            else
            {
                await ctx.RespondAsync("cannot pause something that isnt playing");
            }
        }
    }
}
