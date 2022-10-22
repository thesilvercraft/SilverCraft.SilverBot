using DSharpPlus;

namespace SilverBotDS.Utils
{
    public static class DateTimeUtils
    {
        public static string DateTimeToTimeStamp(this DateTime? dt, TimestampFormat tf = TimestampFormat.RelativeTime, string def = "NA")
        {
            if (dt != null)
            {
                return DateTimeToTimeStamp((DateTime)dt, tf);
            }
            else
            {
                return def;
            }
        }

        public static string DateTimeToTimeStamp(this DateTime dt, TimestampFormat tf = TimestampFormat.RelativeTime)
        {
            return Formatter.Timestamp(dt, tf);
        }
    }
}