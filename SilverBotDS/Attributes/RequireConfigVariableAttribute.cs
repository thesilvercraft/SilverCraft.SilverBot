using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SilverBotDS.Objects;
using System.Threading.Tasks;

namespace SilverBotDS.Attributes;

public class RequireConfigVariableAttribute : CheckBaseAttribute
{
    public RequireConfigVariableAttribute(string variable, object state)
    {
        Variable = variable;
        State = state;
    }

    private string Variable { get; }
    private object State { get; }

    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        var conf = (Config)ctx.CommandsNext.Services.GetService(typeof(Config));
        var compareval = typeof(Config).GetProperty(Variable).GetValue(conf);
        if (Equals(compareval, State))
        {
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}