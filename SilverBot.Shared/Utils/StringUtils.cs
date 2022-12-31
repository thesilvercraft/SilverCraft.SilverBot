/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Text;

namespace SilverBot.Shared.Utils;

public static class StringUtils
{
    /// <summary>
    ///     A random string from an array of strings
    /// </summary>
    /// <param name="vs">the array</param>
    /// <returns>A random string from the array</returns>
    public static string RandomFromArray(this string[] vs)
    {
        return vs.RandomFrom();
    }

    public static string FormatFromDictionary(this string formatString, Dictionary<string, string> valueDict)
    {
        ulong i = 0;
        StringBuilder newFormatString = new(formatString);
        Dictionary<string, ulong> keyToInt = new();
        foreach (var tuple in valueDict.Select(tuple => tuple.Key))
        {
            newFormatString = newFormatString.Replace($"{{{tuple}}}", $"{{{i}}}");
            keyToInt.Add(tuple, i);
            i++;
        }

        return string.Format(newFormatString.ToString(),
            valueDict.OrderBy(x => keyToInt[x.Key]).Select(x => x.Value).ToArray());
    }

    /// <summary>
    ///     Split the string on a specified length
    /// </summary>
    /// <param name="s">The string</param>
    /// <param name="partLength">The length</param>
    /// <returns>An IEnumerable string containing the parts</returns>
    public static IEnumerable<string> SplitInParts(this string s, int partLength)
    {
        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }

        if (partLength <= 0)
        {
            throw new ArgumentException("Part length has to be positive.", nameof(partLength));
        }

        return SplitInPartsIterator(s, partLength);
    }

    private static IEnumerable<string> SplitInPartsIterator(this string s, int partLength)
    {
        for (var i = 0; i < s.Length; i += partLength)
        {
            yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }
    }

    /// <summary>
    ///     Converts a bool to a fancy discord emote
    /// </summary>
    /// <param name="b">The bool</param>
    /// <returns>A string containing the fancy discord emote</returns>
    public static string BoolToEmoteString(bool b)
    {
        return b ? ":white_check_mark:" : ":x:";
    }

    /// <summary>
    /// Removes a substring if it is found at the end
    /// </summary>
    /// <param name="a">the source string</param>
    /// <param name="sub">the substring</param>
    /// <returns>the original string with the substring removed if the substring was at the end</returns>
    public static string RemoveStringFromEnd(this string a, string sub)
    {
        if (!a.EndsWith(sub))
        {
            return a;
        }
        return a[..a.LastIndexOf(sub, StringComparison.Ordinal)];
    }
    /// <summary>
    /// Removes a substring if it is found at the start
    /// </summary>
    /// <param name="a">the source string</param>
    /// <param name="sub">the substring</param>
    /// <returns>the original string with the substring removed if the substring was at the start</returns>
    public static string RemoveStringFromStart(this string a, string sub)
    {
        if (!a.StartsWith(sub))
        {
            return a;
        }
        return a[sub.Length..];
    }
    /// <summary>
    ///     Converts an String[] to String by combining all the strings
    /// </summary>
    /// <param name="arr">The array to convert</param>
    /// <param name="seperator">The seperator to use if any</param>
    /// <returns>A string cotnaining the array</returns>
    public static string ArrayToString(this string[] arr, string seperator = "")
    {
        if (arr is null)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(seperator))
        {
            for (var e = 0; e < arr.Length; e++)
            {
                sb.Append(arr[e]);
                if (e != arr.Length - 1)
                {
                    sb.Append(seperator);
                }
            }
        }
        else
        {
            foreach (var item in arr)
            {
                sb.Append(item);
            }
        }

        return sb.ToString();
    }
}