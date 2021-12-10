using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SilverBotDS.Objects;
using System.Threading.Tasks;

namespace SilverBotDS.Attributes
{
    public class RequireConfigVariableAttribute : CheckBaseAttribute
    {
        private string Variable { get; init; }
        private object State { get; init; }

        public RequireConfigVariableAttribute(string variable, object state)
        {
            Variable = variable;
            State = state;
        }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            Config conf = (Config)ctx.CommandsNext.Services.GetService(typeof(Config));
            var compareval = typeof(Config).GetProperty(Variable).GetValue(conf);
            if (Equals(compareval, State))
            {
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }
    }
}