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
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Tasks;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

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
    private const int MegaByte = 1000000;
    private int MaxBytes(CommandContext ctx)
    {
        return (ctx?.Guild?.PremiumTier) switch
        {
            PremiumTier.Tier_2 => 50 * MegaByte,
            PremiumTier.Tier_3 => 100 * MegaByte,
            _ => 8 * MegaByte,
        };
    }

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
        var textSize = TextMeasurer.Measure(text, new TextOptions(font));
        Image img = new Image<Rgba32>((int)textSize.Width, (int)textSize.Height);
        img.Mutate(x => x.Fill(backColor));
        img.Mutate(x => x.DrawText(text, font, textColor, new PointF(0, 0)));
        return img;
    }

    [Command("caption")]
    [Description("Captions an image")]
    public async Task CaptionImage(CommandContext ctx, SdImage image, [RemainingText] string text)
    {
        await ctx.TriggerTypingAsync();
        using var loadedimg = Image.Load(await image.GetBytesAsync(HttpClient), out var frmt);
        Font JokerFont = new(_captionFont, loadedimg.Width / 10);
        var dr = new TextOptions(JokerFont)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Origin = new PointF(loadedimg.Width / 2, 7f),
            WrappingLength = loadedimg.Width,
            WordBreaking = WordBreaking.BreakAll
        };
        var s = TextMeasurer.Measure(text, dr);
        loadedimg.Mutate(m => m.Resize(new ResizeOptions() { Position = AnchorPositionMode.Bottom, Size = new(loadedimg.Width, loadedimg.Height + (int)(s.Height + 5)), Mode = ResizeMode.BoxPad }));
        loadedimg.Mutate(m => m.FillPolygon(new SolidBrush(Color.White), new PointF(0, 0), new PointF(loadedimg.Width, 0), new PointF(loadedimg.Width, s.Height + 5), new PointF(0, s.Height + 5)));
        loadedimg.Mutate(m => m.DrawText(dr, text, Color.Black));
        await using MemoryStream outStream = new();
        await loadedimg.SaveAsync(outStream, frmt);
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, Filename: "sbimgcaption." + frmt.FileExtensions.First(), content: "there");
    }

    [Command("caption")]
    public async Task CaptionImage(CommandContext ctx, [RemainingText] string text)
    {
        var image = SdImage.FromContext(ctx);
        await CaptionImage(ctx, image, text);
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
        string? content = null)
    {
        content ??= "Command executed with result";
        await new DiscordMessageBuilder().WithContent(content)
            .WithFile(filename, outstream)
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

    private static async Task Send_img_plsAsync(CommandContext ctx, string message)
    {
        await new DiscordMessageBuilder()
            .WithContent(message ?? "Please send one image")
            .WithReply(ctx.Message.Id)
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
    /// <returns>A <see cref="MemoryStream" />
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

    public async Task SendImageStreamIfAllowed(CommandContext ctx, Stream image, string Filename="sbimg.png",string? content=null, Language lang=null)
    {
        lang ??= await Language.GetLanguageFromCtxAsync(ctx);
        if (image.Length > MaxBytes(ctx))
        {
            await Send_img_plsAsync(ctx,
                    string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(image.Length)))
                .ConfigureAwait(false);
        }
        else
        {
            await SendImageStream(ctx, image, Filename, content);
        }
    }
    [Command("jpeg")]
    public async Task Jpegize(CommandContext ctx, [Description("the url of the image")] SdImage image)
    {
        await ctx.TriggerTypingAsync();
        await using var outStream = await Make_jpegnisedAsync(await image.GetBytesAsync(HttpClient));
        await SendImageStreamIfAllowed(ctx, outStream, "sbimg.jpeg", "There you go");
    }

    [Command("jpeg")]
    //[RequireAttachment(argumentCountThatOverloadsCheck: 1)]
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
        var thing = await ResizeAsync(await image.GetBytesAsync(HttpClient), new Size(x, y), format);
        await SendImageStreamIfAllowed(ctx, thing.Item1, $"sbimg.{thing.Item2}", "There you go");
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
   // [RequireAttachment(argumentCountThatOverloadsCheck: 2)]

    public async Task Tint(CommandContext ctx, [Description("the url of the image")] SdImage image,
        [Description(
            "https://docs.sixlabors.com/api/ImageSharp/SixLabors.ImageSharp.Color.html#SixLabors_ImageSharp_Color_TryParse_System_String_SixLabors_ImageSharp_Color__")]
        Color color)
    {
        await ctx.TriggerTypingAsync();
        var thing = await TintAsync(await image.GetBytesAsync(HttpClient), color);
        await using var outStream = thing.Item1;
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, $"sbimg.{thing.Item2}", "There you go");
    }

    [Command("tint")]
    public async Task Tint(CommandContext ctx,
        [Description(
            "https://docs.sixlabors.com/api/ImageSharp/SixLabors.ImageSharp.Color.html#SixLabors_ImageSharp_Color_TryParse_System_String_SixLabors_ImageSharp_Color__")]
        Color color)
    {
        var image = SdImage.FromContext(ctx);
        await Tint(ctx, image, color);
    }

    [Command("silver")]
    //[RequireAttachment(argumentCountThatOverloadsCheck: 1)]
    public async Task Grayscale(CommandContext ctx)
    {
        var image = SdImage.FromContext(ctx);
        await Grayscale(ctx, image);
    }

    [Command("silver")]
    public async Task Grayscale(CommandContext ctx, [Description("the url of the image")] SdImage image)
    {
        await ctx.TriggerTypingAsync();
        var thing = await FilterImgBytes(await image.GetBytesAsync(HttpClient), EFilter.Grayscale);
        await using var outStream = thing.Item1;
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, $"sbimg.{thing.Item2}", "There you go");
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
        while (TextMeasurer.Measure(text, new TextOptions(new Font(_subtitlesFont.Family, size, FontStyle.Bold)))
                   .Width > img.Width)
        {
            size -= 0.05f;
        }
        var dr = new TextOptions(new Font(_subtitlesFont, size))
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Origin = new PointF(952, 880)
        };
        img.Mutate(m =>
            m.DrawText(dr, text, Color.White));
        img.Mutate(m =>
             m.DrawText(dr, text, null, new Pen(Color.Black, 3)));
        await using MemoryStream outStream = new();
        outStream.Position = 0;
        await img.SaveAsPngAsync(outStream);
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, content: $"{jotaro.Mention}: {koichi.Mention}, you truly are a reliable guy.");
    }

    private async Task CommonCodeWithTemplate(CommandContext ctx, string template, Func<Image, Task<Tuple<bool, Image>>> func, bool TriggerTyping = true, string filename = "sbimg.png", IImageEncoder encoder = null)
    {
        encoder ??= new PngEncoder();
        if (TriggerTyping)
        {
            await ctx.TriggerTypingAsync();
        }
        using var img = await Image.LoadAsync(Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(template) ?? throw new TemplateReturningNullException(template));
        var r = await func.Invoke(img);
        if (!r.Item1)
        {
            await Send_img_plsAsync(
                ctx, "Command error, please try again later"
            ).ConfigureAwait(false);
            return;
        }
        await using MemoryStream outStream = new();
        r.Item2.Save(outStream, encoder);
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, filename, "there");
    }

    [Command("seal")]
    [Description("He was forced to use Microsoft Windows when he was 6")]
    public async Task Seal(CommandContext ctx, [RemainingText] string text)
    {
        await CommonCodeWithTemplate(ctx, "SilverBotDS.Templates.cement-seal-clear.gif", (img) =>
        {
            Font jokerFont = new(_jokerFontFamily, img.Width / 10);
            var dr = new TextOptions(jokerFont)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                WrappingLength = img.Width,
                Origin = new PointF(img.Width / 2, img.Height / 6)
            };
            img.Mutate(m => m.DrawText(dr, text, Color.Black/* new PointF(0, 124f)*/));

            return Task.FromResult(new Tuple<bool, Image>(true, img));
        }, filename: "sbseal.gif", encoder: new GifEncoder());
    }

    [Command("linus")]
    [Description("NVIDIA, fuck you.")]
    public async Task Linus(CommandContext ctx, [RemainingText][Description("company,or thing you want linus to swear at")] string company = "NVIDIA")
    {
        await CommonCodeWithTemplate(ctx, "SilverBotDS.Templates.linus-linus-torvalds.gif", (img) =>
        {
            Font font = new(_subtitlesFont.Family, img.Width / 20);
            var dr = new TextOptions(font)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                WrappingLength = img.Width,
                Origin = new PointF(img.Width / 2, img.Height)
            };
            img.Mutate(m => m.DrawText(dr, "so " + company + ", fuck you.", Color.White));
            return Task.FromResult(new Tuple<bool, Image>(true, img));
        }, filename: "sblinus.gif", encoder: new GifEncoder());
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
        await SendImageStreamIfAllowed(ctx, outStream, content: "happy new year!");
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
        await SendImageStreamIfAllowed(ctx, outStream, content: $"adventure time come on grab your friends we will go to very distant lands with {person.Mention} the {(person.IsBot ? "bot" : "human")} and {friendo.Mention} the {(friendo.IsBot ? "bot" : "human")} the fun will never end its adventure time!");
    }

    [Command("mspaint")]
    public async Task Paint(CommandContext ctx, SdImage image)
    {
        await CommonCodeWithTemplate(ctx, "SilverBotDS.Templates.paint_template.png", async (img) =>
        {
            await using var resizedstream =
            (await ResizeAsync(await image.GetBytesAsync(HttpClient), new Size(1771, 984), PngFormat.Instance)).Item1;
            using (var internalimage = await Image.LoadAsync(resizedstream))
            {
                img.Mutate(x => x.DrawImage(internalimage, new Point(132, 95), 1));
            }
            return new Tuple<bool, Image>(true, img);
        }, filename: "sbpaint.png", encoder: new PngEncoder());
    }

    [Command("mspaint")]
    //[RequireAttachment(argumentCountThatOverloadsCheck: 1)]
    public async Task Paint(CommandContext ctx)
    {
        var image = SdImage.FromContext(ctx);
        await Paint(ctx, image);
    }

    [Command("motivate")]
    //[RequireAttachment(0)]
    public async Task Motivate(CommandContext ctx, SdImage image, [RemainingText] string text)
    {
        await CommonCodeWithTemplate(ctx, "SilverBotDS.Templates.motivator_template.png", async (img) =>
        {
            await using var resizedstream =
            (await ResizeAsync(await image.GetBytesAsync(HttpClient), new Size(1027, 684), PngFormat.Instance)).Item1;
            using (var internalimage = await Image.LoadAsync(resizedstream))
            {
                img.Mutate(x => x.DrawImage(internalimage, new Point(126, 83), 1));
            }
            var size = _subtitlesFont.Size;
            while (TextMeasurer.Measure(text, new TextOptions(new Font(_motivateFont.Family, size, FontStyle.Bold)))
                       .Width > img.Width)
            {
                size -= 0.05f;
            }
            var dr = new TextOptions(new Font(_motivateFont, size))
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Origin = new System.Numerics.Vector2(img.Width / 2, img.Height - 10)
            };
            img.Mutate(m => m.DrawText(dr, text, Color.White));
            return new Tuple<bool, Image>(true, img);
        }, filename: "sbimg.png", encoder: new PngEncoder());
    }

    [Command("motivate")]
    //[RequireAttachment(argumentCountThatOverloadsCheck: 1)]
    public async Task Motivate(CommandContext ctx, [RemainingText] string text)
    {
        var image = SdImage.FromContext(ctx);
        await Motivate(ctx, image, text);
    }

    [Command("fail")]
    [Description("epic embed fail")]
    public async Task JokerLaugh(CommandContext ctx, [RemainingText] string text)
    {
        await CommonCodeWithTemplate(ctx, "SilverBotDS.Templates.joker_laugh.gif", (img) =>
        {
            Font jokerFont = new(_jokerFontFamily, img.Width / 10);
            var dr = new TextOptions(jokerFont)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                TextAlignment = TextAlignment.Center,
                WrappingLength = img.Width,
                Origin = new System.Numerics.Vector2(img.Width / 2, img.Height / 20)
            };
            img.Mutate(m => m.DrawText(dr, text, Brushes.Solid(Color.Black)));
            return Task.FromResult(new Tuple<bool, Image>(true, img));
        }, filename: "sbfail.gif", encoder: new GifEncoder());
    }
}