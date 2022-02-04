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
        var gld = await client.GetGuildAsync(cnf.ServerId);
        return (!channelid.HasValue || channelid == cnf.TranslatorModeChannel) && gld.Members.ContainsKey(userid)
               && gld.Members[userid].Roles
               .Any(x => x.Id == cnf.TranslatorRoleId);
    }
}