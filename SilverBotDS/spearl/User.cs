using System;

namespace codename_spearl
{
    internal class User
    {
        /// <summary>
        /// The id of the person
        /// </summary>
        public ulong Discord_id { get; set; }

        /// <summary>
        /// Count of wins
        /// </summary>
        public ulong Wins { get; set; }

        //TODO implement it
        /// <summary>
        /// Not implemented but planned
        /// </summary>
        public Timespan Time_played { get; set; } = new Timespan();
    }

    public class Timespan
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

        public static TimeSpan ToTimeSpan(Timespan timespan)
        {
            return new TimeSpan(timespan.Days, timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.MilliSeconds);
        }
    }
}