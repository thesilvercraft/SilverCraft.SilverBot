using System;
using System.Collections.Generic;
using System.Text;

namespace codename_spearl
{
    internal class User
    {
        /// <summary>
        /// The id of the person
        /// </summary>
        private ulong discord_id { get; set; }

        /// <summary>
        /// Count of wins
        /// </summary>
        private ulong wins { get; set; }

        //TODO implement it
        /// <summary>
        /// Not implemented but planned
        /// </summary>
        public timespan time_played { get; set; } = new timespan();
    }

    public class timespan
    {
        public int Days { get; set; } = 0;
        public int Hours { get; set; } = 0;
        public int Minutes { get; set; } = 0;
        public int Seconds { get; set; } = 0;
        public int MilliSeconds { get; set; } = 0;

        public TimeSpan ToTimeSpan()
        {
            return new TimeSpan(Days, Hours, Minutes, Seconds, MilliSeconds);
        }

        public static TimeSpan ToTimeSpan(timespan timespan)
        {
            return new TimeSpan(timespan.Days, timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.MilliSeconds);
        }
    }
}