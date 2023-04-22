/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

#nullable enable

using System.Runtime.Serialization;
using DSharpPlus.Entities;
using Lavalink4NET.Events;
using Lavalink4NET.Player;

namespace SilverBot.Shared.Objects.Classes
{
    public enum LoopSettings
    {
        [EnumMember(Value = "none")] NotLooping,

        [EnumMember(Value = "song")] LoopingSong,

        [EnumMember(Value = "queue")] LoopingQueue
    }

    public enum FinishReason
    {
        NotFinishedYet,
        TrackEnd,
        Skipped,
        FullySkipped
    }

    public class QueueHistoryPlayback
    {
        public DateTime? TimeStartedPlaying { get; set; }
        public DateTime? TimeFinishedPlaying { get; set; }
        public FinishReason ReasonFinished { get; set; }
    }

    public class QueueHistoryObject
    {
        public LavalinkTrack Track { get; set; }
        public DateTime TimeAdded { get; set; }
        public List<QueueHistoryPlayback> Playbacks { get; set; } = new();
    }

    public enum ChangeType
    {
        QueueChange,
        CurrentSongChange,
        TrackEnd,
        CurrentStateChangePause,
        CurrentStateChangeResume
    }

    public class BetterPlayerEventData
    {
        public ChangeType ChangeType;
    }

    public class BetterVoteLavalinkPlayer : VoteLavalinkPlayer
    {
        public Dictionary<DiscordUser, List<Func<string, DiscordUser, bool>>> OnWebsiteEvent = new();
        public LoopSettings LoopSettings { get; set; } = LoopSettings.NotLooping;
        public EventHandler<BetterPlayerEventData> OnChangedState;
        public ulong LoopTimes { get; set; } = 0;
        public Dictionary<LavalinkTrack, QueueHistoryObject> QueueHistory { get; set; } = new();

        public override Task<int> PlayAsync(LavalinkTrack track, bool enqueue, TimeSpan? startTime = null,
            TimeSpan? endTime = null, bool noReplace = false)
        {
            if (enqueue)
            {
                OnChangedState?.Invoke(this, new BetterPlayerEventData() { ChangeType = ChangeType.QueueChange });
                QueueHistory.Add(track, new QueueHistoryObject() { Track = track, TimeAdded = DateTime.UtcNow });
            }
            else if (QueueHistory.TryGetValue(track, out var queueHistoryObject))
            {
                queueHistoryObject.Playbacks.Add(new QueueHistoryPlayback
                    { ReasonFinished = FinishReason.NotFinishedYet, TimeStartedPlaying = DateTime.UtcNow });
            }

            return base.PlayAsync(track, enqueue, startTime, endTime, noReplace);
        }

        public override Task PauseAsync()
        {
            OnChangedState?.Invoke(this,
                new BetterPlayerEventData() { ChangeType = ChangeType.CurrentStateChangePause });
            return base.PauseAsync();
        }

        public override Task ResumeAsync()
        {
            OnChangedState?.Invoke(this,
                new BetterPlayerEventData() { ChangeType = ChangeType.CurrentStateChangeResume });
            return base.ResumeAsync();
        }

        public override Task SkipAsync(int count = 1)
        {
            return SkipAsync(count, true);
        }

        public Task SkipAsync(int count, bool command)
        {
            if (count <= 0)
            {
                return Task.CompletedTask;
            }

            EnsureNotDestroyed();
            EnsureConnected();
            if (!command && LoopSettings == LoopSettings.LoopingSong && CurrentTrack != null)
            {
                if (QueueHistory.TryGetValue(CurrentTrack, out var queueHistoryObject) &&
                    queueHistoryObject.Playbacks.Count > 0)
                {
                    queueHistoryObject.Playbacks.Last().TimeFinishedPlaying = DateTime.UtcNow;
                    queueHistoryObject.Playbacks.Add(new QueueHistoryPlayback
                        { ReasonFinished = FinishReason.NotFinishedYet, TimeStartedPlaying = DateTime.UtcNow });
                }

                LoopTimes++;
                return PlayAsync(CurrentTrack, false);
            }

            if (!Queue.IsEmpty)
            {
                LoopTimes = 0;
                LavalinkTrack? track = null;
                while (count-- > 0)
                {
                    track = Queue.Dequeue();
                    if (QueueHistory.TryGetValue(track, out var queueHistoryObject) &&
                        queueHistoryObject.Playbacks.Count > 0)
                    {
                        if (count > 0)
                        {
                            queueHistoryObject.Playbacks.Add(new QueueHistoryPlayback
                            {
                                TimeStartedPlaying = DateTime.UtcNow, TimeFinishedPlaying = DateTime.UtcNow,
                                ReasonFinished = FinishReason.FullySkipped
                            });
                        }
                        else
                        {
                            queueHistoryObject.Playbacks.Add(new QueueHistoryPlayback
                            {
                                TimeStartedPlaying = DateTime.UtcNow,
                                ReasonFinished = FinishReason.NotFinishedYet
                            });
                        }
                    }

                    if (LoopSettings is LoopSettings.LoopingQueue)
                    {
                        Queue.Add(track);
                    }
                }

                return PlayAsync(track!, false);
            }

            StopAsync();
            return Task.CompletedTask;
        }

        public event EventHandler<TrackStartedEventArgs>? OnNewTrack;

        public void RemoveOnWebsiteEventHandelers(DiscordUser gaming)
        {
            OnWebsiteEvent.Remove(gaming);
        }

        public void TriggerWebsiteEvent(DiscordUser user, string action)
        {
            foreach (var eventh in OnWebsiteEvent.Values.SelectMany(eventhandler => eventhandler))
            {
                eventh(action, user);
            }
        }

        public override Task OnTrackStartedAsync(TrackStartedEventArgs eventArgs)
        {
            var handler = OnNewTrack;
            handler?.Invoke(this, eventArgs);
            OnChangedState?.Invoke(this, new BetterPlayerEventData() { ChangeType = ChangeType.CurrentSongChange });
            return base.OnTrackStartedAsync(eventArgs);
        }

        public override Task OnTrackEndAsync(TrackEndEventArgs eventArgs)
        {
            OnChangedState?.Invoke(this, new BetterPlayerEventData() { ChangeType = ChangeType.TrackEnd });
            return eventArgs.MayStartNext ? SkipAsync(1, false) : Task.CompletedTask;
        }
    }
}