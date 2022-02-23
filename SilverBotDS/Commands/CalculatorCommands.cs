using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Jering.Javascript.NodeJS;
using SilverBotDS.Attributes;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS.Commands;

[Category("Calculator")]
public partial class CalculatorCommands : SilverBotCommandModule
{
    private const string Jscode =
        "module.exports = (callback, x) => { const mathsteps = require('mathsteps'); const steps = mathsteps.solveEquation(x); class MathStep { constructor(oldval,step, newval ) { this.oldval = oldval; this.step = step; this.newval = newval;  }} var mySteps = []; steps.forEach(step => {mySteps.push(new MathStep(step.oldEquation.ascii(), step.changeType, step.newEquation.ascii()));}); callback(null, mySteps);}";

    public override Task<bool> ExecuteRequirements(Config conf)
    {
        return Task.FromResult(conf.UseNodeJs);
    }

    [Command("calc")]
    public async Task Calc(CommandContext ctx, [RemainingText] string input)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        StringBuilder builder = new("```");
        foreach (var step in
                 await StaticNodeJSService.InvokeFromStringAsync<MathStep[]>(Jscode, args: new object[] { input }))
        {
            builder.Append(step.OldVal).Append(' ').Append(step.Step).Append(' ').AppendLine(step.NewVal);
        }
        builder.Append("```");
        await new DiscordMessageBuilder().WithContent(lang.MathSteps + builder).SendAsync(ctx.Channel);
    }
}