using SilverBotDS.Converters;
using System;
using System.Runtime.Serialization;

namespace SilverBotDS.Exceptions;

[Serializable]
public class AttachmentCountIncorrectException : Exception
{
    public AttachmentCountIncorrectException(AttachmentCountIncorrect count)
    {
        SetAttachmentCount(count);
    }

    public AttachmentCountIncorrectException(AttachmentCountIncorrect count, string message) : base(message)
    {
        SetAttachmentCount(count);
    }

    public AttachmentCountIncorrectException(AttachmentCountIncorrect count, string message, Exception inner) : base(
        message, inner)
    {
        SetAttachmentCount(count);
    }

    // A constructor is needed for serialization when an
    // exception propagates from a remoting server to the client.
    protected AttachmentCountIncorrectException(SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }

    public AttachmentCountIncorrect AttachmentCount { get; private set; }

    public void SetAttachmentCount(AttachmentCountIncorrect value)
    {
        AttachmentCount = value;
    }
}