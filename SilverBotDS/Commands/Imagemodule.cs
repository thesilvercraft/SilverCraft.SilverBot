using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ImageProcessor;
using ImageProcessor.Imaging.Filters.Photo;
using ImageProcessor.Imaging.Formats;
using SilverBotDS.Converters;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    [Cooldown(1, 2, CooldownBucketType.User)]
    public class ImageModule : BaseCommandModule
    {
#pragma warning disable CA1822 // Mark members as static

        private const int MaxBytes = 8388246;

        private const int Quality = 70;

        private static async Task Sendcorrectamountofimages(CommandContext ctx, AttachmentCountIncorrect e, Language lang = null)
        {
            lang ??= (await Language.GetLanguageFromCtxAsync(ctx));
            if (e == AttachmentCountIncorrect.TooLittleAttachments)
            {
                await Send_img_plsAsync(ctx, lang.NoImageGeneric).ConfigureAwait(false);
            }
            else
            {
                await Send_img_plsAsync(ctx, lang.MoreThanOneImageGeneric).ConfigureAwait(false);
            }
        }

        private static MemoryStream Resize(byte[] photoBytes, Size size)
        {
            ISupportedImageFormat format = new PngFormat { Quality = Quality };
            using var inStream = new MemoryStream(photoBytes);
            var outStream = new MemoryStream();
            using var imageFactory = new ImageFactory();
            imageFactory.Load(inStream)
                .Format(format)
                .Resize(size)
                .Save(outStream);
            return outStream;
        }

        private static MemoryStream Make_jpegnised(byte[] photoBytes)
        {
            ISupportedImageFormat format = new JpegFormat { Quality = 1 };
            using var inStream = new MemoryStream(photoBytes);
            var outStream = new MemoryStream();
            using var imageFactory = new ImageFactory();
            imageFactory.Load(inStream)
                .Format(format)
                .Save(outStream);
            return outStream;
        }

        private static MemoryStream Shet_On(byte[] photoBytes)
        {
            ISupportedImageFormat format = new JpegFormat { Quality = 1 };
            using var inStream = new MemoryStream(photoBytes);
            var outStream = new MemoryStream();
            using var imageFactory = new ImageFactory();
            imageFactory.Load(inStream);
            int x = imageFactory.Image.Width, y = imageFactory.Image.Height;
            imageFactory.Format(format)
                .Resize(new Size(imageFactory.Image.Width / 4, imageFactory.Image.Height / 4))
                .Resize(new Size(x, y))
                .Save(outStream);
            return outStream;
        }

        private static async Task Send_img_plsAsync(CommandContext ctx, string e)
        {
            //ToDo if possible make it not get the language 2 times
            Language lang = (await Language.GetLanguageFromCtxAsync(ctx));
            await new DiscordMessageBuilder()
                                             .WithReply(ctx.Message.Id)
                                             .WithEmbed(new DiscordEmbedBuilder()
            .WithTitle(lang.WrongImageCount).WithDescription(e)
            .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)))
                                             .SendAsync(ctx.Channel);
        }

        private static MemoryStream Tint(byte[] photoBytes, string color)
        {
            ISupportedImageFormat format = new PngFormat { Quality = Quality };
            using var inStream = new MemoryStream(photoBytes);
            var outStream = new MemoryStream();
            using var imageFactory = new ImageFactory();
            imageFactory.Load(inStream)
                .Tint(ColorTranslator.FromHtml(color))
                .Format(format)
                .Save(outStream);
            return outStream;
        }

        /// <summary>
        /// Filters an image
        /// </summary>
        /// <param name="photoBytes">The <see cref="byte"/> of the original picture</param>
        /// <param name="filter">The <see cref="IMatrixFilter"/> to use</param>
        /// <returns>A ,<see cref="MemoryStream"/> with a <see cref="PngFormat"/> of 70 Quality</returns>
        private static MemoryStream Filter(byte[] photoBytes, IMatrixFilter filter)
        {
            ISupportedImageFormat format = new PngFormat { Quality = Quality };
            using var inStream = new MemoryStream(photoBytes);
            var outStream = new MemoryStream();
            using var imageFactory = new ImageFactory();
            imageFactory.Load(inStream)
                .Filter(filter)
                .Format(format)
                .Save(outStream);
            return outStream;
        }

        [Command("jpeg")]
        public async Task Jpegize(CommandContext ctx, [Description("the url of the image")] SdImage image)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            await using var outStream = Make_jpegnised(await image.GetBytesAsync());
            if (outStream.Length > MaxBytes)
            {
                await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length))).ConfigureAwait(false);
            }
            else
            {
                await new DiscordMessageBuilder().WithContent(lang.Imagethings.JpegSuccess).WithFile("silverbotimage.jpeg", outStream).SendAsync(ctx.Channel);
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

        [Command("shet")]
        public async Task Shet(CommandContext ctx, [Description("the url of the image")] SdImage image)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            await using var outStream = Shet_On(await image.GetBytesAsync());
            if (outStream.Length > MaxBytes)
            {
                await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length))).ConfigureAwait(false);
            }
            else
            {
                await new DiscordMessageBuilder().WithContent("there is your masterpiece").WithFile("silverbotimage.jpeg", outStream).SendAsync(ctx.Channel);
            }
        }

        [Command("shet")]
        public async Task Shet(CommandContext ctx)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            try
            {
                var image = SdImage.FromContext(ctx);
                await Shet(ctx, image);
            }
            catch (AttachmentCountIncorrectException acie)
            {
                await Sendcorrectamountofimages(ctx, acie.AttachmentCount, lang);
            }
        }

        [Command("resize")]
        public async Task Resize(CommandContext ctx, [Description("the url of the image")] SdImage image, [Description("Width")] int x, [Description("Height")] int y)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            await using var outStream = Resize(await image.GetBytesAsync(), new Size(x, y));
            if (outStream.Length > MaxBytes)
            {
                await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length))).ConfigureAwait(false);
            }
            else
            {
                await new DiscordMessageBuilder().WithContent(lang.Imagethings.ResizeSuccess).WithFile("silverbotimage.png", outStream).SendAsync(ctx.Channel);
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
        public async Task Tint(CommandContext ctx, [Description("the url of the image")] SdImage image, [Description("color in hex like #fffff")] string color)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            await using var outStream = Tint(await image.GetBytesAsync(), color);
            if (outStream.Length > MaxBytes)
            {
                await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length))).ConfigureAwait(false);
            }
            else
            {
                await new DiscordMessageBuilder().WithContent(lang.Imagethings.TintSuccess).WithFile("silverbotimage.png", outStream).SendAsync(ctx.Channel);
            }
        }

        [Command("tint")]
        public async Task Tint(CommandContext ctx, [Description("color in hex like #fffff")] string color)
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
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
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
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            var outStream = Filter(await image.GetBytesAsync(), MatrixFilters.GreyScale);
            if (outStream.Length > MaxBytes)
            {
                await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length))).ConfigureAwait(false);
            }
            else
            {
                await new DiscordMessageBuilder().WithContent(lang.Imagethings.SilverSuccess).WithFile("silverbotimage.png", outStream).SendAsync(ctx.Channel);
            }
        }

        [Command("comic")]
        public async Task Comic(CommandContext ctx)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            try
            {
                var image = SdImage.FromContext(ctx);
                await Comic(ctx, image);
            }
            catch (AttachmentCountIncorrectException acie)
            {
                await Sendcorrectamountofimages(ctx, acie.AttachmentCount, lang);
            }
        }

        [Command("comic")]
        public async Task Comic(CommandContext ctx, [Description("the url of the image")] SdImage image)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            var outStream = Filter(await image.GetBytesAsync(), MatrixFilters.Comic);
            if (outStream.Length > MaxBytes)
            {
                await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length))).ConfigureAwait(false);
            }
            else
            {
                await new DiscordMessageBuilder().WithContent(lang.Imagethings.ComicSuccess).WithFile("silverbotimage.png", outStream).SendAsync(ctx.Channel);
            }
        }

        private static Image DrawText(string text, Font font, Color textColor, Color backColor)
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
        public async Task Text(CommandContext ctx, [Description("the text")] string text, string font = "Diavlo Light")
        {
            var img = DrawText(text, new Font(font, 30.0f), Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255));
            await using var outStream = new MemoryStream();
            img.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
            outStream.Position = 0;
            await new DiscordMessageBuilder().WithContent("there").WithFile("silverbotimage.png", outStream).SendAsync(ctx.Channel);
        }

        private static Bitmap cachedmotivatetemplate;
        private static Bitmap cachedadventuretimetemplate;
        private static Bitmap cachednewyeartemplate;
        private static Bitmap cachedweebreliabletemplate;

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
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            try
            {
                if (cachedweebreliabletemplate == null)
                {
                    var myAssembly = Assembly.GetExecutingAssembly();
                    var myStream = myAssembly.GetManifestResourceStream("SilverBotDS.Templates.weeb_reliable_template.png");
                    if (myStream is null)
                    {
                        Program.SendLog("myAssembly.GetManifestResourceStream(SilverBotDS.Templates.weeb_reliable_template.png) returned null");
                    }
                    cachedweebreliabletemplate = new Bitmap(myStream ?? throw new InvalidOperationException());
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

                GraphicsPath p = new GraphicsPath();
                var rectangle = new Rectangle(267, 894, 1370, 186);
                p.AddString(
                    text,
                    font.FontFamily,
                    (int)font.Style,
                    drawing.DpiY * font.Size / 72,
                    new Rectangle(267, 894, 1370, 186),
                    sf);
                //drawing.FillRectangle(new SolidBrush(Color.Black), p.GetBounds());// make big black box
                /* drawing.DrawString(text,
                                    font,
                                    new SolidBrush(Color.White),
                                    new Rectangle(267, 894, 1370, 186), sf);*/
                drawing.DrawPath(new Pen(new SolidBrush(Color.Black), 2), p);
                drawing.FillPath(new SolidBrush(Color.White), p);
                drawing.Save();
                await using MemoryStream outStream = new();
                outStream.Position = 0;
                copythingy.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
                outStream.Position = 0;
                if (outStream.Length > MaxBytes)
                {
                    await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length))).ConfigureAwait(false);
                }
                else
                {
                    await ctx.RespondAsync(new DiscordMessageBuilder().WithContent($"{jotaro.Mention}: {koichi.Mention}, you truly are a reliable guy.").WithFile("silverbotimage.png", outStream));
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
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            try
            {
                if (cachednewyeartemplate == null)
                {
                    var myAssembly = Assembly.GetExecutingAssembly();

                    var myStream = myAssembly.GetManifestResourceStream("SilverBotDS.Templates.happy_new_year_template.png");
                    if (myStream is null)
                    {
                        Program.SendLog("myAssembly.GetManifestResourceStream(SilverBotDS.Templates.happy_new_year_template.png) returned null");
                    }
                    cachednewyeartemplate = new Bitmap(myStream ?? throw new InvalidOperationException());
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
                    await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length))).ConfigureAwait(false);
                }
                else
                {
                    await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("happy new year!").WithFile("silverbotimage.png", outStream));
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
            MemoryStream stream = new(await new SdImage(user.GetAvatarUrl(ImageFormat.Png, discordsize)).GetBytesAsync());
            Bitmap image = new(stream);
            if (image.Width == size || image.Height == size)
            {
                stream.Position = 0;
                return stream.ToArray();
            }
            else
            {
                stream.Position = 0;
                var resizedstream = Resize(stream.ToArray(), new(size, size));
                resizedstream.Position = 0;
                return resizedstream.ToArray();
            }
        }

        [Command("adventuretime")]
        public async Task AdventureTime(CommandContext ctx, DiscordUser person, DiscordUser friendo)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            try
            {
                if (cachedadventuretimetemplate == null)
                {
                    var myAssembly = Assembly.GetExecutingAssembly();

                    var myStream = myAssembly.GetManifestResourceStream("SilverBotDS.Templates.adventure_time_template.png");
                    if (myStream is null)
                    {
                        Program.SendLog("myAssembly.GetManifestResourceStream(SilverBotDS.Templates.adventure_time_template.png) returned null");
                    }
                    cachedadventuretimetemplate = new Bitmap(myStream ?? throw new InvalidOperationException());
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
                outStream.Position = 0;
                copythingy.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
                outStream.Position = 0;
                if (outStream.Length > MaxBytes)
                {
                    await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length))).ConfigureAwait(false);
                }
                else
                {
                    await ctx.RespondAsync(new DiscordMessageBuilder().WithContent($"adventure time come on grab your friends we will go to very distant lands with {person.Mention} the {(person.IsBot ? "bot" : "human")} and {friendo.Mention} the {(friendo.IsBot ? "bot" : "human")} the fun will never end its adventure time!").WithFile("silverbotimage.png", outStream));
                }
            }
            catch (Exception e)
            {
                Program.SendLog(e);
                throw;
            }
        }

        [Command("motivate")]
        public async Task Motivate(CommandContext ctx, SdImage image, [RemainingText] string text)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            if (cachedmotivatetemplate == null)
            {
                var myAssembly = Assembly.GetExecutingAssembly();
                var myStream = myAssembly.GetManifestResourceStream("SilverBotDS.Templates.motivator_template.png");
                if (myStream is null)
                {
                    Program.SendLog("myAssembly.GetManifestResourceStream(SilverBotDS.Templates.motivator_template.png) returned null");
                }
                cachedmotivatetemplate = new Bitmap(myStream ?? throw new InvalidOperationException());
            }

            var font = new Font("Times New Roman", 100);
            await using var resizedstream = Resize(await image.GetBytesAsync(), new Size(1027, 684));
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
                await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length))).ConfigureAwait(false);
            }
            else
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("there").WithFile("silverbotimage.png", outStream));
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

        [Command("caption")]
        public async Task Caption(CommandContext ctx, SdImage image, [RemainingText] string text)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            await using var inStream = new MemoryStream(await image.GetBytesAsync());
            var bitmap = new Bitmap(inStream);
            int x = bitmap.Width, y = bitmap.Height;
            var font = new Font("Impact", x / 10);
            await using var outStream = new MemoryStream();

            Image img = new Bitmap(1, 1);

            var drawing = Graphics.FromImage(img);

            var textSize = drawing.MeasureString(text, font, x);
            img.Dispose();
            drawing.Dispose();
            img = new Bitmap(x, y + (int)textSize.Height);
            drawing = Graphics.FromImage(img);
            drawing.Clear(Color.FromArgb(255, 255, 255));
            var sf = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };
            drawing.DrawString(text, font, new SolidBrush(Color.FromArgb(0, 0, 0)), new RectangleF(new PointF(0, 0), new SizeF(x, textSize.Height)), sf);
            drawing.DrawImage(bitmap, new Point(0, (int)textSize.Height));
            drawing.Save();
            drawing.Dispose();
            img.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
            outStream.Position = 0;
            if (outStream.Length > MaxBytes)
            {
                await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length))).ConfigureAwait(false);
            }
            else
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("there").WithFile("silverbotimage.png", outStream));
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

        [Command("usertest")]
        public async Task Usertest(CommandContext ctx)
        {
            var img = DrawText(ctx.User.Username + "#" + ctx.User.Discriminator, new Font("Diavlo Light", 30.0f), Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0, 0));

            SdImage image = new(ctx.User.GetAvatarUrl(ImageFormat.Png));

            ISupportedImageFormat format = new PngFormat { Quality = Quality };
            var size = new Size(200, 200);
            await using var inStream = new MemoryStream(await image.GetBytesAsync());
            await using var avatarStream = new MemoryStream();
            await using var outStream = new MemoryStream();
            using (var imageFactory = new ImageFactory())
            {
                imageFactory.Load(inStream)
                            .Resize(size)
                            .Format(format)
                            .Save(avatarStream);
            }
            avatarStream.Position = 0;
            var imanidiot = Image.FromStream(avatarStream);

            Image imge = new Bitmap(800, 240);
            using (var gr = Graphics.FromImage(imge))
            {
                gr.Clear(Color.White);
                gr.DrawImage(imanidiot, new Point(13, 20));
                gr.DrawImage(img, new Point(229, 25));
            }
            imge.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);

            outStream.Position = 0;
            await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("there").WithFile("silverbotimage.png", outStream));
        }
    }
}