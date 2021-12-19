using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SilverBotDS.Objects;

namespace SilverBotDS.Attributes;

public class RequireTranslatorAttribute : CheckBaseAttribute
{
    public RequireTranslatorAttribute(bool inchannel = false)
    {
        InChannel = inchannel;
    }

    private bool InChannel { get; }

    public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        var conf = (Config) ctx.CommandsNext.Services.GetService(typeof(Config));
        return await IsTranslator(conf, ctx.Client, ctx.User.Id, InChannel ? ctx.Channel.Id : null);
    }

    public static async Task<bool> IsTranslator(Config cnf, DiscordClient client, ulong userid, ulong? channelid = null)
    {
        return (await client.GetGuildAsync(cnf.ServerId)).Members.ContainsKey(userid)
               && (await client.GetGuildAsync(cnf.ServerId)).Members[userid].Roles
               .Any(x => x.Id == cnf.TranslatorRoleId)
               && (!channelid.HasValue || channelid == cnf.TranslatorModeChannel);
    }
}