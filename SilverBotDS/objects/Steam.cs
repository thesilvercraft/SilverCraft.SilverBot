using SteamStoreQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS
{
    internal class Steam
    {
        /// <summary>
        /// Search for something on steam
        /// </summary>
        /// <param name="searchQuery">The search query like "call of duty"</param>
        /// <returns>A list of listings</returns>
        public static List<Listing> Search(string searchQuery)
        {
            return Query.Search(searchQuery);
        }
    }
}