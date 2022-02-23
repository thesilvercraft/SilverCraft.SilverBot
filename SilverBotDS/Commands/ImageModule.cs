using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SilverBotDS.Attributes;
using SilverBotDS.Converters;
using SilverBotDS.Exceptions;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SilverBotDS.Utils;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Filters;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace SilverBotDS.Commands;

[Cooldown(1, 2, CooldownBucketType.User)]
[Category("Image")]
public class ImageModule : BaseCommandModule, IRequireFonts
{
    public enum EFilter
    {
        Grayscale,
        Comic
    }

    private const int MaxBytes = 8388246;

    private static readonly JpegEncoder BadJpegEncoder = new()
    {
        Quality = 1
    };

    private readonly FontFamily _captionFont = SystemFonts.Get("Futura Extra Black Condensed");

    private readonly FontFamily _jokerFontFamily = SystemFonts.Get("Futura Extra Black Condensed");

    private readonly Font _motivateFont = new(SystemFonts.Get("Times New Roman"), 100);

    private readonly Font _subtitlesFont = new(SystemFonts.Get("Trebuchet MS"), 100);

    public HttpClient HttpClient { private get; set; }

    public static string[] RequiredFontFamilies => new[]
        {"Impact", "Trebuchet MS", "Times New Roman", "Futura Extra Black Condensed"};

    /// <summary>
    ///     Renders some text
    /// </summary>
    /// <param name="text">the text to render</param>
    /// <param name="font">the font to use</param>
    /// <param name="textColor">the color to use while rendering the text</param>
    /// <param name="backColor">the background color to use</param>
    /// <returns>An <see cref="Image" /></returns>
    public static Image DrawText(string text, Font font, Color textColor, Color backColor)
    {
        var textSize = TextMeasurer.Measure(text, new RendererOptions(font));
        Image img = new Image<Rgba32>((int)textSize.Width, (int)textSize.Height);
        img.Mutate(x => x.Fill(backColor));
        img.Mutate(x => x.DrawText(text, font, textColor, new PointF(0, 0)));
        return img;
    }

    [Command("text")]
    public async Task DrawText(CommandContext ctx, [Description("the text")] string text, string font = "Diavlo Light",
        float size = 30.0f)
    {
        await ctx.TriggerTypingAsync();
        using var img = DrawText(text, new Font(SystemFonts.Get(font), size), Color.FromRgb(0, 0, 0),
            Color.FromRgb(255, 255, 255));
        await using var outStream = new MemoryStream();
        await img.SaveAsPngAsync(outStream);
        outStream.Position = 0;
        await SendImageStream(ctx, outStream);
    }

    public static async Task SendImageStream(CommandContext ctx, Stream outstream, string filename = "sbimg.png",
        string content = null, Language lang = null)
    {
        lang ??= await Language.GetLanguageFromCtxAsync(ctx);
        content ??= lang.Success;
        await new DiscordMessageBuilder().WithEmbed(new DiscordEmbedBuilder()
                .WithDescription(content)
                .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto)))
            .WithFile(filename, outstream).WithReply(ctx.Message.Id).WithAllowedMentions(Mentions.None)
            .SendAsync(ctx.Channel);
    }

#nullable enable

    public static async Task<Tuple<MemoryStream, string>> ResizeAsync(byte[] photoBytes, Size size,
        IImageFormat? format = null)
    {
        using var img = Image.Load(photoBytes, out var frmt);
        if (size.Width == 0)
        {
            size.Width = img.Width;
        }

        if (size.Height == 0)
        {
            size.Height = img.Height;
        }

        img.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = size
        }));
        MemoryStream stream = new();
        if (format != null)
        {
            frmt = format;
        }

        await img.SaveAsync(stream, frmt);
        stream.Position = 0;
        return new Tuple<MemoryStream, string>(stream, frmt.FileExtensions.First());
    }

#nullable disable

    private static async Task<MemoryStream> Make_jpegnisedAsync(byte[] photoBytes)
    {
        using var img = Image.Load(photoBytes);
        MemoryStream stream = new();
        await img.SaveAsJpegAsync(stream, BadJpegEncoder);
        stream.Position = 0;
        return stream;
    }

    private static async Task Send_img_plsAsync(CommandContext ctx, string e, Language lang = null)
    {
        lang ??= await Language.GetLanguageFromCtxAsync(ctx);
        await new DiscordMessageBuilder()
            .WithReply(ctx.Message.Id)
            .WithEmbed(new DiscordEmbedBuilder()
                .WithTitle(lang.WrongImageCount).WithDescription(e)
                .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)))
            .SendAsync(ctx.Channel);
    }

    private static async Task<Tuple<MemoryStream, string>> TintAsync(byte[] photoBytes, Color color)
    {
        var instream = new MemoryStream(photoBytes);
        using var img = Image.Load(instream, out var frmt);
        await instream.DisposeAsync();
        var pixel = color.ToPixel<Rgba32>();
        ColorMatrix matrix = new(pixel.R / 255f, 0, 0, 0, 0, pixel.G / 255f, 0, 0, 0, 0, pixel.B / 255f, 0, 0, 0, 0,
            pixel.A / 255f, 0, 0, 0, 0);
        img.Mutate(x => x.ApplyProcessor(new FilterProcessor(matrix)));
        instream = new MemoryStream
        {
            Position = 0
        };
        img.Save(instream, frmt);
        instream.Position = 0;
        return new Tuple<MemoryStream, string>(instream, frmt.FileExtensions.First());
    }

    /// <summary>
    ///     Filters an image
    /// </summary>
    /// <param name="photoBytes">The <see cref="byte" /> of the original picture</param>
    /// <param name="filter">The <see cref="IMatrixFilter" /> to use</param>
    /// <returns>A <see cref="MemoryStream" /> with a <see cref="PngFormat" /> of 70 Quality</returns>
    private static async Task<Tuple<MemoryStream, string>> FilterImgBytes(byte[] photoBytes, EFilter filter)
    {
        using var img = Image.Load(photoBytes, out var frmt);
        if (filter == EFilter.Grayscale)
        {
            img.Mutate(x => x.Grayscale());
        }

        MemoryStream stream = new();
        await img.SaveAsync(stream, frmt);
        return new Tuple<MemoryStream, string>(stream, frmt.FileExtensions.First());
    }

    [Command("jpeg")]
    public async Task Jpegize(CommandContext ctx, [Description("the url of the image")] SdImage image)
    {
        await ctx.TriggerTypingAsync();
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await using var outStream = await Make_jpegnisedAsync(await image.GetBytesAsync(HttpClient));
        if (outStream.Length > MaxBytes)
        {
            await Send_img_plsAsync(ctx,
                    string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang)
                .ConfigureAwait(false);
        }
        else
        {
            await SendImageStream(ctx, outStream, "silverbotimage.jpeg", lang.JpegSuccess, lang);
        }
    }

    [Command("jpeg")]
    [RequireAttachment(argumentCountThatOverloadsCheck: 1)]
    public async Task Jpegize(CommandContext ctx)
    {
        var image = SdImage.FromContext(ctx);
        await Jpegize(ctx, image);
    }

    [Command("resize")]
    public async Task Resize(CommandContext ctx, [Description("the url of the image")] SdImage image,
        [Description("Width")] int x = 0, [Description("Height")] int y = 0, IImageFormat format = null)
    {
        await ctx.TriggerTypingAsync();
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        var thing = await ResizeAsync(await image.GetBytesAsync(HttpClient), new Size(x, y), format);
        if (thing.Item1.Length > MaxBytes)
        {
            await Send_img_plsAsync(ctx,
                    string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(thing.Item1.Length)), lang)
                .ConfigureAwait(false);
        }
        else
        {
            await SendImageStream(ctx, thing.Item1, $"sbimg.{thing.Item2}", lang.ResizeSuccess, lang);
        }
    }

    [Command("resize")]
    public async Task Resize(CommandContext ctx, SdImage image, IImageFormat format)
    {
        await Resize(ctx, image, 0, 0, format);
    }

    [Command("resize")]
    public async Task Resize(CommandContext ctx, IImageFormat format)
    {
        var image = SdImage.FromContext(ctx);
        await Resize(ctx, image, 0, 0, format);
    }

    [Command("resize")]
    public async Task Resize(CommandContext ctx, [Description("Width")] int x = 0, [Description("Height")] int y = 0,
        IImageFormat format = null)
    {
        var image = SdImage.FromContext(ctx);
        await Resize(ctx, image, x, y, format);
    }

    [Command("tint")]
    public async Task Tint(CommandContext ctx, [Description("the url of the image")] SdImage image,
        [Description(
            "https://docs.sixlabors.com/api/ImageSharp/SixLabors.ImageSharp.Color.html#SixLabors_ImageSharp_Color_TryParse_System_String_SixLabors_ImageSharp_Color__")]
        Color color)
    {
        await ctx.TriggerTypingAsync();
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        var thing = await TintAsync(await image.GetBytesAsync(HttpClient), color);
        await using var outStream = thing.Item1;
        outStream.Position = 0;
        if (outStream.Length > MaxBytes)
        {
            await Send_img_plsAsync(ctx,
                    string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang)
                .ConfigureAwait(false);
        }
        else
        {
            await SendImageStream(ctx, outStream, $"sbimg.{thing.Item2}", lang.TintSuccess, lang);
        }
    }

    [Command("tint")]
    [RequireAttachment(argumentCountThatOverloadsCheck: 2)]
    public async Task Tint(CommandContext ctx,
        [Description(
            "https://docs.sixlabors.com/api/ImageSharp/SixLabors.ImageSharp.Color.html#SixLabors_ImageSharp_Color_TryParse_System_String_SixLabors_ImageSharp_Color__")]
        Color color)
    {
        var image = SdImage.FromContext(ctx);
        await Tint(ctx, image, color);
    }

    [Command("silver")]
    [RequireAttachment(argumentCountThatOverloadsCheck: 1)]
    public async Task Grayscale(CommandContext ctx)
    {
        var image = SdImage.FromContext(ctx);
        await Grayscale(ctx, image);
    }

    [Command("silver")]
    public async Task Grayscale(CommandContext ctx, [Description("the url of the image")] SdImage image)
    {
        await ctx.TriggerTypingAsync();
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        var thing = await FilterImgBytes(await image.GetBytesAsync(HttpClient), EFilter.Grayscale);
        await using var outStream = thing.Item1;
        outStream.Position = 0;
        if (outStream.Length > MaxBytes)
        {
            await Send_img_plsAsync(ctx,
                    string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang)
                .ConfigureAwait(false);
        }
        else
        {
            await SendImageStream(ctx, outStream, $"sbimg.{thing.Item2}", lang.SilverSuccess, lang);
        }
    }

    /// <summary>
    ///     Gets the profile picture of a discord user in a 256x256 bitmap saved to a byte array
    /// </summary>
    /// <param name="user">the user</param>
    /// <returns>a 256x256 bitmap in byte[] format</returns>
    private async Task<byte[]> GetProfilePictureAsync(DiscordUser user, ushort size = 256)
    {
        var discordsize = size;
        if (discordsize == 0 || (discordsize & (discordsize - 1)) != 0)
        {
            discordsize = 1024;
        }

        await using MemoryStream stream =
            new(await new SdImage(user.GetAvatarUrl(ImageFormat.Png, discordsize)).GetBytesAsync(HttpClient));
        using var image = Image.Load(stream);
        if (image.Width == size || image.Height == size)
        {
            stream.Position = 0;
            return stream.ToArray();
        }

        stream.Position = 0;
        await using var resizedstream =
            (await ResizeAsync(stream.ToArray(), new Size(size, size), PngFormat.Instance)).Item1;
        resizedstream.Position = 0;
        return resizedstream.ToArray();
    }

    [Command("reliable")]
    public async Task Reliable(CommandContext ctx)
    {
        await Reliable(ctx, ctx.User, ctx.Client.CurrentUser);
    }

    [Command("reliable")]
    public async Task Reliable(CommandContext ctx, DiscordUser koichi)
    {
        await Reliable(ctx, ctx.User, koichi);
    }

    [Command("reliable")]
    public async Task Reliable(CommandContext ctx, DiscordUser jotaro, DiscordUser koichi)
    {
        await ctx.TriggerTypingAsync();
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        using var img = await Image.LoadAsync(
            Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("SilverBotDS.Templates.weeb_reliable_template.png") ??
            throw new TemplateReturningNullException("SilverBotDS.Templates.weeb_reliable_template.png"));
        await using MemoryStream resizedstreamb = new(await GetProfilePictureAsync(koichi));
        await using MemoryStream resizedstreama = new(await GetProfilePictureAsync(jotaro));
        using (var internalimage = await Image.LoadAsync(resizedstreama))
        {
            img.Mutate(m => m.DrawImage(internalimage, new Point(276, 92), 1));
        }

        using (var internalimage = await Image.LoadAsync(resizedstreamb))
        {
            img.Mutate(m => m.DrawImage(internalimage, new Point(1138, 369), 1));
        }

        var text =
            $"{(ctx.Guild?.Members?.ContainsKey(koichi.Id) != null && ctx.Guild?.Members?[koichi.Id].Nickname != null ? ctx.Guild?.Members?[koichi.Id].Nickname : koichi.Username)}, you truly are a reliable guy.";
        var size = _subtitlesFont.Size;
        while (TextMeasurer.Measure(text, new RendererOptions(new Font(_subtitlesFont.Family, size, FontStyle.Bold)))
                   .Width > img.Width)
        {
            size -= 0.05f;
        }

        var dr = new DrawingOptions();
        dr.TextOptions.HorizontalAlignment = HorizontalAlignment.Center;
        img.Mutate(m =>
            m.DrawText(dr, text, new Font(_subtitlesFont, size), Brushes.Solid(Color.White), new PointF(952, 880)));
        img.Mutate(m =>
            m.DrawText(dr, text, new Font(_subtitlesFont, size), Pens.Solid(Color.Black, 3), new PointF(952, 880)));
        await using MemoryStream outStream = new();
        outStream.Position = 0;
        await img.SaveAsPngAsync(outStream);
        outStream.Position = 0;
        if (outStream.Length > MaxBytes)
        {
            await Send_img_plsAsync(ctx,
                    string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang)
                .ConfigureAwait(false);
        }
        else
        {
            await SendImageStream(ctx, outStream,
                content: $"{jotaro.Mention}: {koichi.Mention}, you truly are a reliable guy.", lang: lang);
        }
    }

    [Command("seal")]
    [Description("He was forced to use Microsoft Windows when he was 6")]
    public async Task Seal(CommandContext ctx, [RemainingText] string text)
    {
        await ctx.TriggerTypingAsync();
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        using var img = await Image.LoadAsync(
            Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "SilverBotDS.Templates.cement-seal-clear.gif"
            ) ?? throw new TemplateReturningNullException(
                "SilverBotDS.Templates.cement-seal-clear.gif"
            )
        );
        Font jokerFont = new(_jokerFontFamily, img.Width / 10);
        var dr = new DrawingOptions();
        dr.TextOptions.HorizontalAlignment = HorizontalAlignment.Center;
        dr.TextOptions.VerticalAlignment = VerticalAlignment.Center;
        dr.TextOptions.WrapTextWidth = img.Width;
        img.Mutate(m => m.DrawText(dr, text, jokerFont, Brushes.Solid(Color.Black), new PointF(0, 124f)));
        await using MemoryStream outStream = new();
        await img.SaveAsync(outStream, new GifEncoder());
        outStream.Position = 0;
        if (outStream.Length > MaxBytes)
        {
            await Send_img_plsAsync(
                ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang
            ).ConfigureAwait(false);
        }
        else
        {
            await SendImageStream(ctx, outStream, "sbseal.gif", "there", lang);
        }
    }

    [Command("happynewyear")]
    public async Task HappyNewYear(CommandContext ctx)
    {
        await HappyNewYear(ctx, ctx.User);
    }

    [Command("happynewyear")]
    public async Task HappyNewYear(CommandContext ctx, DiscordUser person)
    {
        await ctx.TriggerTypingAsync();
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        using var img = await Image.LoadAsync(
            Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("SilverBotDS.Templates.happy_new_year_template.png") ??
            throw new TemplateReturningNullException("SilverBotDS.Templates.happy_new_year_template.png"));
        await using MemoryStream resizedstreama = new(await GetProfilePictureAsync(person, 350));
        using var gamer = new Image<Rgba32>(img.Width, img.Height);
        using (var internalimage = await Image.LoadAsync(resizedstreama))
        {
            gamer.Mutate(m => m.DrawImage(internalimage, new Point(19, 70), 1));
            gamer.Mutate(m => m.DrawImage(img, 1));
        }

        await using MemoryStream outStream = new();
        outStream.Position = 0;
        gamer.Save(outStream, new PngEncoder());
        outStream.Position = 0;
        if (outStream.Length > MaxBytes)
        {
            await Send_img_plsAsync(ctx,
                    string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang)
                .ConfigureAwait(false);
        }
        else
        {
            await SendImageStream(ctx, outStream, content: "happy new year!", lang: lang);
        }
    }

    [RequireGuild]
    [Command("adventuretime")]
    public async Task AdventureTime(CommandContext ctx)
    {
        await AdventureTime(ctx, ctx.Guild.CurrentMember);
    }

    [Command("adventuretime")]
    public async Task AdventureTime(CommandContext ctx, DiscordUser friendo)
    {
        await AdventureTime(ctx, ctx.Member, friendo);
    }

    [Command("adventuretime")]
    public async Task AdventureTime(CommandContext ctx, DiscordUser person, DiscordUser friendo)
    {
        await ctx.TriggerTypingAsync();
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        var img = await Image.LoadAsync(
            Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("SilverBotDS.Templates.adventure_time_template.png") ??
            throw new TemplateReturningNullException("SilverBotDS.Templates.adventure_time_template.png"));
        await using MemoryStream resizedstreama = new(await GetProfilePictureAsync(person));
        await using MemoryStream resizedstreamb = new(await GetProfilePictureAsync(friendo));
        using (var internalimage = await Image.LoadAsync(resizedstreamb))
        {
            img.Mutate(x => x.DrawImage(internalimage, new Point(22, 948), 1));
        }

        using (var internalimage = await Image.LoadAsync(resizedstreama))
        {
            img.Mutate(x => x.DrawImage(internalimage, new Point(557, 699), 1));
        }

        await using MemoryStream outStream = new();
        img.Save(outStream, new PngEncoder());
        outStream.Position = 0;
        if (outStream.Length > MaxBytes)
        {
            await Send_img_plsAsync(ctx,
                    string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang)
                .ConfigureAwait(false);
        }
        else
        {
            await SendImageStream(ctx, outStream,
                content:
                $"adventure time come on grab your friends we will go to very distant lands with {person.Mention} the {(person.IsBot ? "bot" : "human")} and {friendo.Mention} the {(friendo.IsBot ? "bot" : "human")} the fun will never end its adventure time!",
                lang: lang);
        }
    }

    [Command("mspaint")]
    public async Task Paint(CommandContext ctx, SdImage image)
    {
        await ctx.TriggerTypingAsync();
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        using var img =
            await Image.LoadAsync(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("SilverBotDS.Templates.paint_template.png") ??
                throw new TemplateReturningNullException("SilverBotDS.Templates.paint_template.png"));
        await using var resizedstream =
            (await ResizeAsync(await image.GetBytesAsync(HttpClient), new Size(1771, 984), PngFormat.Instance)).Item1;
        using (var internalimage = await Image.LoadAsync(resizedstream))
        {
            img.Mutate(x => x.DrawImage(internalimage, new Point(132, 95), 1));
        }

        await using MemoryStream outStream = new();
        await img.SaveAsync(outStream, new PngEncoder());
        outStream.Position = 0;
        if (outStream.Length > MaxBytes)
        {
            await Send_img_plsAsync(ctx,
                    string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang)
                .ConfigureAwait(false);
        }
        else
        {
            await SendImageStream(ctx, outStream, "SilverPaint.png", "untitled - Paint", lang);
        }
    }

    [Command("mspaint")]
    [RequireAttachment(argumentCountThatOverloadsCheck: 1)]
    public async Task Paint(CommandContext ctx)
    {
        var image = SdImage.FromContext(ctx);
        await Paint(ctx, image);
    }

    [Command("motivate")]
    public async Task Motivate(CommandContext ctx, SdImage image, [RemainingText] string text)
    {
        await ctx.TriggerTypingAsync();
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        using var img =
            await Image.LoadAsync(
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("SilverBotDS.Templates.motivator_template.png") ??
                throw new TemplateReturningNullException("SilverBotDS.Templates.motivator_template.png"));
        await using var resizedstream =
            (await ResizeAsync(await image.GetBytesAsync(HttpClient), new Size(1027, 684), PngFormat.Instance)).Item1;
        using (var internalimage = await Image.LoadAsync(resizedstream))
        {
            img.Mutate(x => x.DrawImage(internalimage, new Point(126, 83), 1));
        }

        var size = _subtitlesFont.Size;
        while (TextMeasurer.Measure(text, new RendererOptions(new Font(_motivateFont.Family, size, FontStyle.Bold)))
                   .Width > img.Width)
        {
            size -= 0.05f;
        }

        var dr = new DrawingOptions();
        dr.TextOptions.HorizontalAlignment = HorizontalAlignment.Center;
        img.Mutate(m => m.DrawText(dr, text, new Font(_motivateFont, size), Brushes.Solid(Color.White),
            new PointF(639.5f, 897.5f)));
        await using MemoryStream outStream = new();
        await img.SaveAsync(outStream, new PngEncoder());
        outStream.Position = 0;
        if (outStream.Length > MaxBytes)
        {
            await Send_img_plsAsync(ctx,
                    string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang)
                .ConfigureAwait(false);
        }
        else
        {
            await SendImageStream(ctx, outStream, content: "there", lang: lang);
        }
    }

    [Command("motivate")]
    [RequireAttachment(argumentCountThatOverloadsCheck: 1)]
    public async Task Motivate(CommandContext ctx, [RemainingText] string text)
    {
        var image = SdImage.FromContext(ctx);
        await Motivate(ctx, image, text);
    }

    [Command("fail")]
    [Description("epic embed fail")]
    public async Task JokerLaugh(CommandContext ctx, [RemainingText] string text)
    {
        await ctx.TriggerTypingAsync();
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        using var img = await Image.LoadAsync(
            Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "SilverBotDS.Templates.joker_laugh.gif"
            ) ?? throw new TemplateReturningNullException(
                "SilverBotDS.Templates.joker_laugh.gif"
            )
        );
        Font jokerFont = new(_jokerFontFamily, img.Width / 10);
        var dr = new DrawingOptions();
        dr.TextOptions.HorizontalAlignment = HorizontalAlignment.Center;
        // x component of pointf is arbitrary and irrelevent since the above alignment option is given
        img.Mutate(m => m.DrawText(dr, text, jokerFont, Brushes.Solid(Color.Black), new PointF(255f, 20f)));
        await using MemoryStream outStream = new();
        await img.SaveAsync(outStream, new GifEncoder());
        outStream.Position = 0;
        if (outStream.Length > MaxBytes)
        {
            await Send_img_plsAsync(
                ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang
            ).ConfigureAwait(false);
        }
        else
        {
            await SendImageStream(ctx, outStream, "sbfail.gif", "there", lang);
        }
    }

    [Command("caption")]
    public async Task Caption(CommandContext ctx, SdImage image, [RemainingText] string text)
    {
        await ctx.TriggerTypingAsync();
        await using var inStream = new MemoryStream(await image.GetBytesAsync(HttpClient));
        using var bitmap = Image.Load(inStream, out var frmt);
        int x = bitmap.Width, y = bitmap.Height;
        var font = new Font(_captionFont, x / 10);
        await using var outStream = new MemoryStream();
        FontRectangle textSize;
        var dr = new DrawingOptions();
        dr.TextOptions.HorizontalAlignment = HorizontalAlignment.Center;
        dr.TextOptions.VerticalAlignment = VerticalAlignment.Center;
        textSize = TextMeasurer.Measure(text, new RendererOptions(font));
        using (var img2 = new Image<Rgba32>(x, y + (int)textSize.Height))
        {
            img2.Mutate(o => o.Fill(Color.FromRgb(255, 255, 255)));
            img2.Mutate(o => o.DrawText(dr, text, font, Brushes.Solid(Color.FromRgb(0, 0, 0)),
                new PointF(img2.Width / 2, textSize.Height / 2)));
            img2.Mutate(o => o.DrawImage(bitmap, new Point(0, (int)textSize.Height), 1));
            img2.Save(outStream, frmt);
        }

        outStream.Position = 0;
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        if (outStream.Length > MaxBytes)
        {
            await Send_img_plsAsync(ctx,
                    string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang)
                .ConfigureAwait(false);
        }
        else
        {
            await SendImageStream(ctx, outStream, content: "there", lang: lang);
        }
    }

    [Command("caption")]
    [RequireAttachment]
    public async Task Caption(CommandContext ctx, [RemainingText] string text)
    {
        var image = SdImage.FromContext(ctx);
        await Caption(ctx, image, text);
    }
}