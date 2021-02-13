using System;

namespace SilverBotDS.Utils
{
    internal class FileSizeUtils
    {
        public static string FormatSize(long bytes)
        {
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, FileSizes.FSizes[counter].Suffix);
        }
    }

    internal class FileSizes
    {
        public static readonly FSize[] FSizes = { new FSize("Byte", "B"), new FSize("Kilobyte", "KB"), new FSize("Megabyte", "MB"), /*Holy shit discord must have changed*/new FSize("Gigabyte", "GB"), /*Uhh what is this onedrive or something*/ new FSize("Terabyte", "TB"), new FSize("Petabyte", "PB"), new FSize("Exabyte", "EB"), new FSize("Zettabyte", "ZB"), new FSize("Yottabyte", "YB") };
    }

    internal class FSize
    {
        /// <summary>
        /// Make a thing
        /// </summary>
        /// <param name="FN">Full name</param>
        /// <param name="SFX">Suffix</param>
        /// <returns></returns>
        public FSize(string FN, string SFX)
        {
            FullName = FN;
            Suffix = SFX;
        }

        /// <summary>
        /// FullName like Byte
        /// </summary>
        public readonly string FullName;

        /// <summary>
        /// Suffix like B
        /// </summary>
        public readonly string Suffix;
    }
}