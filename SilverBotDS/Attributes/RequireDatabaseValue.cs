using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;
using SilverBotDS.Objects;
using System.Threading.Tasks;

namespace SilverBotDS.Attributes
{
    public class RequireGuildDatabaseValue : CheckBaseAttribute
    {
        private string Variable { get; init; }
        private object State { get; init; }
        private bool AllowDirectMessages { get; init; }

        public RequireGuildDatabaseValue(string variable, object state, bool allowdms)
        {
            Variable = variable;
            State = state;
            AllowDirectMessages = allowdms;
        }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            if (ctx.Guild == null)
            {
                return Task.FromResult(AllowDirectMessages);
            }
            DatabaseContext dbctx = (DatabaseContext)ctx.CommandsNext.Services.GetService(typeof(DatabaseContext));
            var guildsettings = dbctx.GetServerSettings(ctx.Guild.Id);
            var compareval = typeof(ServerSettings).GetProperty(Variable).GetValue(guildsettings);
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

    public class RequireGuildDatabaseValueSlash : SlashCheckBaseAttribute
    {
        private string Variable { get; init; }
        private object State { get; init; }
        private bool AllowDirectMessages { get; init; }

        public RequireGuildDatabaseValueSlash(string variable, object state, bool allowdms)
        {
            Variable = variable;
            State = state;
            AllowDirectMessages = allowdms;
        }

        public override Task<bool> ExecuteChecksAsync(InteractionContext ctx)
        {
            if (ctx.Guild == null)
            {
                return Task.FromResult(AllowDirectMessages);
            }
            DatabaseContext dbctx = (DatabaseContext)ctx.Services.GetService(typeof(DatabaseContext));
            var guildsettings = dbctx.GetServerSettings(ctx.Guild.Id);
            var compareval = typeof(ServerSettings).GetProperty(Variable).GetValue(guildsettings);
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