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
using System.Collections;
using System.Linq;
using ImageMagick;
using static System.Net.Mime.MediaTypeNames;
using Image = NetVips.Image;
using DSharpPlus.SlashCommands;

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
public class ImageModule : BaseCommandModule, IRequireAssets
{


    private const int MegaByte = 1000000;

    private static int MaxBytes(dynamic ctx) => (ctx?.Guild?.PremiumTier) switch
    {
        PremiumTier.Tier_2 => 50 * MegaByte,
        PremiumTier.Tier_3 => 100 * MegaByte,
        _ => 8 * MegaByte,
    };

    private static async Task Send_img_plsAsync(CommandContext ctx, string message)
    {
        await new DiscordMessageBuilder()
            .WithContent(message ?? "Please send one image")
            .WithReply(ctx.Message.Id)
            .SendAsync(ctx.Channel);
    }

    public static async Task SendImageStreamIfAllowed(CommandContext ctx, Stream image, bool DisposeOfStream, string Filename = "sbimg.png", string? content = null, Language lang = null, bool dryrun=false)
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
            if(!dryrun)
            {
                await SendImageStream(ctx, image, Filename, content);
            }
        }
        if(DisposeOfStream)
        {
            await image.DisposeAsync();
        }
    }

    public static async Task SendImageStreamIfAllowed(InteractionContext ctx, Stream image, bool DisposeOfStream, string Filename = "sbimg.png", string? content = null, Language lang = null, bool dryrun = false)
    {
        lang ??= await Language.GetLanguageFromCtxAsync(ctx);
        if (image.Length > MaxBytes(ctx))
        {
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(image.Length))));
        }
        else
        {
            if (!dryrun)
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(content).AddFile(Filename,image));
            }
        }
        if (DisposeOfStream)
        {
            await image.DisposeAsync();
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

    private async Task CommonCodeWithTemplate(CommandContext ctx, string template, Func<Image, Task<Tuple<bool, Image>>> func, bool TriggerTyping = true, string filename = "sbimg.png", string? encoder = null, string msgcontent = "there")
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
        await SendImageStreamIfAllowed(ctx, outStream, DisposeOfStream: true, filename, msgcontent);
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
    public static Image LoadFromStream(Stream s, bool? gif = null)
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
        return bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x38 && (bytes[4] == 0x37 || bytes[4] == 0x39) && bytes[5] == 0x61;
    }

    public static Stream ResizeAsyncOP(byte[] photoBytes, int x, int y)
    {
        //TODO Port to libvips
        using var memStream = new MemoryStream(photoBytes);
        var memStream2 = new MemoryStream();

        if (IsAnimated(photoBytes))
        {
            using var picture = new MagickImageCollection(memStream);
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
            var picture = new MagickImage(memStream);
            picture.Resize(x, y);
            picture.Write(memStream2);
            memStream2.Position = 0;
            return memStream2;
        }

    }
    private const string _captionFont = "Futura Extra Black Condensed";


    private const string _subtitlesFont = "Trebuchet MS";

    public HttpClient HttpClient { private get; set; }

    public static string[] RequiredAssets => new[]
    {
        "font://Trebuchet MS",
        "file://fonts/twemoji.otf",
        "file://fonts/caption.otf",
        "file://fonts/caption2.ttf",
        "file://fonts/reddit.ttf",
    };
    public static void AutoFixRequiredAssets(IEnumerable<string> missing)
    {
        var url = "https://github.com/esmBot/esmBot/blob/master/assets/fonts/{0}?raw=true";
        if (missing.Any(x => x.StartsWith("font://")))
        {
            Serilog.Log.Information("Please install the windows fonts, debian `sudo apt install ttf-mscorefonts-installer`, arch https://wiki.archlinux.org/title/Microsoft_fonts");
        }
        using HttpClient client = new();
        client.DefaultRequestHeaders.UserAgent.TryParseAdd("SilverBot.ImageModule.AutoFixRequiredAssets 1 (BOT, https://github.com/thesilvercraft/silvercraftbot)");
        if (!Directory.Exists("fonts"))
        {
            Directory.CreateDirectory("fonts");
        }
        foreach (var missingAsset in missing.Where(x => !x.StartsWith("font://")))
        {
            var fn = missingAsset.RemoveStringFromStart("file://fonts/");
            var r = client.GetAsync(string.Format(url, fn));
            var rr = r.GetAwaiter().GetResult();
            using FileStream fs = new(path: ("fonts/" + fn), FileMode.CreateNew);
            rr.Content.ReadAsStream().CopyTo(fs);
        }
    }


    public async Task CaptionAndSend(CommandContext ctx, Stream input, string text, string extension, string font = _captionFont)
    {
        var loadedimg = LoadFromStream(input).Colourspace(Interpretation.Srgb);
        await CaptionAndSend(ctx, loadedimg, text, extension, font);
    }
    public async Task CaptionAndSend(CommandContext ctx, byte[] input, string text, string extension, string font = _captionFont)
    {
        VOption v = new();
        if (extension == ".gif")
        {
            v.Add("n", -1);
        }
        var loadedimg = Image.NewFromBuffer(input, kwargs: v).Colourspace(Interpretation.Srgb);
        await CaptionAndSend(ctx, loadedimg, text, extension, font);
    }
    public async Task CaptionAndSend(CommandContext ctx, Image loadedimg, string text, string extension, string font = _captionFont)
    {
        await using MemoryStream outStream = new();
        WriteImageToStream(await Caption(loadedimg, text, font), outStream, extension);
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, DisposeOfStream: true, Filename: "sbimgcaption" + extension, content: "there");
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
    public Task<Image> Caption(Image loadedimg, string text, string font = _captionFont)
    {
        if (!loadedimg.HasAlpha())
        {
            loadedimg = loadedimg.Bandjoin(255);
        }
        using var textimga = Image.Text(
            "<span background=\"white\">" + HttpUtility.HtmlEncode(text) + "</span>",
            "Twemoji Color Emoji, " + font + " normal " + (loadedimg.Width / 10),
            width: loadedimg.Width - ((loadedimg.Width / 25) * 2),
            rgba: true,
            align: Align.Centre, fontfile: "fonts/twemoji.otf");
        using var textimgb = textimga.Equal(new int[] { 0, 0, 0, 0 }).BandAnd().Ifthenelse(new int[] { 255, 255, 255, 255 }, textimga).Embed(2, 2, textimga.Width + 4, textimga.Height + 4, Extend.White);
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
            Image final = Image.Arrayjoin(img.ToArray(), 1);
            foreach (var img_frame in img)
            {
                img_frame.Dispose();
            }
            return Task.FromResult(final.Mutate(m => m.Set("page-height", loadedimg.PageHeight + textimgb.Height)));
        }
        else
        {
            return Task.FromResult(textimgb.Join(loadedimg,
                        Direction.Vertical,
                        true,
                        align: Align.Centre,
                        background: new double[] { 0xffffff }));
        }
    }
    private static MemoryStream JPEGSpecialSauce(byte[] photoBytes)
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
    public async Task CaptionImage(CommandContext ctx, [RemainingText] string text) => await CaptionImage(ctx, SdImage.FromContext(ctx), text);

    [Command("fail")]
    [Description("epic embed fail")]
    public async Task JokerLaugh(CommandContext ctx, [RemainingText] string text) => 
        await CommonCodeWithTemplate(ctx, "SilverBotDS.Templates.joker_laugh.gif", async (img) =>
        {
            return new Tuple<bool, Image>(true, await Caption(img, text));
        }, filename: "sbfail.gif", encoder: ".gif");
    //[Command("transform")]
    //[Description("epic 3d perspective transformation test")]
    //public async Task TransformTest(CommandContext ctx, SdImage img)
    //{
    //    //https://stackoverflow.com/a/50219715
    //    var loaded = LoadFromStream(await img.GetByteStream(HttpClient), false);
        
    //    double[] T = new[] {
    //        1.0,
    //        0.3, //makes image go bottom left
    //        0, //nothing
    //        0.2, //makes image go top right
    //        1.0, 
    //        0,//nothing
    //        0.0007,//makes image disapear
    //        0.0007 //makes image disapear
    //    };
    //    Image i = Image.Xyz(loaded.Width, loaded.Height);
    //    var x = (i[0] * T[0] + i[1] * T[1] + T[2]) / (i[0] * T[6] + i[1] * T[7] + 1);
    //    var y = (i[0] * T[3] + i[1] * T[4] + T[5]) / (i[0] * T[6] + i[1] * T[7] + 1);
    //    var m = x.Bandjoin(y);
    //    loaded = loaded.Mapim(m);
    //    using MemoryStream s = new();
    //    WriteImageToStream(loaded, s, ".png");
    //    s.Position = 0;
    //   await SendImageStreamIfAllowed(ctx, s);
    //}
    [Command("yeet")]
    [Description("YEET")]
    public Task Yeet(CommandContext ctx) => Yeet(ctx, SdImage.FromContext(ctx));
    [Command("yeet")]
    [Description("YEET")]
    public async Task Yeet(CommandContext ctx, SdImage img2) =>
    await CommonCodeWithTemplate(ctx, "SilverBotDS.Templates.simba-toss.gif", async (img) =>
    { 
        var x = new Tuple<int, int, int>[] {
            new(143,59,80),
            new(143,59,80),
            new(90,8,80),
            new(55,-25,80),
            new(0,0,0),
            new(0,0,0),
            new(245,93,90),
            new(215,89,80),
            new(178,66,100),
            new(135,32,100),
            new(57,-41,100),//11
            new(0,0,0),//12
            new(120,66,15),//13
            new(115,52,15),//14
            new(123,32,15),//15
            new(143,29,15),//16
            new(162,38,15),//17
            new(167,49,15),//18
            new(169,66,15),//19
            new(168,71,15),//20
            new(165,76,15),//21
            new(164,79,15),//22
            new(162,82,15),//23
            new(159,83,15),//24
            new(161,84,15),//25
            new(160,84,15),//26
            new(161,85,15),//27
            new(162,84,15),//28
            new(154,55,24),
            new(154,40,24),
            new(154,34,24),
            new(75,24,24),
            new(34,13,24),
            new(15,14,24),
            new(0,17,24),
            new(0,0,0),
            new(0,0,0),
            new(0,0,0),
            //after throw, feel free to PR
            new(0,0,0),new(0,0,0),new(0,0,0),new(0,0,0),new(0,0,0),new(0,0,0),new(0,0,0),new(0,0,0),new(0,0,0),new(0,0,0),new(0,0,0),new(0,0,0),new(0,0,0),new(0,0,0),new(0,0,0),new(0,0,0),new(0,0,0),new(0,0,0),new(0,0,0),new(0,0,0)
        };
        return new Tuple<bool, Image>(true, await EpicGifComposite(img, img2, x));
    }, filename: "sbYEET.gif", encoder: ".gif");

    private async Task<Image> EpicGifComposite(Image img, SdImage img2, Tuple<int, int, int>[] gaming )
    {
        if (img.Contains("n-pages") && img.Contains("page-height"))
        {
            if(!img.HasAlpha())
            {
                img = img.Bandjoin(255);
            }
            var img2r = LoadFromStream(await img2.GetByteStream(HttpClient));
            var nPages = (int)img.Get("n-pages");
            if(gaming.Length<nPages)
            {
                nPages = gaming.Length;
            }
            Dictionary<int, Image> imgss = new();
            var sizes = gaming.Select(x => x.Item3).Distinct();
            foreach(var size in sizes)
            {
                imgss[size] = img2r.Resize(((double)size)/img2r.Width);
            }
            List<Image> imgs = new();
            for (int i = 0; i < nPages; i++)
            {
                Image img_frame = img.Crop(0, i * img.PageHeight, img.Width, img.PageHeight);
                if(gaming[i].Item3!=0)
                {
                     img_frame = img_frame.Composite(imgss[gaming[i].Item3], BlendMode.Over, gaming[i].Item1, gaming[i].Item2);
                }
                imgs.Add(img_frame);
            }
            Image final = Image.Arrayjoin(imgs.ToArray(), 1);
            foreach (var img_frame in imgs)
            {
                img_frame.Dispose();
            }
            foreach (var img_frame in imgss)
            {
                img_frame.Value.Dispose();
            }
            return final;
        }
        return null;
    }

    [Command("jpeg")]
    public async Task Jpegize(CommandContext ctx, [Description("the url of the image")] SdImage image)
    {
        await ctx.TriggerTypingAsync();
        await using var outStream = JPEGSpecialSauce(await image.GetBytesAsync(HttpClient));
        await SendImageStreamIfAllowed(ctx, outStream, DisposeOfStream: true, "sbimg.jpeg", "There you go");
    }

    [Command("jpeg")]
    //[RequireAttachment(argumentCountThatOverloadsCheck: 1)]
    public async Task Jpegize(CommandContext ctx) => await Jpegize(ctx, SdImage.FromContext(ctx));
    private static Tuple<MemoryStream, string> Tint(Stream photoStream, Color color, string extension)
    {
        var img = LoadFromStream(photoStream, extension == ".gif");
        bool AlphaProcessing= extension!=".gif";
        if (!img.HasAlpha() && AlphaProcessing)
        {
            var imgwithoutbands = img;
            img = imgwithoutbands.Bandjoin(255);
            imgwithoutbands.Dispose();
        }
        var bands = img.Bandsplit();
        bands[0] = bands[0] * color.R / 255d;
        bands[1] = bands[1] * color.G / 255d;
        bands[2] = bands[2] * color.B / 255d;
        if(AlphaProcessing)
        {
            bands[3] = bands[3] * color.A / 255d;
            img = bands[0].Bandjoin(bands[1], bands[2], bands[3]);
        }
        else
        {
            img = bands[0].Bandjoin(bands[1], bands[2]);
        }
        var s = new MemoryStream
        {
            Position = 0
        };
        WriteImageToStream(img, s, extension);
        return new Tuple<MemoryStream, string>(s, extension);
    }
    [Command("tint")]
    // [RequireAttachment(argumentCountThatOverloadsCheck: 2)]

    public async Task Tint(CommandContext ctx, [Description("the url of the image")] SdImage image,
       [Description("A hex color (RGB OR RGBA), or a dotnet KnownColor")]
          Color color)
    {
        await ctx.TriggerTypingAsync();
        var thing = Tint(await image.GetByteStream(HttpClient), color, image.Url.GetFileExtensionFromUrl());
        await using var outStream = thing.Item1;
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, DisposeOfStream: true, $"sbimg{thing.Item2}", "There you go");
    }

    [Command("tint")]
    public async Task Tint(CommandContext ctx,
        [Description("A hex color (RGB OR RGBA), or a dotnet KnownColor")]
          Color color) => await Tint(ctx, SdImage.FromContext(ctx), color);
    [RequireGuild]
    [Command("adventuretime")]
    public async Task AdventureTime(CommandContext ctx) => await AdventureTime(ctx, ctx.Guild.CurrentMember);
    [Command("adventuretime")]
    public async Task AdventureTime(CommandContext ctx, DiscordUser friendo) => await AdventureTime(ctx, ctx.Member, friendo);
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
        await SendImageStreamIfAllowed(ctx, outStream, DisposeOfStream: true, content: $"adventure time come on grab your friends we will go to very distant lands with {person.Mention} the {(person.IsBot ? "bot" : "human")} and {friendo.Mention} the {(friendo.IsBot ? "bot" : "human")} the fun will never end its adventure time!");
    }
    private async Task CommonCodeWithTemplateGIFMagick(CommandContext ctx, string template, Func<ImageMagick.MagickImageCollection, Task<Tuple<bool, ImageMagick.MagickImageCollection>>> func, bool TriggerTyping = true, string filename = "sbimg.png", ImageMagick.MagickFormat? encoder = null, int quality = 75)
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
        await r.Item2.WriteAsync(outStream, encoder ?? ImageMagick.MagickFormat.Png);
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, DisposeOfStream: true, filename, "there");
    }
    [Command("seal")]
    [Description("He was forced to use Microsoft Windows when he was 6")]
    public async Task Seal(CommandContext ctx, [RemainingText] string text) =>
        await CommonCodeWithTemplate(ctx, "SilverBotDS.Templates.cement-seal-clear.gif", async (img) =>
        {
            return new Tuple<bool, Image>(true, await Caption(img, text));
        }, filename: "sbseal.gif", encoder: ".gif");

    [Command("linus")]
    [Description("NVIDIA, fuck you.")]
    public async Task Linus(CommandContext ctx, [RemainingText][Description("company,or thing you want linus to swear at")] string company = "NVIDIA") => 
        await CommonCodeWithTemplateGIFMagick(ctx, "SilverBotDS.Templates.linus-linus-torvalds.gif", (img) =>
        {
            //TODO PORT TO LIBVIPS
            MagickReadSettings settings = new()
            {
            FillColor = MagickColors.White,
            Font = _subtitlesFont,
            FontPointsize = 30,
            BackgroundColor = MagickColors.Transparent, //new MagickColor(0, 0, 0, 60)
            Width = 498,
            TextGravity = Gravity.Center,
            };
            using var label = new MagickImage($"caption:{$"so {company}, fuck you."}", settings);
            img[0].Composite(label, Gravity.South, 0, 0, CompositeOperator.Over);
            return Task.FromResult(new Tuple<bool, MagickImageCollection>(true, img));
        }, filename: "sblinus.gif", encoder: MagickFormat.Gif);

    [Command("resize")]
    public async Task Resize(CommandContext ctx, [Description("the url of the image")] SdImage image,
      [Description("Width")] int x = 0, [Description("Height")] int y = 0, MagickFormat? format = null)
    {
        await ctx.TriggerTypingAsync();
        var thing = ResizeAsyncOP(await image.GetBytesAsync(HttpClient), x, y, format);
        await SendImageStreamIfAllowed(ctx, thing.Item1, DisposeOfStream: true, $"sbimg.{thing.Item2}", "There you go");
    }

    private Tuple<Stream, string> ResizeAsyncOP(byte[] bytes, int x, int y, MagickFormat? format)
    {
        //TODO PORT TO LIBVIPS
        using var memStream = new MemoryStream(bytes);
        var memStream2 = new MemoryStream();

        if (IsAnimated(bytes))
        {
            using var picture = new MagickImageCollection(memStream);
            foreach (var image in picture)
            {
                image.Resize(x, y);
            }
            picture.Write(memStream2, format ?? MagickFormat.Gif);
            memStream2.Position = 0;
            return new(memStream2, (format ?? MagickFormat.Gif).ToString().ToLowerInvariant());
        }
        else
        {
            var picture = new MagickImage(memStream);
            picture.Resize(x, y);
            picture.Write(memStream2, format ?? picture.Format);
            memStream2.Position = 0;
            return new(memStream2, (format ?? picture.Format).ToString().ToLowerInvariant());
        }
    }

    [Command("resize")]
    public async Task Resize(CommandContext ctx, SdImage image, MagickFormat? format) => await Resize(ctx, image, 0, 0, format);

    [Command("resize")]
    public async Task Resize(CommandContext ctx, MagickFormat? format) => await Resize(ctx, SdImage.FromContext(ctx), 0, 0, format);

    [Command("resize")]
    public async Task Resize(CommandContext ctx, [Description("Width")] int x = 0, [Description("Height")] int y = 0,
        MagickFormat? format = null) => await Resize(ctx, SdImage.FromContext(ctx), x, y, format);
    [Command("reliable")]
    public async Task Reliable(CommandContext ctx) => await Reliable(ctx, ctx.User, ctx.Client.CurrentUser);

    [Command("reliable")]
    public async Task Reliable(CommandContext ctx, DiscordUser koichi) => await Reliable(ctx, ctx.User, koichi);

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
             "<span foreground=\"white\">" + HttpUtility.HtmlEncode(text) + "</span>",
            _subtitlesFont + ", Twitter Color Emoji normal " + 70,
            rgba: true,
            align: Align.Centre, fontfile: "twemoji.otf");
        textimga = textimga.Embed(5, 5, textimga.Width + 10, textimga.Height + 10);
        var textshadow = textimga[3].Gaussblur(1);
        textshadow *= 64;
        textshadow = textshadow.NewFromImage(new int[] { 0, 0, 0 }).Bandjoin(textshadow).Copy(interpretation: Interpretation.Srgb);
        textimga = textshadow.Composite(textimga, BlendMode.Over);
        img = img.Composite(textimga, BlendMode.Over, 960 - (textimga.Width / 2), 975);
        await using MemoryStream outStream = new();
        outStream.Position = 0;
        img.WriteToStream(outStream, ".png");
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, DisposeOfStream: true, content: $"{jotaro.Mention}: {koichi.Mention}, you truly are a reliable guy.");
    }
    [Command("ObMedal")]
    public Task ObMedal(CommandContext ctx)
    {
        return ObMedal(ctx, ctx.User);
    }
    [Command("ObMedal")]
    public async Task ObMedal(CommandContext ctx, DiscordUser obama)
    {
        await CommonCodeWithTemplate(ctx, "SilverBotDS.Templates.obamamedal.jpg", async (img) =>
        {
            using (var internalimage = await GetProfilePictureAsyncStatic(obama))
            {
                var i = img;
                img = img.Composite2(internalimage, BlendMode.Over, 120, 62);
                img = img.Composite2(internalimage, BlendMode.Over, 377, 3);
                i.Dispose();
            }
            return new Tuple<bool, Image>(true, img);
        }, msgcontent: $"{obama.Mention} Awards {obama.Mention} a Medal.");
    }
    [Command("ObMedal")]
    public async Task ObMedal(CommandContext ctx, DiscordUser obama, DiscordUser secondPerson)
    {
        await CommonCodeWithTemplate(ctx, "SilverBotDS.Templates.obamamedal.jpg", async (img) =>
        {
            using (var internalimage = await GetProfilePictureAsyncStatic(obama))
            {
                var i = img;
                img = img.Composite2(internalimage, BlendMode.Over, 120, 62);
                i.Dispose();
            }
            using (var internalimage = await GetProfilePictureAsyncStatic(secondPerson))
            {
                var i = img;
                img = img.Composite2(internalimage, BlendMode.Over, 377, 3);
                i.Dispose();
            }
            return new Tuple<bool, Image>(true, img);
        }, msgcontent: $"{obama.Mention} Awards {obama.Mention} a Medal.");
    }
    [Command("happynewyear")]
    public async Task HappyNewYear(CommandContext ctx) => await HappyNewYear(ctx, ctx.User);
    [Command("happynewyear")]
    public async Task HappyNewYear(CommandContext ctx, DiscordUser person) =>
        await CommonCodeWithTemplate(ctx, "SilverBotDS.Templates.happy_new_year_template.png", async (img) =>
        {
            using var a = (await GetProfilePictureAsyncStatic(person, 350)).Embed(19, 70, img.Width, img.Height, Extend.Black);
            return new Tuple<bool, Image>(true, a.Composite2(img, BlendMode.Over));
        }, filename: "sbnewyear.png", encoder: ".png");


    private static async Task<Tuple<MemoryStream, string>> GrayScaleAsync(byte[] photoBytes, string extension)
    {
        var instream = new MemoryStream(photoBytes);
        var img = LoadFromStream(instream).Colourspace(Interpretation.Bw);
        var outstream = new MemoryStream();
        WriteImageToStream(img, outstream, extension);
        return new Tuple<MemoryStream, string>(outstream, extension);
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
        var thing = await GrayScaleAsync(await image.GetBytesAsync(HttpClient), image.Url.GetFileExtensionFromUrl());
        var outStream = thing.Item1;
        outStream.Position = 0;
        await SendImageStreamIfAllowed(ctx, outStream, DisposeOfStream: true, $"sbimg{thing.Item2}", "There you go");
    }



}