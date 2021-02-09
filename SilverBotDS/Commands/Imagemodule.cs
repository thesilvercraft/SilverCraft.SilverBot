using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ImageProcessor;
using ImageProcessor.Imaging.Filters.Photo;
using ImageProcessor.Imaging.Formats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SilverBotDS
{
    public class Imagemodule : BaseCommandModule
    {
#pragma warning disable CA1822 // Mark members as static

        private const int MaxBytes = 8388246;

        private const int Quality = 70;

        private async Task Sendcorrectamountofimages(CommandContext ctx, AttachmentCountIncorrect e, Language lang = null)
        {
            if (lang == null)
            {
                lang = Language.GetLanguageFromId(ctx.Guild?.Id);
            }
            if (e == AttachmentCountIncorrect.TooLittleAttachments)
            {
                await Send_img_plsAsync(ctx, lang.No_Image_Generic).ConfigureAwait(false);
            }
            else
            {
                await Send_img_plsAsync(ctx, lang.More_Than_One_Image_Generic).ConfigureAwait(false);
            }
        }

        private static MemoryStream Resize(byte[] photoBytes, Size size)
        {
            ISupportedImageFormat format = new PngFormat { Quality = Quality };
            using MemoryStream inStream = new MemoryStream(photoBytes);
            MemoryStream outStream = new MemoryStream();
            using (ImageFactory imageFactory = new ImageFactory())
            {
                imageFactory.Load(inStream)
                            .Format(format)
                            .Resize(size)
                            .Save(outStream);
            }
            return outStream;
        }

        private static MemoryStream Make_jpegnised(byte[] photoBytes)
        {
            ISupportedImageFormat format = new JpegFormat { Quality = Quality };
            using MemoryStream inStream = new MemoryStream(photoBytes);
            MemoryStream outStream = new MemoryStream();
            using (ImageFactory imageFactory = new ImageFactory())
            {
                imageFactory.Load(inStream)
                            .Format(format)
                            .Save(outStream);
            }
            return outStream;
        }

        private async Task Send_img_plsAsync(CommandContext ctx, string e)
        {
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            b.WithTitle("Send **an** image my guy");
            b.WithDescription(e);
            b.WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(DSharpPlus.ImageFormat.Png));
            await ctx.RespondAsync(embed: b.Build());
        }

        private static MemoryStream Tint(byte[] photoBytes, string color)
        {
            ISupportedImageFormat format = new PngFormat { Quality = Quality };
            using MemoryStream inStream = new MemoryStream(photoBytes);
            MemoryStream outStream = new MemoryStream();
            using (ImageFactory imageFactory = new ImageFactory())
            {
                imageFactory.Load(inStream)
                            .Tint(ColorTranslator.FromHtml(color))
                            .Format(format)
                            .Save(outStream);
            }
            return outStream;
        }

        /// <summary>
        /// Filters an image
        /// </summary>
        /// <param name="photoBytes">The <see cref="byte[]"/> of the original picture</param>
        /// <param name="filter">The <see cref="IMatrixFilter"/> to use</param>
        /// <returns>A ,<see cref="MemoryStream"/> with a <see cref="PngFormat"/> of 70 Quality</returns>
        private static MemoryStream Filter(byte[] photoBytes, IMatrixFilter filter)
        {
            ISupportedImageFormat format = new PngFormat { Quality = Quality };
            using MemoryStream inStream = new MemoryStream(photoBytes);
            MemoryStream outStream = new MemoryStream();
            using (ImageFactory imageFactory = new ImageFactory())
            {
                imageFactory.Load(inStream)
                            .Filter(filter)
                            .Format(format)
                            .Save(outStream);
            }
            return outStream;
        }

        [Command("jpeg")]
        public async Task Jpegize(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild?.Id);
            try
            {
                SDImage image = SDImage.FromContext(ctx);
                using MemoryStream outStream = Make_jpegnised(await image.GetBytesAsync());
                if (outStream.Length > MaxBytes)
                {
                    await Send_img_plsAsync(ctx, string.Format(lang.Output_File_Larger_Than_8M, FileSizeUtils.FormatSize(outStream.Length))).ConfigureAwait(false);
                }
                else
                {
                    await ctx.Channel.SendFileAsync("silverbotimage.jpeg", outStream, lang.Imagethings.JPEG_Success);
                }
            }
            catch (AttachmentCountIncorrectException ACIE)
            {
                await Sendcorrectamountofimages(ctx, ACIE.attachmentCount, lang);
            }
        }

        [Command("jpeg")]
        public async Task Jpegize(CommandContext ctx, [Description("the url of the image")] SDImage image)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild?.Id);
            try
            {
                using MemoryStream outStream = Make_jpegnised(await image.GetBytesAsync());
                if (outStream.Length > MaxBytes)
                {
                    await Send_img_plsAsync(ctx, string.Format(lang.Output_File_Larger_Than_8M, FileSizeUtils.FormatSize(outStream.Length))).ConfigureAwait(false);
                }
                else
                {
                    await ctx.Channel.SendFileAsync("silverbotimage.jpeg", outStream, lang.Imagethings.JPEG_Success);
                }
            }
            catch
            {
                throw;
            }
        }

        [Command("resize")]
        public async Task Resize(CommandContext ctx, [Description("the url of the image")] SDImage image, [Description("Width")] int x, [Description("Height")] int y)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild?.Id);
            try
            {
                using MemoryStream outStream = Resize(await image.GetBytesAsync(), new Size(x, y));
                if (outStream.Length > MaxBytes)
                {
                    await Send_img_plsAsync(ctx, string.Format(lang.Output_File_Larger_Than_8M, FileSizeUtils.FormatSize(outStream.Length))).ConfigureAwait(false);
                }
                else
                {
                    await ctx.Channel.SendFileAsync("silverbotimage.png", outStream, lang.Imagethings.Resize_Success);
                }
            }
            catch
            {
                throw;
            }
        }

        [Command("resize")]
        public async Task Resize(CommandContext ctx, [Description("Width")] int x, [Description("Height")] int y)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild?.Id);
            try
            {
                SDImage image = SDImage.FromContext(ctx);
                using MemoryStream outStream = Resize(await image.GetBytesAsync(), new Size(x, y));
                if (outStream.Length > MaxBytes)
                {
                    await Send_img_plsAsync(ctx, string.Format(lang.Output_File_Larger_Than_8M, FileSizeUtils.FormatSize(outStream.Length))).ConfigureAwait(false);
                }
                else
                {
                    await ctx.Channel.SendFileAsync("silverbotimage.png", outStream, lang.Imagethings.Resize_Success);
                }
            }
            catch (AttachmentCountIncorrectException ACIE)
            {
                await Sendcorrectamountofimages(ctx, ACIE.attachmentCount, lang);
            }
        }

        [Command("tint")]
        public async Task Tint(CommandContext ctx, [Description("color in hex like #fffff")] string color)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild?.Id);
            try
            {
                SDImage image = SDImage.FromContext(ctx);
                using MemoryStream outStream = Tint(await image.GetBytesAsync(), color);
                if (outStream.Length > MaxBytes)
                {
                    await Send_img_plsAsync(ctx, string.Format(lang.Output_File_Larger_Than_8M, FileSizeUtils.FormatSize(outStream.Length))).ConfigureAwait(false);
                }
                else
                {
                    await ctx.Channel.SendFileAsync("silverbotimage.png", outStream, lang.Imagethings.Tint_Success);
                }
            }
            catch (AttachmentCountIncorrectException ACIE)
            {
                await Sendcorrectamountofimages(ctx, ACIE.attachmentCount, lang);
            }
        }

        [Command("tint")]
        public async Task Tint(CommandContext ctx, [Description("the url of the image")] SDImage image, [Description("color in hex like #fffff")] string color)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild?.Id);
            try
            {
                using MemoryStream outStream = Tint(await image.GetBytesAsync(), color);
                if (outStream.Length > MaxBytes)
                {
                    await Send_img_plsAsync(ctx, string.Format(lang.Output_File_Larger_Than_8M, FileSizeUtils.FormatSize(outStream.Length))).ConfigureAwait(false);
                }
                else
                {
                    await ctx.Channel.SendFileAsync("silverbotimage.png", outStream, lang.Imagethings.Tint_Success);
                }
            }
            catch
            {
                throw;
            }
        }

        [Command("silver")]
        public async Task Grayscale(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild?.Id);
            try
            {
                SDImage image = SDImage.FromContext(ctx);
                var stream = Filter(await image.GetBytesAsync(), MatrixFilters.GreyScale);
                if (stream.Length > MaxBytes)
                {
                    await Send_img_plsAsync(ctx, string.Format(lang.Output_File_Larger_Than_8M, FileSizeUtils.FormatSize(stream.Length))).ConfigureAwait(false);
                }
                else
                {
                    await ctx.Channel.SendFileAsync("silverbotimage.jpeg", stream, lang.Imagethings.Silver_Success);
                }
            }
            catch (AttachmentCountIncorrectException ACIE)
            {
                await Sendcorrectamountofimages(ctx, ACIE.attachmentCount, lang);
            }
            catch
            {
                throw;
            }
        }

        [Command("silver")]
        public async Task Grayscale(CommandContext ctx, [Description("the url of the image")] SDImage image)
        {
            try
            {
                Language lang = Language.GetLanguageFromId(ctx.Guild?.Id);
                var stream = Filter(await image.GetBytesAsync(), MatrixFilters.GreyScale);
                if (stream.Length > MaxBytes)
                {
                    await Send_img_plsAsync(ctx, string.Format(lang.Output_File_Larger_Than_8M, FileSizeUtils.FormatSize(stream.Length))).ConfigureAwait(false);
                }
                else
                {
                    await ctx.Channel.SendFileAsync("silverbotimage.jpeg", stream, lang.Imagethings.Silver_Success);
                }
            }
            catch
            {
                throw;
            }
        }

        [Command("comic")]
        public async Task Comic(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild?.Id);
            try
            {
                SDImage image = SDImage.FromContext(ctx);
                var stream = Filter(await image.GetBytesAsync(), MatrixFilters.Comic);
                if (stream.Length > MaxBytes)
                {
                    await Send_img_plsAsync(ctx, string.Format(lang.Output_File_Larger_Than_8M, FileSizeUtils.FormatSize(stream.Length))).ConfigureAwait(false);
                }
                else
                {
                    await ctx.Channel.SendFileAsync("silverbotimage.jpeg", stream, lang.Imagethings.Comic_Success);
                }
            }
            catch (AttachmentCountIncorrectException ACIE)
            {
                await Sendcorrectamountofimages(ctx, ACIE.attachmentCount, lang);
            }
            catch
            {
                throw;
            }
        }

        [Command("comic")]
        public async Task Comic(CommandContext ctx, [Description("the url of the image")] SDImage image)
        {
            try
            {
                Language lang = Language.GetLanguageFromId(ctx.Guild?.Id);
                var stream = Filter(await image.GetBytesAsync(), MatrixFilters.Comic);
                if (stream.Length > MaxBytes)
                {
                    await Send_img_plsAsync(ctx, string.Format(lang.Output_File_Larger_Than_8M, FileSizeUtils.FormatSize(stream.Length))).ConfigureAwait(false);
                }
                else
                {
                    await ctx.Channel.SendFileAsync("silverbotimage.jpeg", stream, lang.Imagethings.Comic_Success);
                }
            }
            catch
            {
                throw;
            }
        }

        private static Image DrawText(string text, Font font, Color textColor, Color backColor)
        {
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);
            SizeF textSize = drawing.MeasureString(text, font);
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
            Image img = DrawText(text, new Font(font, 30.0f), Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255));
            using MemoryStream outStream = new MemoryStream();
            img.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
            outStream.Position = 0;
            await ctx.Channel.SendFileAsync("silverbotimage.png", outStream, "there");
        }

        private static Bitmap cachedmotivatetemplate;
        private static Bitmap cachedadventuretimetemplate;

        [RequireGuild()]
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
            Language lang = Language.GetLanguageFromId(ctx.Guild?.Id);
            try
            {
                if (cachedadventuretimetemplate == null)
                {
                    Assembly myAssembly = Assembly.GetExecutingAssembly();

                    Stream myStream = myAssembly.GetManifestResourceStream("SilverBotDS.Templates.adventure_time_template.png");
                    if (myStream is null)
                    {
                        Console.Write("FOCK");
                    }
                    cachedadventuretimetemplate = new Bitmap(myStream);
                }
                using MemoryStream resizedstreama = new(await new SDImage(person.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 256)).GetBytesAsync());
                using MemoryStream resizedstreamb = new(await new SDImage(friendo.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 256)).GetBytesAsync());
                using (Bitmap copythingy = new Bitmap(cachedadventuretimetemplate))
                {
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
                    using MemoryStream outStream = new();
                    outStream.Position = 0;
                    copythingy.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
                    outStream.Position = 0;
                    if (outStream.Length > MaxBytes)
                    {
                        await ctx.RespondAsync("hey vsauce here, your image is larger than 8mb and discord wont let me send it");
                    }
                    else
                    {
                        await ctx.Channel.SendFileAsync("silverbotimage.png", outStream, $"adventure time come on grab your friends we will go to very distant lands with {person.Mention} the {(friendo.IsBot ? "bot" : "human")} and {friendo.Mention} the {(friendo.IsBot ? "bot" : "human")} the fun will never end its adventure time!", mentions: Mentions.None);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
                throw;
            }
        }

        [Command("motivate")]
        public async Task Motivate(CommandContext ctx, string text)
        {
            try
            {
                SDImage image = SDImage.FromContext(ctx);
                await Motivate(ctx, image, text);
            }
            catch (AttachmentCountIncorrectException ACIE)
            {
                await Sendcorrectamountofimages(ctx, ACIE.attachmentCount);
            }
        }

        [Command("motivate")]
        public async Task Motivate(CommandContext ctx, SDImage image, string text)
        {
            if (cachedmotivatetemplate == null)
            {
                Assembly myAssembly = Assembly.GetExecutingAssembly();
                Stream myStream = myAssembly.GetManifestResourceStream("SilverBotDS.Templates.motivator_template.png");
                cachedmotivatetemplate = new Bitmap(myStream);
            }
            var font = new Font("Times New Roman", 100);
            using MemoryStream resizedstream = Resize(await image.GetBytesAsync(), new Size(1027, 684));
            using (Bitmap copythingy = new Bitmap(cachedmotivatetemplate))
            {
                var drawing = Graphics.FromImage(copythingy);
                using (Bitmap internalimage = new(resizedstream))
                {
                    drawing.DrawImageUnscaled(internalimage, new Point(126, 83));
                }
                StringFormat sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };
                drawing.DrawString(text, font, new SolidBrush(Color.White), new RectangleF(new PointF(119, 793), new SizeF(1041, 109)), sf);
                drawing.Save();
                using MemoryStream outStream = new();
                outStream.Position = 0;
                copythingy.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
                outStream.Position = 0;
                if (outStream.Length > MaxBytes)
                {
                    await ctx.RespondAsync("hey vsauce here, your image is larger than 8mb and discord wont let me send it");
                }
                else
                {
                    await ctx.Channel.SendFileAsync("silverbotimage.png", outStream, "there");
                }
            }
        }

        [Command("motivateold")]
        public async Task motivateold(CommandContext ctx, string text)
        {
            try
            {
                SDImage image = SDImage.FromContext(ctx);
                await Motivateold(ctx, image, text);
            }
            catch (AttachmentCountIncorrectException ACIE)
            {
                await Sendcorrectamountofimages(ctx, ACIE.attachmentCount);
            }
        }

        [Command("motivateold")]
        public async Task Motivateold(CommandContext ctx, SDImage image, string text)
        {
            using MemoryStream inStream = new MemoryStream(await image.GetBytesAsync());
            var bitmap = new Bitmap(inStream);
            int x = bitmap.Width, y = bitmap.Height;
            var font = new Font("Times New Roman", x / 10);
            using MemoryStream outStream = new MemoryStream();

            Image img = new Bitmap(1, 1);

            Graphics drawing = Graphics.FromImage(img);

            SizeF textSize = drawing.MeasureString(text, font, x);
            img.Dispose();
            drawing.Dispose();
            img = new Bitmap(x, y + (int)textSize.Height);
            drawing = Graphics.FromImage(img);
            drawing.Clear(Color.FromArgb(0, 0, 0));
            StringFormat sf = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };
            drawing.DrawString(text, font, new SolidBrush(Color.White), new RectangleF(new PointF(0, y), new SizeF(x, textSize.Height)), sf);
            drawing.DrawImage(bitmap, new Point(0, 0));
            drawing.Save();
            drawing.Dispose();
            img.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
            outStream.Position = 0;
            if (outStream.Length > MaxBytes)
            {
                await ctx.RespondAsync("hey vsauce here, your image is larger than 8mb and discord wont let me send it");
            }
            else
            {
                await ctx.Channel.SendFileAsync("silverbotimage.png", outStream, "there");
            }
        }

        [Command("caption")]
        public async Task Caption(CommandContext ctx, string text)
        {
            try
            {
                SDImage image = SDImage.FromContext(ctx);
                await Caption(ctx, image, text);
            }
            catch (AttachmentCountIncorrectException ACIE)
            {
                await Sendcorrectamountofimages(ctx, ACIE.attachmentCount);
            }
        }

        [Command("caption")]
        public async Task Caption(CommandContext ctx, SDImage image, string text)
        {
            using MemoryStream inStream = new MemoryStream(await image.GetBytesAsync());
            var bitmap = new Bitmap(inStream);
            int x = bitmap.Width, y = bitmap.Height;
            var font = new Font("Impact", x / 10);
            using MemoryStream outStream = new MemoryStream();

            Image img = new Bitmap(1, 1);

            Graphics drawing = Graphics.FromImage(img);

            SizeF textSize = drawing.MeasureString(text, font, x);
            img.Dispose();
            drawing.Dispose();
            img = new Bitmap(x, y + (int)textSize.Height);
            drawing = Graphics.FromImage(img);
            drawing.Clear(Color.FromArgb(255, 255, 255));
            StringFormat sf = new StringFormat
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
                await ctx.RespondAsync("hey vsauce here, your image is larger than 8mb and discord wont let me send it");
            }
            else
            {
                await ctx.Channel.SendFileAsync("silverbotimage.png", outStream, "there");
            }
        }

        [Command("usertest")]
        public async Task Usertest(CommandContext ctx)
        {
            Image img = DrawText(ctx.User.Username + "#" + ctx.User.Discriminator, new Font("Diavlo Light", 30.0f), Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0, 0));

            SDImage image = new(ctx.User.GetAvatarUrl(DSharpPlus.ImageFormat.Png));

            ISupportedImageFormat format = new PngFormat { Quality = Quality };
            Size size = new Size(200, 200);
            using MemoryStream inStream = new MemoryStream(await image.GetBytesAsync());
            using MemoryStream avatarStream = new MemoryStream();
            using MemoryStream outStream = new MemoryStream();
            using (ImageFactory imageFactory = new ImageFactory())
            {
                imageFactory.Load(inStream)
                            .Resize(size)
                            .Format(format)
                            .Save(avatarStream);
            }
            avatarStream.Position = 0;
            Image imanidiot = Image.FromStream(avatarStream);

            Image imge = new Bitmap(800, 240);
            using (Graphics gr = Graphics.FromImage(imge))
            {
                gr.Clear(Color.White);
                gr.DrawImage(imanidiot, new Point(13, 20));
                gr.DrawImage(img, new Point(229, 25));
            }
            imge.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);

            outStream.Position = 0;
            await ctx.Channel.SendFileAsync("silverbotimage.png", outStream, "there");
        }
    }
}