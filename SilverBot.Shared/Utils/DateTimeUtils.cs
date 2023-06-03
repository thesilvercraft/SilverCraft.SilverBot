/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using DSharpPlus;

namespace SilverBot.Shared.Utils
{
    public static class DateTimeUtils
    {
        public static DateTime CalculateNextFriday()
        {
            var friday=DateTime.Now;
            if (friday.DayOfWeek >= DayOfWeek.Friday) //if magically we change to friday the exact millisecond this method is called, or if its saturday
            {
                friday = friday.AddDays(1);
            }
            return friday.AddDays(DayOfWeek.Friday-friday.DayOfWeek).AddHours(-friday.Hour).AddMinutes(-friday.Minute).AddSeconds(-friday.Second)
                .AddMilliseconds(-friday.Millisecond).AddMicroseconds(-friday.Microsecond);
        }
        public static DateTime CalculateNextSaturday()
        {
            var saturday=DateTime.Now;
            if (saturday.DayOfWeek == DayOfWeek.Saturday)
            {
               saturday= saturday.AddDays(1);
            }
            return saturday.AddDays(DayOfWeek.Saturday-saturday.DayOfWeek).AddHours(-saturday.Hour).AddMinutes(-saturday.Minute).AddSeconds(-saturday.Second)
                .AddMilliseconds(-saturday.Millisecond).AddMicroseconds(-saturday.Microsecond);
        }
        public static string DateTimeToTimeStamp(this DateTime? dt, TimestampFormat tf = TimestampFormat.RelativeTime,
            string def = "NA")
        {
            return dt != null ? DateTimeToTimeStamp((DateTime)dt, tf) : def;
        }

        public static string DateTimeToTimeStamp(this DateTime dt, TimestampFormat tf = TimestampFormat.RelativeTime)
        {
            return Formatter.Timestamp(dt, tf);
        }
    }
}