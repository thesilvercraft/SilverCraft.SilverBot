using DSharpPlus.Entities;
using SilverBotDS.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SilverBotDS.Objects.Database.Classes
{
    public class ServerStatString
    {
        public ServerStatString()
        {
        }

        public ServerStatString(string template)
        {
            Template = template;
        }

       

        public string Template { get; set; }

        public string Serialize(DiscordGuild guild)
        {
            return StringUtils.FormatFromDictionary(Template, GetStringDictionary(guild));
        }

        public static Dictionary<string, string> GetStringDictionary(DiscordGuild guild)
        {
            return new Dictionary<string, string>
            {
                ["AllMembersCount"] = NumberUtils.FormatSize(guild.MemberCount),
                ["MemberCount"] = NumberUtils.FormatSize(guild.Members.LongCount(x => !x.Value.IsBot)),
                ["BotsCount"] = NumberUtils.FormatSize(guild.Members.LongCount(x => x.Value.IsBot)),
                ["BoostCount"] = NumberUtils.FormatSize((int)guild.PremiumSubscriptionCount)
            };
        }
    }
}