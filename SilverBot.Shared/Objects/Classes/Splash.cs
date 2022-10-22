using DSharpPlus.Entities;
using SilverBotDS.Utils;

namespace SilverBotDS.Objects;

public class Splash
{
    public Splash()
    {
    }

    public Splash(string namewithparameters, ActivityType type)
    {
        Name = namewithparameters;
        Type = type;
    }

    public string Name { get; set; }
    public ActivityType Type { get; set; }
    public string StreamUrl { get; set; }

    public static Splash GetFromDiscordActivity(DiscordActivity discordActivity)
    {
        return new()
        {
            Name = discordActivity.Name,
            Type = discordActivity.ActivityType,
            StreamUrl = discordActivity.StreamUrl
        };
    }

    public DiscordActivity GetDiscordActivity(Dictionary<string, string> pairs)
    {
        return new(StringUtils.FormatFromDictionary(Name, pairs), Type)
        {
            StreamUrl = StreamUrl
        };
    }
}