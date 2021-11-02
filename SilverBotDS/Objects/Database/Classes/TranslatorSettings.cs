using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SilverBotDS.Objects.Database.Classes
{
    public class TranslatorSettings
    {
        public TranslatorSettings()
        {
            CustomLanguages = new List<Language>();
        }

        [Key]
        public ulong Id { get; init; }

        public bool IsTranslator { get; set; }

        public Language CurrentCustomLanguage { get; set; }
        public ICollection<Language> CustomLanguages { get; set; } = new List<Language>();
    }
}