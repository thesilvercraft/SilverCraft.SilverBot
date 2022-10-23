using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GiphyDotNet.Model.GiphyImage;
using ImageMagick;
using SilverBotDS.Converters;
using SilverBotDS.Exceptions;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SilverBotDS.Utils;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
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

    private static int MaxBytes(CommandContext ctx)
    {
        return (ctx?.Guild?.PremiumTier) switch
        {
            PremiumTier.Tier_2 => 50 * MegaByte,
            PremiumTier.Tier_3 => 100 * MegaByte,
            _ => 8 * MegaByte,
        };
    }

    private static async Task Send_img_plsAsync(CommandContext ctx, string message)
    {
        await new DiscordMessageBuilder()
            .WithContent(message ?? "Please send one image")
            .WithReply(ctx.Message.Id)
            .SendAsync(ctx.Channel);
    }

    public static async Task SendImageStreamIfAllowed(CommandContext ctx, Stream image, string Filename = "sbimg.png", string? content = null, Language lang = null)
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

    public static async Task SendImageStream(CommandContext ctx, Stream outstream, string filename = "sbimg.png",
      string? content = null)
    {
        content ??= "Command executed with result";
        await new DiscordMessageBuilder().WithContent(content)
            .WithFile(filename, outstream)
            .SendAsync(ctx.Channel);
    }

    private async Task CommonCodeWithTemplate(CommandContext ctx, string template, Func<MagickImage, Task<Tuple<bool, MagickImage>>> func, bool TriggerTyping = true, string filename = "sbimg.png", MagickFormat? encoder = null, int quality = 75)
    {
        if (TriggerTyping)
        {
            await ctx.TriggerTypingAsync();
        }
        using var inputStream = (Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(template) ?? throw new TemplateReturningNullException(template));
        using var img = new MagickImage(inputStream);
        var r = await func.Invoke(img);
        if (!r.Item1)
        {
            await Send_img_plsAsync(
                ctx, "Command error, please try again later"
            ).ConfigureAwait(false);
            return;
        }
        await using MemoryStream outStream = new();
        await r.Item2.WriteAsync(outStream, encoder ?? MagickFormat.Png);
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, filename, "there");
    }
    private async Task CommonCodeWithTemplateGIF(CommandContext ctx, string template, Func<MagickImageCollection, Task<Tuple<bool, MagickImageCollection>>> func, bool TriggerTyping = true, string filename = "sbimg.png", MagickFormat? encoder = null, int quality = 75)
    {
        if (TriggerTyping)
        {
            await ctx.TriggerTypingAsync();
        }
        using var inputStream = (Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(template) ?? throw new TemplateReturningNullException(template));
        using var img = new MagickImageCollection(inputStream);
        var r = await func.Invoke(img);
        if (!r.Item1)
        {
            await Send_img_plsAsync(
                ctx, "Command error, please try again later"
            ).ConfigureAwait(false);
            return;
        }
        await using MemoryStream outStream = new();
        await r.Item2.WriteAsync(outStream, encoder ?? MagickFormat.Png);
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, filename, "there");
    }

    private readonly string _captionFont = "Futura Extra Black Condensed";

    private readonly string _jokerFontFamily = "Futura Extra Black Condensed";

    private readonly string _motivateFont = "Times New Roman";

    private readonly string _subtitlesFont = "Trebuchet MS";

    public HttpClient HttpClient { private get; set; }

    public static string[] RequiredFontFamilies => new[]
        {"Impact", "Trebuchet MS", "Times New Roman", "Futura Extra Black Condensed"};

    /*
        /// <summary>
        ///     Renders some text
        /// </summary>
        /// <param name="text">the text to render</param>
        /// <param name="font">the font to use</param>
        /// <param name="textColor">the color to use while rendering the text</param>
        /// <param name="backColor">the background color to use</param>
        /// <returns>An <see cref="Image" /></returns>
        public static SKBitmap DrawText(string text, SKFont font, SKColor textColor, SKColor backColor)
        {
            using var paint = new SKPaint(font) { Color=textColor};
            var textSize = paint.MeasureText(text);
            SKBitmap img = new((int)textSize, (int)paint.FontMetrics.CapHeight);
            using SKCanvas canvas = new(img);
            canvas.Clear(backColor);
            canvas.DrawText(text, SKPoint.Empty, paint);
            return img;
        }*/

    [Command("caption")]
    [Description("Captions an image")]
    public async Task CaptionImage(CommandContext ctx, SdImage image, [RemainingText] string text)
    {
        await ctx.TriggerTypingAsync();
        var bytes = await image.GetBytesAsync(HttpClient);
        await using MemoryStream outStream = new();
        string extension = "png";
        if (IsAnimated(bytes))
        {

        }
        else
        {
            using var loadedimg = new MagickImage(bytes);
            using MagickImageCollection c = new();
            MagickReadSettings settings = new()
            {
                FillColor =  MagickColors.Black,
                Font = _captionFont,
                FontPointsize = loadedimg.Width/10,
                BackgroundColor = MagickColors.White,
                TextGravity = Gravity.Center,
                Width = loadedimg.Width,
            };
            using var imgtxt = new MagickImage($"caption:{text}",settings);
            c.Add(loadedimg);
            c.Add(imgtxt);
            await c.AppendVertically().WriteAsync(outStream,loadedimg.Format);
            extension = FileExtension(loadedimg.Format);
        }
            outStream.Position = 0;

        await SendImageStreamIfAllowed(ctx, outStream, Filename: "sbimgcaption." + extension, content: "there");
    }

    [Command("caption")]
    public async Task CaptionImage(CommandContext ctx, [RemainingText] string text)
    {
        var image = SdImage.FromContext(ctx);
        await CaptionImage(ctx, image, text);
    }
    /*
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

  */

#nullable enable
    public static bool IsAnimated(byte[] bytes)
    {
        if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x38 && (bytes[4] == 0x37 || bytes[4] == 0x39) && bytes[5] == 0x61)
        {
            return true;
        }
        return false;

    }
    public static async Task<Tuple<MemoryStream, string>> ResizeAsync(byte[] photoBytes, int x, int y,
        MagickFormat? format = null, int quality = 75)
    {
        MemoryStream stream = new();
        var op = await ResizeAsyncOP(photoBytes, x, y, format, quality);
        if (op.Item2 != null)
        {
            format ??= op.Item2.Format;
            await op.Item2.WriteAsync(stream, (MagickFormat)format);
        }
        else if (op.Item1 != null)
        {
            format ??= MagickFormat.Gif;
            await op.Item1.WriteAsync(stream, (MagickFormat)format);
        }

        return new Tuple<MemoryStream, string>(stream, FileExtension((MagickFormat)format));
    }
    public static async Task<Tuple<MagickImageCollection?, MagickImage?>> ResizeAsyncOP(byte[] photoBytes, int x, int y,
       MagickFormat? format = null, int quality = 75)
    {
        using var memStream = new MemoryStream(photoBytes);
        if (IsAnimated(photoBytes))
        {
            var picture = new MagickImageCollection(memStream);
            foreach (var image in picture)
            {
                image.Resize(x, y);
            }
            format ??= MagickFormat.Gif;
            return new(picture, null);
        }
        else
        {
            var picture = new MagickImage(memStream);
            picture.Resize(x, y);
            format ??= picture.Format;
            return new(null, picture);
        }

    }
    public static string FileExtension(MagickFormat format)
    {
        return format switch
        {
            MagickFormat.Unknown => "bin",
            MagickFormat.ThreeFr => "3fr",
            MagickFormat.ThreeG2 => "3g2",
            MagickFormat.ThreeGp => "3gp",
            MagickFormat.A => "A",
            MagickFormat.Aai => "Aai",
            MagickFormat.Ai => "Ai",
            MagickFormat.APng => "APng",
            MagickFormat.Art => "Art",
            MagickFormat.Arw => "Arw",
            MagickFormat.Ashlar => "Ashlar",
            MagickFormat.Avi => "Avi",
            MagickFormat.Avif => "Avif",
            MagickFormat.Avs => "Avs",
            MagickFormat.B => "B",
            MagickFormat.Bayer => "Bayer",
            MagickFormat.Bayera => "Bayera",
            MagickFormat.Bgr => "Bgr",
            MagickFormat.Bgra => "Bgra",
            MagickFormat.Bgro => "Bgro",
            MagickFormat.Bmp or MagickFormat.Bmp2 or MagickFormat.Bmp3 => "bmp",
            MagickFormat.Brf => "Brf",
            MagickFormat.C => "C",
            MagickFormat.Cal => "Cal",
            MagickFormat.Cals => "Cals",
            MagickFormat.Canvas => "Canvas",
            MagickFormat.Caption => "Caption",
            MagickFormat.Cin => "cin",
            MagickFormat.Cip => "Cip",
            MagickFormat.Clip => "Clip",
            MagickFormat.Clipboard => "Clipboard",
            MagickFormat.Cmyk => "Cmyk",
            MagickFormat.Cmyka => "Cmyka",
            MagickFormat.Cr2 => "Cr2",
            MagickFormat.Cr3 => "Cr3",
            MagickFormat.Crw => "Crw",
            MagickFormat.Cube => "Cube",
            MagickFormat.Cur => "Cur",
            MagickFormat.Cut => "Cut",
            MagickFormat.Data => "Data",
            MagickFormat.Dcm => "Dcm",
            MagickFormat.Dcr => "Dcr",
            MagickFormat.Dcraw => "Dcraw",
            MagickFormat.Dcx => "Dcx",
            MagickFormat.Dds => "Dds",
            MagickFormat.Dfont => "Dfont",
            MagickFormat.Dib => "Dib",
            MagickFormat.Dng => "Dng",
            MagickFormat.Dpx => "Dpx",
            MagickFormat.Dxt1 => "Dxt1",
            MagickFormat.Dxt5 => "Dxt5",
            MagickFormat.Emf => "Emf",
            MagickFormat.Epdf => "Epdf",
            MagickFormat.Epi => "Epi",
            MagickFormat.Eps => "Eps",
            MagickFormat.Eps2 => "Eps2",
            MagickFormat.Eps3 => "Eps3",
            MagickFormat.Epsf => "Epsf",
            MagickFormat.Epsi => "Epsi",
            MagickFormat.Ept => "Ept",
            MagickFormat.Ept2 => "Ept2",
            MagickFormat.Ept3 => "Ept3",
            MagickFormat.Erf => "Erf",
            MagickFormat.Exr => "Exr",
            MagickFormat.Farbfeld => "Farbfeld",
            MagickFormat.Fax => "Fax",
            MagickFormat.Ff => "Ff",
            MagickFormat.File => "File",
            MagickFormat.Fits => "Fits",
            MagickFormat.Fl32 => "Fl32",
            MagickFormat.Flv => "Flv",
            MagickFormat.Fractal => "Fractal",
            MagickFormat.Fts => "Fts",
            MagickFormat.Ftxt => "Ftxt",
            MagickFormat.G => "G",
            MagickFormat.G3 => "G3",
            MagickFormat.G4 => "G4",
            MagickFormat.Gif or MagickFormat.Gif87 => "gif",
            MagickFormat.Gradient => "Gradient",
            MagickFormat.Gray => "Gray",
            MagickFormat.Graya => "Graya",
            MagickFormat.Group4 => "Group4",
            MagickFormat.Hald => "Hald",
            MagickFormat.Hdr => "Hdr",
            MagickFormat.Heic => "Heic",
            MagickFormat.Heif => "Heif",
            MagickFormat.Histogram => "Histogram",
            MagickFormat.Hrz => "Hrz",
            MagickFormat.Htm => "Htm",
            MagickFormat.Html => "Html",
            MagickFormat.Http => "Http",
            MagickFormat.Https => "Https",
            MagickFormat.Icb => "Icb",
            MagickFormat.Ico => "ico",
            MagickFormat.Icon => "ico",
            MagickFormat.Iiq => "Iiq",
            MagickFormat.Info => "Info",
            MagickFormat.Inline => "Inline",
            MagickFormat.Ipl => "Ipl",
            MagickFormat.Isobrl => "Isobrl",
            MagickFormat.Isobrl6 => "Isobrl6",
            MagickFormat.J2c => "J2c",
            MagickFormat.J2k => "J2k",
            MagickFormat.Jng => "Jng",
            MagickFormat.Jnx => "Jnx",
            MagickFormat.Jp2 => "Jp2",
            MagickFormat.Jpc => "Jpc",
            MagickFormat.Jpe => "Jpe",
            MagickFormat.Jpg or MagickFormat.Jpeg => "jpg",
            MagickFormat.Jpm => "Jpm",
            MagickFormat.Jps => "Jps",
            MagickFormat.Jpt => "Jpt",
            MagickFormat.Json => "Json",
            MagickFormat.Jxl => "Jxl",
            MagickFormat.K => "K",
            MagickFormat.K25 => "K25",
            MagickFormat.Kdc => "Kdc",
            MagickFormat.Label => "Label",
            MagickFormat.M => "M",
            MagickFormat.M2v => "M2v",
            MagickFormat.M4v => "M4v",
            MagickFormat.Mac => "Mac",
            MagickFormat.Map => "Map",
            MagickFormat.Mask => "Mask",
            MagickFormat.Mat => "Mat",
            MagickFormat.Matte => "Matte",
            MagickFormat.Mef => "Mef",
            MagickFormat.Miff => "Miff",
            MagickFormat.Mkv => "Mkv",
            MagickFormat.Mng => "Mng",
            MagickFormat.Mono => "Mono",
            MagickFormat.Mov => "Mov",
            MagickFormat.Mp4 => "Mp4",
            MagickFormat.Mpc => "Mpc",
            MagickFormat.Mpeg => "Mpeg",
            MagickFormat.Mpg => "Mpg",
            MagickFormat.Mrw => "Mrw",
            MagickFormat.Msl => "Msl",
            MagickFormat.Msvg => "Msvg",
            MagickFormat.Mtv => "Mtv",
            MagickFormat.Mvg => "Mvg",
            MagickFormat.Nef => "Nef",
            MagickFormat.Nrw => "Nrw",
            MagickFormat.Null => "Null",
            MagickFormat.O => "O",
            MagickFormat.Ora => "Ora",
            MagickFormat.Orf => "Orf",
            MagickFormat.Otb => "Otb",
            MagickFormat.Otf => "Otf",
            MagickFormat.Pal => "Pal",
            MagickFormat.Palm => "Palm",
            MagickFormat.Pam => "Pam",
            MagickFormat.Pango => "Pango",
            MagickFormat.Pattern => "Pattern",
            MagickFormat.Pbm => "Pbm",
            MagickFormat.Pdb => "Pdb",
            MagickFormat.Pdf => "Pdf",
            MagickFormat.Pdfa => "Pdfa",
            MagickFormat.Pef => "Pef",
            MagickFormat.Pes => "Pes",
            MagickFormat.Pfa => "Pfa",
            MagickFormat.Pfb => "Pfb",
            MagickFormat.Pfm => "Pfm",
            MagickFormat.Pgm => "Pgm",
            MagickFormat.Phm => "Phm",
            MagickFormat.Pgx => "Pgx",
            MagickFormat.Picon => "Picon",
            MagickFormat.Pict => "Pict",
            MagickFormat.Pix => "Pix",
            MagickFormat.Pjpeg => "Pjpeg",
            MagickFormat.Plasma => "Plasma",
            MagickFormat.Png or MagickFormat.Png00 or MagickFormat.Png24 or MagickFormat.Png32 or MagickFormat.Png48 or MagickFormat.Png64 or MagickFormat.Png8 => "png",
            MagickFormat.Pnm => "Pnm",
            MagickFormat.Pocketmod => "Pocketmod",
            MagickFormat.Ppm => "Ppm",
            MagickFormat.Ps => "Ps",
            MagickFormat.Ps2 => "Ps2",
            MagickFormat.Ps3 => "Ps3",
            MagickFormat.Psb => "Psb",
            MagickFormat.Psd => "Psd",
            MagickFormat.Ptif => "Ptif",
            MagickFormat.Pwp => "Pwp",
            MagickFormat.Qoi => "Qoi",
            MagickFormat.R => "R",
            MagickFormat.RadialGradient => "RadialGradient",
            MagickFormat.Raf => "Raf",
            MagickFormat.Ras => "Ras",
            MagickFormat.Raw => "Raw",
            MagickFormat.Rgb => "Rgb",
            MagickFormat.Rgb565 => "Rgb565",
            MagickFormat.Rgba => "Rgba",
            MagickFormat.Rgbo => "Rgbo",
            MagickFormat.Rgf => "Rgf",
            MagickFormat.Rla => "Rla",
            MagickFormat.Rle => "Rle",
            MagickFormat.Rmf => "Rmf",
            MagickFormat.Rsvg => "Rsvg",
            MagickFormat.Rw2 => "Rw2",
            MagickFormat.Scr => "Scr",
            MagickFormat.Sct => "Sct",
            MagickFormat.Sfw => "Sfw",
            MagickFormat.Sgi => "Sgi",
            MagickFormat.Shtml => "Shtml",
            MagickFormat.Six => "Six",
            MagickFormat.Sixel => "Sixel",
            MagickFormat.SparseColor => "SparseColor",
            MagickFormat.Sr2 => "Sr2",
            MagickFormat.Srf => "Srf",
            MagickFormat.Stegano => "Stegano",
            MagickFormat.Sun => "Sun",
            MagickFormat.Svg => "Svg",
            MagickFormat.Svgz => "Svgz",
            MagickFormat.Text => "Text",
            MagickFormat.Tga => "Tga",
            MagickFormat.Tif => "Tif",
            MagickFormat.Tiff => "Tiff",
            MagickFormat.Tiff64 => "Tiff64",
            MagickFormat.Tile => "Tile",
            MagickFormat.Tim => "Tim",
            MagickFormat.Tm2 => "Tm2",
            MagickFormat.Ttc => "Ttc",
            MagickFormat.Ttf => "Ttf",
            MagickFormat.Txt => "Txt",
            MagickFormat.Ubrl => "Ubrl",
            MagickFormat.Ubrl6 => "Ubrl6",
            MagickFormat.Uil => "Uil",
            MagickFormat.Uyvy => "Uyvy",
            MagickFormat.Vda => "Vda",
            MagickFormat.Vicar => "Vicar",
            MagickFormat.Vid => "Vid",
            MagickFormat.WebM => "WebM",
            MagickFormat.Viff => "Viff",
            MagickFormat.Vips => "Vips",
            MagickFormat.Vst => "Vst",
            MagickFormat.WebP => "WebP",
            MagickFormat.Wbmp => "Wbmp",
            MagickFormat.Wmf => "Wmf",
            MagickFormat.Wmv => "Wmv",
            MagickFormat.Wpg => "Wpg",
            MagickFormat.X3f => "X3f",
            MagickFormat.Xbm => "Xbm",
            MagickFormat.Xc => "Xc",
            MagickFormat.Xcf => "Xcf",
            MagickFormat.Xpm => "Xpm",
            MagickFormat.Xps => "Xps",
            MagickFormat.Xv => "Xv",
            MagickFormat.Y => "Y",
            MagickFormat.Yaml => "Yaml",
            MagickFormat.Ycbcr => "Ycbcr",
            MagickFormat.Ycbcra => "Ycbcra",
            MagickFormat.Yuv => "Yuv",
            MagickFormat.Pcd => "Pcd",
            MagickFormat.Pcds => "Pcds",
            MagickFormat.Pcl => "Pcl",
            MagickFormat.Pct => "Pct",
            MagickFormat.Pcx => "Pcx",
            _ => throw new NotImplementedException(),
        };
    }

#nullable disable

    private static async Task<MemoryStream> Make_jpegnisedAsync(byte[] photoBytes)
    {
        using MemoryStream stream1 = new(photoBytes);
        using var picture = new MagickImage(stream1);
        MemoryStream stream = new();
        picture.Quality = 1;
        picture.Write(stream, MagickFormat.Jpeg);
        stream.Position = 0;
        return stream;
    }


    private static async Task<Tuple<MemoryStream, string>> TintAsync(byte[] photoBytes, MagickColor color)
    {
        var instream = new MemoryStream(photoBytes);
        using var img = new MagickImage(instream);
        img.ColorMatrix(new MagickColorMatrix(4,
            color.R / 255f, 0, 0, 0,
            0, color.G / 255f, 0, 0,
            0, 0, color.B / 255f, 0,
            0, 0, 0, color.A / 255f));
        instream = new MemoryStream
        {
            Position = 0
        };
        img.Write(instream);
        instream.Position = 0;
        return new Tuple<MemoryStream, string>(instream, FileExtension(img.Format));
    }

    /// <summary>
    ///     Filters an image
    /// </summary>
    /// <param name="photoBytes">The <see cref="byte" /> of the original picture</param>
    /// <param name="filter">The <see cref="IMatrixFilter" /> to use</param>
    /// <returns>A <see cref="MemoryStream" />
    private static async Task<Tuple<MemoryStream, string>> FilterImgBytes(byte[] photoBytes, EFilter filter)
    {
        var instream = new MemoryStream(photoBytes);

        using var img = new MagickImage(instream);
        if (filter == EFilter.Grayscale)
        {
            img.Grayscale();
        }
        else
        {
            img.OilPaint();
        }

        await img.WriteAsync(instream);
        return new Tuple<MemoryStream, string>(instream, FileExtension(img.Format));
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
        [Description("Width")] int x = 0, [Description("Height")] int y = 0, MagickFormat? format = null)
    {
        await ctx.TriggerTypingAsync();
        var thing = await ResizeAsync(await image.GetBytesAsync(HttpClient), x, y, format);
        await SendImageStreamIfAllowed(ctx, thing.Item1, $"sbimg.{thing.Item2}", "There you go");
    }

    [Command("resize")]
    public async Task Resize(CommandContext ctx, SdImage image, MagickFormat? format)
    {
        await Resize(ctx, image, 0, 0, format);
    }

    [Command("resize")]
    public async Task Resize(CommandContext ctx, MagickFormat? format)
    {
        var image = SdImage.FromContext(ctx);
        await Resize(ctx, image, 0, 0, format);
    }

    [Command("resize")]
    public async Task Resize(CommandContext ctx, [Description("Width")] int x = 0, [Description("Height")] int y = 0,
        MagickFormat? format = null)
    {
        var image = SdImage.FromContext(ctx);
        await Resize(ctx, image, x, y, format);
    }

    [Command("tint")]
    // [RequireAttachment(argumentCountThatOverloadsCheck: 2)]

    public async Task Tint(CommandContext ctx, [Description("the url of the image")] SdImage image,
        [Description(
              "https://docs.sixlabors.com/api/ImageSharp/SixLabors.ImageSharp.Color.html#SixLabors_ImageSharp_Color_TryParse_System_String_SixLabors_ImageSharp_Color__")]
          MagickColor color)
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
          MagickColor color)
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
    private async Task<Tuple<MagickImageCollection?, MagickImage?>> GetProfilePictureAsync(DiscordUser user, ushort size = 256)
    {
        var discordsize = size;
        if (discordsize == 0 || (discordsize & (discordsize - 1)) != 0)
        {
            discordsize = 1024;
        }
        await using MemoryStream stream =
         new(await new SdImage(user.GetAvatarUrl(ImageFormat.Auto, discordsize)).GetBytesAsync(HttpClient));
        if (discordsize == size)
        {

            //dont resize
            if (user.AvatarHash.StartsWith("a_"))
            {
                return new(new(stream), null);

            }
            else
            {
                return new(null, new(stream));

            }
        }

        using var image = new MagickImage(stream);

        if (image.Width == size || image.Height == size)
        {
            stream.Position = 0;
            if (user.AvatarHash.StartsWith("a_"))
            {
                return new(new(stream), null);

            }
            else
            {
                return new(null, new(stream));

            }
        }
        stream.Position = 0;

        return await ResizeAsyncOP(stream.ToArray(), size, size);

    }
    /// <summary>
    ///     Gets the profile picture of a discord user in a 256x256 bitmap saved to a byte array
    /// </summary>
    /// <param name="user">the user</param>
    /// <returns>a 256x256 bitmap in byte[] format</returns>
    private async Task<MagickImage> GetProfilePictureAsyncStatic(DiscordUser user, ushort size = 256)
    {
        var discordsize = size;
        if (discordsize == 0 || (discordsize & (discordsize - 1)) != 0)
        {
            discordsize = 1024;
        }
        await using MemoryStream stream =
         new(await new SdImage(user.GetAvatarUrl(ImageFormat.Png, discordsize)).GetBytesAsync(HttpClient));
        if (discordsize == size)
        {

            return new(stream);


        }

        using var image = new MagickImage(stream);

        if (image.Width == size || image.Height == size)
        {
            stream.Position = 0;
            return new(stream);

        }
        stream.Position = 0;

        return (await ResizeAsyncOP(stream.ToArray(), size, size)).Item2;

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
        using var img = new MagickImage(
            Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("SilverBotDS.Templates.weeb_reliable_template.png") ??
            throw new TemplateReturningNullException("SilverBotDS.Templates.weeb_reliable_template.png"));


        using (var internalimage = await GetProfilePictureAsyncStatic(jotaro))
        {
            img.Composite(internalimage, 276, 92, CompositeOperator.Over);
        }
        using (var internalimage = await GetProfilePictureAsyncStatic(koichi))
        {
            img.Composite(internalimage, 1138, 369, CompositeOperator.Over);
        }
        var text =
            $"{(ctx.Guild?.Members?.ContainsKey(koichi.Id) != null && ctx.Guild?.Members?[koichi.Id].Nickname != null ? ctx.Guild?.Members?[koichi.Id].Nickname : koichi.Username)}, you truly are a reliable guy.";
        MagickReadSettings settings = new()
        {
            FillColor = MagickColors.White,
            StrokeColor = MagickColors.Black,
            StrokeWidth = 1,
            Font = _subtitlesFont,
            FontPointsize = 70,
            BackgroundColor = MagickColors.Transparent, //new MagickColor(0, 0, 0, 60)
            Width = img.Width,
            TextGravity = Gravity.Center,
        };
        using var label = new MagickImage($"caption:{text}", settings);
        img.Composite(label, Gravity.South, 0, 40, CompositeOperator.Over);

        await using MemoryStream outStream = new();
        outStream.Position = 0;
        await img.WriteAsync(outStream);
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, content: $"{jotaro.Mention}: {koichi.Mention}, you truly are a reliable guy.");
    }
    /*public async Task ReliableGif(Tuple<MagickImageCollection?, MagickImage?> a, Tuple<MagickImageCollection?, MagickImage?> b, MagickImage img)
    {
        MagickImageCollection collection = new();
        bool aisgif = a.Item1 != null;
        var agif = a.Item1;
        bool bisgif = b.Item1 != null;
        var bgif = b.Item1;

        if (aisgif && bisgif)
        {
            agif.
        }


    }
    static int gcf(int a, int b)
    {
        while (b != 0)
        {
            int t = b;
            b = a % b;
            a = t;
        }
        return a;
    }*/

    [Command("seal")]
    [Description("He was forced to use Microsoft Windows when he was 6")]
    public async Task Seal(CommandContext ctx, [RemainingText] string text)
    {
        await CommonCodeWithTemplateGIF(ctx, "SilverBotDS.Templates.cement-seal-clear.gif", (img) =>
        {
            MagickReadSettings settings = new()
            {
                FillColor = MagickColors.Black,
                Font = "FExBlkCnBT.ttf",
                FontPointsize = 64,
                BackgroundColor = MagickColors.Transparent, //new MagickColor(0, 0, 0, 60)
                Width = 640,
                TextGravity = Gravity.Center,
            };
            using var label = new MagickImage($"caption:{text}", settings);
            img[0].Composite(label, Gravity.North, 0, 0, CompositeOperator.Over);
            return Task.FromResult(new Tuple<bool, MagickImageCollection>(true, img));
        }, filename: "sbseal.gif", encoder: MagickFormat.Gif);
    }

    [Command("linus")]
    [Description("NVIDIA, fuck you.")]
    public async Task Linus(CommandContext ctx, [RemainingText][Description("company,or thing you want linus to swear at")] string company = "NVIDIA")
    {
        await CommonCodeWithTemplateGIF(ctx, "SilverBotDS.Templates.linus-linus-torvalds.gif", (img) =>
        {
            MagickReadSettings settings = new()
            {
                FillColor = MagickColors.White,
                Font = _subtitlesFont,
                FontPointsize = 30,
                BackgroundColor = MagickColors.Transparent, //new MagickColor(0, 0, 0, 60)
                Width = 498,
                TextGravity = Gravity.Center,
            };
            using var label = new MagickImage($"caption:{"so " + company + ", fuck you."}", settings);
            img[0].Composite(label, Gravity.South, 0, 0, CompositeOperator.Over);

            return Task.FromResult(new Tuple<bool, MagickImageCollection>(true, img));
        }, filename: "sblinus.gif", encoder: MagickFormat.Gif);
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
        using var img = new MagickImage(
            Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("SilverBotDS.Templates.happy_new_year_template.png") ??
            throw new TemplateReturningNullException("SilverBotDS.Templates.happy_new_year_template.png"));
        using var a = (await GetProfilePictureAsyncStatic(person, 350));
        using var gamer = new MagickImage(MagickColors.Transparent, img.Width, img.Height);

        gamer.Composite(a, 19, 70);
        gamer.Composite(img);
        await using MemoryStream outStream = new();
        outStream.Position = 0;
        await gamer.WriteAsync(outStream);
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
        var img = new MagickImage(
            Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("SilverBotDS.Templates.adventure_time_template.png") ??
            throw new TemplateReturningNullException("SilverBotDS.Templates.adventure_time_template.png"));
     
        using (var internalimage = await GetProfilePictureAsyncStatic(person))
        {
            img.Composite(internalimage,22, 948);
        }
        using (var internalimage = await GetProfilePictureAsyncStatic(friendo))
        {
            img.Composite(internalimage, 557, 699);
        }
        await using MemoryStream outStream = new();
        await img.WriteAsync(outStream);
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, content: $"adventure time come on grab your friends we will go to very distant lands with {person.Mention} the {(person.IsBot ? "bot" : "human")} and {friendo.Mention} the {(friendo.IsBot ? "bot" : "human")} the fun will never end its adventure time!");
    }

    [Command("mspaint")]
    public async Task Paint(CommandContext ctx, SdImage image)
    {
        await CommonCodeWithTemplate(ctx, "SilverBotDS.Templates.paint_template.png", async (img) =>
        {
            await using var resizedstream =
            (await ResizeAsync(await image.GetBytesAsync(HttpClient), 1771, 984, MagickFormat.Png)).Item1;
            using (var internalimage = new MagickImage(resizedstream))
            {
                img.Composite(internalimage, 132, 95);
            }
            return new Tuple<bool, MagickImage>(true, img);
        }, filename: "sbpaint.png", encoder: MagickFormat.Png);
    }

    [Command("mspaint")]
    //[RequireAttachment(argumentCountThatOverloadsCheck: 1)]
    public async Task Paint(CommandContext ctx)
    {
        var image = SdImage.FromContext(ctx);
        await Paint(ctx, image);
    }
    /*
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
    } */
}