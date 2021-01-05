using System;
using System.Collections.Generic;
using System.Text;

namespace SilverBotDS
{
    internal class Stringutils
    {
        /// <summary>
        /// A random string from an array of strings
        /// </summary>
        /// <param name="vs">the array</param>
        /// <returns>A random string from the array</returns>
        public static string RandomFromArray(string[] vs)
        {
            RandomGenerator randomGenerator = new RandomGenerator();
            if (vs.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(vs.Length), "Array lenght must be above 0");
            }
            return vs[randomGenerator.Next(0, vs.Length - 1)];
        }

        /// <summary>
        /// Split the string on a specified lenght
        /// </summary>
        /// <param name="s">The string</param>
        /// <param name="partLength">The lenght</param>
        /// <returns>An IEnumerable<string> containing the parts</returns>
        public static IEnumerable<string> SplitInParts(string s, int partLength)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            if (partLength <= 0)
            {
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));
            }

            for (int i = 0; i < s.Length; i += partLength)
            {
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
            }
        }

        /// <summary>
        /// Converts a bool to a fancy discord emote
        /// </summary>
        /// <param name="b">The bool</param>
        /// <returns>A string containing the fancy discord emote</returns>
        public static string BoolToEmoteString(bool b)
        {
            if (b)
            {
                return ":white_check_mark:";
            }
            else
            {
                return ":x:";
            }
        }

        /// <summary>
        /// Converts an String[] to String by combining all the strings
        /// </summary>
        /// <param name="arr">The array to convert</param>
        /// <param name="seperator">The seperator to use if any</param>
        /// <returns>A string cotnaining the array</returns>
        public static string ArrayToString(string[] arr, string seperator = "")
        {
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrEmpty(seperator))
            {
                foreach (var item in arr)
                {
                    sb.Append(item);
                }
            }
            else
            {
                int e = 0;
                foreach (var item in arr)
                {
                    e++;
                    sb.Append(item);
                    if (e != arr.Length)
                    {
                        sb.Append(seperator);
                    }
                }
            }
            return sb.ToString();
        }
    }
}