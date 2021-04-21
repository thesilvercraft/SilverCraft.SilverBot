using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace SilverBotDS.Utils
{
    public class RandomGenerator : IDisposable
    {
        private readonly RNGCryptoServiceProvider _csp;
        private bool disposedValue;

        public RandomGenerator() => _csp = new RNGCryptoServiceProvider();

        public int Next(int minValue, int maxExclusiveValue)
        {
            if (minValue >= maxExclusiveValue)
            {
                var argumentOutOfRangeException = new ArgumentOutOfRangeException(nameof(minValue), "minValue must be lower than maxExclusiveValue");
                throw argumentOutOfRangeException;
            }

            var diff = (long)maxExclusiveValue - minValue;
            var upperBound = uint.MaxValue / diff * diff;

            uint ui;
            do
            {
                ui = GetRandomUInt();
            } while (ui >= upperBound);
            return (int)(minValue + (ui % diff));
        }

        private uint GetRandomUInt()
        {
            var randomBytes = GenerateRandomBytes(sizeof(uint));
            return BitConverter.ToUInt32(randomBytes, 0);
        }

        private byte[] GenerateRandomBytes(int bytesNumber)
        {
            var buffer = new byte[bytesNumber];
            _csp.GetBytes(buffer);
            return buffer;
        }

        /// <summary>
        /// warning returns base64 might want to use RandomABCString
        /// </summary>
        /// <param name="length">how much bytes m8</param>
        /// <returns>a base64 thingy</returns>
        private string RandomString(int length)
        {
            var tokenData = GenerateRandomBytes(length);
            return Convert.ToBase64String(tokenData);
        }

        public string RandomAbcString(int length, double timespan = 1.5)
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _csp.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}