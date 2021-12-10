using DSharpPlus.CommandsNext;
using System.Threading.Tasks;

namespace SilverBotDS.Objects.Classes
{
    public class SilverBotCommandModule: BaseCommandModule
    {
        public virtual Task<bool> ExecuteRequirements(Config conf)
        {
            return Task.FromResult(true);
        }
    }
    public interface IRequireFonts
    {
    }
}
