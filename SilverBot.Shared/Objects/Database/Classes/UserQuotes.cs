/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using System.ComponentModel.DataAnnotations;

namespace SilverBotDS.Objects.Database.Classes;

public class UserQuote
{
    /// <summary>
    ///     A 6 character long string unique to the quote
    /// </summary>
    [Key]
    public string QuoteId { get; set; }

    public ulong UserId { get; set; }
    public string QuoteContent { get; set; }
    public DateTime TimeStamp { get; set; }
}