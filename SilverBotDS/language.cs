using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS
{
    class language
    {
        public string Hi { get; set; } = "Hi {0}!";
        public string TimeInUtc { get; set; } = "Time in UTC is {0}";
        public string Timeformat { get; set; } = "G"; //https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings?redirectedfrom=MSDN
        public static language GetLanguageFromId(ulong id)
        {
            //TODO: IMPLEMENT THE THING LOL
            return new language();
        }
    }
}
