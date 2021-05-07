namespace SilverBotDS.Utils
{
    internal class NumberUtils
    {
        protected NumberUtils()
        {
        }

        private static readonly string[] divisors = new string[]
        {
          "",
          "K", // Thousand
          "M", // Million
          "B", // Billion
          "T", // Trillion
          "Qa", // Quadrillion
          "Qi", // Quintillion
        };

        public static string FormatSize(long bytes)
        {
            var counter = 0;
            long number = bytes;
            while (number / 1000 >= 1)
            {
                number /= 1000;
                counter++;
            }
            return $"{number}{divisors[counter]}";
        }
    }
}