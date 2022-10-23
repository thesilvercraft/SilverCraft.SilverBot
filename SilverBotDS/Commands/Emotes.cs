//TODO
/*using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SilverBotDS.Converters;
using SilverBotDS.Exceptions;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using SixLabors.ImageSharp;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands;

[Category("Emotes")]
public class Emotes : BaseCommandModule
{
    public DatabaseContext Database { private get; set; }
    public Config Config { private get; set; }
    public HttpClient HttpClient { private get; set; }

    [Command("addemote")]
    [Aliases("addemoji")]
    [RequireGuild]
    [RequirePermissions(Permissions.ManageEmojis)]
    public async Task AddEmote(CommandContext ctx, [Description("Name like `Kappa`")] string name,
        [Description("Url of the thing")] SdImage url)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        var bytes = await url.GetBytesAsync(HttpClient);
        var st = new MemoryStream(bytes);
        try
        {
            if (bytes.Length > 256000)
            {
                await ctx.RespondAsync(string.Format(lang.EmoteWasLargerThan256K, FileSizeUtils.FormatSize(st.Length)));
                st = (await ImageModule.ResizeAsync(bytes, new Size(128, 128))).Item1;
            }

            var emote = await ctx.Guild.CreateEmojiAsync(name, st,
                reason: "Added via silverbot by " + ctx.User.Username);

            await ctx.RespondAsync(emote.ToString());
        }
        finally
        {
            await st.DisposeAsync();
        }
    }

    [Command("addemote")]
    [RequireGuild]
    [RequirePermissions(Permissions.ManageEmojis)]
    public async Task AddEmote(CommandContext ctx, [Description("Name like `Kappa`")] string name)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        try
        {
            var image = SdImage.FromContext(ctx);
            await AddEmote(ctx, name, image);
        }
        catch (AttachmentCountIncorrectException acie)
        {
            if (acie.AttachmentCount is AttachmentCountIncorrect.TooLittleAttachments)
            {
                await ctx.RespondAsync(lang.NoImageGeneric);
            }
            else if (acie.AttachmentCount is AttachmentCountIncorrect.TooManyAttachments)
            {
                await ctx.RespondAsync(lang.MoreThanOneImageGeneric);
            }
        }
    }
}*/