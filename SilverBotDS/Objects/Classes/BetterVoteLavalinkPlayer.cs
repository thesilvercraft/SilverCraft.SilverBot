using Lavalink4NET.Events;
using Lavalink4NET.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SilverBotDS.Objects.Classes
{
    internal enum VoteSettings
    {
        [EnumMember(Value = "none")]
        NotLooping,

        [EnumMember(Value = "song")]
        LoopingSong,

        [EnumMember(Value = "queue")]
        LoopingQueue
    }

    internal class BetterVoteLavalinkPlayer : VoteLavalinkPlayer
    {
        public VoteSettings LoopSettings { get; set; } = VoteSettings.NotLooping;

        public BetterVoteLavalinkPlayer()
        {
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
            if (!command && LoopSettings == VoteSettings.LoopingSong && CurrentTrack != null)
            {
                return PlayAsync(CurrentTrack, false);
            }
            else if (!Queue.IsEmpty)
            {
                LavalinkTrack? track = null;
                while (count-- > 0)
                {
                    track = Queue.Dequeue();
                    if (LoopSettings is VoteSettings.LoopingQueue)
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