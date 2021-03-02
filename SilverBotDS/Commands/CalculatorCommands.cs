using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;
using Jering.Javascript.NodeJS;
using System.Text;
using DSharpPlus.Entities;

namespace SilverBotDS.Commands
{
    internal partial class CalculatorCommands : BaseCommandModule
    {
        [Command("calculateparserold")]
        [Description("Calculate a math expression using MathParser.org-mXparser")]
        public async Task CalculateOld(CommandContext ctx, [RemainingText] string input)
        {
            Expression e = new Expression(input);
            await OwnerOnly.SendBestRepresentationAsync(e.calculate(), ctx);
        }

        private const string JSCODE = "module.exports = (callback, x) => { const mathsteps = require('mathsteps'); const steps = mathsteps.solveEquation(x); class MathStep { constructor(oldval,step, newval ) { this.oldval = oldval; this.step = step; this.newval = newval;  }} var mySteps = []; steps.forEach(step => {mySteps.push(new MathStep(step.oldEquation.ascii(), step.changeType, step.newEquation.ascii()));}); callback(null, mySteps);}";

        [Command("calc")]
        public async Task Calc(CommandContext ctx, [RemainingText] string input)
        {
            StringBuilder builder = new();
            foreach (MathStep step in await StaticNodeJSService.InvokeFromStringAsync<MathStep[]>(moduleString: JSCODE, args: new object[] { input }))
            {
                builder.AppendLine($"{step.OldVal} {step.Step} {step.NewVal}");
            }
            await new DiscordMessageBuilder().WithContent("Math steps" + " ```" + builder.ToString() + "```").SendAsync(ctx.Channel);
        }
    }
}