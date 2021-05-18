using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SilverBotDS.Objects.Classes
{
    public class Oauth
    {
        public class Oauththingy
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            public ulong UId { get => Convert.ToUInt64(Id); }

            [JsonPropertyName("username")]
            public string Username { get; set; }

            [JsonPropertyName("avatar")]
            public string Avatar { get; set; }

            [JsonPropertyName("discriminator")]
            public string Discriminator { get; set; }

            [JsonPropertyName("public_flags")]
            public int Public_flags { get; set; }

            [JsonPropertyName("flags")]
            public int Flags { get; set; }

            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("verified")]
            public bool Verified { get; set; }

            [JsonPropertyName("locale")]
            public string Locale { get; set; }

            [JsonPropertyName("mfa_enabled")]
            public bool Mfa_enabled { get; set; }
        }

        public class Token
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }

            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }

            [JsonPropertyName("expires_in")]
            public ulong ExpiresIn { get; set; }

            [JsonPropertyName("refresh_token")]
            public string RefreshToken { get; set; }

            [JsonPropertyName("scope")]
            public string Scope { get; set; }
        }

        public class Guild
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            public ulong UId { get => Convert.ToUInt64(Id); }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("icon")]
            public string Icon { get; set; }

            [JsonPropertyName("owner")]
            public bool Owner { get; set; }

            [JsonPropertyName("permissions")]
            public string Permissions { get; set; }

            [JsonPropertyName("features")]
#pragma warning disable CA1819 // Properties should not return arrays
            public string[] Features { get; set; } // should be removed if possbile violates CA1819

#pragma warning restore CA1819 // Properties should not return arrays
        }
    }
}