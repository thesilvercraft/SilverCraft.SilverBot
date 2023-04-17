/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using SilverBot.Shared.Attributes;
using SilverBot.Shared.Objects.Classes;
using SilverBot.Shared.Objects.Database.Classes;
using CategoryAttribute = SilverBot.Shared.Attributes.CategoryAttribute;
using SilverBot.Shared;
namespace SilverBotDS.Commands;

[RequireGuild]
[Category("Audio")]
[RequireModuleGuildEnabled(EnabledModules.Audio, false)]
public class Audio : BaseCommandModule
{
    public NeutralAudio NeutralAudio { private get; set; }

    [Command("playnext")]
    [Description("Tell me to play a song next in the queue")]
    [Aliases("pn")]
    public Task PlayNext(CommandContext ctx, [RemainingText] SongORSongs song) => 
    NeutralAudio.PlayNext(new BasedCommandContext(ctx), song);
    

    [Command("play")]
    [Description("Tell me to play a song")]
    [Aliases("p")]
    public Task Play(CommandContext ctx) => NeutralAudio.Play(new BasedCommandContext(ctx));

    [Command("musiccontroller")]
    [Description("Controls music playback")]
    public Task MusicController(CommandContext ctx) => NeutralAudio.MusicController(new BasedCommandContext(ctx));
    
    [Command("play")]
    public Task Play(CommandContext ctx, [RemainingText] SongORSongs song) => NeutralAudio.Play(new BasedCommandContext(ctx), song);

    [Command("volume")]
    [Aliases("vol")]
    [RequireDj]
    [Description("Change the volume 1-100%")]
    public Task Volume(CommandContext ctx, ushort volume) => NeutralAudio.Volume(new BasedCommandContext(ctx), volume);

    [Command("seek")]
    [RequireDj]
    [Description("Seeks to the specified time")]
    public Task Seek(CommandContext ctx, TimeSpan time) => NeutralAudio.Seek(new BasedCommandContext(ctx), time);


    [Command("clearqueue")]
    [RequireDj]
    [Description("Clears the queue")]
    public Task ClearQueue(CommandContext ctx) => NeutralAudio.ClearQueue(new BasedCommandContext(ctx));

    [Command("shuffle")]
    [RequireDj]
    [Description("Shuffles the queue")]
    public Task Shuffle(CommandContext ctx) => NeutralAudio.Shuffle(new BasedCommandContext(ctx));

    [Command("export")]
    [Description("Export the queue")]
    public Task ExportQueue(CommandContext ctx, string? playlistName = null) => NeutralAudio.ExportQueue(new BasedCommandContext(ctx), playlistName);

    [Command("remove")]
    [Description("Remove song X from the queue. 0 < index > queue length")]
    public Task Remove(CommandContext ctx, int songindex) => NeutralAudio.Remove(new BasedCommandContext(ctx), songindex);

    [Command("queuehistory")]
    [Description("check what was playing")]
    [Aliases("qh", "prevplaying", "pq")]
    public Task QueueHistory(CommandContext ctx) => NeutralAudio.QueueHistory(new BasedCommandContext(ctx));

    [Command("queue")]
    [Description("check whats playing rn and whats next")]
    [Aliases("np", "nowplaying", "q")]
    public Task Queue(CommandContext ctx) => NeutralAudio.Queue(new BasedCommandContext(ctx));

    [Command("loop")]
    [Aliases("repeat")]
    [Description("Loops nothing/the queue/the currently playing song")]
    public Task Loop(CommandContext ctx, [Description("Has to be none, song or queue")] LoopSettings settings) => NeutralAudio.Loop(new BasedCommandContext(ctx), settings);


    [Command("pause")]
    [Description("pause the current song")]
    public Task Pause(CommandContext ctx) => NeutralAudio.Pause(new BasedCommandContext(ctx));

    [Command("ovh")]
    [Description("get the lyrics from ovh")]
    public Task Ovh(CommandContext ctx, string name, string artist) => NeutralAudio.Ovh(new BasedCommandContext(ctx), name, artist);

    [Command("songaliases")]
    [Description("Get the hardcoded aliases in silverbots code")]
    public Task Aliases(CommandContext ctx) => NeutralAudio.Aliases(new BasedCommandContext(ctx));

    [Command("resume")]
    [Description("resume the current song")]
    public Task Resume(CommandContext ctx) => NeutralAudio.Resume(new BasedCommandContext(ctx));

    [Command("join")]
    [Description("to join the voice chat you're in")]
    public Task Join(CommandContext ctx) => NeutralAudio.Join(new BasedCommandContext(ctx));


    [Command("forceskip")]
    [Description("skip a song. dj only command")]
    [RequireDj]
    [Aliases("fs")]
    public Task Skip(CommandContext ctx) => NeutralAudio.Skip(new BasedCommandContext(ctx));

    [Command("voteskip")]
    [Description("skip a song")]
    [Aliases("skip")]
    public Task VoteSkip(CommandContext ctx) => NeutralAudio.VoteSkip(new BasedCommandContext(ctx));

    [Command("forcedisconnect")]
    [Description("Tell me to leave your channel of the voice type, without checking if its in a vc")]
    [Aliases("fuckoffisntworking")]
    [RequireDj]
    public Task ForceDisconnect(CommandContext ctx) => NeutralAudio.VoteSkip(new BasedCommandContext(ctx));

    [Command("disconnect")]
    [Description("Tell me to leave your channel of the voice type")]
    [Aliases("fuckoff", "minecraftbedrockisbetter", "fockoff", "leave")]
    [RequireDj]
    public Task Disconnect(CommandContext ctx) => NeutralAudio.Disconnect(new BasedCommandContext(ctx));
}