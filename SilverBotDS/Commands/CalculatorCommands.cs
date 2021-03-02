using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;

namespace SilverBotDS.Commands
{
    internal class CalculatorCommands : BaseCommandModule
    {
        [Command("calculate")]
        [Description("Calculate a math expression using MathParser.org-mXparser")]
        [Aliases("calc", "🖩", "calculator")]
        public async Task Calculate(CommandContext ctx, [RemainingText] string input)
        {
            Expression e = new Expression(input);
            await OwnerOnly.SendBestRepresentationAsync(e.calculate(), ctx);
        }
    }
}