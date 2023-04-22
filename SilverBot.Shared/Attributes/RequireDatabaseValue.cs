/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.DependencyInjection;
using SilverBot.Shared.Objects.Database;
using SilverBot.Shared.Objects.Database.Classes;

namespace SilverBot.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequireModuleGuildEnabled : CheckBaseAttribute
    {
        public RequireModuleGuildEnabled(EnabledModules module, bool allowdms)
        {
            Module = module;
            AllowDirectMessages = allowdms;
        }

        public EnabledModules Module { get; }
        public bool AllowDirectMessages { get; }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (ctx.Guild == null)
            {
                return Task.FromResult(AllowDirectMessages);
            }

            using var dbCtx = ctx.Services.GetService<DatabaseContext>();
            var guildSettings = dbCtx?.serverSettings.Select(ServerSettings => new
            {
                ServerSettings.ServerId, ServerSettings.EnabledModules
            }).FirstOrDefault(x => x.ServerId == ctx.Guild.Id);
            if (guildSettings != null)
            {
                return Task.FromResult(guildSettings.EnabledModules.HasFlag(Module));
            }

            dbCtx?.GetServerSettings(ctx.Guild.Id);
            guildSettings = new
            {
                ServerId = ctx.Guild.Id,
                EnabledModules = EnabledModules.AllExceptReminders
            };
            return Task.FromResult(guildSettings.EnabledModules.HasFlag(Module));
        }
    }
}