/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;
using SilverBotDS.Objects;

namespace SilverBotDS.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequireGuildDatabaseValueAttribute : CheckBaseAttribute
{
    public RequireGuildDatabaseValueAttribute(string variable, object state, bool allowdms)
    {
        Variable = variable;
        State = state;
        AllowDirectMessages = allowdms;
    }

    private string Variable { get; }
    private object State { get; }
    private bool AllowDirectMessages { get; }

    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        if (ctx.Guild == null)
        {
            return Task.FromResult(AllowDirectMessages);
        }

        using var dbctx = (DatabaseContext)ctx.CommandsNext.Services.GetService(typeof(DatabaseContext));
        var guildsettings = dbctx.GetServerSettings(ctx.Guild.Id);
        var compareval = typeof(ServerSettings).GetProperty(Variable).GetValue(guildsettings);
        if (Equals(compareval, State))
        {
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequireGuildDatabaseValueSlashAttribute : SlashCheckBaseAttribute
{
    public RequireGuildDatabaseValueSlashAttribute(string variable, object state, bool allowdms)
    {
        Variable = variable;
        State = state;
        AllowDirectMessages = allowdms;
    }

    public string Variable { get; }
    public object State { get; }
    public bool AllowDirectMessages { get; }

    public override Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        if (ctx.Interaction.GuildId == null)
        {
            return Task.FromResult(AllowDirectMessages);
        }
        using var dbctx = (DatabaseContext)ctx.Services.GetService(typeof(DatabaseContext));
        var guildsettings = dbctx.GetServerSettings(ctx.Guild.Id);
        var compareval = typeof(ServerSettings).GetProperty(Variable).GetValue(guildsettings);
        if (Equals(compareval, State))
        {
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}