using System.Text;

namespace SilverBotDS
{
    internal class stringutils
    {
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