using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace SilverBotDS.Utils;

public static class RandomGenerator
{
    public static int Next(int minValue, int maxExclusiveValue)
    {
        if (minValue < maxExclusiveValue)
        {
            return RandomNumberGenerator.GetInt32(minValue, maxExclusiveValue);
        }

        var argumentOutOfRangeException =
            new ArgumentOutOfRangeException(nameof(minValue), "minValue must be lower than maxExclusiveValue");
        throw argumentOutOfRangeException;
    }

    public static uint GetRandomUInt()
    {
        var randomBytes = GenerateRandomBytes(sizeof(uint));
        return BitConverter.ToUInt32(randomBytes, 0);
    }

    public static byte[] GenerateRandomBytes(int bytesNumber)
    {
        return RandomNumberGenerator.GetBytes(bytesNumber);
    }

    /// <summary>
    ///     warning returns base64 might want to use RandomABCString
    /// </summary>
    /// <param name="length">how much bytes m8</param>
    /// <returns>a base64 thingy</returns>
    public static string RandomString(int length)
    {
        var tokenData = GenerateRandomBytes(length);
        return Convert.ToBase64String(tokenData);
    }

    public static string RandomAbcString(int length, double timespan = 1.5)
    {
        try
        {
            return Regex.Replace(RandomString(length), @"[^\w]", string.Empty,
                RegexOptions.None, TimeSpan.FromSeconds(timespan));
        }
        // If we timeout when replacing invalid characters,
        // we should return Empty.
        catch (RegexMatchTimeoutException)
        {
            return string.Empty;
        }
    }
}