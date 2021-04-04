using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using SilverBotDS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SilverBotDS.Converters
{
    public class SdImage : IDisposable
    {
        public string Url;
        private byte[] _bytes;
        private bool disposedValue;
        public SdImage()
            {
            }
        public SdImage(string url) => Url = url;

        public static SdImage FromContext(CommandContext ctx)
        {
            var img = FromAttachments(ctx.Message.Attachments);
            return img;
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
                _bytes = await Program.GetHttpClient().GetByteArrayAsync(Url);
            }
            return _bytes;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                Url = null;
                _bytes = null;
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SdImage()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public enum AttachmentCountIncorrect : byte
    {
        TooMuchAttachments,
        TooLittleAttachments
    }

    public class SdImageConverter : IArgumentConverter<SdImage>
    {
        private readonly Regex _expression = new("http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\\(\\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private readonly Regex _emote = new("<(a)?:(?<name>.+?):(?<id>.+?)>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public async Task<Optional<SdImage>> ConvertAsync(string value, CommandContext ctx)
        {
            var m = _emote.Match(value);

            if (m.Success)
            {
                return Optional.FromValue(new SdImage($"https://cdn.discordapp.com/emojis/{m.Groups["id"].Value}"));
            }

            if (_expression.IsMatch(value))
            {
                if (CodeEnv.Monika.Contains(value))
                {
                    await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithContent("i will not allow such behaviour..... It appears as SilverBot is having some **technical difficulties**, we will retry in a sec...").SendAsync(ctx.Channel);
                    await Task.Delay(250);
                }
                return Optional.FromValue(new SdImage(value));
            }
            return Optional.FromNoValue<SdImage>();
        }
    }
}