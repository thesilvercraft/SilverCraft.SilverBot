using System;

namespace SilverBotDS.Exceptions
{
    [Serializable]
    public class DBLAException : Exception
    {
        public DBLAException()
        {
        }

        public DBLAException(string error)
            : base($"DBLA returned an error: {error}")
        {
        }
    }
}