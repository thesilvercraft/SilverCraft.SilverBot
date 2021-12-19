using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using SilverBotDS.Objects.Database.Classes;

namespace SilverBotDS.Objects;

public class ServerSettings
{
    public ServerSettings()
    {
        LangName = "en";
        EmotesOptin = false;
        ServerStatsCategoryId = null;
        RepeatThings = false;
        WebShot = false;
        Prefixes = Array.Empty<string>();
    }

    [Key] public ulong ServerId { get; init; }

    public string LangName { get; set; }
    public bool EmotesOptin { get; set; }
    public ulong? ServerStatsCategoryId { get; set; }

    [NotMapped]
    public ServerStatString[] ServerStatsTemplates
    {
        get => JsonSerializer.Deserialize<ServerStatString[]>(string.IsNullOrWhiteSpace(ServerStatsTemplatesInJson)
            ? "[]"
            : ServerStatsTemplatesInJson);
        set => ServerStatsTemplatesInJson = JsonSerializer.Serialize(value);
    }

    public string ServerStatsTemplatesInJson { get; set; }
    public bool RepeatThings { get; set; }
    public bool WebShot { get; set; }

    [NotMapped]
    public string[] Prefixes
    {
        get => JsonSerializer.Deserialize<string[]>(string.IsNullOrWhiteSpace(PrefixesInJson) ? "[]" : PrefixesInJson);
        set => PrefixesInJson = JsonSerializer.Serialize(value);
    }

    public string PrefixesInJson { get; set; }
}