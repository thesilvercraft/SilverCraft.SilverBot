using DSharpPlus.Entities;
using SilverBotDS.Utils;
using System.Collections.Generic;

namespace SilverBotDS.Objects
{
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

        public static Splash GetFromDiscordActivity(DiscordActivity discordActivity) => new()
        {
            Name = discordActivity.Name,
            Type = discordActivity.ActivityType,
            StreamUrl = discordActivity.StreamUrl
        };

        public DiscordActivity GetDiscordActivity(Dictionary<string, string> pairs) => new(StringUtils.FormatFromDictionary(Name, pairs), Type)
        {
            StreamUrl = StreamUrl
        };

        public string Name { get; set; }
        public ActivityType Type { get; set; }
        public string StreamUrl { get; set; }
    }
}