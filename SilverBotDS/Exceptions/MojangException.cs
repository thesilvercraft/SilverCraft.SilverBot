using System;

namespace SilverBotDS.Exceptions;

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

    public MojangException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
    }
}