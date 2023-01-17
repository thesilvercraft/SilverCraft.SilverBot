/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Humanizer;
using Lavalink4NET;
using Lavalink4NET.Artwork;
using Lavalink4NET.Decoding;
using Lavalink4NET.Lyrics;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using Microsoft.Extensions.DependencyInjection;
using SilverBot.Shared;
using SilverBot.Shared.Attributes;
using SilverBot.Shared.Exceptions;
using SilverBot.Shared.Objects;
using SilverBot.Shared.Objects.Classes;
using SilverBot.Shared.Objects.Language;
using SilverBot.Shared.Utils;
using SilverBotDS.Converters;

namespace SilverBotDS.Commands.Slash
{
    //TODO merge with Audio.cs
    public class AudioSlash : ApplicationCommandModule
    {
        public LavalinkNode AudioService { private get; set; }
        public LyricsService LyricsService { private get; set; }

        public Config Config { private get; set; }
        public HttpClient HttpClient { private get; set; }
        public ArtworkService ArtworkService { private get; set; }
        public LanguageService LanguageService { private get; set; }

        private bool IsInVc(BaseContext ctx) => IsInVc(ctx, AudioService);

        private static bool IsInVc(BaseContext ctx, LavalinkNode lavalinkNode) => lavalinkNode.HasPlayer(ctx.Guild.Id) &&
                   lavalinkNode.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id) is not null and not { State: PlayerState.NotConnected } and not { State: PlayerState.Destroyed };
       
       

    
        private TimeSpan TimeTillSongPlays(QueuedLavalinkPlayer player, int song)
        {
            if (player.LoopMode == PlayerLoopMode.Track)
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

       public async Task<SongORSongs?> ConvertToSong(InteractionContext ctx, string value, Language? language=null)
           {
            //TODO MERGE WITH SongOrSongsConverter.cs
            var conf = Config;
            language ??= await ctx.GetLanguageAsync(language);
            if (!IsInVc(ctx, AudioService))
            {
                await MakeSureUserIsInVC(ctx, language);
                await StaticJoin(ctx, AudioService);
            }
            if (conf.SongAliases.ContainsKey(value))
            {
                value = conf.SongAliases[value];
            }
            if (value.EndsWith(".json"))
            {
                var client = HttpClient;
                if (client is not null)
                {
                    var tracks =
                        JsonSerializer.Deserialize<SilverBotPlaylist>(await(await client.GetAsync(value)).Content
                            .ReadAsStringAsync());
                    if (!string.IsNullOrEmpty(tracks.PlaylistTitle))
                    {
                        await ctx.SendMessageAsync(string.Format(language.LoadedSilverBotPlaylistWithTitle, tracks.PlaylistTitle), language: language);
                    }

                    return new SongORSongs(TrackDecoder.DecodeTrack(tracks.Identifiers[0]), null,
                        tracks.Identifiers.Skip(1).Select(x => TrackDecoder.DecodeTrack(x)).ToAsyncEnumerable(),
                        TimeSpan.FromMilliseconds(tracks.CurrentSongTimems));
                }
            }
            var track = await AudioService.GetTracksAsync(value);
            if (track?.Any() == true)
            {
                return (new SongORSongs(track.First(), null, track.Skip(1).ToAsyncEnumerable()));
            }

            track = new[] { await AudioService.GetTrackAsync(value, SearchMode.YouTube) };

            if (track?.Any() == true)
            {
                return (new SongORSongs(track.First(), null, track.Skip(1).ToAsyncEnumerable()));
            }

            track = new[] { await AudioService.GetTrackAsync(value, SearchMode.SoundCloud) };
            if (track?.Any() == true)
            {
                return (new SongORSongs(track.First(), null, track.Skip(1).ToAsyncEnumerable()));
            }

            await ctx.SendMessageAsync( string.Format(language.NoResults, value), language: language);
            return null;

           }
        [SlashCommand("playnext", "Tell me to play a song next in the queue")]

        public async Task PlayNext(InteractionContext ctx, [Option("songname","the song to add next in the queue")] string sg)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await ctx.DeferAsync();
            var song = await ConvertToSong(ctx, sg, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (song.Song != null)
            {
                await player.PlayTopAsync(song.Song);
                var dmb = new DiscordEmbedBuilder()
                        .AddRequestedByFooter(ctx,lang)
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
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(dmb));
                
            }

            if (song.GetRestOfSongs is not null)
            {
                ulong countofsongs = 0;
                await foreach (var t in song.GetRestOfSongs)
                {
                    await player.PlayTopAsync(t);
                    countofsongs++;
                }

                if (countofsongs != 0)
                {
                    await ctx.SendMessageAsync( string.Format(lang.AddedXAmountOfSongs, countofsongs));
                }
            }
        }

      
      
 //TODO: PLAY ATTACHMENTS AND ATTACHMENTS OF REPLIED MESSAGES
        [SlashCommand("play", "Tell me to play a song")]

        public async Task Play(InteractionContext ctx, [Option("songname", "the song to add next in the queue")] string sg)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await ctx.DeferAsync();
            var song = await ConvertToSong(ctx, sg, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (song.Song != null)
            {
                var pos = await player.PlayAsync(song.Song, true, song.SongStartTime);
                if (pos == 0)
                {
                    var artworkUri = await ArtworkService.ResolveAsync(song.Song);
                    await ctx.SendMessageAsync(
                        string.Format(lang.NowPlaying, song.Song.Title + lang.SongByAuthor + song.Song.Author),
                        url: song.Song.Uri.ToString(), imageUrl: artworkUri?.ToString(), language: lang);
                }
                else
                {
                    var emb = new DiscordEmbedBuilder()
                            .AddRequestedByFooter(ctx,lang)
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
                    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(emb));
                }
            }

            if (song.GetRestOfSongs is not null)
            {
                ulong countofsongs = 0;
                await foreach (var t in song.GetRestOfSongs)
                {
                    await player.PlayAsync(t, true);
                    countofsongs++;
                }

                if (countofsongs != 0)
                {
                    await ctx.SendMessageAsync(string.Format(lang.AddedXAmountOfSongs, countofsongs));
                }
            }
        }

        [RequireDjSlash]
        [SlashCommand("volume", "Change the volume 1-100%")]

        public async Task Volume(InteractionContext ctx, [Option("volume", "1-100%")] long volume)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            if (!IsInVc(ctx))
            {
                await ctx.SendMessageAsync( lang.NotConnected, language: lang);
                return;
            }

            var channel = ctx.Member?.VoiceState?.Channel;
            if (channel == null)
            {
               await ctx.SendMessageAsync( lang.UserNotConnected, language: lang);
                return;
            }

            if (volume is < 0 or > 100)
            {
               await ctx.SendMessageAsync( lang.VolumeNotCorrect, language: lang);
                return;
            }

            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            await player?.SetVolumeAsync(volume / 100f, true)!;
        }

        [RequireDjSlash]
        [SlashCommand("seek", "Seeks to the specified time")]

        public async Task Seek(InteractionContext ctx, [Option("time", "time to seek to")]  TimeSpan? time)
        {
            await ctx.DeferAsync();
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            try
            {
                await player.SeekPositionAsync( (TimeSpan)time);
            }
            catch (NotSupportedException)
            {
               await ctx.SendMessageAsync( lang.TrackCanNotBeSeeked, language: lang);
            }
        }
        public async Task MakeSureBotIsInVC(InteractionContext ctx, Language lang)
        {
            if (!IsInVc(ctx))
            {
               await ctx.SendMessageAsync(lang.NotConnected, language: lang);
                throw new BotNotInVCException("bot not in voice chat");
            }
        }
        public static async Task MakeSureUserIsInVC(InteractionContext ctx, Language lang)
        {
            if (ctx.Member?.VoiceState?.Channel == null)
            {
               await ctx.SendMessageAsync( lang.UserNotConnected, language: lang);
                throw new UserNotInVCException("user not in voice chat");
            }
        }
        public async Task MakeSureBothAreInVC(InteractionContext ctx, Language lang)
        {
            await MakeSureBotIsInVC(ctx, lang);
            await MakeSureUserIsInVC(ctx, lang);
        }
      
        public async Task MakeSurePlayerIsntNull(InteractionContext ctx, Language lang, BetterVoteLavalinkPlayer? player)
        {
            if (player == null)
            {
               await ctx.SendMessageAsync( lang.UnknownError, language: lang);
                throw new PlayerIsNullException("player returned null");
            }
        }
        [RequireDjSlash]
        [SlashCommand("clearqueue", "Clears the queue")]
        public async Task ClearQueue(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            await MakeSurePlayerIsntNull(ctx, lang, player);
            if (player.Queue.Count == 0 && player.State != PlayerState.NotPlaying)
            {
                await ctx.SendMessageAsync( lang.NothingInQueueToRemove, language: lang);
                return;
            }
            var cnt = player.Queue.Clear();
           await ctx.SendMessageAsync(
                string.Format(lang.RemovedXSongOrSongs, cnt, cnt == 1 ? lang.RemovedSong : lang.RemovedSongs),
                language: lang);
        }

        [RequireDjSlash]
        [SlashCommand("shuffle", "Shuffles the queue")]

        public async Task Shuffle(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            player.Queue.Shuffle();
          await ctx.SendMessageAsync( lang.ShuffledSuccess, language: lang);
        }

   
        [SlashCommand("export", "Export the queue")]

        public async Task ExportQueue(InteractionContext ctx, [Option("playlistName", "playlist name")]  string? playlistName = null)
        {
            await ctx.DeferAsync();
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            var queue = player.Queue.Select(x => x.Identifier).ToList();
            queue.Insert(0, player.CurrentTrack.Identifier);
            using var os = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new SilverBotPlaylist
            {
                Identifiers = queue.ToArray(),
                CurrentSongTimems = player.Position.Position.TotalMilliseconds,
                PlaylistTitle = playlistName
            })));
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddFile("queue.json", os));
        }

   
        [SlashCommand("remove", "Remove song X from the queue. 0 < index > queue length")]

        public async Task Remove(InteractionContext ctx, [Option("songindex", "song index")]  long songindex)
        {
            await ctx.DeferAsync();
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (songindex < 0 || songindex > player.Queue.Count)
            {
               await ctx.SendMessageAsync( lang.SongNotExist, language: lang);
                return;
            }

            var thingy = player.Queue[(int)songindex - 1];
            player.Queue.RemoveAt((int)songindex - 1);
          await ctx.SendMessageAsync( lang.RemovedFront + thingy.Title + lang.SongByAuthor + thingy.Author,
                language: lang);
        }
        [SlashCommand("loop", "Loops nothing/the queue/the currently playing song\"")]

        public async Task Loop(InteractionContext ctx, [Option("looptype", "What do you want looped?")] LoopSettings settings)
        {
            await ctx.DeferAsync();
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);

            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            player.LoopSettings = settings;
            switch (settings)
            {
                case LoopSettings.NotLooping:
                    await ctx.SendMessageAsync( lang.NotLooping, language: lang);
                    break;

                case LoopSettings.LoopingSong:
                    await ctx.SendMessageAsync( lang.LoopingSong, language: lang);
                    break;

                case LoopSettings.LoopingQueue:
                    await ctx.SendMessageAsync( lang.LoopingQueue, language: lang);
                    break;

                default:
                    throw new InvalidOperationException("Unexpected value settings = " + settings);
            }
        }
            

        [SlashCommand("pause", "pause the current song")]
        public async Task Pause(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (player.State != PlayerState.Playing)
            {
               await ctx.SendMessageAsync( lang.NotPlaying, language: lang);
                return;
            }

            await player.PauseAsync();
        }

        [SlashCommand("resume", "resume the current song")]

        public async Task Resume(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (player.State != PlayerState.Paused)
            {
                await ctx.SendMessageAsync( lang.NotPaused, language: lang);
                return;
            }
            await player.ResumeAsync();
        }

        [SlashCommand("join", "join the voice chat you're in")]

        public async Task Join(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            await StaticJoin(ctx, AudioService);
        }

        public static async Task StaticJoin(InteractionContext ctx, LavalinkNode audioService)
        {
            var lang = await ctx.Services.GetService<LanguageService>().FromCtxAsync(ctx);
            if (IsInVc(ctx, audioService))
            {
               await ctx.SendMessageAsync( lang.AlreadyConnected, language: lang);
                return;
            }
            await MakeSureUserIsInVC(ctx, lang);
            await audioService.JoinAsync<BetterVoteLavalinkPlayer>(ctx.Guild.Id, (ctx.Member?.VoiceState?.Channel).Id,
                true);
            await ctx.SendMessageAsync( string.Format(lang.Joined, (ctx.Member?.VoiceState?.Channel).Name),
                language: lang);
        }

        [RequireDjSlash]
        [SlashCommand("forceskip", "skip a song. dj only command")]

        public async Task Skip(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (player.State != PlayerState.Playing)
            {
                await ctx.SendMessageAsync( lang.NotPlaying, language: lang);
                return;
            }
            var trackbefore = player.CurrentTrack;
            await player.SkipAsync();
            var trackafter = player.CurrentTrack;
           await ctx.SendMessageAsync(
                string.Format(lang.SkippedNP, trackbefore.Title, trackafter == null ? lang.QueueNothing : trackafter?.Title),
                language: lang, imageUrl: trackafter == null ? null : (await ArtworkService.ResolveAsync(trackafter))?.ToString());
        }
        [SlashCommand("leave", "disconnect from voice channel")]
        [RequireDjSlash]
        public async Task Disconnect(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            await player.DisconnectAsync();
            await ctx.SendMessageAsync( string.Format(lang.Left, ctx.Member?.VoiceState?.Channel.Name), language: lang);
        }
        [SlashCommand("skip", "vote skip")]

        public async Task VoteSkip(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (player.State != PlayerState.Playing)
            {
                await ctx.SendMessageAsync( lang.NotPlaying, language: lang);
                return;
            }
            var trackbefore = player.CurrentTrack;
            if (await new RequireDjSlashAttribute().ExecuteChecksAsync(ctx))
            {
              await ctx.SendMessageAsync( lang.CanForceSkip, language: lang);
            }
            var thing = await player.VoteAsync(ctx.Member.Id);
            if (thing.WasSkipped)
            {
                await ctx.SendMessageAsync( 
                    string.Format(lang.SkippedNP, trackbefore.Title,
                        player.CurrentTrack == null ? lang.QueueNothing : player.CurrentTrack.Title), imageUrl: (await ArtworkService.ResolveAsync(player.CurrentTrack))?.ToString(), language: lang);
            }
            else if (thing.WasAdded)
            {
              await ctx.SendMessageAsync(  lang.Voted, language: lang);
            }
            else
            {
               await ctx.SendMessageAsync( lang.AlreadyVoted, language: lang);
            }
        }
        
    }
}
