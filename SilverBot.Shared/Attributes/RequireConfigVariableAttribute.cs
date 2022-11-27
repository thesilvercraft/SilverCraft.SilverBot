/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SilverBotDS.Objects;

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