using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SilverBotDS;

public interface IAnalyse
{
    Task EmitEvent(DiscordUser userId, string eventName, IDictionary<string, object> args);
}

public class ConsoleAnalytics : IAnalyse
{
    public Task EmitEvent(DiscordUser userId, string eventName, IDictionary<string, object> args)
    {
        //do nothing for now
        return Task.CompletedTask;
    }
}