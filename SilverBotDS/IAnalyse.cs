using DSharpPlus.Entities;
using Segment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SilverBotDS;

public interface IAnalyse
{
    Task EmitEvent(DiscordUser userId, string eventName, IDictionary<string, object> args);
}

public class SegmentIo : IAnalyse
{
    public SegmentIo(string token)
    {
        Analytics.Initialize(token);
    }

    public Task EmitEvent(DiscordUser userId, string eventName, IDictionary<string, object> args)
    {
        Analytics.Client.Track(userId.Id.ToString(), eventName, args);
        return Task.CompletedTask;
    }
}