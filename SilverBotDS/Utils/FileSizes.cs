namespace SilverBotDS.Utils;

internal class FileSizes
{
    public static readonly FSize[] FSizes =
    {
        new("Byte", "B"), new("Kilobyte", "KB"), new("Megabyte", "MB"), /*Holy shit discord must have changed*/
        new("Gigabyte", "GB"), /*Uhh what is this onedrive or something*/ new("Terabyte", "TB"), new("Petabyte", "PB"),
        new("Exabyte", "EB"), new("Zettabyte", "ZB"), new("Yottabyte", "YB")
    };

    protected FileSizes()
    {
    }
}