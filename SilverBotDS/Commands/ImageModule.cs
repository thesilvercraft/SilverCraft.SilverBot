using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using SilverBotDS.Converters;
using SilverBotDS.Exceptions;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Size = SixLabors.ImageSharp.Size;
using SImage = SixLabors.ImageSharp.Image;
using SColor = SixLabors.ImageSharp.Color;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using Point = System.Drawing.Point;
using SolidBrush = System.Drawing.SolidBrush;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;
using SixLabors.ImageSharp.Formats.Jpeg;
using static SilverBotDS.Commands.ImageModule;
using RectangleF = System.Drawing.RectangleF;
using PointF = System.Drawing.PointF;
using SizeF = System.Drawing.SizeF;
using System.Drawing.Imaging;
using Rectangle = System.Drawing.Rectangle;
using ImageFormat = DSharpPlus.ImageFormat;

namespace SilverBotDS.Commands
{
    [Cooldown(1, 2, CooldownBucketType.User)]
    public class ImageModule : BaseCommandModule
    {
#pragma warning disable CA1822 // Mark members as static

        private const int MaxBytes = 8388246;

        public HttpClient HttpClient { private get; set; }

        private static async Task Sendcorrectamountofimages(CommandContext ctx, AttachmentCountIncorrect e, Language lang = null)
        {
            lang ??= await Language.GetLanguageFromCtxAsync(ctx);
            if (e == AttachmentCountIncorrect.TooLittleAttachments)
            {
                await Send_img_plsAsync(ctx, lang.NoImageGeneric, lang).ConfigureAwait(false);
            }
            else
            {
                await Send_img_plsAsync(ctx, lang.MoreThanOneImageGeneric, lang).ConfigureAwait(false);
            }
        }

        public static async Task SendImageStream(CommandContext ctx, Stream outstream, string filename = "sbimg.png", string content = null, Language lang = null)
        {
            lang ??= await Language.GetLanguageFromCtxAsync(ctx);
            content ??= lang.Success;
            await new DiscordMessageBuilder().WithContent(content).WithFile(filename, outstream).WithReply(ctx.Message.Id).WithAllowedMentions(Mentions.None).SendAsync(ctx.Channel);
        }

        public static async Task<MemoryStream> ResizeAsync(byte[] photoBytes, Size size)
        {
            using SImage img = SImage.Load(photoBytes);
            img.Mutate(x => x.Resize(size));
            MemoryStream stream = new();
            await img.SaveAsPngAsync(stream);
            stream.Position = 0;
            return stream;
        }

        private static readonly JpegEncoder BadJpegEncoder = new()
        {
            Quality = 1
        };

        private static async Task<MemoryStream> Make_jpegnisedAsync(byte[] photoBytes)
        {
            using SImage img = SImage.Load(photoBytes);
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

        private static async Task<MemoryStream> TintAsync(byte[] photoBytes, SColor color)
        {
            var instream = new MemoryStream(photoBytes);
            using var img = Image.FromStream(instream);
            await instream.DisposeAsync();
            var pixel = color.ToPixel<Rgba32>();
            using var newImage = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppPArgb);
            newImage.SetResolution(img.HorizontalResolution, img.VerticalResolution);
            float[][] colorMatrixElements =
                     {
                        new[] { pixel.R / 255f, 0, 0, 0, 0 }, // Red
                        new[] { 0, pixel.G / 255f, 0, 0, 0 }, // Green
                        new[] { 0, 0, pixel.B / 255f, 0, 0 }, // Blue
                        new[] { 0, 0, 0, pixel.A / 255f, 0 }, // Alpha
                        new float[] { 0, 0, 0, 0, 1 }
                    };
            using (var graphics = Graphics.FromImage(newImage))
            {
                using var attributes = new ImageAttributes();
                var colorMatrix = new System.Drawing.Imaging.ColorMatrix(colorMatrixElements);
                attributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                graphics.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attributes);
            }
            instream = new MemoryStream
            {
                Position = 0
            };
            newImage.Save(instream, System.Drawing.Imaging.ImageFormat.Png);
            instream.Position = 0;
            return instream;
        }

        public enum EFilter
        {
            Grayscale,
            Comic
        }

        /// <summary>
        /// Filters an image
        /// </summary>
        /// <param name="photoBytes">The <see cref="byte"/> of the original picture</param>
        /// <param name="filter">The <see cref="IMatrixFilter"/> to use</param>
        /// <returns>A ,<see cref="MemoryStream"/> with a <see cref="PngFormat"/> of 70 Quality</returns>
        private static MemoryStream Filter(byte[] photoBytes, EFilter filter)
        {
            using SImage img = SImage.Load(photoBytes);
            if (filter == EFilter.Grayscale)
            {
                img.Mutate(x => x.Grayscale());
            }
            MemoryStream stream = new();
            img.SaveAsPngAsync(stream);
            return stream;
        }

        [Command("template")]
        public async Task Template(CommandContext ctx)
        {
            if (ctx.Message.Attachments.Count == 1)
            {
                ImageSteps e = await ImageSteps.Create(ctx.Message.Attachments[0].Url, HttpClient);
                var steps = new List<Step>(); var interactivity = ctx.Client.GetInteractivity();
                foreach (var step in e.steps)
                {
                    if (step is TemplateStep)
                    {
                        steps.Add(step);
                    }
                    else if (step is PictureStep step1)
                    {
                        if (step1.isPfp)
                        {
                            await ctx.RespondAsync(new DiscordMessageBuilder().WithContent($"Ping or send an id of the person you want in the photo step({steps.Count + 1} out of {e.steps.Length} steps)"));
                            var msg = await interactivity.WaitForMessageAsync((a) => { return a.Author.Id == ctx.User.Id; });
                            if (msg.TimedOut)
                            {
                                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("Timed out"));
                                return;
                            }
                            if (msg.Result.MentionedUsers.Count == 1)
                            {
                                steps.Add(new PictureStep(step1.x, step1.y, step1.xSize, step1.ySize, msg.Result.MentionedUsers[0].GetAvatarUrl(ImageFormat.Png), true));
                                continue;
                            }
                            var user = await ctx.Client.GetUserAsync(Convert.ToUInt64(msg.Result.Content));
                            if (user != null)
                            {
                                steps.Add(new PictureStep(step1.x, step1.y, step1.xSize, step1.ySize, user.GetAvatarUrl(ImageFormat.Png), true));
                            }
                        }
                        else
                        {
                            await ctx.RespondAsync(new DiscordMessageBuilder().WithContent($"Send an image you want in the image step ({steps.Count + 1} out of {e.steps.Length} steps)"));
                            var msg = await interactivity.WaitForMessageAsync((a) => { return a.Author.Id == ctx.User.Id; });
                            if (msg.TimedOut)
                            {
                                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("Timed out"));
                                return;
                            }
                            var Sdimg = SdImage.FromAttachments(msg.Result.Attachments);
                            if (Sdimg != null)
                            {
                                steps.Add(new PictureStep(step1.x, step1.y, step1.xSize, step1.ySize, Sdimg.Url, false));
                            }
                        }
                    }
                    else
                    {
                        //bug oh no
                    }
                }
                using var outStream = new MemoryStream();
                using (var exec = await e.ExecuteStepsAsync(steps.ToArray()))
                {
                    exec.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
                }
                outStream.Position = 0;
                await SendImageStream(ctx, outStream, "silverbotimage.png");
            }
            else
            {
                if (ctx.Message.Attachments.Count == 0)
                {
                    await Sendcorrectamountofimages(ctx, AttachmentCountIncorrect.TooLittleAttachments);
                }
                else
                {
                    await Sendcorrectamountofimages(ctx, AttachmentCountIncorrect.TooMuchAttachments);
                }
            }
        }

        [Command("jpeg")]
        public async Task Jpegize(CommandContext ctx, [Description("the url of the image")] SdImage image)
        {
            await ctx.TriggerTypingAsync();
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            await using var outStream = await Make_jpegnisedAsync(await image.GetBytesAsync(HttpClient));
            if (outStream.Length > MaxBytes)
            {
                await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang).ConfigureAwait(false);
            }
            else
            {
                await SendImageStream(ctx, outStream, "silverbotimage.jpeg", lang.Imagethings.JpegSuccess, lang);
            }
        }

        [Command("jpeg")]
        public async Task Jpegize(CommandContext ctx)
        {
            try
            {
                var image = SdImage.FromContext(ctx);
                await Jpegize(ctx, image);
            }
            catch (AttachmentCountIncorrectException acie)
            {
                await Sendcorrectamountofimages(ctx, acie.AttachmentCount);
            }
        }

        [Command("resize")]
        public async Task Resize(CommandContext ctx, [Description("the url of the image")] SdImage image, [Description("Width")] int x, [Description("Height")] int y)
        {
            await ctx.TriggerTypingAsync();
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            await using var outStream = await ResizeAsync(await image.GetBytesAsync(HttpClient), new Size(x, y));
            if (outStream.Length > MaxBytes)
            {
                await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang).ConfigureAwait(false);
            }
            else
            {
                await SendImageStream(ctx, outStream, content: lang.Imagethings.ResizeSuccess, lang: lang);
            }
        }

        [Command("resize")]
        public async Task Resize(CommandContext ctx, [Description("Width")] int x, [Description("Height")] int y)
        {
            try
            {
                var image = SdImage.FromContext(ctx);
                await Resize(ctx, image, x, y);
            }
            catch (AttachmentCountIncorrectException acie)
            {
                await Sendcorrectamountofimages(ctx, acie.AttachmentCount);
            }
        }

        [Command("tint")]
        public async Task Tint(CommandContext ctx, [Description("the url of the image")] SdImage image, [Description("https://docs.sixlabors.com/api/ImageSharp/SixLabors.ImageSharp.Color.html#SixLabors_ImageSharp_Color_TryParse_System_String_SixLabors_ImageSharp_Color__")] SColor color)
        {
            await ctx.TriggerTypingAsync();
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            await using var outStream = await TintAsync(await image.GetBytesAsync(HttpClient), color);
            outStream.Position = 0;
            if (outStream.Length > MaxBytes)
            {
                await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang).ConfigureAwait(false);
            }
            else
            {
                await SendImageStream(ctx, outStream, content: lang.Imagethings.TintSuccess, lang: lang);
            }
        }

        [Command("tint")]
        public async Task Tint(CommandContext ctx, [Description("https://docs.sixlabors.com/api/ImageSharp/SixLabors.ImageSharp.Color.html#SixLabors_ImageSharp_Color_TryParse_System_String_SixLabors_ImageSharp_Color__")] SColor color)
        {
            try
            {
                var image = SdImage.FromContext(ctx);
                await Tint(ctx, image, color);
            }
            catch (AttachmentCountIncorrectException acie)
            {
                await Sendcorrectamountofimages(ctx, acie.AttachmentCount);
            }
        }

        [Command("silver")]
        public async Task Grayscale(CommandContext ctx)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            try
            {
                var image = SdImage.FromContext(ctx);
                await Grayscale(ctx, image);
            }
            catch (AttachmentCountIncorrectException acie)
            {
                await Sendcorrectamountofimages(ctx, acie.AttachmentCount, lang);
            }
        }

        [Command("silver")]
        public async Task Grayscale(CommandContext ctx, [Description("the url of the image")] SdImage image)
        {
            await ctx.TriggerTypingAsync();
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            await using var outStream = Filter(await image.GetBytesAsync(HttpClient), EFilter.Grayscale);
            outStream.Position = 0;
            if (outStream.Length > MaxBytes)
            {
                await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang).ConfigureAwait(false);
            }
            else
            {
                await SendImageStream(ctx, outStream, content: lang.Imagethings.SilverSuccess, lang: lang);
            }
        }

        /// <summary>
        /// Renders some text
        /// </summary>
        /// <param name="text">the text to render</param>
        /// <param name="font">the font to use</param>
        /// <param name="textColor">the color to use while rendering the text</param>
        /// <param name="backColor">the background color to use</param>
        /// <returns>An <see cref="Image"/></returns>
        public static Image DrawText(string text, Font font, Color textColor, Color backColor)
        {
            Image img = new Bitmap(1, 1);
            var drawing = Graphics.FromImage(img);
            var textSize = drawing.MeasureString(text, font);
            img.Dispose();
            drawing.Dispose();
            img = new Bitmap((int)textSize.Width, (int)textSize.Height);
            drawing = Graphics.FromImage(img);
            drawing.Clear(backColor);
            Brush textBrush = new SolidBrush(textColor);
            drawing.DrawString(text, font, textBrush, 0, 0);
            drawing.Save();
            textBrush.Dispose();
            drawing.Dispose();
            return img;
        }

        [Command("text")]
        public async Task Text(CommandContext ctx, [Description("the text")] string text, string font = "Diavlo Light", float size = 30.0f)
        {
            await ctx.TriggerTypingAsync();
            var img = DrawText(text, new Font(font, size), Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255));
            await using var outStream = new MemoryStream();
            img.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
            outStream.Position = 0;
            await SendImageStream(ctx, outStream);
        }

        private Bitmap cachedmotivatetemplate;
        private Bitmap cachedadventuretimetemplate;
        private Bitmap cachednewyeartemplate;
        private Bitmap cachedweebreliabletemplate;
        private Bitmap cachedpaintreliabletemplate;

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
            try
            {
                if (cachedweebreliabletemplate == null)
                {
                    var myAssembly = Assembly.GetExecutingAssembly();
                    await using var myStream = myAssembly.GetManifestResourceStream("SilverBotDS.Templates.weeb_reliable_template.png");
                    cachedweebreliabletemplate = new Bitmap(myStream ?? throw new TemplateReturningNullException("SilverBotDS.Templates.weeb_reliable_template.png"));
                }
                await using MemoryStream resizedstreamb = new(await GetProfilePictureAsync(koichi, 256));
                await using MemoryStream resizedstreama = new(await GetProfilePictureAsync(jotaro, 256));
                using var copythingy = new Bitmap(cachedweebreliabletemplate.Width, cachedweebreliabletemplate.Height);
                var drawing = Graphics.FromImage(copythingy);
                drawing.DrawImageUnscaled(cachedweebreliabletemplate, new Point(0, 0));
                using (Bitmap internalimage = new(resizedstreama))
                {
                    drawing.DrawImageUnscaled(internalimage, new Point(276, 92));
                }
                using (Bitmap internalimage = new(resizedstreamb))
                {
                    drawing.DrawImageUnscaled(internalimage, new Point(1138, 369));
                }
                var text = (ctx.Guild?.Members?.ContainsKey(koichi.Id) != null && ctx.Guild?.Members?[koichi.Id].Nickname != null ? ctx.Guild?.Members?[koichi.Id].Nickname : koichi.Username) + ", you truly are a reliable guy.";
                var font = new Font("Trebuchet MS", 100);
                var sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                };
                while (drawing.MeasureString(text,
        new Font(font.FontFamily, font.Size, font.Style)).Width > 1300)
                {
                    font = new Font(font.FontFamily, font.Size - 1f, font.Style);
                }

                var p = new GraphicsPath();
                p.AddString(
                    text,
                    font.FontFamily,
                    (int)font.Style,
                    drawing.DpiY * font.Size / 72,
                    new Rectangle(267, 894, 1370, 186),
                    sf);
                drawing.DrawPath(new System.Drawing.Pen(new SolidBrush(Color.Black), 2), p);
                drawing.FillPath(new SolidBrush(Color.White), p);
                drawing.Save();
                await using MemoryStream outStream = new();
                outStream.Position = 0;
                copythingy.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
                outStream.Position = 0;
                if (outStream.Length > MaxBytes)
                {
                    await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang).ConfigureAwait(false);
                }
                else
                {
                    await SendImageStream(ctx, outStream, content: $"{jotaro.Mention}: {koichi.Mention}, you truly are a reliable guy.", lang: lang);
                }
            }
            catch (Exception e)
            {
                Program.SendLog(e);
                throw;
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
            try
            {
                if (cachednewyeartemplate == null)
                {
                    var myAssembly = Assembly.GetExecutingAssembly();
                    await using var myStream = myAssembly.GetManifestResourceStream("SilverBotDS.Templates.happy_new_year_template.png");
                    cachednewyeartemplate = new Bitmap(myStream ?? throw new TemplateReturningNullException("SilverBotDS.Templates.happy_new_year_template.png"));
                }
                await using MemoryStream resizedstreama = new(await GetProfilePictureAsync(person, 350));
                using var copythingy = new Bitmap(cachednewyeartemplate.Width, cachednewyeartemplate.Height);
                var drawing = Graphics.FromImage(copythingy);
                using (Bitmap internalimage = new(resizedstreama))
                {
                    drawing.DrawImageUnscaled(internalimage, new Point(19, 70));
                }
                drawing.DrawImageUnscaled(cachednewyeartemplate, new Point(0, 0));
                drawing.Save();
                await using MemoryStream outStream = new();
                outStream.Position = 0;
                copythingy.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
                outStream.Position = 0;
                if (outStream.Length > MaxBytes)
                {
                    await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang).ConfigureAwait(false);
                }
                else
                {
                    await SendImageStream(ctx, outStream, content: "happy new year!", lang: lang);
                }
            }
            catch (Exception e)
            {
                Program.SendLog(e);
                throw;
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
            try
            {
                if (cachedadventuretimetemplate == null)
                {
                    var myAssembly = Assembly.GetExecutingAssembly();

                    await using var myStream = myAssembly.GetManifestResourceStream("SilverBotDS.Templates.adventure_time_template.png");
                    cachedadventuretimetemplate = new Bitmap(myStream ?? throw new TemplateReturningNullException("SilverBotDS.Templates.adventure_time_template.png"));
                }

                await using MemoryStream resizedstreama = new(await GetProfilePictureAsync(person));
                await using MemoryStream resizedstreamb = new(await GetProfilePictureAsync(friendo));
                using var copythingy = new Bitmap(cachedadventuretimetemplate);
                var drawing = Graphics.FromImage(copythingy);

                using (Bitmap internalimage = new(resizedstreamb))
                {
                    drawing.DrawImageUnscaled(internalimage, new Point(22, 948));
                }

                using (Bitmap internalimage = new(resizedstreama))
                {
                    drawing.DrawImageUnscaled(internalimage, new Point(557, 699));
                }
                drawing.Save();
                await using MemoryStream outStream = new();
                copythingy.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
                outStream.Position = 0;
                if (outStream.Length > MaxBytes)
                {
                    await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang).ConfigureAwait(false);
                }
                else
                {
                    await SendImageStream(ctx, outStream, content: $"adventure time come on grab your friends we will go to very distant lands with {person.Mention} the {(person.IsBot ? "bot" : "human")} and {friendo.Mention} the {(friendo.IsBot ? "bot" : "human")} the fun will never end its adventure time!", lang: lang);
                }
            }
            catch (Exception e)
            {
                Program.SendLog(e);
                throw;
            }
        }

        /// <summary>
        /// Gets the profile picture of a discord user in a 256x256 bitmap saved to a byte array
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
            await using MemoryStream stream = new(await new SdImage(user.GetAvatarUrl(ImageFormat.Png, discordsize)).GetBytesAsync(HttpClient));
            using Bitmap image = new(stream);
            if (image.Width == size || image.Height == size)
            {
                stream.Position = 0;
                return stream.ToArray();
            }
            else
            {
                stream.Position = 0;
                await using var resizedstream = await ResizeAsync(stream.ToArray(), new(size, size));
                resizedstream.Position = 0;
                return resizedstream.ToArray();
            }
        }

        [Command("mspaint")]
        public async Task Paint(CommandContext ctx, SdImage image)
        {
            await ctx.TriggerTypingAsync();
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            if (cachedpaintreliabletemplate == null)
            {
                var myAssembly = Assembly.GetExecutingAssembly();
                await using var myStream = myAssembly.GetManifestResourceStream("SilverBotDS.Templates.paint_template.png");
                cachedpaintreliabletemplate = new Bitmap(myStream ?? throw new TemplateReturningNullException("SilverBotDS.Templates.paint_template.png"));
            }
            await using var resizedstream = await ResizeAsync(await image.GetBytesAsync(HttpClient), new Size(1771, 984));
            using var copythingy = new Bitmap(cachedpaintreliabletemplate);
            var drawing = Graphics.FromImage(copythingy);
            using (Bitmap internalimage = new(resizedstream))
            {
                drawing.DrawImageUnscaled(internalimage, new Point(132, 95));
            }

            drawing.Save();
            await using MemoryStream outStream = new();
            copythingy.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
            outStream.Position = 0;
            if (outStream.Length > MaxBytes)
            {
                await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang).ConfigureAwait(false);
            }
            else
            {
                await SendImageStream(ctx, outStream, filename: "SilverPaint.png", content: "untitled - Paint", lang: lang);
            }
        }

        [Command("mspaint")]
        public async Task Paint(CommandContext ctx)
        {
            try
            {
                var image = SdImage.FromContext(ctx);
                await Paint(ctx, image);
            }
            catch (AttachmentCountIncorrectException acie)
            {
                await Sendcorrectamountofimages(ctx, acie.AttachmentCount);
            }
        }

        [Command("motivate")]
        public async Task Motivate(CommandContext ctx, SdImage image, [RemainingText] string text)
        {
            await ctx.TriggerTypingAsync();
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            if (cachedmotivatetemplate == null)
            {
                var myAssembly = Assembly.GetExecutingAssembly();
                await using var myStream = myAssembly.GetManifestResourceStream("SilverBotDS.Templates.motivator_template.png");
                cachedmotivatetemplate = new Bitmap(myStream ?? throw new TemplateReturningNullException("SilverBotDS.Templates.motivator_template.png"));
            }
            var font = new Font("Times New Roman", 100);
            await using var resizedstream = await ResizeAsync(await image.GetBytesAsync(HttpClient), new Size(1027, 684));
            using var copythingy = new Bitmap(cachedmotivatetemplate);
            var drawing = Graphics.FromImage(copythingy);
            using (Bitmap internalimage = new(resizedstream))
            {
                drawing.DrawImageUnscaled(internalimage, new Point(126, 83));
            }
            var sf = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };
            while (drawing.MeasureString(text,
        new Font(font.FontFamily, font.Size, font.Style)).Width > 1041)
            {
                font = new Font(font.FontFamily, font.Size - 0.5f, font.Style);
            }
            drawing.DrawString(text, font, new SolidBrush(Color.White), new RectangleF(new PointF(119, 793), new SizeF(1041, 209)), sf);
            drawing.Save();
            await using MemoryStream outStream = new();
            outStream.Position = 0;
            copythingy.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
            outStream.Position = 0;
            if (outStream.Length > MaxBytes)
            {
                await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang).ConfigureAwait(false);
            }
            else
            {
                await SendImageStream(ctx, outStream, content: "there", lang: lang);
            }
        }

        [Command("motivate")]
        public async Task Motivate(CommandContext ctx, [RemainingText] string text)
        {
            try
            {
                var image = SdImage.FromContext(ctx);
                await Motivate(ctx, image, text);
            }
            catch (AttachmentCountIncorrectException acie)
            {
                await Sendcorrectamountofimages(ctx, acie.AttachmentCount);
            }
        }

        private readonly StringFormat stringFormat = new()
        {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Center
        };

        [Command("caption")]
        public async Task Caption(CommandContext ctx, SdImage image, [RemainingText] string text)
        {
            await ctx.TriggerTypingAsync();
            await using var inStream = new MemoryStream(await image.GetBytesAsync(HttpClient));
            using var bitmap = new Bitmap(inStream);
            int x = bitmap.Width, y = bitmap.Height;
            var font = new Font("Impact", x / 10);
            await using var outStream = new MemoryStream();
            SizeF textSize;
            using (Image img = new Bitmap(1, 1))
            {
                using var draw = Graphics.FromImage(img);
                textSize = draw.MeasureString(text, font, x);
            }
            using (var img2 = new Bitmap(x, y + (int)textSize.Height))
            {
                using (var draw2 = Graphics.FromImage(img2))
                {
                    draw2.Clear(Color.FromArgb(255, 255, 255));
                    draw2.DrawString(text, font, new SolidBrush(Color.FromArgb(0, 0, 0)), new RectangleF(new PointF(0, 0), new SizeF(x, textSize.Height)), stringFormat);
                    draw2.DrawImage(bitmap, new Point(0, (int)textSize.Height));
                    draw2.Save();
                }
                img2.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
            }
            outStream.Position = 0;
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            if (outStream.Length > MaxBytes)
            {
                await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang).ConfigureAwait(false);
            }
            else
            {
                await SendImageStream(ctx, outStream, content: "there", lang: lang);
            }
        }

        [Command("caption")]
        public async Task Caption(CommandContext ctx, [RemainingText] string text)
        {
            try
            {
                var image = SdImage.FromContext(ctx);
                await Caption(ctx, image, text);
            }
            catch (AttachmentCountIncorrectException acie)
            {
                await Sendcorrectamountofimages(ctx, acie.AttachmentCount);
            }
        }
    }
}