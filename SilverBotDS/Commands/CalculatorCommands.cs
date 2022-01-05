using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Jering.Javascript.NodeJS;
using org.mariuszgromada.math.mxparser;
using SilverBotDS.Attributes;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;

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

    [Command("calculateparserold")]
    [Description("Calculate a math expression using MathParser.org-mXparser")]
    public async Task CalculateOld(CommandContext ctx, [RemainingText] string input)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await new DiscordMessageBuilder()
            .WithContent($"{lang.MathSteps} ```json\n{new Expression(input).calculate()}```").SendAsync(ctx.Channel);
    }

    [Command("calc")]
    public async Task Calc(CommandContext ctx, [RemainingText] string input)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        StringBuilder builder = new("```");
        foreach (var step in
                 await StaticNodeJSService.InvokeFromStringAsync<MathStep[]>(Jscode, args: new object[] {input}))
        {
            builder.Append(step.OldVal).Append(' ').Append(step.Step).Append(' ').AppendLine(step.NewVal);
        }

        if (string.IsNullOrEmpty(builder.ToString()) || builder.ToString() == "```")
        {
            await CalculateOld(ctx, input);
            return;
        }

        builder.Append("```");
        await new DiscordMessageBuilder().WithContent(lang.MathSteps + builder).SendAsync(ctx.Channel);
    }
}