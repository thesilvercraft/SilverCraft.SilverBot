using DSharpPlus.Entities;
using Segment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SilverBotDS
{
    public interface IAnalyse
    {
        Task EmitEvent(DiscordUser UserId, string EventName, IDictionary<string, object> Args);
    }

    public class SegmentIo : IAnalyse
    {
        public SegmentIo(string token)
        {
            Analytics.Initialize(token);
        }

        public Task EmitEvent(DiscordUser User, string EventName, IDictionary<string, object> Args)
        {
            Analytics.Client.Track(User.Id.ToString(), EventName, Args);
            return Task.CompletedTask;
        }
    }
}