using DSharpPlus.CommandsNext;

namespace SilverBotDS.Objects.Classes;

public interface IHaveExecutableRequirements 
{
    public Task<bool> ExecuteRequirements(Config conf);
}

public interface IRequireAssets
{
}