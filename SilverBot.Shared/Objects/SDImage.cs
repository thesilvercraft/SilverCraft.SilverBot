/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Text.RegularExpressions;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using SilverBot.Shared.Exceptions;

namespace SilverBot.Shared.Objects
{
    public class SdImage : IDisposable
    {
        private byte[] _bytes;
        private bool _disposedValue;
        public string Url;

        public SdImage()
        {
        }

        public SdImage(string url)
        {
            Url = url;
        }

        public SdImage(DiscordUser user)
        {
            Url = user.GetAvatarUrl(ImageFormat.Png, 2048);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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

                var m = SdImageConverter.UrLregex.Match(ctx.Message.Content);
                if (m.Success)
                {
                    return new SdImage(m.Value);
                }
            }

            if (ctx.Message.Attachments.Count == 0)
            {
                throw new AttachmentCountIncorrectException(AttachmentCountIncorrect.TooLittleAttachments);
            }

            throw new AttachmentCountIncorrectException(AttachmentCountIncorrect.TooManyAttachments);
        }

        public static SdImage FromAttachments(IReadOnlyList<DiscordAttachment> attachments)
        {
            if (attachments.Count == 1)
            {
                return new SdImage(attachments[0].Url);
            }

            if (attachments.Count == 0)
            {
                throw new AttachmentCountIncorrectException(AttachmentCountIncorrect.TooLittleAttachments);
            }

            throw new AttachmentCountIncorrectException(AttachmentCountIncorrect.TooManyAttachments);
        }

        public async Task<byte[]> GetBytesAsync(HttpClient httpClient)
        {
            return _bytes ??= await httpClient.GetByteArrayAsync(Url);
        }

        public async Task<Stream> GetByteStream(HttpClient httpClient)
        {
            return _bytes == null ? await httpClient.GetStreamAsync(Url) : new MemoryStream(_bytes);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                Url = null;
                _bytes = null;
                _disposedValue = true;
            }
        }

        ~SdImage()
        {
            Dispose(false);
        }
    }

    public enum AttachmentCountIncorrect : byte
    {
        TooManyAttachments,
        TooLittleAttachments
    }

    public class SdImageConverter : IArgumentConverter<SdImage>
    {
        public static readonly Regex UrLregex =
            new("http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\\(\\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static readonly Regex Emote = new("<(a)?:(?<name>.+?):(?<id>.+?)>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static readonly Regex User = new("<@(?<id>.+?)>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public async Task<Optional<SdImage>> ConvertAsync(string value, CommandContext ctx)
        {
            var m = Emote.Match(value);
            if (m.Success)
            {
                return Optional.FromValue(new SdImage($"https://cdn.discordapp.com/emojis/{m.Groups["id"].Value}"));
            }

            var u = User.Match(value);
            if (u.Success)
            {
                return Optional.FromValue(
                    new SdImage(await ctx.Client.GetUserAsync(Convert.ToUInt64(u.Groups["id"].Value))));
            }

            if (UrLregex.IsMatch(value))
            {
                return Optional.FromValue(new SdImage(value));
            }

            return Optional.FromNoValue<SdImage>();
        }
    }
}