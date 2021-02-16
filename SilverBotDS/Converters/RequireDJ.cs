using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS.Converters
{
    public class RequireDJAttribute : CheckBaseAttribute
    {
        public RequireDJAttribute()
        {
        }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => Task.FromResult(ctx.Member.Roles.Any(e => e.CheckPermission(DSharpPlus.Permissions.ManageChannels) == DSharpPlus.PermissionLevel.Allowed || e.Name.ToLower().Contains("dj")));
    }
}