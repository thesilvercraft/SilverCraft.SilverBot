using System.Text.Json.Serialization;

namespace SilverBot.Shared.Objects.Classes.OAuth
{
    public class OAuthUser
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
}