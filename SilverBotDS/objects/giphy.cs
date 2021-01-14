using GiphyDotNet.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS
{
    internal class GiphyO
    {
        private static Giphy giphy = new Giphy();

        /// <summary>
        /// sets the giphy to the specified object
        /// </summary>
        /// <param name="x">the object to set it to</param>
        public static void Set(Giphy x)
        {
            giphy = x;
        }

        /// <summary>
        /// Get the giphy
        /// </summary>
        /// <returns>The giphy</returns>
        public static Giphy Get()
        {
            return giphy;
        }
    }
}