/*
 *  This file uses a substantial portion of Lavalink4NET's code (which is avaliable publicly on https://github.com/angelobreuer/Lavalink4NET).
 *  As this file contains portions of Lavalink4NET's code, Lavalink4NET's License shall be stated here:
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2021
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.VoiceNext;
using Lavalink4NET;
using Lavalink4NET.Events;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using VideoLibrary;
using System.Text.RegularExpressions;

namespace SnowdPlayer
{


    public class SnowService : IDisposable
    {
        public SnowService(VoiceNextExtension vn, bool allowlocalfiles=false)
        {
            VoiceNextExtension = vn;
            AllowLocalFiles = allowlocalfiles;
        }

        private bool disposedValue;

        private bool AllowLocalFiles { get; set; }
        public event AsyncEventHandler<TrackEndEventArgs> TrackEnd;
        public event AsyncEventHandler<TrackExceptionEventArgs> TrackException;
        public event AsyncEventHandler<TrackStartedEventArgs> TrackStarted;
        public event AsyncEventHandler<TrackStuckEventArgs> TrackStuck;

        private VoiceNextExtension VoiceNextExtension { get; init; }
        protected Dictionary<ulong, SnowPlayer> Players { get; } = new();

        private static T CreateDefaultFactory<T>() where T : SnowPlayer, new() => new T();

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
        public Task<TPlayer> JoinAsync<TPlayer>(ulong guildId, ulong voiceChannelId, bool selfDeaf = false, bool selfMute = false) where TPlayer : SnowPlayer, new()
            => JoinAsync(CreateDefaultFactory<TPlayer>, guildId, voiceChannelId, selfDeaf, selfMute);

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
        public Task<SnowPlayer> JoinAsync(ulong guildId, ulong voiceChannelId, bool selfDeaf = false, bool selfMute = false)
           => JoinAsync(CreateDefaultFactory<SnowPlayer>, guildId, voiceChannelId, selfDeaf, selfMute);

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
        public async Task<TPlayer> JoinAsync<TPlayer>(
            PlayerFactory<TPlayer> playerFactory, ulong guildId, ulong voiceChannelId, bool selfDeaf = false,
            bool selfMute = false) where TPlayer : SnowPlayer
        {
            EnsureNotDisposed();

            var player = GetPlayer<TPlayer>(guildId);

            if (player is null)
            {
                Players[guildId] = player = SnowPlayer.CreatePlayer(
                    playerFactory, VoiceNextExtension, guildId, false);
            }

            if (!player.VoiceChannelId.HasValue || player.VoiceChannelId != voiceChannelId)
            {
                await player.ConnectAsync(voiceChannelId, selfDeaf, selfMute);
            }

            return player;
        }

        /// <summary>
        /// Gets an already made player
        /// </summary>
        /// <typeparam name="TPlayer">The type of the player</typeparam>
        /// <param name="guildId">The id of the guild the player is in</param>
        /// <returns>A player that is the type of <typeparamref name="TPlayer"/></returns>
        public TPlayer GetPlayer<TPlayer>(ulong guildId) where TPlayer : SnowPlayer
        {
            EnsureNotDisposed();
            if (!Players.ContainsKey(guildId))
            {
                return null;
            }
            if (!Players.TryGetValue(guildId, out var gaming))
            {
                return null;
            }
            if (gaming.disposedValue)
            {
                Players.Remove(guildId);
                return null;
            }
            if (gaming is TPlayer player)
            {
                return player;
            }
            else
            {
                throw new InvalidOperationException("A player was found but the player type differs from the found one.");
            }
        }

        new public bool HasPlayer(ulong id)
        {
            EnsureNotDisposed();
            return Players.ContainsKey(id);
        }

        private void EnsureNotDisposed()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(SnowService));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    foreach (var p in Players)
                    {
                        p.Value.Dispose();
                    }
                    Players.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SnowService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public LavalinkPlayer GetPlayer(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<TPlayer> GetPlayers<TPlayer>() where TPlayer : LavalinkPlayer
        {
            throw new NotImplementedException();
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
        Regex YTRgx = new(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)");
        public async Task<SnowTrack> GetTrackAsync(string query, SearchMode mode = SearchMode.None, bool noCache = false, CancellationToken cancellationToken = default)
        {
            if (Uri.TryCreate(query, UriKind.Absolute, out Uri result))
            {
                if (result.IsFile)
                {
                   // return StreamProvider.Local;
                }

               // return GetStreamProvider(result.Host, result.AbsolutePath);
            }
            if (YTRgx.IsMatch(query))
            {
                var video = await YouTube.Default.GetVideoAsync(query);
                if (video != null)
                {
                    return new SnowTrack(query, video.Info.Author, TimeSpan.FromSeconds(video.Info.LengthSeconds ?? 0), video.Info.LengthSeconds == null, true, query, video.Info.Title, YTRgx.Match(query).Groups[1].Value, StreamProvider.YouTube);
                }
            }
            return null;
        }

        public Task<IEnumerable<SnowTrack>> GetTracksAsync(string query, SearchMode mode = SearchMode.None, bool noCache = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TrackLoadResponsePayload> LoadTracksAsync(string query, SearchMode mode = SearchMode.None, bool noCache = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public delegate TPlayer PlayerFactory<TPlayer>() where TPlayer : SnowPlayer;

    public class SnowTrack : LavalinkTrack
    {
        public static SnowTrack FromLavalinkTrack(LavalinkTrack b) => new(b.Identifier, b.Author, b.Duration, b.IsLiveStream, b.IsSeekable, b.Source, b.Title, b.TrackIdentifier, b.Provider);
        public SnowTrack(string identifier, LavalinkTrackInfo info) : base(identifier, info)
        {
        }

        public SnowTrack(string identifier, string author, TimeSpan duration, bool isLiveStream, bool isSeekable, string source, string title, string trackIdentifier, StreamProvider provider) : base(identifier, author, duration, isLiveStream, isSeekable, source, title, trackIdentifier, provider)
        {
        }
    }

    public class SnowPlayer : IDisposable
    {

        public bool disposedValue;

        public ulong? VoiceChannelId { get; internal set; }
        public ulong GuildId { get; private set; }
        public bool DisconnectOnStop { get; private set; }
        public VoiceNextExtension VoiceNext { get; private set; }
        public SnowTrack CurrentTrack { get; private set; }
        public PlayerState State { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SnowPlayer()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Initializes a new instance of a player.
        /// </summary>
        /// <param name="playerFactory">the player factory</param>
        /// <param name="guildId">the identifier of the guild that is controlled by the player</param>
        /// <param name="disconnectOnStop">
        ///     a value indicating whether the player should stop after the track finished playing
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="playerFactory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="socket"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="client"/> is <see langword="null"/>.
        /// </exception>
        internal static T CreatePlayer<T>(
            PlayerFactory<T> playerFactory, VoiceNextExtension vne,
            ulong guildId, bool disconnectOnStop) where T : SnowPlayer
        {
            if (playerFactory is null)
            {
                throw new ArgumentNullException(nameof(playerFactory));
            }

            var player = playerFactory();

            player.GuildId = guildId;
            player.DisconnectOnStop = disconnectOnStop;
            player.VoiceNext = vne;
            return player;
        }

        private async Task<string> GetURLAsync(LavalinkTrack track)
        {
            Console.WriteLine(track.Provider);
            if (track.Provider == StreamProvider.YouTube)
            {
                var video = (await YouTube.Default.GetAllVideosAsync(track.Source)).First(x=>x.AdaptiveKind==AdaptiveKind.Audio);
                Console.WriteLine(video.Uri);
                return video.Uri;
            }
            return track.Source;
        }
        /// <summary>
        ///     Plays the specified <paramref name="track"/> asynchronously.
        /// </summary>
        /// <param name="track">the track to play</param>
        /// <param name="startTime">the track start position</param>
        /// <param name="endTime">the track end position</param>
        /// <param name="noReplace">
        ///     a value indicating whether the track play should be ignored if the same track is
        ///     currently playing
        /// </param>
        /// <returns>a task that represents the asynchronous operation</returns>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the player is not connected to a voice channel
        /// </exception>
        /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
        public virtual async Task PlayAsync(SnowTrack track, TimeSpan? startTime = null,
            TimeSpan? endTime = null, bool noReplace = false)
        {
            CurrentTrack = track ?? throw new ArgumentNullException(nameof(track));
            //startTime ??= track.Position;
            var filePath = await GetURLAsync(track);
            Console.WriteLine(filePath);
            if (filePath != null)
            {
                FFmpeg = Process.Start(new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $@"-i ""{filePath}"" -err_detect ignore_err -ac 2 -bufsize 69420k -f s16le -ar 48000 pipe:1",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                });
                cancellationToken1 = new();
                _ = Task.Run(async () => await FFmpeg.StandardOutput.BaseStream.CopyToAsync(vnc.GetTransmitSink(), cancellationToken: cancellationToken1.Token));
                FFmpeg.Exited += FFmpeg_Exited;
                State = PlayerState.Playing;
            }

        }

        private void FFmpeg_Exited(object sender, EventArgs e)
        {
            OnTrackEndAsync().GetAwaiter().GetResult();
        }
        public virtual Task OnTrackEndAsync()
        {
            if (DisconnectOnStop)
            {
                return DisconnectAsync();
            }
            else
            {
                // The track ended, set to not playing
                State = PlayerState.NotPlaying;
                CurrentTrack = null;
            }

            return Task.CompletedTask;
        }

        public virtual Task DisconnectAsync()
        {
            if (cancellationToken1 != null)
            {
                cancellationToken1.Cancel();
                cancellationToken1 = new();
            }
            if (!FFmpeg.HasExited)
            {
                FFmpeg.Kill();
            }
            vnc.Disconnect();
            State = PlayerState.NotConnected;
            return Task.CompletedTask;
        }

        public virtual Task PauseAsync()
        {
            if (State == PlayerState.Playing)
            {
                cancellationToken1.Cancel();
                State = PlayerState.Paused;
            }
            return Task.CompletedTask;
        }
        public virtual Task ResumeAsync()
        {
            if (State == PlayerState.Paused)
            {
                cancellationToken1 = new();
                _ = Task.Run(async () => await FFmpeg.StandardOutput.BaseStream.CopyToAsync(vnc.GetTransmitSink(), cancellationToken: cancellationToken1.Token));
                State = PlayerState.Playing;
            }
            return Task.CompletedTask;
        }
        public virtual async Task StopAsync()
        {
            if (DisconnectOnStop)
            {
                await DisconnectAsync();
            }
            else
            {
                if (cancellationToken1 != null)
                {
                    cancellationToken1.Cancel();
                    cancellationToken1 = new();
                }
                if (!FFmpeg.HasExited)
                {
                    FFmpeg.Kill();
                }
                State = PlayerState.NotPlaying;
            }
        }
        private CancellationTokenSource cancellationToken1 {  get; set; }
        private Process FFmpeg { get; set; }
        private VoiceNextConnection vnc { get; set; }

        public Task PlayTopAsync(LavalinkTrack song)
        {
            throw new NotImplementedException();
        }

        internal async Task ConnectAsync(ulong voiceChannelId, bool selfDeaf, bool selfMute)
        {
            var channel = await VoiceNext.Client.GetChannelAsync(voiceChannelId);
            vnc = await channel.ConnectAsync();
        }
    }
}