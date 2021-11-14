using System;

namespace SilverBotDS.Utils
{
    internal static class ArrayUtils
    {
        public static T RandomFromArray<T>(T[] vs) => vs.Length == 0
                ? throw new ArgumentOutOfRangeException(nameof(vs), "Array must not be empty")
                : vs[RandomGenerator.Next(0, vs.Length - 1)];
    }
}