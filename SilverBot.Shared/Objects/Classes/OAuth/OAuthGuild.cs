/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Text.Json.Serialization;

namespace SilverBot.Shared.Objects.Classes.OAuth;

public class OAuthGuild
    {
        [JsonPropertyName("id")] public string Id { get; set; }

        public ulong UId => Convert.ToUInt64(Id);

        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("icon")] public string Icon { get; set; }

        [JsonPropertyName("owner")] public bool Owner { get; set; }

        [JsonPropertyName("permissions")] public string Permissions { get; set; }

        [JsonPropertyName("features")]
        public string[] Features { get; set; } // should be removed if possbile violates CA1819
    }
