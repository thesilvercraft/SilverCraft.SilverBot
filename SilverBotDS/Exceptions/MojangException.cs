using System;

namespace SilverBotDS.Exceptions
{
    [Serializable]
    public class MojangException : Exception
    {
        public MojangException()
        {
        }

        public MojangException(string error, string errormessage)
            : base($"Mojang returned an error: {error} {errormessage}")
        {
        }
    }
}