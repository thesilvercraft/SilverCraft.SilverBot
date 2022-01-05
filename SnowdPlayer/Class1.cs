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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.VoiceNext;
using Lavalink4NET.Events;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using VideoLibrary;
using Xabe.FFmpeg;

namespace SnowdPlayer;

/// <summary>
///     A thread-safe queue for <see cref="SnowTrack" />.
/// </summary>
public class SnowQueue : IList<SnowTrack>
{
    private readonly List<SnowTrack> _list;
    private readonly object _syncRoot;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SnowQueue" /> class.
    /// </summary>
    public SnowQueue()
    {
        _list = new List<SnowTrack>();
        _syncRoot = new object();
    }

    /// <summary>
    ///     Gets a value indicating whether the queue is empty.
    /// </summary>
    /// <remarks>
    ///     This property is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    public bool IsEmpty => Count == 0;

    /// <summary>
    ///     Gets or sets the enqueued tracks.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    public IReadOnlyList<SnowTrack> Tracks
    {
        get
        {
            lock (_syncRoot)
            {
                // return new array
                return _list.ToArray();
            }
        }

        set
        {
            lock (_syncRoot)
            {
                // clear old tracks
                _list.Clear();

                // add new tracks
                _list.AddRange(value);
            }
        }
    }

    /// <summary>
    ///     Gets the number of queued tracks.
    /// </summary>
    /// <remarks>
    ///     This property is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    public int Count
    {
        get
        {
            lock (_syncRoot)
            {
                return _list.Count;
            }
        }
    }

    /// <summary>
    ///     Gets a value indicating whether the queue is read-only.
    /// </summary>
    /// <remarks>
    ///     This property is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    public bool IsReadOnly => false;

    /// <summary>
    ///     Gets or sets the track at the specified <paramref name="index" />.
    /// </summary>
    /// <remarks>
    ///     This indexer property is thread-safe, so it can be used from multiple threads at
    ///     once safely.
    /// </remarks>
    /// <param name="index">the zero-based position</param>
    /// <returns>the track at the specified <paramref name="index" /></returns>
    public SnowTrack this[int index]
    {
        get
        {
            lock (_syncRoot)
            {
                return _list[index];
            }
        }

        set
        {
            // a track can not be null
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            lock (_syncRoot)
            {
                _list[index] = value;
            }
        }
    }

    /// <summary>
    ///     Adds a track at the end of the queue.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    /// <param name="track">the track to add</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="track" /> is <see langword="null" />.
    /// </exception>
    public void Add(SnowTrack track)
    {
        if (track is null)
        {
            throw new ArgumentNullException(nameof(track));
        }

        lock (_syncRoot)
        {
            _list.Add(track);
        }
    }

    /// <summary>
    ///     Gets a value indicating whether the specified <paramref name="track" /> is in the queue.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    /// <param name="track">the track to find</param>
    /// <returns>
    ///     a value indicating whether the specified <paramref name="track" /> is in the queue
    /// </returns>
    public bool Contains(SnowTrack track)
    {
        if (track is null)
        {
            throw new ArgumentNullException(nameof(track));
        }

        lock (_syncRoot)
        {
            return _list.Contains(track);
        }
    }

    /// <summary>
    ///     Copies all tracks to the specified <paramref name="array" /> at the specified
    ///     <paramref name="arrayIndex" />.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    /// <param name="array">the array to the tracks to</param>
    /// <param name="arrayIndex">the zero-based writing start index</param>
    public void CopyTo(SnowTrack[] array, int arrayIndex)
    {
        lock (_syncRoot)
        {
            _list.CopyTo(array, arrayIndex);
        }
    }

    /// <summary>
    ///     Gets the track enumerator.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    /// <returns>the track enumerator</returns>
    public IEnumerator<SnowTrack> GetEnumerator()
    {
        lock (_syncRoot)
        {
            return _list.ToList().GetEnumerator();
        }
    }

    /// <summary>
    ///     Gets the zero-based index of the specified <paramref name="track" />.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    /// <param name="track">the track to locate</param>
    /// <returns>the zero-based index of the specified <paramref name="track" /></returns>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="track" /> is <see langword="null" />.
    /// </exception>
    public int IndexOf(SnowTrack track)
    {
        if (track is null)
        {
            throw new ArgumentNullException(nameof(track));
        }

        lock (_syncRoot)
        {
            return _list.IndexOf(track);
        }
    }

    /// <summary>
    ///     Inserts the specified <paramref name="track" /> at the specified <paramref name="index" />.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    /// <param name="index">the zero-based index to insert (e.g. 0 = top)</param>
    /// <param name="track">the track to insert</param>
    public void Insert(int index, SnowTrack track)
    {
        lock (_syncRoot)
        {
            _list.Insert(index, track);
        }
    }

    /// <summary>
    ///     Tries to remove the specified <paramref name="track" /> from the queue.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    /// <param name="track">the track to remove</param>
    /// <returns>
    ///     a value indicating whether the track was found and removed from the queue
    /// </returns>
    public bool Remove(SnowTrack track)
    {
        lock (_syncRoot)
        {
            return _list.Remove(track);
        }
    }

    /// <summary>
    ///     Removes a track at the specified <paramref name="index" />.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    /// <param name="index">the index to remove the track</param>
    public void RemoveAt(int index)
    {
        lock (_syncRoot)
        {
            _list.RemoveAt(index);
        }
    }

    /// <summary>
    ///     Clears the queue.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    void ICollection<SnowTrack>.Clear()
    {
        lock (_syncRoot)
        {
            _list.Clear();
        }
    }

    /// <summary>
    ///     Gets the track enumerator.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    /// <returns>the track enumerator</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        lock (_syncRoot)
        {
            return _list.ToArray().GetEnumerator();
        }
    }

    /// <summary>
    ///     Adds all specified <paramref name="tracks" /> to the queue.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    /// <param name="tracks">the tracks to enqueue</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="tracks" /> enumerable is <see langword="null" />.
    /// </exception>
    public void AddRange(IEnumerable<SnowTrack> tracks)
    {
        if (tracks is null)
        {
            throw new ArgumentNullException(nameof(tracks));
        }

        lock (_syncRoot)
        {
            _list.AddRange(tracks);
        }
    }

    /// <summary>
    ///     Clears all tracks from the queue.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    /// <returns>the number of tracks removed</returns>
    public int Clear()
    {
        lock (_syncRoot)
        {
            var tracks = _list.Count;
            _list.Clear();
            return tracks;
        }
    }

    /// <summary>
    ///     Dequeues a track using the FIFO method.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    /// <returns>the dequeued track</returns>
    /// <exception cref="InvalidOperationException">
    ///     thrown if no tracks were in the queue
    /// </exception>
    public SnowTrack Dequeue()
    {
        lock (_syncRoot)
        {
            if (_list.Count == 0)
            {
                throw new InvalidOperationException("No tracks in to dequeue.");
            }

            var track = _list[0];
            _list.RemoveAt(0);
            return track;
        }
    }

    /// <summary>
    ///     Deletes all duplicate tracks from the queue.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    public void Distinct()
    {
        lock (_syncRoot)
        {
            // no distinct needed, only a single track enqueued (there can not be any duplicate
            // tracks in queue)
            if (_list.Count <= 1)
            {
                return;
            }

            // distinct tracks
            var tracks = _list.GroupBy(track => track.Identifier)
                .Select(s => s.First())
                .ToArray();

            // clear old tracks
            _list.Clear();

            // add distinct tracks
            _list.AddRange(tracks);
        }
    }

    /// <summary>
    ///     Removes all tracks that matches the specified <paramref name="predicate" />.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    /// <param name="predicate">the track predicate</param>
    /// <returns>the number of tracks removed</returns>
    public int RemoveAll(Predicate<SnowTrack> predicate)
    {
        lock (_syncRoot)
        {
            return _list.RemoveAll(predicate);
        }
    }

    /// <summary>
    ///     Removes all <paramref name="count" /> tracks from the specified <paramref name="index" />.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    /// <param name="index">the start index (zero-based)</param>
    /// <param name="count">the number of tracks to remove</param>
    public void RemoveRange(int index, int count)
    {
        lock (_syncRoot)
        {
            _list.RemoveRange(index, count);
        }
    }

    /// <summary>
    ///     Shuffles / mixes all tracks in the queue.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    public void Shuffle()
    {
        lock (_syncRoot)
        {
            // no shuffling needed
            if (_list.Count <= 2)
            {
                return;
            }

            // shuffle tracks
            var tracks = _list.OrderBy(s => Guid.NewGuid()).ToArray();

            // clear old tracks
            _list.Clear();

            // add shuffled tracks
            _list.AddRange(tracks);
        }
    }

#nullable enable

    /// <summary>
    ///     Tries to dequeue a track using the FIFO method.
    /// </summary>
    /// <remarks>
    ///     This method is thread-safe, so it can be used from multiple threads at once safely.
    /// </remarks>
    /// <param name="track">the dequeued track; or default is the result is <see langword="false" />.</param>
    /// <exception cref="InvalidOperationException">
    ///     thrown if no tracks were in the queue
    /// </exception>
    /// <returns>a value indicating whether a track was dequeued.</returns>
    public bool TryDequeue(out SnowTrack? track)
    {
        lock (_syncRoot)
        {
            if (_list.Count == 0)
            {
                track = default;
                return false;
            }

            track = _list[0];
            _list.RemoveAt(0);
            return true;
        }
    }

#nullable disable
}

public class SnowService : IDisposable
{
    private readonly Regex YTRgx = new(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)");

    private bool disposedValue;

    public SnowService(VoiceNextExtension vn, bool allowlocalfiles = false, bool useyoutubedl = false)
    {
        VoiceNextExtension = vn;
        AllowLocalFiles = allowlocalfiles;
        UseYoutubeDL = useyoutubedl;
    }

    private bool UseYoutubeDL { get; }

    private bool AllowLocalFiles { get; }

    private VoiceNextExtension VoiceNextExtension { get; }
    protected Dictionary<ulong, BaseSnowPlayer> Players { get; } = new();

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~SnowService()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public event AsyncEventHandler<TrackEndEventArgs> TrackEnd;

    public event AsyncEventHandler<TrackExceptionEventArgs> TrackException;

    public event AsyncEventHandler<TrackStartedEventArgs> TrackStarted;

    public event AsyncEventHandler<TrackStuckEventArgs> TrackStuck;

    private static T CreateDefaultFactory<T>() where T : BaseSnowPlayer, new()
    {
        return new T();
    }

    /// <inheritdoc />
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public Task<TPlayer> JoinAsync<TPlayer>(ulong guildId, ulong voiceChannelId, bool selfDeaf = false,
        bool selfMute = false) where TPlayer : BaseSnowPlayer, new()
    {
        return JoinAsync(CreateDefaultFactory<TPlayer>, guildId, voiceChannelId, selfDeaf, selfMute);
    }

    /// <inheritdoc />
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public Task<BaseSnowPlayer> JoinAsync(ulong guildId, ulong voiceChannelId, bool selfDeaf = false,
        bool selfMute = false)
    {
        return JoinAsync(CreateDefaultFactory<BaseSnowPlayer>, guildId, voiceChannelId, selfDeaf, selfMute);
    }

    /// <inheritdoc />
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public async Task<TPlayer> JoinAsync<TPlayer>(
        PlayerFactory<TPlayer> playerFactory, ulong guildId, ulong voiceChannelId, bool selfDeaf = false,
        bool selfMute = false) where TPlayer : BaseSnowPlayer
    {
        EnsureNotDisposed();

        var player = GetPlayer<TPlayer>(guildId);

        if (player is null)
        {
            Players[guildId] = player = BaseSnowPlayer.CreatePlayer(
                playerFactory, VoiceNextExtension, guildId, false, UseYoutubeDL);
        }

        if (!player.VoiceChannelId.HasValue || player.VoiceChannelId != voiceChannelId)
        {
            await player.ConnectAsync(voiceChannelId, selfDeaf, selfMute);
        }

        return player;
    }

    /// <summary>
    ///     Gets an already made player
    /// </summary>
    /// <typeparam name="TPlayer">The type of the player</typeparam>
    /// <param name="guildId">The id of the guild the player is in</param>
    /// <returns>A player that is the type of <typeparamref name="TPlayer" /></returns>
    public TPlayer GetPlayer<TPlayer>(ulong guildId) where TPlayer : BaseSnowPlayer
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

        throw new InvalidOperationException("A player was found but the player type differs from the found one.");
    }

    public bool HasPlayer(ulong id)
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

    public async Task<SnowTrack> GetTrackAsync(string query, SearchMode mode = SearchMode.None, bool noCache = false,
        CancellationToken cancellationToken = default)
    {
        /*if(UseYoutubeDL)
        {
            var prc = Process.Start(new ProcessStartInfo
            {
                FileName = "youtube-dl",
                Arguments = $@"--format best --get-url ""{query}""",
                RedirectStandardOutput = true,
                UseShellExecute = false
            });
            prc.WaitForExit();
            if(prc.ExitCode==0)
            {
                HttpClient client = new();
                var filename = Path.GetTempFileName()+".mp3";
                using Stream file = File.OpenWrite(filename);
                await (await (await client.GetAsync(prc.StandardOutput.ReadToEnd(), cancellationToken)).Content.ReadAsStreamAsync(cancellationToken)).CopyToAsync(file, cancellationToken);
                file.Close();
                IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(filename, cancellationToken);
                File.Delete(filename);
                return new SnowTrack(query, "not implemented", mediaInfo.AudioStreams.First().Duration, false, true, query, "not implemented", query, StreamProvider.Http);
            }
        }*/
        if (YTRgx.IsMatch(query))
        {
            var video = await YouTube.Default.GetVideoAsync(query);
            if (video != null)
            {
                return new SnowTrack(query, video.Info.Author, TimeSpan.FromSeconds(video.Info.LengthSeconds ?? 0),
                    video.Info.LengthSeconds == null, true, query, video.Info.Title, YTRgx.Match(query).Groups[1].Value,
                    StreamProvider.YouTube);
            }
        }

        if (Uri.TryCreate(query, UriKind.Absolute, out var result))
        {
            if (result.IsFile && AllowLocalFiles)
            {
                var mediaInfo = await FFmpeg.GetMediaInfo(query, cancellationToken);
                return new SnowTrack(query, "not implemented", mediaInfo.AudioStreams.First().Duration, false, true,
                    query, "not implemented", query, StreamProvider.Local);
            }
            else
            {
                var mediaInfo = await FFmpeg.GetMediaInfo(query, cancellationToken);
                return new SnowTrack(query, "not implemented", mediaInfo.AudioStreams.First().Duration, false, true,
                    query, "not implemented", query, StreamProvider.Http);
            }
        }

        return null;
    }

    public Task<IEnumerable<SnowTrack>> GetTracksAsync(string query, SearchMode mode = SearchMode.None,
        bool noCache = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TrackLoadResponsePayload> LoadTracksAsync(string query, SearchMode mode = SearchMode.None,
        bool noCache = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public delegate TPlayer PlayerFactory<out TPlayer>() where TPlayer : BaseSnowPlayer;

public class SnowTrack : LavalinkTrack
{
    public SnowTrack(string identifier, LavalinkTrackInfo info) : base(identifier, info)
    {
    }

    public SnowTrack(string identifier, string author, TimeSpan duration, bool isLiveStream, bool isSeekable,
        string source, string title, string trackIdentifier, StreamProvider provider) : base(identifier, author,
        duration, isLiveStream, isSeekable, source, title, trackIdentifier, provider)
    {
    }

    public static SnowTrack FromLavalinkTrack(LavalinkTrack b)
    {
        return new SnowTrack(b.Identifier, b.Author, b.Duration, b.IsLiveStream, b.IsSeekable, b.Source, b.Title,
            b.TrackIdentifier, b.Provider);
    }
}

public class QueuedSnowPlayer : BaseSnowPlayer
{
    private readonly bool _disconnectOnStop;

    /// <summary>
    ///     Initializes a new instance of the <see cref="QueuedLavalinkPlayer" /> class.
    /// </summary>
    public QueuedSnowPlayer()
    {
        Queue = new SnowQueue();
        // use own disconnect on stop logic
        _disconnectOnStop = DisconnectOnStop;
        DisconnectOnStop = false;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the current playing track should be looped.
    /// </summary>
    public bool IsLooping { get; set; }

    /// <summary>
    ///     Gets the track queue.
    /// </summary>
    public SnowQueue Queue { get; }

    /// <summary>
    ///     Asynchronously triggered when a track ends.
    /// </summary>
    /// <param name="eventArgs">the track event arguments</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    public override Task OnTrackEndAsync(BaseSnowPlayer baseSnow, TrackEndReason reason, string trackidentifier)
    {
        // Note: It is intended that 'await base.OnTrackEndAsync(baseSnow,reason,trackidentifier);' is avoided here to
        // suppress the State update
        if (!Queue.IsEmpty)
        {
            return SkipAsync();
        }

        if (_disconnectOnStop)
        {
            return DisconnectAsync();
        }

        return StopAsync(false);
    }

    /// <summary>
    ///     Plays the specified <paramref name="track" /> asynchronously.
    /// </summary>
    /// <param name="track">the track to play</param>
    /// <param name="startTime">the track start position</param>
    /// <param name="endTime">the track end position</param>
    /// <param name="noReplace">
    ///     a value indicating whether the track play should be ignored if the same track is
    ///     currently playing
    /// </param>
    /// <returns>
    ///     a task that represents the asynchronous operation
    ///     <para>the position in the track queue ( <c>0</c> = now playing)</para>
    /// </returns>
    public new virtual Task<int> PlayAsync(SnowTrack track, TimeSpan? startTime = null,
        TimeSpan? endTime = null, bool noReplace = false)
    {
        return PlayAsync(track, true, startTime, endTime, noReplace);
    }

    /// <summary>
    ///     Plays the specified <paramref name="track" /> asynchronously.
    /// </summary>
    /// <param name="track">the track to play</param>
    /// <param name="enqueue">
    ///     a value indicating whether the track should be enqueued in the track queue
    /// </param>
    /// <param name="startTime">the track start position</param>
    /// <param name="endTime">the track end position</param>
    /// <param name="noReplace">
    ///     a value indicating whether the track play should be ignored if the same track is
    ///     currently playing
    /// </param>
    /// <returns>
    ///     a task that represents the asynchronous operation
    ///     <para>the position in the track queue ( <c>0</c> = now playing)</para>
    /// </returns>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    public virtual async Task<int> PlayAsync(SnowTrack track, bool enqueue,
        TimeSpan? startTime = null, TimeSpan? endTime = null, bool noReplace = false)
    {
        // check if the track should be enqueued (if a track is already playing)
        if (enqueue && (Queue.Count > 0 || State == PlayerState.Playing || State == PlayerState.Paused))
        {
            // add the track to the queue
            Queue.Add(track);

            // return track queue position
            return Queue.Count;
        }

        // play the track immediately
        await base.PlayAsync(track, startTime, endTime, noReplace);

        // 0 = now playing
        return 0;
    }

    /// <summary>
    ///     Plays the specified <paramref name="track" /> at the top of the queue asynchronously.
    /// </summary>
    /// <param name="track">the track to play</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    public virtual async Task PlayTopAsync(SnowTrack track)
    {
        if (track is null)
        {
            throw new ArgumentNullException(nameof(track));
        }

        // play track if none is playing
        if (State == PlayerState.NotPlaying)
        {
            await PlayAsync(track, false);
        }
        // the player is currently playing a track, enqueue the track at top
        else
        {
            Queue.Insert(0, track);
        }
    }

    /// <summary>
    ///     Pushes a track between the current asynchronously.
    /// </summary>
    /// <param name="track">the track to push between the current</param>
    /// <param name="push">
    ///     a value indicating whether the track should only played when a track is playing currently.
    /// </param>
    /// <remarks>
    ///     Note: This feature is experimental. This will stop playing the current track and
    ///     start playing the specified <paramref name="track" /> after the track is finished the
    ///     track will restart at the stopped position. This can be useful for example
    ///     soundboards (playing an air-horn or something).
    /// </remarks>
    /// <returns>
    ///     a task that represents the asynchronous operation. The task result is a value
    ///     indicating whether the track was pushed between the current (
    ///     <see
    ///         langword="true" />
    ///     ) or the specified track was simply started (
    ///     <see
    ///         langword="false" />
    ///     ), because there is no track playing.
    /// </returns>
    public virtual async Task<bool> PushTrackAsync(SnowTrack track, bool push = false)
    {
        // star track immediately
        if (State == PlayerState.NotPlaying)
        {
            if (push)
            {
                return false;
            }

            await PlayAsync(track, false);
            return false;
        }

        // create clone and set starting position TODO
        // var oldTrack = CurrentTrack!.WithPosition(TrackPosition);

        // enqueue old track with starting position
        Queue.Add(CurrentTrack);

        // push track
        await PlayAsync(track, false);
        return true;
    }

    /// <summary>
    ///     Skips the current track asynchronously.
    /// </summary>
    /// <param name="count">the number of tracks to skip</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    public virtual Task SkipAsync(int count = 1)
    {
        // no tracks to skip
        if (count <= 0)
        {
            return Task.CompletedTask;
        }

        // the looping option is enabled, repeat current track, does not matter how often we skip
        if (IsLooping && CurrentTrack != null)
        {
            return PlayAsync(CurrentTrack, false);
        }
        // tracks are enqueued

        if (!Queue.IsEmpty)
        {
            SnowTrack? track = null;

            while (count-- > 0)
            {
                // no more tracks in queue
                if (Queue.Count < 1)
                {
                    // no tracks found
                    return DisconnectAsync();
                }

                // dequeue track
                track = Queue.Dequeue();
            }

            // a track to play was found, dequeue and play
            StopAsync().GetAwaiter().GetResult();
            return PlayAsync(track!, false);
        }
        // no tracks queued, stop player and disconnect if specified

        if (_disconnectOnStop)
        {
            DisconnectAsync();
        }
        else
        {
            StopAsync();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Stops playing the current track and disconnects asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    public override Task DisconnectAsync()
    {
        Queue.Clear();
        return base.DisconnectAsync();
    }

    /// <summary>
    ///     Stops playing the current track asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    public override Task StopAsync()
    {
        return StopAsync(true);
    }

    /// <summary>
    ///     Stops playing the current track asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    public Task StopAsync(bool clearqueue)
    {
        if (clearqueue)
        {
            Queue.Clear();
        }

        return base.StopAsync();
    }
}

public class BaseSnowPlayer : IDisposable
{
    public bool disposedValue;

    private Task PlayerTask;

    public ulong? VoiceChannelId { get; internal set; }
    public ulong GuildId { get; private set; }
    public bool DisconnectOnStop { get; set; }
    public VoiceNextExtension VoiceNext { get; private set; }
    public SnowTrack CurrentTrack { get; private set; }
    public PlayerState State { get; private set; }

    private bool UseYoutubeDL { get; set; }

    private CancellationTokenSource cancellationToken1 { get; set; }
    private Process FFmpeg { get; set; }
    private VoiceNextConnection vnc { get; set; }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~SnowPlayer()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

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

    /// <summary>
    ///     Initializes a new instance of a player.
    /// </summary>
    /// <param name="playerFactory">the player factory</param>
    /// <param name="guildId">the identifier of the guild that is controlled by the player</param>
    /// <param name="disconnectOnStop">
    ///     a value indicating whether the player should stop after the track finished playing
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="playerFactory" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="socket" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="client" /> is <see langword="null" />.
    /// </exception>
    internal static T CreatePlayer<T>(
        PlayerFactory<T> playerFactory, VoiceNextExtension vne,
        ulong guildId, bool disconnectOnStop, bool useyoutbedl) where T : BaseSnowPlayer
    {
        if (playerFactory is null)
        {
            throw new ArgumentNullException(nameof(playerFactory));
        }

        var player = playerFactory();
        player.GuildId = guildId;
        player.DisconnectOnStop = disconnectOnStop;
        player.VoiceNext = vne;
        player.State = PlayerState.NotConnected;
        player.UseYoutubeDL = useyoutbedl;
        return player;
    }

    private async Task<string> GetURLAsync(LavalinkTrack track)
    {
        if (track.Provider == StreamProvider.YouTube)
        {
            var video = (await YouTube.Default.GetAllVideosAsync(track.Source))
                .Where(x => x.AdaptiveKind == AdaptiveKind.Audio).OrderByDescending(x => x.AudioBitrate).Select(x =>
                {
                    Console.WriteLine(x.Uri);
                    return x;
                }).First();
            return video.Uri;
        }

        if (UseYoutubeDL)
        {
            var prc = Process.Start(new ProcessStartInfo
            {
                FileName = "youtube-dl",
                Arguments = $@"--format best --get-url ""{track.Source}""",
                RedirectStandardOutput = true,
                UseShellExecute = false
            });
            prc.WaitForExit();
            return prc.StandardOutput.ReadToEnd();
        }

        if (track.Provider == StreamProvider.Http || track.Provider == StreamProvider.Local)
        {
            return track.Source;
        }

        return track.Source;
    }

    /// <summary>
    ///     Plays the specified <paramref name="track" /> asynchronously.
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
        startTime ??= track.Position;
        /*if(State != PlayerState.NotPlaying)
        {
            var reason = State switch
            {
                PlayerState.Playing => "is already playing something",
                PlayerState.Destroyed => "is destroyed",
                PlayerState.NotConnected => "is not connected",
                PlayerState.NotPlaying => "is not playing, WAIT NO WHAT",
                PlayerState.Paused => "is already playing something even though that thing is paused",
               _ => "is hacking or the code is terrribly out of date",
            };
            throw new InvalidOperationException($"Don't play something while the player {reason}");
        }*/
        var filePath = await GetURLAsync(track);
        await Task.Delay(5000);
        if (filePath != null)
        {
            FFmpeg = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments =
                    $@"-i ""{filePath}"" -err_detect ignore_err -ac 2 -f s16le {(startTime.HasValue ? "-ss " + startTime.Value.ToFFmpeg() : "")}{(endTime.HasValue ? " -to " + endTime.Value.ToFFmpeg() : "")} -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            });
            cancellationToken1 = new CancellationTokenSource();
            PlayerTask = Task.Run(async () =>
                await FFmpeg.StandardOutput.BaseStream.CopyToAsync(vnc.GetTransmitSink(),
                    cancellationToken: cancellationToken1.Token));
            FFmpeg.EnableRaisingEvents = true;
            FFmpeg.Exited += FFmpeg_Exited;
            State = PlayerState.Playing;
        }
    }

    private void FFmpeg_Exited(object sender, EventArgs e)
    {
        OnTrackEndAsync(this, TrackEndReason.Finished, CurrentTrack.TrackIdentifier).GetAwaiter().GetResult();
    }

    public virtual Task OnTrackEndAsync(BaseSnowPlayer baseSnow, TrackEndReason reason, string trackidentifier)
    {
        if (DisconnectOnStop)
        {
            return DisconnectAsync();
        }

        // The track ended, set to not playing
        State = PlayerState.NotPlaying;
        CurrentTrack = null;

        return Task.CompletedTask;
    }

    public virtual Task DisconnectAsync()
    {
        if (cancellationToken1 != null)
        {
            cancellationToken1.Cancel();
            cancellationToken1 = new CancellationTokenSource();
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
            cancellationToken1 = new CancellationTokenSource();
            PlayerTask = Task.Run(async () =>
                await FFmpeg.StandardOutput.BaseStream.CopyToAsync(vnc.GetTransmitSink(),
                    cancellationToken: cancellationToken1.Token));
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
                cancellationToken1 = new CancellationTokenSource();
            }

            if (!FFmpeg.HasExited)
            {
                FFmpeg.Kill();
            }

            State = PlayerState.NotPlaying;
        }
    }

    public Task PlayTopAsync(LavalinkTrack song)
    {
        throw new NotImplementedException();
    }

    internal async Task ConnectAsync(ulong voiceChannelId, bool selfDeaf, bool selfMute)
    {
        var channel = await VoiceNext.Client.GetChannelAsync(voiceChannelId);
        vnc = await channel.ConnectAsync();
        State = PlayerState.NotPlaying;
    }
}