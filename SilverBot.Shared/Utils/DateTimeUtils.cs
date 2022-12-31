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