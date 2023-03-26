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
using SilverBotDS.Commands.Slash;

namespace SilverBotDS.Converters;

public class SongOrSongsConverter : IArgumentConverter<SongORSongs>
{
    public static async Task<SongORSongs?> ConvertToSong(InteractionContext ctx, string value,
        Language? language = null)
    {
        var conf = ctx.Services.GetService<Config>();
        var languageService = ctx.Services.GetService<LanguageService>();
        var audioService = ctx.Services.GetService<LavalinkNode>();
        language ??= await languageService.FromCtxAsync(ctx);
        if (!IsInVc(ctx, audioService))
        {
            if (ctx.Member?.VoiceState?.Channel == null)
            {
                await ctx.SendMessageAsync(language.UserNotConnected, language: language);
                return null;
            }

            await NeutralAudio.StaticJoin(new UnBasedCommandContext(ctx), audioService);
        }

        if (conf.SongAliases.ContainsKey(value))
        {
            value = conf.SongAliases[value];
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
        if (track.Any())
        {
            return (new SongORSongs(track.First(), null, track.Skip(1).Cast<LavalinkTrack>().ToAsyncEnumerable()));
        }

        track = new[] { await audioService.GetTrackAsync(value, SearchMode.YouTube) };
        if (track.Any() && track.First() is not null)
        {
            return (new SongORSongs(track.First(), null, track.Skip(1).Cast<LavalinkTrack>().ToAsyncEnumerable()));
        }

        track = new[] { await audioService.GetTrackAsync(value, SearchMode.SoundCloud) };
        if (track.Any() && track.First() is not null)
        {
            return (new SongORSongs(track.First(), null, track.Skip(1).Cast<LavalinkTrack>().ToAsyncEnumerable()));
        }

        await ctx.SendMessageAsync(string.Format(language.NoResults, value), language: language);
        return null;
    }

    public async Task<Optional<SongORSongs>> ConvertAsync(string value, CommandContext ctx)
    {
        var conf = ctx.Services.GetService<Config>();
        var languageService = ctx.Services.GetService<LanguageService>();
        var audioService = ctx.Services.GetService<LavalinkNode>();
        var lang = await languageService.FromCtxAsync(ctx);
        if (!IsInVc(ctx, audioService))
        {
            if (ctx.Member?.VoiceState?.Channel == null)
            {
                await ctx.SendMessageAsync(lang.UserNotConnected, language: lang);
                return new Optional<SongORSongs>();
            }

            await NeutralAudio.StaticJoin(new BasedCommandContext(ctx), audioService);
            await ctx.TriggerTypingAsync();
        }

        if (conf.SongAliases.ContainsKey(value))
        {
            value = conf.SongAliases[value];
        }

        await ctx.TriggerTypingAsync();
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
                        string.Format(lang.LoadedSilverBotPlaylistWithTitle, tracks.PlaylistTitle), language: lang);
                }

                return new Optional<SongORSongs>(new SongORSongs(TrackDecoder.DecodeTrack(tracks.Identifiers[0]), null,
                    tracks.Identifiers.Skip(1).Select(TrackDecoder.DecodeTrack).ToAsyncEnumerable(),
                    TimeSpan.FromMilliseconds(tracks.CurrentSongTimems)));
            }
        }

        IEnumerable<LavalinkTrack?>? track = await audioService.GetTracksAsync(value);
        if (track.Any())
        {
            return new Optional<SongORSongs>(new SongORSongs(track.First(), null,
                track.Skip(1).Cast<LavalinkTrack>().ToAsyncEnumerable()));
        }

        track = new[] { await audioService.GetTrackAsync(value, SearchMode.YouTube) };
        if (track.Any() && track.First() is not null)
        {
            return new Optional<SongORSongs>(new SongORSongs(track.First(), null,
                track.Skip(1).Cast<LavalinkTrack>().ToAsyncEnumerable()));
        }

        track = new[] { await audioService.GetTrackAsync(value, SearchMode.SoundCloud) };
        if (track.Any() && track.First() is not null)
        {
            return new Optional<SongORSongs>(new SongORSongs(track.First(), null,
                track.Skip(1).Cast<LavalinkTrack>().ToAsyncEnumerable()));
        }

        await ctx.SendMessageAsync(string.Format(lang.NoResults, value), language: lang);
        return new Optional<SongORSongs>();
    }

    private static bool IsInVc(CommandContext ctx, IAudioService audioService) =>
        audioService.HasPlayer(ctx.Guild.Id) &&
        audioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id) is not null and not { State: PlayerState.NotConnected}
                                                                                   and not { State: PlayerState.Destroyed };
    private static bool IsInVc(BaseContext ctx, IAudioService audioService) => 
        audioService.HasPlayer(ctx.Guild.Id) &&
        audioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id) is not null and not { State: PlayerState.NotConnected}
                                                                                   and not { State: PlayerState.Destroyed };
}