using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GiphyDotNet.Model.GiphyImage;
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
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;
using NetVips;
using System.Web;
using static NetVips.Enums;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace SilverBotDS.Commands;

//THIS FILE CONTAINS CODE FROM THE ESMBOT PROJECT: https://github.com/esmBot/esmBot
/*
ESMBOT license:
MIT License

Copyright (c) 2022 Essem

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/
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

    private async Task CommonCodeWithTemplate(CommandContext ctx, string template, Func<Image, Task<Tuple<bool, Image>>> func, bool TriggerTyping = true, string filename = "sbimg.png", string? encoder = null, int quality = 75)
    {
        if (TriggerTyping)
        {
            await ctx.TriggerTypingAsync();
        }
        using var inputStream = (Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(template) ?? throw new TemplateReturningNullException(template));
        VOption v = new();

        using var img = LoadFromStream(inputStream);
        var r = await func.Invoke(img);
        if (!r.Item1)
        {
            await Send_img_plsAsync(
                ctx, "Command error, please try again later"
            ).ConfigureAwait(false);
            return;
        }
        await using MemoryStream outStream = new();
        WriteImageToStream(r.Item2, outStream, encoder ?? ".png");
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, filename, "there");
    }

    /// <summary>
    ///     Gets the profile picture of a discord user in a 256x256 bitmap saved to a byte array
    /// </summary>
    /// <param name="user">the user</param>
    /// <returns>a 256x256 bitmap in byte[] format</returns>
    private async Task<Image> GetProfilePictureAsyncStatic(DiscordUser user, ushort size = 256)
    {
        var discordsize = size;
        if (discordsize == 0 || (discordsize & (discordsize - 1)) != 0)
        {
            discordsize = 1024;
        }
        MemoryStream stream =
         new(await new SdImage(user.GetAvatarUrl(ImageFormat.Png, discordsize)).GetBytesAsync(HttpClient));
        if (discordsize == size)
        {
            return LoadFromStream(stream);
        }
        using var image = LoadFromStream(stream);
        if (image.Width == size || image.Height == size)
        {
            stream.Position = 0;
            return LoadFromStream(stream);
        }
        stream.Position = 0;
        return LoadFromStream(ResizeAsyncOP(stream.ToArray(), size, size));
    }
    Image LoadFromStream(Stream s, bool? gif = null)
    {
        if (s.CanSeek && gif == null)
        {
            s.Seek(0, SeekOrigin.Begin);
            byte[] buffer = new byte[6];
            s.Read(buffer, 0, buffer.Length);
            gif = buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x38 && (buffer[4] == 0x37 || buffer[4] == 0x39) && buffer[5] == 0x61;
            s.Seek(0, SeekOrigin.Begin);
        }
        VOption v = new();
        if (gif == true)
        {
            v.Add("n", -1);
        }
        return Image.NewFromStream(s, kwargs: v);
    }
    public static bool IsAnimated(byte[] bytes)
    {
        if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x38 && (bytes[4] == 0x37 || bytes[4] == 0x39) && bytes[5] == 0x61)
        {
            return true;
        }
        return false;

    }
  
    public static Stream ResizeAsyncOP(byte[] photoBytes, int x, int y)
    {
        //TODO Port to libvips
        using var memStream = new MemoryStream(photoBytes);
        var memStream2 = new MemoryStream();

        if (IsAnimated(photoBytes))
        {
            using var picture = new ImageMagick.MagickImageCollection(memStream);
            foreach (var image in picture)
            {
                image.Resize(x, y);
            }
            picture.Write(memStream2);
            memStream2.Position = 0;
            return memStream2;
        }
        else
        {
            var picture = new ImageMagick.MagickImage(memStream);
            picture.Resize(x, y);
            picture.Write(memStream2);
            memStream2.Position = 0;
            return memStream2;
        }

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
    public async Task CaptionAndSend(CommandContext ctx, Stream input, string text, string extension, string font = "Roboto Condensed")
    {
        VOption v = new();
        var loadedimg = LoadFromStream(input).Colourspace(Interpretation.Srgb);
        await CaptionAndSend(ctx, loadedimg, text, extension, font);
    }
    public async Task CaptionAndSend(CommandContext ctx, byte[] input, string text, string extension, string font = "Roboto Condensed")
    {
        VOption v = new();
        if (extension == ".gif")
        {
            v.Add("n", -1);
        }
        var loadedimg = Image.NewFromBuffer(input, kwargs: v).Colourspace(Interpretation.Srgb);
        await CaptionAndSend(ctx, loadedimg, text, extension, font);
    }
    public async Task CaptionAndSend(CommandContext ctx, Image loadedimg, string text, string extension, string font = "Roboto Condensed")
    {
        await using MemoryStream outStream = new();
        VOption v = new();
        if (extension == ".gif")
        {
            v.Add("dither", 0);
            v.Add("reoptimise", 1);
        }
        WriteImageToStream((await Caption(loadedimg, text, extension, font)), outStream, extension);
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, Filename: "sbimgcaption" + extension, content: "there");
    }
    public static void WriteImageToStream(Image w, Stream s, string extension)
    {
        VOption v = new();
        if (extension == ".gif")
        {
            v.Add("dither", 0);
            v.Add("reoptimise", 1);
        }
        w.WriteToStream(s, extension, v);
    }
    public async Task<Image> Caption(Image loadedimg, string text, string extension, string font = "Roboto Condensed")
    {
        if (!loadedimg.HasAlpha())
        {
            loadedimg = loadedimg.Bandjoin(255);
        }
        using var textimga = Image.Text(
            "<span background=\"white\">" + HttpUtility.HtmlEncode(text) + "</span>",
            font + ", Twitter Color Emoji normal " + loadedimg.Width / 10,
            width: loadedimg.Width - ((loadedimg.Width / 25) * 2),
            rgba: true,
            align: Align.Centre, fontfile: "twemoji.otf");
        using var textimgb = textimga.Equal(new int[] { 0, 0, 0, 0 }).BandAnd().Ifthenelse(new int[] { 255, 255, 255, 255 }, textimga);
        if (loadedimg.Contains("n-pages") && loadedimg.Contains("page-height"))
        {
            var nPages = (int)loadedimg.Get("n-pages");
            List<Image> img = new();
            for (int i = 0; i < nPages; i++)
            {
                using Image img_frame = loadedimg.Crop(0, i * loadedimg.PageHeight, loadedimg.Width, loadedimg.PageHeight);
                Image frame = textimgb.Join(
                img_frame, Direction.Vertical,
                true,
                align: Align.Centre,
                background: new double[] { 0xffffff });
                img.Add(frame);
            }
            Image final;
            final = Image.Arrayjoin(img.ToArray(), 1);
            foreach (var img_frame in img)
            {
                img_frame.Dispose();
            }
            return final.Mutate(m => { m.Set("page-height", loadedimg.PageHeight + textimgb.Height); });
        }
        else
        {
            return textimgb.Join(loadedimg,
                        Direction.Vertical,
                        true,
                        align: Align.Centre,
                        background: new double[] { 0xffffff });
        }
    }
    private static async Task<MemoryStream> JPEGSpecialSauce(byte[] photoBytes)
    {
        //https://github.com/esmBot/esmBot/blob/master/natives/jpeg.cc
        using var picture = Image.NewFromBuffer(photoBytes);
        MemoryStream stream = new();
        VOption v = new()
        {
            { "Q", 1 },
            { "strip", true }
        };
        picture.WriteToStream(stream, ".jpeg", v);
        stream.Position = 0;
        return stream;
    }
    [Command("caption")]
    [Description("Captions an image")]
    public async Task CaptionImage(CommandContext ctx, SdImage image, [RemainingText] string text)
    {
        //https://github.com/esmBot/esmBot/blob/master/natives/caption.cc
        await ctx.TriggerTypingAsync();
        var bytes = await image.GetBytesAsync(HttpClient);
        string extension = image.Url.GetFileExtensionFromUrl();
        await CaptionAndSend(ctx, bytes, text, extension);
    }

    [Command("caption")]
    public async Task CaptionImage(CommandContext ctx, [RemainingText] string text)
    {
        var image = SdImage.FromContext(ctx);
        await CaptionImage(ctx, image, text);
    }

    [Command("fail")]
    [Description("epic embed fail")]
    public async Task JokerLaugh(CommandContext ctx, [RemainingText] string text)
    {
        await CommonCodeWithTemplate(ctx, "SilverBotDS.Templates.joker_laugh.gif", async (img) =>
        {
            return new Tuple<bool, Image>(true, await Caption(img, text, ".gif", font: _jokerFontFamily));
        }, filename: "sbfail.gif", encoder: ".gif");
    }

    [Command("jpeg")]
    public async Task Jpegize(CommandContext ctx, [Description("the url of the image")] SdImage image)
    {
        await ctx.TriggerTypingAsync();
        await using var outStream = await JPEGSpecialSauce(await image.GetBytesAsync(HttpClient));
        await SendImageStreamIfAllowed(ctx, outStream, "sbimg.jpeg", "There you go");
    }

    [Command("jpeg")]
    //[RequireAttachment(argumentCountThatOverloadsCheck: 1)]
    public async Task Jpegize(CommandContext ctx)
    {
        var image = SdImage.FromContext(ctx);
        await Jpegize(ctx, image);
    }
    private static async Task<Tuple<MemoryStream, string>> TintAsync(byte[] photoBytes, Color color)
    {
        var img = Image.NewFromBuffer(photoBytes);
        if (!img.HasAlpha())
        {
            var imgwithoutbands = img;
            img = imgwithoutbands.Bandjoin(255);
            imgwithoutbands.Dispose();
        }
        var bands = img.Bandsplit();
        bands[0] = bands[0] * color.R / 255d;
        bands[1] = bands[1] * color.G / 255d;
        bands[2] = bands[2] * color.B / 255d;
        bands[3] = bands[3] * color.A / 255d;
        img = bands[0].Bandjoin(bands[1..3]);
        var s = new MemoryStream
        {
            Position = 0
        };
        WriteImageToStream(img, s, ".png");
        return new Tuple<MemoryStream, string>(s, ".png");
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
        await SendImageStreamIfAllowed(ctx, outStream, $"sbimg{thing.Item2}", "There you go");
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
        var img = LoadFromStream(
            Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("SilverBotDS.Templates.adventure_time_template.png") ??
            throw new TemplateReturningNullException("SilverBotDS.Templates.adventure_time_template.png"));
        var i = img;
        img = img.Composite2(await GetProfilePictureAsyncStatic(person), BlendMode.Over, 22, 948);
        i.Dispose();
        using var internalimageb = await GetProfilePictureAsyncStatic(friendo);
        i = img;
        img = img.Composite2(internalimageb, BlendMode.Over, 557, 699);
        i.Dispose();
        await using MemoryStream outStream = new();
        WriteImageToStream(img, outStream, ".png");
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, content: $"adventure time come on grab your friends we will go to very distant lands with {person.Mention} the {(person.IsBot ? "bot" : "human")} and {friendo.Mention} the {(friendo.IsBot ? "bot" : "human")} the fun will never end its adventure time!");
    }
    private async Task CommonCodeWithTemplateGIFMagick(CommandContext ctx, string template, Func<ImageMagick.MagickImageCollection, Task<Tuple<bool, ImageMagick.MagickImageCollection>>> func, bool TriggerTyping = true, string filename = "sbimg.png", ImageMagick.MagickFormat? encoder = null, int quality = 75)
    {
        if (TriggerTyping)
        {
            await ctx.TriggerTypingAsync();
        }
        using var inputStream = (Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(template) ?? throw new TemplateReturningNullException(template));
        using var img = new ImageMagick.MagickImageCollection(inputStream);
        var r = await func.Invoke(img);
        if (!r.Item1)
        {
            await Send_img_plsAsync(
                ctx, "Command error, please try again later"
            ).ConfigureAwait(false);
            return;
        }
        await using MemoryStream outStream = new();
        await r.Item2.WriteAsync(outStream, encoder ?? ImageMagick.MagickFormat.Png);
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, filename, "there");
    }
    [Command("seal")]
    [Description("He was forced to use Microsoft Windows when he was 6")]
    public async Task Seal(CommandContext ctx, [RemainingText] string text)
    {
        await CommonCodeWithTemplate(ctx, "SilverBotDS.Templates.cement-seal-clear.gif", async (img) =>
        {
            return new Tuple<bool, Image>(true, await Caption(img, text, ".gif", font: _jokerFontFamily));
        }, filename: "sbseal.gif", encoder: ".gif");
    }

    [Command("linus")]
    [Description("NVIDIA, fuck you.")]
    public async Task Linus(CommandContext ctx, [RemainingText][Description("company,or thing you want linus to swear at")] string company = "NVIDIA")
    {
        await CommonCodeWithTemplateGIFMagick(ctx, "SilverBotDS.Templates.linus-linus-torvalds.gif", (img) =>
        {
            //TODO PORT TO LIBVIPS

            ImageMagick.MagickReadSettings settings = new()
            {
                FillColor = ImageMagick.MagickColors.White,
                Font = _subtitlesFont,
                FontPointsize = 30,
                BackgroundColor = ImageMagick.MagickColors.Transparent, //new MagickColor(0, 0, 0, 60)
                Width = 498,
                TextGravity = ImageMagick.Gravity.Center,
            };
            using var label = new ImageMagick.MagickImage($"caption:{"so " + company + ", fuck you."}", settings);
            img[0].Composite(label, ImageMagick.Gravity.South, 0, 0, ImageMagick.CompositeOperator.Over);

            return Task.FromResult(new Tuple<bool, ImageMagick.MagickImageCollection>(true, img));
        }, filename: "sblinus.gif", encoder: ImageMagick.MagickFormat.Gif);
    }

    [Command("resize")]
    public async Task Resize(CommandContext ctx, [Description("the url of the image")] SdImage image,
      [Description("Width")] int x = 0, [Description("Height")] int y = 0, ImageMagick.MagickFormat? format = null)
    {
        await ctx.TriggerTypingAsync();
        var thing = ResizeAsyncOP(await image.GetBytesAsync(HttpClient), x, y,format);
        await SendImageStreamIfAllowed(ctx, thing.Item1, $"sbimg.{thing.Item2}", "There you go");
    }

    private Tuple<Stream,string> ResizeAsyncOP(byte[] bytes, int x, int y, ImageMagick.MagickFormat? format)
    {
        //TODO PORT TO LIBVIPS
        using var memStream = new MemoryStream(bytes);
        var memStream2 = new MemoryStream();

        if (IsAnimated(bytes))
        {
            using var picture = new ImageMagick.MagickImageCollection(memStream);
            foreach (var image in picture)
            {
                image.Resize(x, y);
            }
            picture.Write(memStream2,format?? ImageMagick.MagickFormat.Gif);
            memStream2.Position = 0;
            return new(memStream2, (format ?? ImageMagick.MagickFormat.Gif).ToString().ToLowerInvariant());
        }
        else
        {
            var picture = new ImageMagick.MagickImage(memStream);
            picture.Resize(x, y);
            picture.Write(memStream2, format ?? picture.Format);
            memStream2.Position = 0;
            return new(memStream2,(format ?? picture.Format).ToString().ToLowerInvariant());
        }
    }

    [Command("resize")]
    public async Task Resize(CommandContext ctx, SdImage image, ImageMagick.MagickFormat? format)
    {
        await Resize(ctx, image, 0, 0, format);
    }

    [Command("resize")]
    public async Task Resize(CommandContext ctx, ImageMagick.MagickFormat? format)
    {
        var image = SdImage.FromContext(ctx);
        await Resize(ctx, image, 0, 0, format);
    }

    [Command("resize")]
    public async Task Resize(CommandContext ctx, [Description("Width")] int x = 0, [Description("Height")] int y = 0,
        ImageMagick.MagickFormat? format = null)
    {
        var image = SdImage.FromContext(ctx);
        await Resize(ctx, image, x, y, format);
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
        var img = LoadFromStream(
            Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("SilverBotDS.Templates.weeb_reliable_template.png") ??
            throw new TemplateReturningNullException("SilverBotDS.Templates.weeb_reliable_template.png"));

        using (var internalimage = await GetProfilePictureAsyncStatic(jotaro))
        {
            var i = img;
            img = img.Composite2(internalimage, BlendMode.Over, 276, 92);
            i.Dispose();
        }
        using (var internalimage = await GetProfilePictureAsyncStatic(koichi))
        {
            var i = img;
            img = img.Composite2(internalimage, BlendMode.Over, 1138, 369);
            i.Dispose();
        }
        var text =
            $"{(ctx.Guild?.Members?.ContainsKey(koichi.Id) != null && ctx.Guild?.Members?[koichi.Id].Nickname != null ? ctx.Guild?.Members?[koichi.Id].Nickname : koichi.Username)}, you truly are a reliable guy.";
        if (!img.HasAlpha())
        {
            var i = img;
            img = img.Bandjoin(255);
            i.Dispose();

        }
        var textimga = Image.Text(
             "<span foreground=\"white\">"+HttpUtility.HtmlEncode(text)+"</span>",
            _subtitlesFont + ", Twitter Color Emoji normal " + 70,
            rgba: true,
            align: Align.Centre, fontfile: "twemoji.otf");
        textimga=textimga.Embed(5,5,textimga.Width+10,textimga.Height+10);
        var textshadow = textimga[3].Gaussblur(1);
        textshadow *= 64;
        textshadow = textshadow.NewFromImage(new int[] { 0, 0, 0 }).Bandjoin(textshadow).Copy(interpretation: Interpretation.Srgb);
        textimga = textshadow.Composite(textimga, BlendMode.Over);
        img = img.Composite(textimga, BlendMode.Over, 960 - (textimga.Width/2), 975);
        await using MemoryStream outStream = new();
        outStream.Position = 0;
        img.WriteToStream(outStream,".png");
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, content: $"{jotaro.Mention}: {koichi.Mention}, you truly are a reliable guy.");
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
    /*
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
    
    

#nullable disable

 


 

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

    /*
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
    }*/
    


}