using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS.Utils
{
    internal static class ArrayUtils
    {
        public static T RandomFromArray<T>(T[] vs)
        {
            return vs.Length == 0
                ? throw new ArgumentOutOfRangeException(nameof(vs), "Array must not be empty")
                : vs[new RandomGenerator().Next(0, vs.Length - 1)];
        }
    }
}