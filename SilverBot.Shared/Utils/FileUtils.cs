/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

namespace SilverBot.Shared.Utils
{
    public static class FileUtils
    {
        /// <summary>
        ///     https://stackoverflow.com/questions/23228378/is-there-any-way-to-get-the-file-extension-from-a-url
        /// </summary>
        /// <param name="url">the url</param>
        /// <returns>the extension</returns>
        public static string GetFileExtensionFromUrl(this string url)
        {
            url = url.Split('?')[0];
            url = url.Split('/').Last();
            return url.Contains('.') ? url[url.LastIndexOf('.')..] : "";
        }
    }
}