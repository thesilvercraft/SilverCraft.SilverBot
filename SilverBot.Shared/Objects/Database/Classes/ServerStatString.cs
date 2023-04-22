/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using DSharpPlus.Entities;
using SilverBot.Shared.Utils;

namespace SilverBot.Shared.Objects.Database.Classes
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
}