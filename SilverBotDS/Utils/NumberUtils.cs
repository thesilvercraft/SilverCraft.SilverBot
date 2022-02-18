namespace SilverBotDS.Utils;

public static class NumberUtils
{
    private static readonly string[] Divisors =
    {
        "",
        "K", // Thousand
        "M", // Million
        "B", // Billion
        "T", // Trillion
        "Qa", // Quadrillion
        "Qi" // Quintillion
    };


    public static string FormatSize(this long bytes)
    {
        var counter = 0;
        var number = bytes;
        while (number / 1000 >= 1)
        {
            number /= 1000;
            counter++;
        }

        return $"{number}{Divisors[counter]}";
    }
}