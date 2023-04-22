/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

namespace SilverBot.Shared.Utils
{
    internal class FSize
    {
        /// <summary>
        ///     FullName like Byte
        /// </summary>
        public readonly string FullName;

        /// <summary>
        ///     Suffix like B
        /// </summary>
        public readonly string Suffix;

        /// <summary>
        ///     Make a thing
        /// </summary>
        /// <param name="fn">Full name</param>
        /// <param name="sfx">Suffix</param>
        /// <returns></returns>
        public FSize(string fn, string sfx)
        {
            FullName = fn;
            Suffix = sfx;
        }
    }
}