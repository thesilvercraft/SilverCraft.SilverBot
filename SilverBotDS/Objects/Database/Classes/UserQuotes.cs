using System;
using System.ComponentModel.DataAnnotations;

namespace SilverBotDS.Objects.Database.Classes
{
    public class UserQuote
    {
        /// <summary>
        /// A 6 character long string unique to the quote
        /// </summary>
        [Key]
        public string QuoteId { get; set; }

        public ulong UserId { get; set; }
        public string QuoteContent { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}