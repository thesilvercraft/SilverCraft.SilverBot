using LiteDB;

namespace SilverBotDS.Objects
{
    public class DbLang
    {
        /// <summary>
        /// The id for the server or user
        /// </summary>
        [BsonId(false)]
        public ulong DId { get; init; }

        /// <summary>
        /// The two (to four) letter code for the language
        /// </summary>
        public string Name { get; init; }
    }
}