/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
namespace SilverBot.Shared.Utils;

internal class FileSizes
{
    public static readonly FSize[] FSizes =
    {
        new("Byte", "B"), new("Kilobyte", "KB"), new("Megabyte", "MB"), /*Holy shit discord must have changed*/
        new("Gigabyte", "GB"), /*Uhh what is this onedrive or something*/ new("Terabyte", "TB"), new("Petabyte", "PB"),
        new("Exabyte", "EB"), new("Zettabyte", "ZB"), new("Yottabyte", "YB")
    };

    protected FileSizes()
    {
    }
}