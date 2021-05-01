using DSharpPlus.Entities;
using SilverBotDS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static Splash GetFromDiscordActivity(DiscordActivity discordActivity)
        {
            return new Splash
            {
                Name = discordActivity.Name,
                Type = discordActivity.ActivityType,
                StreamUrl = discordActivity.StreamUrl
            };
        }

        public DiscordActivity GetDiscordActivity(Dictionary<string, string> pairs) => new(StringUtils.FormatFromDictionary(Name, pairs), Type)
        {
            StreamUrl = StreamUrl
        };

        public string Name;
        public ActivityType Type;
        public string StreamUrl;
    }
}