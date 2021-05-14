using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using SilverBotDS.Commands;
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
            if (ctx.Message.ReferencedMessage is not null)
            {
                if (ctx.Message.ReferencedMessage.Attachments.Count == 1)
                {
                    return FromAttachments(ctx.Message.ReferencedMessage.Attachments);
                }
                else
                {
                    //TODO: implement getting the image as a link
                }
            }
            if (ctx.Message.Attachments.Count == 0)
            {
                throw new AttachmentCountIncorrectException(AttachmentCountIncorrect.TooLittleAttachments);
            }
            else
            {
                throw new AttachmentCountIncorrectException(AttachmentCountIncorrect.TooMuchAttachments);
            }
        }

        //TODO: implement gifs and stickers
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

        public async Task<byte[]> GetBytesAsync(HttpClient httpClient)
        {
            if (_bytes == null)
            {
                _bytes = await httpClient.GetByteArrayAsync(Url);
            }
            return _bytes;
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
        TooMuchAttachments,
        TooLittleAttachments
    }

    public class SdImageConverter : IArgumentConverter<SdImage>
    {
        private readonly Regex _expression = new("http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\\(\\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private readonly Regex _emote = new("<(a)?:(?<name>.+?):(?<id>.+?)>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private readonly Regex _user = new("<@!(?<id>.+?)>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

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
            if (_expression.IsMatch(value))
            {
                if (CodeEnv.Monika.Contains(value))
                {
                    await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithContent("i will not allow such behavior..... It appears as SilverBot is having some **technical difficulties**, we will retry in a sec...").SendAsync(ctx.Channel);
                    await Task.Delay(250);
                }
                return Optional.FromValue(new SdImage(value));
            }
            return Optional.FromNoValue<SdImage>();
        }
    }
}