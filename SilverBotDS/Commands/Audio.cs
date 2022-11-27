/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Humanizer;
using Lavalink4NET;
using Lavalink4NET.Artwork;
using Lavalink4NET.Lyrics;
using Lavalink4NET.Player;
using SilverBotDS.Converters;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SilverBotDS.Utils;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands;

[RequireGuild]
[Category("Audio")]
public class Audio : BaseCommandModule
{
    public LavalinkNode AudioService { private get; set; }
    public LyricsService LyricsService { private get; set; }

    public Config Config { private get; set; }

    public SpotifyClient SpotifyClient { private get; set; }
    public ArtworkService ArtworkService { private get; set; }

    private bool IsInVc(CommandContext ctx) => IsInVc(ctx, AudioService);

    private static bool IsInVc(CommandContext ctx, LavalinkNode lavalinkNode) => lavalinkNode.HasPlayer(ctx.Guild.Id) &&
               lavalinkNode.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id) is not null and not { State: PlayerState.NotConnected } and not { State: PlayerState.Destroyed };

    private static async Task SendNowPlayingMessage(CommandContext ctx, string title = "", string message = "",
        string imageurl = "", string url = "", Language? language = null)
    {
        language ??= await Language.GetLanguageFromCtxAsync(ctx);
        var embedBuilder = new DiscordEmbedBuilder()
            .WithFooter(language.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto))
            .WithColor(await ColorUtils.GetSingleAsync());
        var messageBuilder = new DiscordMessageBuilder();
        if (!string.IsNullOrEmpty(message))
        {
            embedBuilder.WithDescription(message);
        }

        if (!string.IsNullOrEmpty(title))
        {
            embedBuilder.WithTitle(title);
        }

        if (!string.IsNullOrEmpty(imageurl))
        {
            embedBuilder.WithThumbnail(imageurl);
        }

        if (!string.IsNullOrEmpty(url))
        {
            embedBuilder.WithUrl(url);
        }

        await messageBuilder
            .WithReply(ctx.Message.Id)
            .WithEmbed(embedBuilder.Build())
            .SendAsync(ctx.Channel);
    }

    public static async Task SendSimpleMessage(CommandContext ctx, string title = "", string message = "", string image = "",
        Language? language = null)
    {
        language ??= await Language.GetLanguageFromCtxAsync(ctx);
        var embedBuilder = new DiscordEmbedBuilder()
            .WithFooter(language.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto))
            .WithColor(await ColorUtils.GetSingleAsync());
        var messageBuilder = new DiscordMessageBuilder();
        if (!string.IsNullOrEmpty(message))
        {
            embedBuilder.WithDescription(message);
        }

        if (!string.IsNullOrEmpty(title))
        {
            embedBuilder.WithTitle(title);
        }
        if (!string.IsNullOrEmpty(image))
        {
            embedBuilder.WithThumbnail(image);
        }
        await messageBuilder
            .WithReply(ctx.Message.Id)
            .WithEmbed(embedBuilder.Build())
            .SendAsync(ctx.Channel);
    }

    private TimeSpan TimeTillSongPlays(QueuedLavalinkPlayer player, int song)
    {
        if (player.IsLooping)
        {
            return TimeSpan.MaxValue;
        }

        TimeSpan time;
        if (player.CurrentTrack.IsLiveStream)
        {
            time = TimeSpan.FromHours(20) - player.Position.Position;
        }
        else
        {
            time = player.CurrentTrack.Duration - player.Position.Position;
        }

        for (var i = 0; i < song - 1; i++)
        {
            time += player.Queue[i].Duration;
        }

        return time;
    }

    [Command("playnext")]
    [Description("Tell me to play a song next in the queue")]
    [Aliases("pn")]
    public async Task PlayNext(CommandContext ctx, [RemainingText] SongORSongs song)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
        if (song.Song != null)
        {
            await player.PlayTopAsync(song.Song);
            var dmb = new DiscordEmbedBuilder()
                    .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto))
                    .WithTitle(string.Format(lang.Enqueued, song.Song.Title + lang.SongByAuthor + song.Song.Author))
                    .WithUrl(song.Song.Uri.ToString())
                    .AddField(lang.TimeTillTrackPlays,
                        player.LoopSettings == LoopSettings.LoopingSong
                            ? lang.SongTimeLeftSongLooping
                            : TimeTillSongPlays(player, 1).Humanize(culture: lang.GetCultureInfo()));

            var art = await ArtworkService.ResolveAsync(song.Song);
            if (art != null)
            {
                dmb.WithThumbnail(art);
            }
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
          .WithEmbed(dmb).SendAsync(ctx.Channel);
        }

        if (song.GetRestOfSongs is not null)
        {
            ulong countofsongs = 0;
            await foreach (var t in song.GetRestOfSongs)
            {
                if (t is not null)
                {
                    await player.PlayTopAsync(t);
                    countofsongs++;
                }
            }

            if (countofsongs != 0)
            {
                await SendNowPlayingMessage(ctx, string.Format(lang.AddedXAmountOfSongs, countofsongs));
            }
        }
    }

    [Command("play")]
    [Description("Tell me to play a song")]
    [Aliases("p")]
    public async Task Play(CommandContext ctx)
    {
        if (ctx.Message.Attachments.Count == 1)
        {
            await Play(ctx,
                (SongORSongs)await ctx.CommandsNext.ConvertArgument(ctx.Message.Attachments[0].Url, ctx,
                    typeof(SongORSongs)));
        }
        else if (ctx.Message.ReferencedMessage?.Attachments.Count == 1)
        {
            await Play(ctx,
                (SongORSongs)await ctx.CommandsNext.ConvertArgument(ctx.Message.ReferencedMessage.Attachments[0].Url,
                    ctx, typeof(SongORSongs)));
        }
        else
        {
            await Resume(ctx);
        }
    }

    [Command("play")]
    public async Task Play(CommandContext ctx, [RemainingText] SongORSongs song)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
        if (song.Song != null)
        {
            var pos = await player.PlayAsync(song.Song, true, song.SongStartTime);
            if (pos == 0)
            {
                var artworkUri = await ArtworkService.ResolveAsync(song.Song);
                await SendNowPlayingMessage(ctx,
                    string.Format(lang.NowPlaying, song.Song.Title + lang.SongByAuthor + song.Song.Author),
                    url: song.Song.Uri.ToString(), imageurl: artworkUri?.ToString(), language: lang);
            }
            else
            {
                var emb = new DiscordEmbedBuilder()
                        .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto))
                        .WithTitle(string.Format(lang.Enqueued, song.Song.Title + lang.SongByAuthor + song.Song.Author))
                        .WithUrl(song.Song.Uri.ToString())
                        .AddField(lang.TimeTillTrackPlays,
                            player.LoopSettings == LoopSettings.LoopingSong
                                ? lang.SongTimeLeftSongLooping
                                : TimeTillSongPlays(player, pos).Humanize(culture: lang.GetCultureInfo()));
                var trkart = await ArtworkService.ResolveAsync(song.Song);
                if (trkart != null)
                {
                    emb.WithThumbnail(trkart);
                }
                await new DiscordMessageBuilder()
                    .WithReply(ctx.Message.Id)
                    .WithEmbed(emb)
                    .SendAsync(ctx.Channel);
            }
        }

        if (song.GetRestOfSongs is not null)
        {
            ulong countofsongs = 0;
            await foreach (var t in song.GetRestOfSongs)
            {
                if (t is not null)
                {
                    await player.PlayAsync(t, true);
                    countofsongs++;
                }
            }

            if (countofsongs != 0)
            {
                await SendNowPlayingMessage(ctx, string.Format(lang.AddedXAmountOfSongs, countofsongs));
            }
        }
    }

    [Command("volume")]
    [Aliases("vol")]
    [RequireDj]
    [Description("Change the volume 1-100%")]
    public async Task Volume(CommandContext ctx, ushort volume)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        if (!IsInVc(ctx))
        {
            await SendSimpleMessage(ctx, lang.NotConnected, language: lang);
            return;
        }

        var channel = ctx.Member?.VoiceState?.Channel;
        if (channel == null)
        {
            await SendSimpleMessage(ctx, lang.UserNotConnected, language: lang);
            return;
        }

        if (volume is < 0 or > 100)
        {
            await SendSimpleMessage(ctx, lang.VolumeNotCorrect, language: lang);
            return;
        }

        var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
        await player.SetVolumeAsync(volume / 100f, true);
    }

    [Command("seek")]
    [RequireDj]
    [Description("Seeks to the specified time")]
    public async Task Seek(CommandContext ctx, TimeSpan time)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await MakeSureBothAreInVC(ctx, lang);
        var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
        try
        {
            await player.SeekPositionAsync(time);
        }
        catch (NotSupportedException)
        {
            await SendSimpleMessage(ctx, lang.TrackCanNotBeSeeked, language: lang);
        }
    }
    public async Task MakeSureBotIsInVC(CommandContext ctx, Language lang)
    {
        if (!IsInVc(ctx))
        {
            await SendSimpleMessage(ctx, lang.NotConnected, language: lang);
            throw new BotNotInVCException("bot not in voice chat");
        }
    }
    public static async Task MakeSureUserIsInVC(CommandContext ctx, Language lang)
    {
        var channel = ctx.Member?.VoiceState?.Channel;
        if (channel == null)
        {
            await SendSimpleMessage(ctx, lang.UserNotConnected, language: lang);
            throw new UserNotInVCException("user not in voice chat");

        }
    }
    public async Task MakeSureBothAreInVC(CommandContext ctx, Language lang)
    {
        await MakeSureBotIsInVC(ctx, lang);
        await MakeSureUserIsInVC(ctx, lang);
    }
    public class BotNotInVCException : Exception
    {
        public BotNotInVCException(string? message) : base(message)
        {
        }

        public BotNotInVCException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

      
    }
    public class UserNotInVCException : Exception
    {
        public UserNotInVCException(string? message) : base(message)
        {
        }

        public UserNotInVCException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

    }
    public class PlayerIsNullException : Exception
    {
        public PlayerIsNullException(string? message) : base(message)
        {
        }

        public PlayerIsNullException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

       
    }
    public async Task MakeSurePlayerIsntNull(CommandContext ctx, Language lang, BetterVoteLavalinkPlayer? player)
    {
        if (player == null)
        {
            await SendSimpleMessage(ctx, lang.UnknownError, language: lang);
            throw new PlayerIsNullException("player returned null");
        }
    }
    [Command("clearqueue")]
    [RequireDj]
    [Description("Clears the queue")]
    public async Task ClearQueue(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await MakeSureBothAreInVC(ctx, lang);
        var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
        await MakeSurePlayerIsntNull(ctx, lang, player);
        if (player.Queue.Count == 0 && player.State != PlayerState.NotPlaying)
        {
            await SendSimpleMessage(ctx, lang.NothingInQueueToRemove, language: lang);
            return;
        }
        var cnt = player.Queue.Clear();
        await SendSimpleMessage(ctx,
            string.Format(lang.RemovedXSongOrSongs, cnt, cnt == 1 ? lang.RemovedSong : lang.RemovedSongs),
            language: lang);
    }

    [Command("shuffle")]
    [RequireDj]
    [Description("Shuffles the queue")]
    public async Task Shuffle(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await MakeSureBothAreInVC(ctx, lang);
        var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
        player.Queue.Shuffle();
        await SendSimpleMessage(ctx, lang.ShuffledSuccess, language: lang);
    }

    [Command("export")]
    [Description("Export the queue")]
    public async Task ExportQueue(CommandContext ctx, string? playlistName = null)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await MakeSureBothAreInVC(ctx, lang);
        var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
        var queue = player.Queue.Select(x => x.Identifier).ToList();
        queue.Insert(0, player.CurrentTrack.Identifier);
        await OwnerOnly.SendStringFileWithContent(ctx, "", JsonSerializer.Serialize(new SilverBotPlaylist
        {
            Identifiers = queue.ToArray(),
            CurrentSongTimems = player.Position.Position.TotalMilliseconds,
            PlaylistTitle = playlistName
        }), "queue.json");
    }

    [Command("remove")]
    [Description("Remove song X from the queue. 0 < index > queue length")]
    public async Task Remove(CommandContext ctx, int songindex)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await MakeSureBothAreInVC(ctx, lang);
        var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
        if (songindex < 0 || songindex > player.Queue.Count)
        {
            await SendSimpleMessage(ctx, lang.SongNotExist, language: lang);
            return;
        }

        var thingy = player.Queue[songindex - 1];
        player.Queue.RemoveAt(songindex - 1);
        await SendSimpleMessage(ctx, lang.RemovedFront + thingy.Title + lang.SongByAuthor + thingy.Author,
            language: lang);
    }

    [Command("queuehistory")]
    [Description("check what was playing")]
    [Aliases("qh", "prevplaying", "pq")]
    public async Task QueueHistory(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await MakeSureBothAreInVC(ctx, lang);
        var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
        if (player.QueueHistory.Count == 0)
        {
            await SendSimpleMessage(ctx, lang.NothingInQueueHistory, language: lang);
            return;
        }

        var pages = new List<Page>();
        for (var i = 0; i < player.QueueHistory.Count; i++)
        {
            pages.Add(new Page(embed: new DiscordEmbedBuilder().WithTitle(player.QueueHistory[i].Item1.Title)
                .WithUrl(player.QueueHistory[i].Item1.Uri.ToString()).WithColor(await ColorUtils.GetSingleAsync())
                .AddField(lang.TimeWhenTrackPlayed, Formatter.Timestamp(player.QueueHistory[i].Item2))
                .WithAuthor(string.Format(lang.PageNuget, i + 1, player.QueueHistory.Count))));
        }

        var interactivity = ctx.Client.GetInteractivity();
        await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages);
    }

    [Command("queue")]
    [Description("check whats playing rn and whats next")]
    [Aliases("np", "nowplaying", "q")]
    public async Task Queue(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await MakeSureBothAreInVC(ctx, lang);
        var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
        if (player.Queue.Count == 0 && player.State != PlayerState.Playing)
        {
            await SendSimpleMessage(ctx, lang.NothingInQueue, language: lang);
            return;
        }
        var e = new DiscordEmbedBuilder().WithTitle(player.CurrentTrack.Title)
                .WithUrl(player.CurrentTrack.Uri.ToString()).WithColor(await ColorUtils.GetSingleAsync())
                .WithAuthor(string.Format(lang.PageNuget, 1, player.Queue.Count + 1))
                .AddField(lang.SongLength, player.CurrentTrack.Duration.ToString())

                .AddField(lang.SongTimePosition, player.Position.Position.ToString()).AddField(lang.SongTimeLeft,
                    player.LoopSettings == LoopSettings.LoopingSong
                        ? lang.SongTimeLeftSongLoopingCurrent
                        : (player.CurrentTrack.Duration - player.Position.Position).Humanize(
                            culture: lang.GetCultureInfo()));
        var url = await ArtworkService.ResolveAsync(player.CurrentTrack);
        if (url != null)
        {
            e.WithThumbnail(url);
        }
        if (player.LoopTimes > 0)
        {
            e.AddField(lang.TimesTrackLooped, player.LoopTimes + (player.LoopTimes != 1 ? " times" : " time"));
        }
        var pages = new List<Page>
        {
           new(embed:e)
        };
        for (var i = 0; i < player.Queue.Count; i++)
        {
            var timetillsongplays = TimeTillSongPlays(player, i + 1);
            var em = new DiscordEmbedBuilder().WithTitle(player.Queue[i].Title)
                .WithUrl(player.Queue[i].Uri.ToString()).WithColor(await ColorUtils.GetSingleAsync())

                .AddField(lang.TimeTillTrackPlays,
                    player.LoopSettings == LoopSettings.LoopingSong
                        ? lang.SongTimeLeftSongLooping
                        : timetillsongplays.Humanize(culture: lang.GetCultureInfo()))
                .WithAuthor(string.Format(lang.PageNuget, i + 2, player.Queue.Count + 1));
            var artwrk = await ArtworkService.ResolveAsync(player.CurrentTrack);
            if (artwrk != null)
            {
                em.WithThumbnail(artwrk);
            }
            pages.Add(new Page(embed: em));
        }

        var interactivity = ctx.Client.GetInteractivity();
        await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages);
    }

    [Command("loop")]
    [Aliases("repeat")]
    [Description("Loops nothing/the queue/the currently playing song")]
    public async Task Loop(CommandContext ctx, [Description("Has to be none, song or queue")] LoopSettings settings)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await MakeSureBothAreInVC(ctx, lang);

        var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
        player.LoopSettings = settings;
        switch (settings)
        {
            case LoopSettings.NotLooping:
                await SendSimpleMessage(ctx, lang.NotLooping, language: lang);
                break;

            case LoopSettings.LoopingSong:
                await SendSimpleMessage(ctx, lang.LoopingSong, language: lang);
                break;

            case LoopSettings.LoopingQueue:
                await SendSimpleMessage(ctx, lang.LoopingQueue, language: lang);
                break;

            default:
                throw new InvalidOperationException("Unexpected value settings = " + settings);
        }
    }

    [Command("pause")]
    [Description("pause the current song")]
    public async Task Pause(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await MakeSureBothAreInVC(ctx, lang);
        var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
        if (player.State != PlayerState.Playing)
        {
            await SendSimpleMessage(ctx, lang.NotPlaying, language: lang);
            return;
        }

        await player.PauseAsync();
    }

    [Command("ovh")]
    [Description("get the lyrics from ovh")]
    public async Task Ovh(CommandContext ctx, string name, string artist)
    {
        var lyrics = await LyricsService.GetLyricsAsync(artist, name);
        if (string.IsNullOrEmpty(lyrics))
        {
            await SendSimpleMessage(ctx, "Lyrics not found");
            return;
        }

        await SendSimpleMessage(ctx, "Lyrics", $"```{lyrics}```");
    }

    [Command("songaliases")]
    [Description("Get the hardcoded aliases in silverbots code")]
    public async Task Aliases(CommandContext ctx)
    {
        StringBuilder bob = new();
        foreach (var song in Config.SongAliases)
        {
            bob.Append(Formatter.InlineCode(song.Key)).Append(" - ").AppendLine(Formatter.InlineCode(song.Value));
        }

        await SendSimpleMessage(ctx, message: bob.ToString());
    }

    [Command("resume")]
    [Description("resume the current song")]
    public async Task Resume(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await MakeSureBothAreInVC(ctx, lang);
        var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
        if (player.State != PlayerState.Paused)
        {
            await SendSimpleMessage(ctx, lang.NotPaused, language: lang);
            return;
        }
        await player.ResumeAsync();
    }

    [Command("join")]
    [Description("to join the voice chat you're in")]
    public async Task Join(CommandContext ctx)
    {
        await StaticJoin(ctx, AudioService);
    }

    public static async Task StaticJoin(CommandContext ctx, LavalinkNode audioService)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        if (IsInVc(ctx, audioService))
        {
            await SendSimpleMessage(ctx, lang.AlreadyConnected, language: lang);
            return;
        }
        await MakeSureUserIsInVC(ctx, lang);
        await audioService.JoinAsync<BetterVoteLavalinkPlayer>(ctx.Guild.Id, (ctx.Member?.VoiceState?.Channel).Id,
            true);
        await SendSimpleMessage(ctx, string.Format(lang.Joined, (ctx.Member?.VoiceState?.Channel).Name),
            language: lang);
    }

    [Command("forceskip")]
    [Description("skip a song. dj only command")]
    [RequireDj]
    [Aliases("fs")]
    public async Task Skip(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await MakeSureBothAreInVC(ctx, lang);

        var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);

        if (player.State != PlayerState.Playing)
        {
            await SendSimpleMessage(ctx, lang.NotPlaying, language: lang);
            return;
        }

        var trackbefore = player.CurrentTrack;
        await player.SkipAsync();
        var trackafter = player.CurrentTrack;
        await SendSimpleMessage(ctx,
            string.Format(lang.SkippedNP, trackbefore.Title, trackafter == null ? lang.QueueNothing : trackafter?.Title),
            language: lang, image: trackafter == null ? null : (await ArtworkService.ResolveAsync(trackafter))?.ToString());
    }

    [Command("voteskip")]
    [Description("skip a song")]
    [Aliases("skip")]
    public async Task VoteSkip(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await MakeSureBothAreInVC(ctx, lang);
        var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
        if (player.State != PlayerState.Playing)
        {
            await SendSimpleMessage(ctx, lang.NotPlaying, language: lang);
            return;
        }

        var trackbefore = player.CurrentTrack;
        if (await new RequireDjAttribute().ExecuteCheckAsync(ctx, false))
        {
            await SendSimpleMessage(ctx, lang.CanForceSkip, language: lang);
        }

        var thing = await player.VoteAsync(ctx.Member.Id);
        if (thing.WasSkipped)
        {
            await SendSimpleMessage(ctx,
                string.Format(lang.SkippedNP, trackbefore.Title,
                    player.CurrentTrack == null ? lang.QueueNothing : player.CurrentTrack.Title), image: (await ArtworkService.ResolveAsync(player.CurrentTrack))?.ToString(), language: lang);
        }
        else if (thing.WasAdded)
        {
            await SendSimpleMessage(ctx, lang.Voted, language: lang);
        }
        else
        {
            await SendSimpleMessage(ctx, lang.AlreadyVoted, language: lang);
        }
    }

    [Command("forcedisconnect")]
    [Description("Tell me to leave your channel of the voice type, without checking if its in a vc")]
    [Aliases("fuckoffisntworking")]
    [RequireDj]
    public async Task ForceDisconnect(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await MakeSureUserIsInVC(ctx, lang);

        var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
        await player.DisconnectAsync();
        await player.DestroyAsync();
        player.Dispose();
        
    }

    [Command("disconnect")]
    [Description("Tell me to leave your channel of the voice type")]
    [Aliases("fuckoff", "minecraftbedrockisbetter", "fockoff", "leave")]
    [RequireDj]
    public async Task Disconnect(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await MakeSureBothAreInVC(ctx, lang);
        var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
        await player.DisconnectAsync();
        await SendSimpleMessage(ctx, string.Format(lang.Left, ctx.Member?.VoiceState?.Channel.Name), language: lang);
    }
}