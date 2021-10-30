using System;
using System.ComponentModel.DataAnnotations;

namespace SilverBotDS.Objects.Database.Classes
{
    public class PlannedEvent
    {
        /// <summary>
        /// A random assortment of characters
        /// </summary>
        [Key]
        public string EventID { get; set; }

        /// <summary>
        /// The time this event needs to trigger
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// The type of the event. <br/> See: <see cref="PlannedEventType"/>
        /// </summary>
        public PlannedEventType Type { get; set; }

        /// <summary>
        /// The id of the user that requested for this event's creation.
        /// </summary>
        public ulong UserID { get; set; }

        /// <summary>
        /// The channel where this event was requested to be created in.
        /// </summary>
        public ulong ChannelID { get; set; }

        /// <summary>
        /// The message where this event was requested to be created in.
        /// </summary>
        public ulong MessageID { get; set; }

        /// <summary>
        /// The id of the first response from the bot, optional.
        /// </summary>
        public ulong? ResponseMessageID { get; set; }

        /// <summary>
        /// A boolean that keeps track if something is finished, this should be set to true the millisecond something has been finished and this class should be DELETED from the database.
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