/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace SilverBot.Shared.Utils;

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