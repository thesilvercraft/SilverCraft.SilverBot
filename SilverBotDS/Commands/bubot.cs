using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SilverBotDS.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    [Group("bubot")]
    [Aliases("bb")]
    [Description("stolen commands from bubot")]
    internal class Bubot : BaseCommandModule
    {
        private readonly Font BibiFont = new("Arial", 30, FontStyle.Bold);
        private readonly Brush WhiteBrush = new SolidBrush(Color.White);
        private readonly Brush GrayBrush = new SolidBrush(Color.Gray);

        [Command("silveryeet")]
        [Description("Sends SilverYeet.gif")]
        public async Task Silveryeet(CommandContext ctx)
        {
            await new DiscordMessageBuilder().WithContent("https://cdn.discordapp.com/attachments/751246248102592567/823475242822533120/SilverYeet.gif").WithReply(ctx.Message.Id).WithAllowedMentions(Mentions.None).SendAsync(ctx.Channel);
        }

        [Command("WeWillFockYou")]
        [Description("Gives a Youtube link for the We Will Fock You video.")]
        public async Task WeWillFockYou(CommandContext ctx)
        {
            await new DiscordMessageBuilder().WithContent("https://youtu.be/lLN3caSQI1w").WithReply(ctx.Message.Id).WithAllowedMentions(Mentions.None).SendAsync(ctx.Channel);
        }

        [Command("bibi")]
        [Description("Makes a image with Bibi.")]
        public async Task Bibi(CommandContext ctx, [RemainingText][Description("bibi is")] string input)
        {
            input = "bibi is " + input;
            int randomnumber;
            using (var random = new RandomGenerator())
            {
                randomnumber = random.Next(0, 12);
            }
            var myAssembly = Assembly.GetExecutingAssembly();
            var myStream = myAssembly.GetManifestResourceStream($"SilverBotDS.Templates.Bibi.{randomnumber}.png");
            float size = BibiFont.Size;
            using var picture = new Bitmap(new Bitmap(myStream ?? throw new InvalidOperationException()));
            using (Graphics g = Graphics.FromImage(picture))
            {
                while (g.MeasureString(input, new Font(BibiFont.FontFamily, size, BibiFont.Style)).Width > picture.Width)
                {
                    size -= 0.05f;
                }
                Console.WriteLine(size);
                g.DrawString(input, new Font(BibiFont.FontFamily, size, BibiFont.Style), randomnumber == 10 || randomnumber == 9 ? GrayBrush : WhiteBrush, new PointF(4, 230));
                g.Save();
            }
            await using var outStream = new MemoryStream();
            picture.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
            outStream.Position = 0;
            await ImageModule.SendImageStream(ctx, outStream, content: input);
        }
    }
}