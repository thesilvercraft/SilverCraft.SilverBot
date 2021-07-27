using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using SilverBotDS.Commands;
using SilverBotDS.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public SdImage(DiscordUser user)
        {
            Url = user.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 2048);
        }

        public static SdImage FromContext(CommandContext ctx)
        {
            if (ctx.Message.Attachments.Count == 1)
            {
                return FromAttachments(ctx.Message.Attachments);
            }
            if (ctx.Message.Stickers.Count == 1)
            {
                return new SdImage(ctx.Message.Stickers[0].StickerUrl);
            }
            if (ctx.Message.ReferencedMessage is not null)
            {
                if (ctx.Message.ReferencedMessage.Attachments.Count == 1)
                {
                    return FromAttachments(ctx.Message.ReferencedMessage.Attachments);
                }
                if (ctx.Message.ReferencedMessage.Stickers.Count == 1)
                {
                    return new SdImage(ctx.Message.ReferencedMessage.Stickers[0].StickerUrl);
                }
                var m = SdImageConverter.URLregex.Match(ctx.Message.Content);
                if (m.Success)
                {
                    return new SdImage(m.Value);
                }
            }
            if (ctx.Message.Attachments.Count == 0)
            {
                throw new AttachmentCountIncorrectException(AttachmentCountIncorrect.TooLittleAttachments);
            }
            else
            {
                throw new AttachmentCountIncorrectException(AttachmentCountIncorrect.TooManyAttachments);
            }
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
                    throw new AttachmentCountIncorrectException(AttachmentCountIncorrect.TooManyAttachments);
                }
            }
        }

        public async Task<byte[]> GetBytesAsync(HttpClient httpClient)
        {
            return _bytes ??= await httpClient.GetByteArrayAsync(Url);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Url = null;
                _bytes = null;
                disposedValue = true;
            }
        }

        ~SdImage() => Dispose(disposing: false);

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public enum AttachmentCountIncorrect : byte
    {
        TooManyAttachments,
        TooLittleAttachments
    }

    public class SdImageConverter : IArgumentConverter<SdImage>
    {
        public static readonly Regex URLregex = new("http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\\(\\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static readonly Regex _emote = new("<(a)?:(?<name>.+?):(?<id>.+?)>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static readonly Regex _user = new("<@!(?<id>.+?)>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public async Task<Optional<SdImage>> ConvertAsync(string value, CommandContext ctx)
        {
            var m = _emote.Match(value);
            if (m.Success)
            {
                return Optional.FromValue(new SdImage($"https://cdn.discordapp.com/emojis/{m.Groups["id"].Value}"));
            }
            var u = _user.Match(value);
            if (u.Success)
            {
                return Optional.FromValue(new SdImage(await ctx.Client.GetUserAsync(Convert.ToUInt64(u.Groups["id"].Value))));
            }
            if (URLregex.IsMatch(value))
            {
                return Optional.FromValue(new SdImage(value));
            }
            return Optional.FromNoValue<SdImage>();
        }
    }
}