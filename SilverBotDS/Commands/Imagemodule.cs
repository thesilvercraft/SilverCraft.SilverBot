using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using ImageProcessor;
using ImageProcessor.Imaging.Filters.Photo;
using ImageProcessor.Imaging.Formats;
using SilverBotDS.Converters;
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
using System.Xml.Serialization;

namespace SilverBotDS.Commands
{
    [Cooldown(1, 2, CooldownBucketType.User)]
    public class ImageModule : BaseCommandModule
    {
#pragma warning disable CA1822 // Mark members as static

        private const int MaxBytes = 8388246;

        private const int Quality = 70;
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

        public static MemoryStream Resize(byte[] photoBytes, Size size, bool centered = true)
        {
            ISupportedImageFormat format = new PngFormat { Quality = Quality };
            using var inStream = new MemoryStream(photoBytes);
            var outStream = new MemoryStream();
            using var imageFactory = new ImageFactory();
            imageFactory.Load(inStream)
                .Format(format)
                .Resize(new ImageProcessor.Imaging.ResizeLayer(size, anchorPosition: centered ? ImageProcessor.Imaging.AnchorPosition.Center : ImageProcessor.Imaging.AnchorPosition.TopLeft))
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

        [Command("template")]
        public async Task Template(CommandContext ctx)
        {
            var serializer = new XmlSerializer(typeof(Step[]), "SilverBotDS.Objects");
            serializer.Serialize(Console.Out, new Step[] { new TemplateStep(0, 0, @"D:\Users\filip\source\repos\SilverBotDS\SilverBotDS\Templates\happy_new_year_template.png", false), new PictureStep(19, 70, 350, 350, null, true) });
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

                Console.WriteLine("e");
                using var outStream = new MemoryStream();
                using (var exec = await e.ExecuteStepsAsync(steps.ToArray()))
                {
                    exec.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
                    Console.WriteLine("eee");
                }
                Console.WriteLine("eeee");
                outStream.Position = 0;
                await SendImageStream(ctx, outStream, "silverbotimage.png");
                Console.WriteLine("eeeee");
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
            await using var outStream = Make_jpegnised(await image.GetBytesAsync());
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

        [Command("shet")]
        public async Task Shet(CommandContext ctx, [Description("the url of the image")] SdImage image)
        {
            await ctx.TriggerTypingAsync();
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            await using var outStream = Shet_On(await image.GetBytesAsync());
            if (outStream.Length > MaxBytes)
            {
                await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang).ConfigureAwait(false);
            }
            else
            {
                await SendImageStream(ctx, outStream, "silverbotimage.jpeg", lang.Imagethings.JpegSuccess, lang);
            }
        }

        [Command("shet")]
        public async Task Shet(CommandContext ctx)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
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
            await ctx.TriggerTypingAsync();
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            await using var outStream = Resize(await image.GetBytesAsync(), new Size(x, y));
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
        public async Task Tint(CommandContext ctx, [Description("the url of the image")] SdImage image, [Description("color in hex like #fffff")] string color)
        {
            await ctx.TriggerTypingAsync();
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            await using var outStream = Tint(await image.GetBytesAsync(), color);
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
            await using var outStream = Filter(await image.GetBytesAsync(), MatrixFilters.GreyScale);
            if (outStream.Length > MaxBytes)
            {
                await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang).ConfigureAwait(false);
            }
            else
            {
                await SendImageStream(ctx, outStream, content: lang.Imagethings.SilverSuccess, lang: lang);
            }
        }

        [Command("comic")]
        public async Task Comic(CommandContext ctx)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
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
            await ctx.TriggerTypingAsync();
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            await using var outStream = Filter(await image.GetBytesAsync(), MatrixFilters.Comic);
            if (outStream.Length > MaxBytes)
            {
                await Send_img_plsAsync(ctx, string.Format(lang.OutputFileLargerThan8M, FileSizeUtils.FormatSize(outStream.Length)), lang).ConfigureAwait(false);
            }
            else
            {
                await SendImageStream(ctx, outStream, content: lang.Imagethings.ComicSuccess, lang: lang);
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
            await ctx.TriggerTypingAsync();
            var img = DrawText(text, new Font(font, 30.0f), Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255));
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

                var p = new GraphicsPath();
                p.AddString(
                    text,
                    font.FontFamily,
                    (int)font.Style,
                    drawing.DpiY * font.Size / 72,
                    new Rectangle(267, 894, 1370, 186),
                    sf);
                drawing.DrawPath(new Pen(new SolidBrush(Color.Black), 2), p);
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
            await using MemoryStream stream = new(await new SdImage(user.GetAvatarUrl(ImageFormat.Png, discordsize)).GetBytesAsync());
            using Bitmap image = new(stream);
            if (image.Width == size || image.Height == size)
            {
                stream.Position = 0;
                return stream.ToArray();
            }
            else
            {
                stream.Position = 0;
                await using var resizedstream = Resize(stream.ToArray(), new(size, size));
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
                if (myStream is null)
                {
                    Program.SendLog("myAssembly.GetManifestResourceStream(SilverBotDS.Templates.paint_template.png) returned null");
                }
                cachedpaintreliabletemplate = new Bitmap(myStream ?? throw new InvalidOperationException());
            }
            await using var resizedstream = Resize(await image.GetBytesAsync(), new Size(1771, 984), true);
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
            await using var inStream = new MemoryStream(await image.GetBytesAsync());
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

        [Command("usertest")]
        public async Task Usertest(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
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
            using (var imanidiot = Image.FromStream(avatarStream))
            {
                Image imge = new Bitmap(800, 240);
                using (var gr = Graphics.FromImage(imge))
                {
                    gr.Clear(Color.White);
                    gr.DrawImage(imanidiot, new Point(13, 20));
                    using var img = DrawText(ctx.User.Username + "#" + ctx.User.Discriminator, new Font("Diavlo Light", 30.0f), Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0, 0));
                    gr.DrawImage(img, new Point(229, 25));
                }
                imge.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
            }
            outStream.Position = 0;
            await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("there").WithFile("silverbotimage.png", outStream));
        }
    }
}