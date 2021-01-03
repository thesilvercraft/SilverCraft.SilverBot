using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace codename_spearl
{
    internal class Game
    {
        public async Task<ushort> AddUserAsync(User user)
        {
            if (Users.Contains(user))
            {
                return Status.useralreadyjoined;
            }
            try
            {
                Users.Add(user);
                return Status.success;
            }
            catch
            {
                //i fucked up something ahhhhhhhh
                throw;
            }
        }

        private ConcurrentBag<User> Users { get; set; }
    }

    internal class Status
    {
        public static ushort faliure = 0;
        public static ushort success = 1;
        public static ushort useralreadyjoined = 2;
        public static ushort usernotin = 3;
        public static ushort nousers = 5;
    }
}