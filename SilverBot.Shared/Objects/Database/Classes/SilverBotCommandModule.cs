using DSharpPlus.CommandsNext;

namespace SilverBotDS.Objects.Classes;

public class SilverBotCommandModule : BaseCommandModule
{
    public virtual Task<bool> ExecuteRequirements(Config conf)
    {
        return Task.FromResult(true);
    }
}

public interface IRequireFonts
{
}