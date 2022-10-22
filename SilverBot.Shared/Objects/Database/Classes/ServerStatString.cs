using DSharpPlus.Entities;
using SilverBotDS.Utils;

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
        return Template.FormatFromDictionary(dict);
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