namespace SilverBotDS.Utils
{
    internal class FileSizes
    {
        protected FileSizes()
        {
        }

        public static readonly FSize[] FSizes = { new FSize("Byte", "B"), new FSize("Kilobyte", "KB"), new FSize("Megabyte", "MB"), /*Holy shit discord must have changed*/new FSize("Gigabyte", "GB"), /*Uhh what is this onedrive or something*/ new FSize("Terabyte", "TB"), new FSize("Petabyte", "PB"), new FSize("Exabyte", "EB"), new FSize("Zettabyte", "ZB"), new FSize("Yottabyte", "YB") };
    }
}