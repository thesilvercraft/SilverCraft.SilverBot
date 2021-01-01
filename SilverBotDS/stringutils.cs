using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS
{
    class stringutils
    {

        public static string BoolToEmoteString(bool b)
        {
            if (b)
            
                return ":white_check_mark:";
            
            else
            
                return ":x:";
            
        }
    }
}
