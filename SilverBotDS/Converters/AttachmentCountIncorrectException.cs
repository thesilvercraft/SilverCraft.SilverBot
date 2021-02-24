using System;

namespace SilverBotDS.Converters
{
    [Serializable]
    public class AttachmentCountIncorrectException : Exception
    {
        public AttachmentCountIncorrectException(AttachmentCountIncorrect count) : base() => AttachmentCount = count;

        public AttachmentCountIncorrectException(AttachmentCountIncorrect count, string message) : base(message) => AttachmentCount = count;

        public AttachmentCountIncorrectException(AttachmentCountIncorrect count, string message, Exception inner) : base(message, inner) => AttachmentCount = count;

        public AttachmentCountIncorrect AttachmentCount;

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected AttachmentCountIncorrectException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}