/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

namespace SilverBot.Shared.Utils
{
    public static class NumberUtils
    {
        private static readonly string[] Divisors =
        {
            "",
            "K", // Thousand
            "M", // Million
            "B", // Billion
            "T", // Trillion
            "Qa", // Quadrillion
            "Qi" // Quintillion
        };

        public static string FormatSize(this long bytes)
        {
            var counter = 0;
            var number = bytes;
            while (number / 1000 >= 1)
            {
                number /= 1000;
                counter++;
            }

            return $"{number}{Divisors[counter]}";
        }
    }
}