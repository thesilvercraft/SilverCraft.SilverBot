using SilverBotDS.Objects.Database.Classes;
using SilverBotDS.Objects.Database.Classes.ReactionRole;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace SilverBotDS.Objects;

public class ServerSettings
{
    [Key] public Guid ServerSettingsId { get; set; }
    public ulong ServerId { get; set; }

    public string LangName { get; set; } = "en";
    public bool EmotesOptin { get; set; }
    public ulong? ServerStatsCategoryId { get; set; } = null;

    [NotMapped]
    public ServerStatString[] ServerStatsTemplates
    {
        get => JsonSerializer.Deserialize<ServerStatString[]>(string.IsNullOrWhiteSpace(ServerStatsTemplatesInJson)
            ? "[]"
            : ServerStatsTemplatesInJson);
        set => ServerStatsTemplatesInJson = JsonSerializer.Serialize(value);
    }

    public string ServerStatsTemplatesInJson { get; set; } = "[]";
    public bool RepeatThings { get; set; }
    public bool Reminders { get; set; }

    [NotMapped]
    public string[] Prefixes
    {
        get => JsonSerializer.Deserialize<string[]>(string.IsNullOrWhiteSpace(PrefixesInJson) ? "[]" : PrefixesInJson);
        set => PrefixesInJson = JsonSerializer.Serialize(value);
    }

    public string PrefixesInJson { get; set; } = "[]";
    public virtual List<ReactionRoleMapping> ReactionRoleMappings { get; set; } = new();
}