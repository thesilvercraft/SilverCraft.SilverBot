/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
namespace SilverBot.Shared.Utils;

public static class ArrayUtils
{
    public static T RandomFrom<T>(this T[] vs)
    {
        return vs.Length == 0
            ? throw new ArgumentOutOfRangeException(nameof(vs), "Array must not be empty")
            : vs[RandomGenerator.Next(0, vs.Length - 1)];
    }
}