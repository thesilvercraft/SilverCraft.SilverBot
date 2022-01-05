using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using Lavalink4NET;
using Lavalink4NET.Decoding;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using SilverBotDS.Commands;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SpotifyAPI.Web;

namespace SilverBotDS.Converters;

public class SongOrSongsConverter : IArgumentConverter<SongORSongs>
{
    private static readonly Regex PlaylistRegex =
        new(
            @"^(https:\/\/open\.spotify\.com\/user\/spotify\/playlist\/|https:\/\/open\.spotify\.com\/playlist\/|spotify:user:spotify:playlist:|spotify:playlist:)([a-zA-Z0-9]+)(.*)$",
            RegexOptions.Compiled);

    private static readonly Regex TrackRegex =
        new(
            @"^(https:\/\/open\.spotify\.com\/user\/spotify\/track\/|https:\/\/open\.spotify\.com\/track\/|spotify:user:spotify:track:|spotify:track:)([a-zA-Z0-9]+)(.*)$",
            RegexOptions.Compiled);

    private static readonly Regex AlbumRegex =
        new(@"^(https:\/\/open\.spotify\.com\/album\/|spotify:album:)([a-zA-Z0-9]+)(.*)$", RegexOptions.Compiled);

    public async Task<Optional<SongORSongs>> ConvertAsync(string value, CommandContext ctx)
    {
        var spotifyClient = (SpotifyClient) ctx.CommandsNext.Services.GetService(typeof(SpotifyClient));
        var conf = (Config) ctx.CommandsNext.Services.GetService(typeof(Config));
        var audioService = (LavalinkNode) ctx.CommandsNext.Services.GetService(typeof(LavalinkNode));
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        if (!IsInVc(ctx, audioService))
        {
            if (ctx.Member?.VoiceState?.Channel == null)
            {
                await Audio.SendSimpleMessage(ctx, lang.UserNotConnected, language: lang);
                return new Optional<SongORSongs>();
            }

            await Audio.StaticJoin(ctx, audioService);
            await ctx.TriggerTypingAsync();
        }

        if (conf.SongAliases.ContainsKey(value))
        {
            value = conf.SongAliases[value];
        }

        await ctx.TriggerTypingAsync();
        if (value.EndsWith(".json"))
        {
            var client = (HttpClient) ctx.CommandsNext.Services.GetService(typeof(HttpClient));
            if (client is not null)
            {
                var tracks =
                    JsonSerializer.Deserialize<SilverBotPlaylist>(await (await client.GetAsync(value)).Content
                        .ReadAsStringAsync());
                if (!string.IsNullOrEmpty(tracks.PlaylistTitle))
                {
                    await Audio.SendSimpleMessage(ctx,
                        string.Format(lang.LoadedSilverBotPlaylistWithTitle, tracks.PlaylistTitle), language: lang);
                }

                return new Optional<SongORSongs>(new SongORSongs(TrackDecoder.DecodeTrack(tracks.Identifiers[0]), null,
                    tracks.Identifiers.Skip(1).Select(x => TrackDecoder.DecodeTrack(x)).ToAsyncEnumerable(),
                    TimeSpan.FromMilliseconds(tracks.CurrentSongTimems)));
            }
        }

        if (spotifyClient is not null && IsSpotifyString(value))
        {
            var m = TrackRegex.Match(value);
            if (m.Success)
            {
                var song = await spotifyClient.Tracks.Get(m.Groups[2].Value);
                return new Optional<SongORSongs>(new SongORSongs(
                    await audioService.GetTrackAsync($"{song.Name} {song.Artists[0].Name}", SearchMode.YouTube), null,
                    null));
            }

            m = AlbumRegex.Match(value);
            if (m.Success)
            {
                var album = await spotifyClient.Albums.Get(m.Groups[2].Value);
                return new Optional<SongORSongs>(new SongORSongs(
                    await audioService.GetTrackAsync(
                        $"{album.Tracks.Items[0].Name} {album.Tracks.Items[0].Artists[0].Name}", SearchMode.YouTube),
                    album.Name, GetTracksUsingAlbum(album, audioService)));
            }

            m = PlaylistRegex.Match(value);
            if (m.Success)
            {
                var playlist = await spotifyClient.Playlists.Get(m.Groups[2].Value);
                var firstsong = (FullTrack) playlist.Tracks.Items.First(e => e.Track is FullTrack).Track;
                return new Optional<SongORSongs>(new SongORSongs(
                    await audioService.GetTrackAsync($"{firstsong.Name} {firstsong.Artists[0].Name}",
                        SearchMode.YouTube), playlist.Name, GetTracksUsingPlaylist(playlist, audioService)));
            }
        }

        var track = await audioService.GetTracksAsync(value);
        if (track?.Any() != true)
        {
            track = new[] {await audioService.GetTrackAsync(value, SearchMode.YouTube)};

            if (track?.Any() != true)
            {
                track = new[] {await audioService.GetTrackAsync(value, SearchMode.SoundCloud)};
                if (track?.Any() != true)
                {
                    await Audio.SendSimpleMessage(ctx, string.Format(lang.NoResults, value), language: lang);
                    return new Optional<SongORSongs>();
                }
            }
        }

        return new Optional<SongORSongs>(new SongORSongs(track.First(), null, track.Skip(1).ToAsyncEnumerable()));
    }

    private bool IsSpotifyString(string url)
    {
        return TrackRegex.IsMatch(url) || AlbumRegex.IsMatch(url) || PlaylistRegex.IsMatch(url);
    }

    private async IAsyncEnumerable<LavalinkTrack> GetTracksUsingAlbum(FullAlbum album, LavalinkNode audioService,
        uint skipsongs = 1)
    {
        foreach (var song in album.Tracks.Items)
        {
            if (skipsongs != 0)
            {
                skipsongs--;
                continue;
            }

            var e = await audioService.GetTrackAsync($"{song.Name} {song.Artists[0].Name}", SearchMode.YouTube);
            if (e is not null)
            {
                yield return e;
            }
        }
    }

    private async IAsyncEnumerable<LavalinkTrack> GetTracksUsingPlaylist(FullPlaylist playlist,
        LavalinkNode audioService, uint skipsongs = 1)
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

                var e = await audioService.GetTrackAsync($"{track.Name} {track.Artists[0].Name}", SearchMode.YouTube);
                if (e is not null)
                {
                    yield return e;
                }
            }
        }
    }

    private bool IsInVc(CommandContext ctx, LavalinkNode audioService)
    {
        return audioService.HasPlayer(ctx.Guild.Id) &&
               audioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id) is not null &&
               (audioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id).State != PlayerState.NotConnected
                || audioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id).State != PlayerState.Destroyed);
    }
}