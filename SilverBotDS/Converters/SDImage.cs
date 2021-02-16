using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using SilverBotDS.Objects;

namespace SilverBotDS.Converters
{
    public class SdImage
    {
        public string Url;
        private byte[] _bytes;

        public SdImage(string url) => Url = url;

        public static SdImage FromContext(CommandContext ctx)
        {
            return FromAttachments(ctx.Message.Attachments);
        }

        public static SdImage FromAttachments(IReadOnlyList<DiscordAttachment> attachments)
        {
            if (attachments.Count == 1)
            {
                return new SdImage(attachments[0].Url);
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
            if (_bytes == null)
            {
                _bytes = await WebClient.Get().GetByteArrayAsync(Url);
            }
            return _bytes;
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
        public AttachmentCountIncorrectException(AttachmentCountIncorrect count) : base() => AttachmentCount = count;

        public AttachmentCountIncorrectException(AttachmentCountIncorrect count, string message) : base(message) => AttachmentCount = count;

        public AttachmentCountIncorrectException(AttachmentCountIncorrect count, string message, Exception inner) : base(message, inner) => AttachmentCount = count;

        public AttachmentCountIncorrect AttachmentCount;

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected AttachmentCountIncorrectException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class SdImageConverter : IArgumentConverter<SdImage>
    {
        private readonly Regex _expression = new Regex("http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\\(\\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+");

        public Task<Optional<SdImage>> ConvertAsync(string value, CommandContext ctx)
        {
            if (_expression.IsMatch(value))
            {
                return Task.FromResult(Optional.FromValue(new SdImage(value)));
            }
            return Task.FromResult(Optional.FromNoValue<SdImage>());
        }
    }
}