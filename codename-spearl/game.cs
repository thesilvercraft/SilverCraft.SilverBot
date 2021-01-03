using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codename_spearl
{
    internal class game
    {
        private async Task<ushort> addUserAsync(User user)
        {
            if (users.Contains(user))
            {
                return status.useralreadyjoined;
            }
            try
            {
                users.Add(user);
                return status.success;
            }
            catch
            {
                //i fucked up something ahhhhhhhh
                throw;
            }
        }

        private ConcurrentBag<User> users { get; set; }
    }

    internal class status
    {
        public static ushort faliure = 0;
        public static ushort success = 1;
        public static ushort useralreadyjoined = 2;
        public static ushort usernotin = 3;
    }
}