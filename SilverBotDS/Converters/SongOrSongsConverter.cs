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
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SilverBot.Shared.Objects;
using SilverBot.Shared.Objects.Classes;
using SilverBot.Shared.Objects.Language;

namespace SilverBotDS.Converters;

public class SongOrSongsConverter : IArgumentConverter<SongORSongs>
{

    public async Task<Optional<SongORSongs>> ConvertAsync(string value, CommandContext ctx)
    {
        var conf = ctx.CommandsNext.Services.GetService<Config>();
        var languageService =  ctx.CommandsNext.Services.GetService<LanguageService>();
        var audioService =  ctx.CommandsNext.Services.GetService<LavalinkNode>();
        var lang = await languageService.FromCtxAsync(ctx);
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
            var client = ctx.CommandsNext.Services.GetService<HttpClient>();
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

        var track = await audioService.GetTracksAsync(value);
        if (track?.Any() == true)
        {
            return new Optional<SongORSongs>(new SongORSongs(track.First(), null, track.Skip(1).ToAsyncEnumerable()));
        }
        track = new[] { await audioService.GetTrackAsync(value, SearchMode.YouTube) };
        if (track?.Any() == true)
        {
            return new Optional<SongORSongs>(new SongORSongs(track.First(), null, track.Skip(1).ToAsyncEnumerable()));
        }
        track = new[] { await audioService.GetTrackAsync(value, SearchMode.SoundCloud) };
        if (track?.Any() == true)
        {
            return new Optional<SongORSongs>(new SongORSongs(track.First(), null, track.Skip(1).ToAsyncEnumerable()));
        }
        await Audio.SendSimpleMessage(ctx, string.Format(lang.NoResults, value), language: lang);
        return new Optional<SongORSongs>();
    }

    private bool IsInVc(CommandContext ctx, LavalinkNode audioService)
    {
        return audioService.HasPlayer(ctx.Guild.Id) &&
               audioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id) is not null &&
               (audioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id).State != PlayerState.NotConnected
                || audioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id).State != PlayerState.Destroyed);
    }
}