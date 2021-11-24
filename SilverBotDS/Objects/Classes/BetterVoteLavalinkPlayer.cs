using DSharpPlus.Entities;
using Lavalink4NET.Events;
using Lavalink4NET.Player;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;

#nullable enable

namespace SilverBotDS.Objects.Classes
{
    public enum LoopSettings
    {
        [EnumMember(Value = "none")]
        NotLooping,

        [EnumMember(Value = "song")]
        LoopingSong,

        [EnumMember(Value = "queue")]
        LoopingQueue
    }

    public class BetterVoteLavalinkPlayer : VoteLavalinkPlayer
    {
        public LoopSettings LoopSettings { get; set; } = LoopSettings.NotLooping;
        public ulong LoopTimes { get; set; } = 0;
        public List<Tuple<LavalinkTrack, DateTime, bool>> QueueHistory { get; set; } = new();

        public override Task SkipAsync(int count = 1)
        {
            return SkipAsync(count, true);
        }

        public override Task<int> PlayAsync(LavalinkTrack track, bool enqueue, TimeSpan? startTime = null, TimeSpan? endTime = null, bool noReplace = false)
        {
            if (enqueue)
            {
                QueueHistory.Add(new(track, DateTime.UtcNow, false));
            }
            return base.PlayAsync(track, enqueue, startTime, endTime, noReplace);
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
                QueueHistory.Add(new(CurrentTrack, DateTime.UtcNow, false));
                return PlayAsync(CurrentTrack, false);
            }
            else if (!Queue.IsEmpty)
            {
                LavalinkTrack? track = null;
                while (count-- > 0)
                {
                    track = Queue.Dequeue();
                    QueueHistory.Add(new(track, DateTime.UtcNow, true));
                    if (LoopSettings is LoopSettings.LoopingQueue)
                    {
                        Queue.Add(track);
                    }
                }
                return PlayAsync(track!, false);
            }
            else
            {
                StopAsync();
            }
            return Task.CompletedTask;
        }

        public Dictionary<DiscordUser,List<Func<string,DiscordUser,bool>>> OnWebsiteEvent=new();

        public event EventHandler<TrackStartedEventArgs>? OnNewTrack;

        public void RemoveOnWebsiteEventHandelers(DiscordUser gaming)
        {
            OnWebsiteEvent.Remove(gaming);
        }

        public void TriggerWebsiteEvent(DiscordUser user, string action)
        {
            foreach (var eventhandler in OnWebsiteEvent.Values)
            {
                foreach (var eventh in eventhandler)
                {
                    eventh(action, user);
                }
            }
        }

        public override Task OnTrackStartedAsync(TrackStartedEventArgs eventArgs)
        {
            EventHandler<TrackStartedEventArgs>? handler = OnNewTrack;
            handler?.Invoke(this, eventArgs);
            return base.OnTrackStartedAsync(eventArgs);
        }

        public override Task OnTrackEndAsync(TrackEndEventArgs eventArgs)
        {
            if (eventArgs.MayStartNext)
            {
                return SkipAsync(1, false);
            }
            else
            {
                return Task.CompletedTask;
            }
        }
    }
}