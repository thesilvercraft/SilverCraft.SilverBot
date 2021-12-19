using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilverBotDS.Utils;

public static class StringUtils
{
    /// <summary>
    ///     A random string from an array of strings
    /// </summary>
    /// <param name="vs">the array</param>
    /// <returns>A random string from the array</returns>
    public static string RandomFromArray(string[] vs)
    {
        return ArrayUtils.RandomFromArray(vs);
    }

    public static string FormatFromDictionary(string formatString, Dictionary<string, string> valueDict)
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
    public static IEnumerable<string> SplitInParts(string s, int partLength)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));
        if (partLength <= 0) throw new ArgumentException("Part length has to be positive.", nameof(partLength));
        return SplitInPartsIterator(s, partLength);
    }

    private static IEnumerable<string> SplitInPartsIterator(string s, int partLength)
    {
        for (var i = 0; i < s.Length; i += partLength) yield return s.Substring(i, Math.Min(partLength, s.Length - i));
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
    ///     Converts an String[] to String by combining all the strings
    /// </summary>
    /// <param name="arr">The array to convert</param>
    /// <param name="seperator">The seperator to use if any</param>
    /// <returns>A string cotnaining the array</returns>
    public static string ArrayToString(string[] arr, string seperator = "")
    {
        if (arr is null) return string.Empty;
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(seperator))
            for (var e = 0; e < arr.Length; e++)
            {
                sb.Append(arr[e]);
                if (e != arr.Length - 1) sb.Append(seperator);
            }
        else
            foreach (var item in arr)
                sb.Append(item);

        return sb.ToString();
    }
}