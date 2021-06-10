using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SilverBotDS.Exceptions;
using SilverBotDS.Utils;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    //[Group("bubot")]
    //[Aliases("bb")]
    // [Description("stolen commands from bubot")]
    internal class Bubot : BaseCommandModule
    {
        private readonly Font BibiFont = new(SystemFonts.Find("Arial"), 30, FontStyle.Bold);
        private readonly Color WhiteColor = Color.White;
        private readonly Color GrayColor = Color.Gray;

        [Command("silveryeet")]
        [Description("Sends SilverYeet.gif")]
        public async Task Silveryeet(CommandContext ctx) => await new DiscordMessageBuilder().WithContent("https://cdn.discordapp.com/attachments/751246248102592567/823475242822533120/SilverYeet.gif").WithReply(ctx.Message.Id).WithAllowedMentions(Mentions.None).SendAsync(ctx.Channel);

        [Command("WeWillFockYou")]
        [Description("Gives a Youtube link for the We Will Fock You video.")]
        public async Task WeWillFockYou(CommandContext ctx) => await new DiscordMessageBuilder().WithContent("https://youtu.be/lLN3caSQI1w").WithReply(ctx.Message.Id).WithAllowedMentions(Mentions.None).SendAsync(ctx.Channel);

        [Command("bibi")]
        [Description("Makes a image with Bibi.")]
        [Cooldown(1, 2, CooldownBucketType.User)]
        public async Task Bibi(CommandContext ctx, [RemainingText][Description("Bibi is")] string input)
        {
            await ctx.TriggerTypingAsync();
            input = "Bibi is " + input;
            int randomnumber;
            using (var random = new RandomGenerator())
            {
                randomnumber = random.Next(1, 18);
            }
            using Image picture = await Image.LoadAsync(Assembly.GetExecutingAssembly().GetManifestResourceStream($"SilverBotDS.Templates.Bibi.{randomnumber}.png") ?? throw new TemplateReturningNullException($"SilverBotDS.Templates.Bibi.{randomnumber}.png"));
            float size = BibiFont.Size;
            while (TextMeasurer.Measure(input, new RendererOptions(new Font(BibiFont.Family, size, FontStyle.Bold))).Width > picture.Width)
            {
                size -= 0.05f;
            }
            picture.Mutate(x => x.DrawText(input, new Font(BibiFont.Family, size, FontStyle.Bold), randomnumber is 10 or 9 ? GrayColor : WhiteColor, new PointF(4, 230)));
            await using var outStream = new MemoryStream();
            await picture.SaveAsPngAsync(outStream);
            outStream.Position = 0;
            randomnumber = 0;
            await ImageModule.SendImageStream(ctx, outStream, content: input);
        }
    }
}