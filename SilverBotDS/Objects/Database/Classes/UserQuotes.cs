using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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