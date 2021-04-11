using System;

namespace SilverBotDS.Converters
{
    [Serializable]
    public class AttachmentCountIncorrectException : Exception
    {
        public AttachmentCountIncorrectException(AttachmentCountIncorrect count) => SetAttachmentCount(count);

        public AttachmentCountIncorrectException(AttachmentCountIncorrect count, string message) : base(message) => SetAttachmentCount(count);

        public AttachmentCountIncorrectException(AttachmentCountIncorrect count, string message, Exception inner) : base(message, inner) => SetAttachmentCount(count);

        private AttachmentCountIncorrect attachmentCount;

        public AttachmentCountIncorrect AttachmentCount => attachmentCount;

        public void SetAttachmentCount(AttachmentCountIncorrect value)
        {
            attachmentCount = value;
        }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected AttachmentCountIncorrectException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}