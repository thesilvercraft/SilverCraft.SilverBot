using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using Lavalink4NET;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using SilverBotDS.Commands;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lavalink4NET.Decoding;

namespace SilverBotDS.Converters
{
    public class SongOrSongsConverter : IArgumentConverter<SongORSongs>
    {
        private static readonly Regex PlaylistRegex =
              new(
                  @"^(https:\/\/open\.spotify\.com\/user\/spotify\/playlist\/|https:\/\/open\.spotify\.com\/playlist\/|spotify:user:spotify:playlist:|spotify:playlist:)([a-zA-Z0-9]+)(.*)$", RegexOptions.Compiled);

        private static readonly Regex TrackRegex =
            new(
                @"^(https:\/\/open\.spotify\.com\/user\/spotify\/track\/|https:\/\/open\.spotify\.com\/track\/|spotify:user:spotify:track:|spotify:track:)([a-zA-Z0-9]+)(.*)$", RegexOptions.Compiled);

        private static readonly Regex AlbumRegex = new(@"^(https:\/\/open\.spotify\.com\/album\/|spotify:album:)([a-zA-Z0-9]+)(.*)$", RegexOptions.Compiled);

        private readonly IReadOnlyDictionary<string, string> aliases = new Dictionary<string, string>
        {
           {"we will fock you", "https://youtu.be/lLN3caSQI1w"},
           {"special for bub", "https://www.youtube.com/watch?v=y1TJBgpGrd8"},
           {"velimir's favorite","https://www.youtube.com/watch?v=mdqU6Erw3kk"},
           {"cmpc music","https://www.youtube.com/playlist?list=PLgeUxNS5wZ89J7tzjHCMxfArAr4o-eaux"},
           {"shut the fock up","https://cdn.discordapp.com/attachments/789617572608868404/840360529294000148/PINK_GUY_-_STFU-OLpeX4RRo28.mp3"},
           {"fock you","https://cdn.discordapp.com/attachments/789617572608868404/840361885656285204/CeeLo_Green_-_FUCK_YOU_Official_Video-pc0mxOXbWIU.mp3"},
           {"meme playlist","https://www.youtube.com/playlist?list=PLiiTWcm0RsKj8toM1CoxDbjDZftLYDFo1"},
           {"evening mix","https://www.youtube.com/playlist?list=PLiiTWcm0RsKhoGLQA84m1ag9QV5AH1usW"},
           {"kae chill beats","https://www.youtube.com/playlist?list=PLB7khNwMQ_sPGMqBYhS3_3u57U33JK2jB"}
        };

        private bool IsSpotifyString(string url) => TrackRegex.IsMatch(url) || AlbumRegex.IsMatch(url) || PlaylistRegex.IsMatch(url);

        private async IAsyncEnumerable<LavalinkTrack> GetTracksUsingAlbum(FullAlbum album, LavalinkNode AudioService, uint skipsongs = 1)
        {
            foreach (var song in album.Tracks.Items)
            {
                if (skipsongs != 0)
                {
                    skipsongs--;
                    continue;
                }
                var e = await AudioService.GetTrackAsync($"{song.Name} {song.Artists[0].Name}", SearchMode.YouTube);
                if (e is not null)
                {
                    yield return e;
                }
            }
        }

        private async IAsyncEnumerable<LavalinkTrack> GetTracksUsingPlaylist(FullPlaylist playlist, LavalinkNode AudioService, uint skipsongs = 1)
        {
            foreach (var song in playlist.Tracks.Items)
            {
                if (song.Track is FullTrack track)
                {
                    if (skipsongs != 0)
                    {
                        skipsongs--;
                        continue;
                    }
                    var e = await AudioService.GetTrackAsync($"{track.Name} {track.Artists[0].Name}", SearchMode.YouTube);
                    if (e is not null)
                    {
                        yield return e;
                    }
                }
            }
        }

        private bool IsInVc(CommandContext ctx, LavalinkNode AudioService)
        {
            return AudioService.HasPlayer(ctx.Guild.Id) && AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id) is not null && (AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id).State != PlayerState.NotConnected
                                                                                                                                                                  || AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id).State != PlayerState.Destroyed);
        }

        public async Task<Optional<SongORSongs>> ConvertAsync(string value, CommandContext ctx)
        {
            SpotifyClient spotifyClient = (SpotifyClient)ctx.CommandsNext.Services.GetService(typeof(SpotifyClient));
            LavalinkNode AudioService = (LavalinkNode)ctx.CommandsNext.Services.GetService(typeof(LavalinkNode));
            var lang = await Language.GetLanguageFromCtxAsync(ctx);

            if (!IsInVc(ctx, AudioService))
            {
                if (ctx.Member?.VoiceState?.Channel == null)
                {
                    await Audio.SendSimpleMessage(ctx, lang.UserNotConnected, language: lang);
                    return new();
                }
                else
                {
                    await Audio.StaticJoin(ctx, AudioService);
                    await ctx.TriggerTypingAsync();
                }
            }
            if (aliases.ContainsKey(value))
            {
                value = aliases[value];
            }
            await ctx.TriggerTypingAsync();
            if (value.EndsWith(".json"))
            {
                var client = (HttpClient)ctx.CommandsNext.Services.GetService(typeof(HttpClient));
                if (client is not null)
                {
                    var tracks = JsonSerializer.Deserialize<SerialisableQueue>(await (await client.GetAsync(value)).Content.ReadAsStringAsync());
                    return new(new(TrackDecoder.DecodeTrack(tracks.Identifiers.First()), null, tracks.Identifiers.Skip(1).Select(x => TrackDecoder.DecodeTrack(x)).ToAsyncEnumerable(), TimeSpan.FromMilliseconds(tracks.CurrentSongTimems)));
                }
            }
            if (spotifyClient is not null && IsSpotifyString(value))
            {
                var m = TrackRegex.Match(value);

                if (m.Success)
                {
                    var song = await spotifyClient.Tracks.Get(m.Groups[2].Value);
                    return new(new SongORSongs(await AudioService.GetTrackAsync($"{song.Name} {song.Artists[0].Name}", SearchMode.YouTube), null, null));
                }

                m = AlbumRegex.Match(value);
                if (m.Success)
                {
                    var album = await spotifyClient.Albums.Get(m.Groups[2].Value);
                    return new(new SongORSongs(await AudioService.GetTrackAsync($"{album.Tracks.Items[0].Name} {album.Tracks.Items[0].Artists[0].Name}", SearchMode.YouTube), album.Name, GetTracksUsingAlbum(album, AudioService)));
                }
                m = PlaylistRegex.Match(value);
                if (m.Success)
                {
                    var playlist = await spotifyClient.Playlists.Get(m.Groups[2].Value);
                    var firstsong = (FullTrack)playlist.Tracks.Items.First(e => e.Track is FullTrack).Track;
                    return new(new SongORSongs(await AudioService.GetTrackAsync($"{firstsong.Name} {firstsong.Artists[0].Name}", SearchMode.YouTube), playlist.Name, GetTracksUsingPlaylist(playlist, AudioService)));
                }
            }
            var track = await AudioService.GetTracksAsync(value, SearchMode.None);
            if (track is null || !track.Any())
            {
                track = new LavalinkTrack[] { await AudioService.GetTrackAsync(value, SearchMode.YouTube) };

                if (track is null || !track.Any())
                {
                    track = new LavalinkTrack[] { await AudioService.GetTrackAsync(value, SearchMode.SoundCloud) };
                    if (track is null || !track.Any())
                    {
                        await Audio.SendSimpleMessage(ctx, string.Format(lang.NoResults, value), language: lang);
                        return new();
                    }
                }
            }
            return new(new(track.First(), null, track.Skip(1).ToAsyncEnumerable()));
        }
    }
}
