using DSharpPlus.CommandsNext;

namespace SilverBot.Shared
{
    public interface ISpecialModuleRegistration
    {
        public Task Register(CommandsNextExtension commandsNextExtension);
    }
}