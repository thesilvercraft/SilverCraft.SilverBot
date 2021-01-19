using ImageProcessor;
using ImageProcessor.Imaging.Filters.Photo;
using ImageProcessor.Imaging.Formats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Reflection;

namespace SilverBotDS
{
    public class Imagemodule : BaseCommandModule
    {
#pragma warning disable CA1822 // Mark members as static
        //private static Config dconfig = new Config();

        //public static void SetConfig(Config GetConfig)
        //{
        //    dconfig = GetConfig;
        //}
        //TODO fucking change the messages to a language thingy
        [Command("jpeg")]
        public async Task Jpegize(CommandContext ctx)
        {
            var attachments = ctx.Message.Attachments;
            if (attachments.Count == 0)
            {
                await Send_img_plsAsync(ctx, "You didnt attach a image").ConfigureAwait(false);
                return;
            }
            else if (attachments.Count > 1)
            {
                await Send_img_plsAsync(ctx, "You attached more than one image").ConfigureAwait(false);
                return;
            }
            using WebClient myWebClient = new WebClient();
            using MemoryStream outStream = Make_jpegnised(myWebClient.DownloadData(attachments.ElementAt(0).Url));
            await ctx.Channel.SendFileAsync("silverbotimage.jpeg", outStream, "There ya go a jpegnized image");
        }

        [Command("jpeg")]
        public async Task Jpegize(CommandContext ctx, [Description("the url of the image")] string url)
        {
            using WebClient myWebClient = new WebClient();
            try
            {
                using MemoryStream outStream = Make_jpegnised(myWebClient.DownloadData(url));
                await ctx.Channel.SendFileAsync("silverbotimage.jpeg", outStream, "There ya go a jpegnized image");
            }
            catch
            {
                throw;
            }
        }

        private static MemoryStream Make_jpegnised(byte[] photoBytes)
        {
            ISupportedImageFormat format = new JpegFormat { Quality = 1 };
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

        private async Task Send_img_plsAsync(CommandContext ctx, string gay)
        {
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            b.WithTitle("Send an image my guy");
            b.WithDescription(gay);
            b.WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(DSharpPlus.ImageFormat.Png));
            await ctx.RespondAsync(embed: b.Build());
        }

        [Command("tint")]
        public async Task Tint(CommandContext ctx, [Description("color in hex like #fffff")] string color)
        {
            var attachments = ctx.Message.Attachments;
            if (attachments.Count == 0)
            {
                await Send_img_plsAsync(ctx, "You didnt attach a image").ConfigureAwait(false);
                return;
            }
            else if (attachments.Count > 1)
            {
                await Send_img_plsAsync(ctx, "You attached more than one image").ConfigureAwait(false);
                return;
            }
            await ctx.Channel.SendFileAsync("silverbotimage.png", Tint(new WebClient().DownloadData(attachments.ElementAt(0).Url), color), "There ya go a tinted image");
        }

        [Command("tint")]
        public async Task Tint(CommandContext ctx, [Description("the url of the image")] string url, [Description("color in hex like #fffff")] string color)
        {
            try
            {
                await ctx.Channel.SendFileAsync("silverbotimage.jpeg", Tint(new WebClient().DownloadData(url), color), "There ya go a tinted image");
            }
            catch
            {
                throw;
            }
        }

        private static MemoryStream Tint(byte[] photoBytes, string color)
        {
            ISupportedImageFormat format = new PngFormat { Quality = 70 };
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

        [Command("silver")]
        public async Task Silver(CommandContext ctx)
        {
            var attachments = ctx.Message.Attachments;
            if (attachments.Count == 0)
            {
                await Send_img_plsAsync(ctx, "You didnt attach a image").ConfigureAwait(false);
                return;
            }
            else if (attachments.Count > 1)
            {
                await Send_img_plsAsync(ctx, "You attached more than one image").ConfigureAwait(false);
                return;
            }
            await ctx.Channel.SendFileAsync("silverbotimage.png", Filter(new WebClient().DownloadData(attachments.ElementAt(0).Url), MatrixFilters.GreyScale), "There ya go a silver image");
        }

        [Command("silver")]
        public async Task Silver(CommandContext ctx, [Description("the url of the image")] string url)
        {
            try
            {
                await ctx.Channel.SendFileAsync("silverbotimage.png", Filter(new WebClient().DownloadData(url), MatrixFilters.GreyScale), "There ya go a silver image");
            }
            catch
            {
                throw;
            }
        }

        private static MemoryStream Filter(byte[] photoBytes, IMatrixFilter e)
        {
            ISupportedImageFormat format = new PngFormat { Quality = 70 };
            using MemoryStream inStream = new MemoryStream(photoBytes);
            MemoryStream outStream = new MemoryStream();
            using (ImageFactory imageFactory = new ImageFactory())
            {
                imageFactory.Load(inStream)
                            .Filter(e)
                            .Format(format)
                            .Save(outStream);
            }
            return outStream;
        }

        [Command("comic")]
        public async Task Comic(CommandContext ctx)
        {
            var attachments = ctx.Message.Attachments;
            if (attachments.Count == 0)
            {
                await Send_img_plsAsync(ctx, "You didnt attach a image").ConfigureAwait(false);
                return;
            }
            else if (attachments.Count > 1)
            {
                await Send_img_plsAsync(ctx, "You attached more than one image").ConfigureAwait(false);
                return;
            }
            await ctx.Channel.SendFileAsync("silverbotimage.png", Filter(new WebClient().DownloadData(attachments.ElementAt(0).Url), MatrixFilters.Comic), "There ya go a image with the comic filter");
        }

        [Command("comic")]
        public async Task Comic(CommandContext ctx, [Description("the url of the image")] string url)
        {
            try
            {
                await ctx.Channel.SendFileAsync("silverbotimage.png", Filter(new WebClient().DownloadData(url), MatrixFilters.Comic), "There ya go a silver image");
            }
            catch
            {
                throw;
            }
        }

        private static Image DrawText(string text, Font font, System.Drawing.Color textColor, System.Drawing.Color backColor)
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
        public async Task Text(CommandContext ctx, [Description("the text")] string text)
        {
            System.Drawing.Image img = DrawText(text, new Font("Diavlo Light", 30.0f), System.Drawing.Color.FromArgb(0, 0, 0), System.Drawing.Color.FromArgb(255, 255, 255));
            using MemoryStream outStream = new MemoryStream();
            img.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
            outStream.Position = 0;
            await ctx.Channel.SendFileAsync("silverbotimage.png", outStream, "there");
        }

        [Command("usertest")]
        public async Task Usertest(CommandContext ctx)
        {
            Image img = DrawText(ctx.User.Username + "#" + ctx.User.Discriminator, new Font("Diavlo Light", 30.0f), System.Drawing.Color.FromArgb(0, 0, 0), System.Drawing.Color.FromArgb(0, 0, 0, 0));
            using WebClient myWebClient = new WebClient();
            string url = ctx.User.GetAvatarUrl(DSharpPlus.ImageFormat.Png);
            byte[] photoBytes = myWebClient.DownloadData(url);
            ISupportedImageFormat format = new PngFormat { Quality = 70 };
            Size size = new Size(200, 200);
            using MemoryStream inStream = new MemoryStream(photoBytes);
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
                gr.Clear(System.Drawing.Color.White);
                gr.DrawImage(imanidiot, new Point(13, 20));
                gr.DrawImage(img, new Point(229, 25));
            }
            imge.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);

            outStream.Position = 0;
            await ctx.Channel.SendFileAsync("silverbotimage.png", outStream, "there");
        }
    }
}