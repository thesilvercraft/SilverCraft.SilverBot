namespace SilverBotDS.Utils;

internal class NumberUtils
{
    private static readonly string[] divisors =
    {
        "",
        "K", // Thousand
        "M", // Million
        "B", // Billion
        "T", // Trillion
        "Qa", // Quadrillion
        "Qi" // Quintillion
    };

    protected NumberUtils()
    {
    }

    public static string FormatSize(long bytes)
    {
        var counter = 0;
        var number = bytes;
        while (number / 1000 >= 1)
        {
            number /= 1000;
            counter++;
        }

        return $"{number}{divisors[counter]}";
    }
}