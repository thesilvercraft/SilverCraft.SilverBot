using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SilverBotDS.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverBotDS.Attributes
{
    public class RequireTranslatorAttribute : CheckBaseAttribute
    {
        private bool InChannel { get; set; }

        public RequireTranslatorAttribute(bool inchannel = false)
        {
            InChannel = inchannel;
        }

        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            Config conf = (Config)ctx.CommandsNext.Services.GetService(typeof(Config));
            return await IsTranslator(conf, ctx.Client, ctx.User.Id, InChannel ? ctx.Channel.Id : null);
        }

        public static async Task<bool> IsTranslator(Config cnf, DiscordClient client, ulong userid, ulong? channelid = null)
        {
            return (await client.GetGuildAsync(cnf.ServerId)).Members.ContainsKey(userid)
                   && (await client.GetGuildAsync(cnf.ServerId)).Members[userid].Roles.Any(x => x.Id == cnf.TranslatorRoleId)
                   && (!channelid.HasValue || (channelid == cnf.TranslatorModeChannel));
        }
    }
}