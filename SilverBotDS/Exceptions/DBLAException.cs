using System;

namespace SilverBotDS.Exceptions
{
    [Serializable]
    public class DblaException : Exception
    {
        public DblaException()
        {
        }

        public DblaException(string error)
            : base($"DBLA returned an error: {error}")
        {
        }
    }
}