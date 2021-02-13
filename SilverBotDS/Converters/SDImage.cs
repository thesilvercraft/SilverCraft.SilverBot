using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SilverBotDS
{
    public class SDImage
    {
        public string URL;
        private byte[] bytes;

        public SDImage(string _URL) => URL = _URL;

        public static SDImage FromContext(CommandContext ctx)
        {
            return FromAttachments(ctx.Message.Attachments);
        }

        public static SDImage FromAttachments(IReadOnlyList<DiscordAttachment> attachments)
        {
            if (attachments.Count == 1)
            {
                return new SDImage(attachments[0].Url);
            }
            else
            {
                if (attachments.Count == 0)
                {
                    throw new AttachmentCountIncorrectException(AttachmentCountIncorrect.TooLittleAttachments);
                }
                else
                {
                    throw new AttachmentCountIncorrectException(AttachmentCountIncorrect.TooMuchAttachments);
                }
            }
        }

        public async Task<byte[]> GetBytesAsync()
        {
            if (bytes == null)
            {
                bytes = await WebClient.Get().GetByteArrayAsync(URL);
            }
            return bytes;
        }
    }

    public enum AttachmentCountIncorrect : byte
    {
        TooMuchAttachments,
        TooLittleAttachments
    }

    [Serializable]
    public class AttachmentCountIncorrectException : Exception
    {
        public AttachmentCountIncorrectException(AttachmentCountIncorrect count) : base() => attachmentCount = count;

        public AttachmentCountIncorrectException(AttachmentCountIncorrect count, string message) : base(message) => attachmentCount = count;

        public AttachmentCountIncorrectException(AttachmentCountIncorrect count, string message, Exception inner) : base(message, inner) => attachmentCount = count;

        public AttachmentCountIncorrect attachmentCount;

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected AttachmentCountIncorrectException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class SDImageConverter : IArgumentConverter<SDImage>
    {
        private readonly Regex expression = new Regex("http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\\(\\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+");

        public Task<Optional<SDImage>> ConvertAsync(string value, CommandContext ctx)
        {
            if (expression.IsMatch(value))
            {
                return Task.FromResult(Optional.FromValue(new SDImage(value)));
            }
            return Task.FromResult(Optional.FromNoValue<SDImage>());
        }
    }
}