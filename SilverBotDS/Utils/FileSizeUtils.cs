using System;

namespace SilverBotDS.Utils;

public static class FileSizeUtils
{
    public static string FormatSize(this long bytes)
    {
        var counter = 0;
        decimal number = bytes;
        while (Math.Round(number / 1024) >= 1)
        {
            number /= 1024;
            counter++;
        }

        return string.Format("{0:n1}{1}", number, FileSizes.FSizes[counter].Suffix);
    }
}