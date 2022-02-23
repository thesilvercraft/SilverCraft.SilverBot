using DSharpPlus.Entities;
using SilverBotDS.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverBotDS.Objects.Database.Classes;

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

    public string Serialize(Dictionary<string, string> dict)
    {
        return StringUtils.FormatFromDictionary(Template, dict);
    }

    public static async Task<Dictionary<string, string>> GetStringDictionaryAsync(DiscordGuild guild)
    {
        var allmembers = await guild.GetAllMembersAsync();
        return new Dictionary<string, string>
        {
            ["AllMembersCount"] = NumberUtils.FormatSize(guild.MemberCount),
            ["MemberCount"] = NumberUtils.FormatSize(allmembers.LongCount(x => !x.IsBot)),
            ["BotsCount"] = NumberUtils.FormatSize(allmembers.LongCount(x => x.IsBot)),
            ["BoostCount"] = NumberUtils.FormatSize((int)guild.PremiumSubscriptionCount)
        };
    }
}