using System.ComponentModel.DataAnnotations;

namespace SilverBotDS.Objects.Database.Classes;

public class TranslatorSettings
{
    [Key] public ulong Id { get; init; }

    public bool IsTranslator { get; set; }

    public Language CurrentCustomLanguage { get; set; }
    public ICollection<Language> CustomLanguages { get; set; } = new List<Language>();
}