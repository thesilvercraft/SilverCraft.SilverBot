using System;

namespace SilverBotDS.Exceptions;

[Serializable]
public class PissException : Exception
{
    public PissException(int gallons)
        : base($"There {(gallons == 1 ? "is" : "are")} {gallons} {(gallons == 1 ? "gallon" : "gallons")} of piss.")
    {
    }
}