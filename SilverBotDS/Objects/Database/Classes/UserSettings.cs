using System.ComponentModel.DataAnnotations;

namespace SilverBotDS.Objects
{
    public class UserSettings
    {
        /// <summary>
        /// The user id
        /// </summary>
        [Key]
        public ulong Id { get; init; }

        /// <summary>
        /// The two (to four) letter code for the language
        /// </summary>
        public string LangName { get; set; }

        public bool IsBanned { get; set; }
    }
}