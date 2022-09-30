using System;

namespace SilverBotDS.Utils;

public static class ArrayUtils
{
    public static T RandomFrom<T>(this T[] vs)
    {
        return vs.Length == 0
            ? throw new ArgumentOutOfRangeException(nameof(vs), "Array must not be empty")
            : vs[RandomGenerator.Next(0, vs.Length - 1)];
    }
}