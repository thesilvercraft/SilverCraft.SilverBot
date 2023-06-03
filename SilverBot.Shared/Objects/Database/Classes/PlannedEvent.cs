/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.ComponentModel.DataAnnotations;

namespace SilverBot.Shared.Objects.Database.Classes
{
    public class PlannedEvent
    {
        public PlannedEvent()
        {
            Data ??= string.Empty;
        }
        /// <summary>
        ///     A random assortment of characters
        /// </summary>
        [Key]
        public string EventID { get; set; }

        /// <summary>
        ///     The time this event needs to trigger
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        ///     The type of the event. <br /> See: <see cref="PlannedEventType" />
        /// </summary>
        public PlannedEventType Type { get; set; }

        /// <summary>
        ///     The id of the user that requested for this event's creation.
        /// </summary>
        public ulong UserID { get; set; }

        /// <summary>
        ///     The channel where this event was requested to be created in.
        /// </summary>
        public ulong ChannelID { get; set; }

        /// <summary>
        ///     The message where this event was requested to be created in.
        /// </summary>
        public ulong MessageID { get; set; }

        /// <summary>
        ///     The id of the first response from the bot, optional.
        /// </summary>
        public ulong? ResponseMessageID { get; set; }

        /// <summary>
        /// A short string represting data related to this event
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        ///     A boolean that keeps track if something is finished, this should be set to true the millisecond something has been
        ///     finished and this class should be DELETED from the database.
        /// </summary>
        public bool Handled { get; set; }
    }

    public enum PlannedEventType
    {
        EmojiPoll,
        GiveAway,
        Reminder
    }
}