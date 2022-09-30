namespace SilverBotDS.Utils;

internal class FSize
{
    /// <summary>
    ///     FullName like Byte
    /// </summary>
    public readonly string FullName;

    /// <summary>
    ///     Suffix like B
    /// </summary>
    public readonly string Suffix;

    /// <summary>
    ///     Make a thing
    /// </summary>
    /// <param name="fn">Full name</param>
    /// <param name="sfx">Suffix</param>
    /// <returns></returns>
    public FSize(string fn, string sfx)
    {
        FullName = fn;
        Suffix = sfx;
    }
}