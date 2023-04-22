/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using DSharpPlus.Entities;
using SilverBot.Shared.Utils;

namespace SilverBot.Shared.Objects.Classes
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

        public string Name { get; set; }
        public ActivityType Type { get; set; }
        public string StreamUrl { get; set; }

        public static Splash GetFromDiscordActivity(DiscordActivity discordActivity)
        {
            return new Splash
            {
                Name = discordActivity.Name,
                Type = discordActivity.ActivityType,
                StreamUrl = discordActivity.StreamUrl
            };
        }

        public DiscordActivity GetDiscordActivity(Dictionary<string, string> pairs)
        {
            return new DiscordActivity(StringUtils.FormatFromDictionary(Name, pairs), Type)
            {
                StreamUrl = StreamUrl
            };
        }
    }
}