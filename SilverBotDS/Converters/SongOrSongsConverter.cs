/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using Lavalink4NET;
using Lavalink4NET.Decoding;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using SilverBotDS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using SilverBot.Shared.Objects;
using SilverBot.Shared.Objects.Classes;
using SilverBot.Shared.Objects.Language;
using SilverBot.Shared;
using SilverBot.Shared.Utils;
using SilverBotDS.Commands.Slash;

namespace SilverBotDS.Converters;

public class SongOrSongsConverter : IArgumentConverter<SongORSongs>
{
    private const string JellyStart = "sjelly:";
      public static async Task<SongORSongs?> ConvertToSongSB(ISilverBotContext ctx, string value,
        Language? language = null)
    {
        var conf = ctx.Services.GetService<Config>();
        var languageService = ctx.Services.GetService<LanguageService>();
        var audioService = ctx.Services.GetService<LavalinkNode>();
        language ??= await languageService?.FromCtxAsync(ctx)!;
        if ( !IsInVc(ctx, audioService))
        {
            if (ctx.Member?.VoiceState?.Channel == null)
            {
                await ctx.SendMessageAsync(language.UserNotConnected, language: language);
                return null;
            }
            await NeutralAudio.StaticJoin(ctx, audioService, language:language);
        }

        if (conf.SongAliases.ContainsKey(value))
        {
            value = conf.SongAliases[value];
        }
        var lookupService = ctx.Services.GetService<ITrackOrAlbumLookupService>();
        if (lookupService != null && value.StartsWith(JellyStart))
        {
            var x = await lookupService.TryGettingTrackOrAlbum(value.RemoveStringFromStart(JellyStart));
            if (x is not null)
            {
                List<LavalinkTrack> tracks = new();
                foreach (var song in x)
                {
                    var s = await audioService.GetTrackAsync(song);
                    byte tries = 3;
                    while (s is null && tries!=0)
                    {
                        s = await audioService.GetTrackAsync(song);
                        tries--;
                    }

                    if (s != null)
                    {
                        tracks.Add(s);
                    }

                    await Task.Delay(10);
                }
                return new SongORSongs(tracks.First(), null,
                    tracks.Skip(1).ToAsyncEnumerable());
            }
        }
        if (value.EndsWith(".json"))
        {
            var client = ctx.Services.GetService<HttpClient>();
            if (client is not null)
            {
                var tracks =
                    JsonSerializer.Deserialize<SilverBotPlaylist>(await (await client.GetAsync(value)).Content
                        .ReadAsStringAsync());
                if (!string.IsNullOrEmpty(tracks.PlaylistTitle))
                {
                    await ctx.SendMessageAsync(
                        string.Format(language.LoadedSilverBotPlaylistWithTitle, tracks.PlaylistTitle),
                        language: language);
                }

                return (new SongORSongs(TrackDecoder.DecodeTrack(tracks.Identifiers[0]), null,
                    tracks.Identifiers.Skip(1).Select(TrackDecoder.DecodeTrack).ToAsyncEnumerable(),
                    TimeSpan.FromMilliseconds(tracks.CurrentSongTimems)));
            }
        }

        IEnumerable<LavalinkTrack?>? track = await audioService.GetTracksAsync(value);
        var lavalinkTracks = track.ToList();
        if (lavalinkTracks.Any() && lavalinkTracks.First() is {} firstRawTrack)
        {
            return (new SongORSongs(firstRawTrack, null, lavalinkTracks.Skip(1).Cast<LavalinkTrack>().ToAsyncEnumerable()));
        }

        track = new[] { await audioService.GetTrackAsync(value, SearchMode.YouTube) };
        if (track.Any() && track.First() is { } firstYtTrack)
        {
            return (new SongORSongs(firstYtTrack, null, track.Skip(1).Cast<LavalinkTrack>().ToAsyncEnumerable()));
        }

        track = new[] { await audioService.GetTrackAsync(value, SearchMode.SoundCloud) };
        if (track.Any() && track.First() is  { } firstSCTrack)
        {
            return (new SongORSongs(firstSCTrack, null, track.Skip(1).Cast<LavalinkTrack>().ToAsyncEnumerable()));
        }

        await ctx.SendMessageAsync(string.Format(language.NoResults, value), language: language);
        return null;
    }
    public static async Task<SongORSongs?> ConvertToSong(InteractionContext ctx, string value,
        Language? language = null)
    {
     
        return await ConvertToSongSB(new UnBasedCommandContext(ctx), value, language);
    }

     
    public async Task<Optional<SongORSongs>> ConvertAsync(string value, CommandContext ctx)
    {
        return await ConvertToSongSB(new BasedCommandContext(ctx), value) ?? Optional.FromNoValue<SongORSongs>();
    }


    private static bool IsInVc(ISilverBotContext ctx, IAudioService audioService) =>
        ctx.Guild != null &&
        audioService.HasPlayer(ctx.Guild.Id) &&
        audioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id) is not null and not { State: PlayerState.NotConnected}
            and not { State: PlayerState.Destroyed };
 
}