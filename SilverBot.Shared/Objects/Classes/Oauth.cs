/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Text.Json.Serialization;

namespace SilverBot.Shared.Objects.Classes;

public class Oauth
{
    public class Oauththingy
    {
        [JsonPropertyName("id")] public string Id { get; set; }

        public ulong UId => Convert.ToUInt64(Id);

        [JsonPropertyName("username")] public string Username { get; set; }

        [JsonPropertyName("avatar")] public string Avatar { get; set; }

        [JsonPropertyName("discriminator")] public string Discriminator { get; set; }

        [JsonPropertyName("public_flags")] public int Public_flags { get; set; }

        [JsonPropertyName("flags")] public int Flags { get; set; }

        [JsonPropertyName("email")] public string Email { get; set; }

        [JsonPropertyName("verified")] public bool Verified { get; set; }

        [JsonPropertyName("locale")] public string Locale { get; set; }

        [JsonPropertyName("mfa_enabled")] public bool Mfa_enabled { get; set; }
    }

    public class Token
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; }

        [JsonPropertyName("token_type")] public string TokenType { get; set; }

        [JsonPropertyName("expires_in")] public ulong ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; }

        [JsonPropertyName("scope")] public string Scope { get; set; }
    }

    public class Guild
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
}