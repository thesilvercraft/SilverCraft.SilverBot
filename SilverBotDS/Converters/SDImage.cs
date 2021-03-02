using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using SilverBotDS.Commands;
using SilverBotDS.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SilverBotDS.Converters
{
    public class SdImage
    {
        public string Url;
        private byte[] _bytes;

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
                _bytes = await NetClient.Get().GetByteArrayAsync(Url);
            }
            return _bytes;
        }
    }

    public enum AttachmentCountIncorrect : byte
    {
        TooMuchAttachments,
        TooLittleAttachments
    }

    public class SdImageConverter : IArgumentConverter<SdImage>
    {
        private readonly Regex _expression = new Regex("http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\\(\\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+");

        public async Task<Optional<SdImage>> ConvertAsync(string value, CommandContext ctx)
        {
            if (ctx.Client.Guilds.Values.Any())
            {
                foreach (var guild in ctx.Client.Guilds.Values)
                {
                    var emoji = guild.Emojis.Values.FirstOrDefault(x => x.ToString() == value);
                    if (emoji != default)
                    {
                        return Optional.FromValue(new SdImage(emoji.Url));
                    }
                }
            }

            if (_expression.IsMatch(value))
            {
                if (CodeEnv.Monika.Contains(value))
                {
                    await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithContent("i will not allow such behaviour..... It appears as SilverBot is having some **technical difficulties**, we will retry in a sec...").SendAsync(ctx.Channel);
                    System.Threading.Thread.Sleep(250);
                }
                return Optional.FromValue(new SdImage(value));
            }
            return Optional.FromNoValue<SdImage>();
        }
    }
}