using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using SilverBotDS.Attributes;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    [Category("Bubot")]
    internal class Bubot : BaseCommandModule
    {
        private readonly Font BibiFont = new(SystemFonts.Get("Arial"), 30, FontStyle.Bold);
        private int BibiPictureCount { get { return Directory.EnumerateFiles(config.LocalBibiPictures).Count(x => x.EndsWith(".png")); } }
        public Config config { private get; set; }

        [Command("silveryeet")]
        [Description("Sends SilverYeet.gif")]
        public async Task Silveryeet(CommandContext ctx) => await new DiscordMessageBuilder().WithContent("https://cdn.discordapp.com/attachments/751246248102592567/823475242822533120/SilverYeet.gif").WithReply(ctx.Message.Id).WithAllowedMentions(Mentions.None).SendAsync(ctx.Channel);

        [Command("WeWillFockYou")]
        [Description("Gives a Youtube link for the legendary We Will Fock You video.")]
        public async Task WeWillFockYou(CommandContext ctx) => await new DiscordMessageBuilder().WithContent("https://youtu.be/lLN3caSQI1w").WithReply(ctx.Message.Id).WithAllowedMentions(Mentions.None).SendAsync(ctx.Channel);

        [Command("bibi")]
        [Description("Makes a image with the great cat Bibi.")]
        [Cooldown(1, 2, CooldownBucketType.User)]
        public async Task Bibi(CommandContext ctx, [RemainingText][Description("Bibi is")] string input)
        {
            await ctx.TriggerTypingAsync();
            input = $"bibi is {input}";
            int randomnumber;
            using (var random = new RandomGenerator())
            {
                randomnumber = random.Next(1, BibiPictureCount);
            }
            using Image picture = await Image.LoadAsync($"{config.LocalBibiPictures}{randomnumber}.png");
            float size = BibiFont.Size;
            while (TextMeasurer.Measure(input, new RendererOptions(new Font(BibiFont.Family, size, FontStyle.Bold))).Width > picture.Width)
            {
                size -= 0.05f;
            }
            picture.Mutate(
                x => x.DrawText(
                    input, new Font(BibiFont.Family, size, FontStyle.Bold), randomnumber is 10 or 9 ? Color.Gray : Color.White, new PointF(4, 230)
                )
            );
            await using var outStream = new MemoryStream();
            await picture.SaveAsPngAsync(outStream);
            outStream.Position = 0;
            randomnumber = 0;
            await ImageModule.SendImageStream(ctx, outStream, content: input);
        }
    }

    [Group("bibiLibrary")]
    [Aliases("bibilib")]
    [Description("Access the great cat bibi library.")]
    [Category("Bubot")]
    internal class BibiLib : BaseCommandModule
    {
        public BibiLib()
        {
            BibiDescText = GetBibiDescText();
            BibiFullDescText = GetBibiFullDescText();
        }

        private readonly string[] BibiDescText;
        private readonly string[] BibiFullDescText;
        public Config config { private get; set; }

        private string[] GetBibiDescText()
        {
            using StreamReader reader = new(config.BibiLibCutOutConfig);
            return JsonSerializer.Deserialize<string[]>(reader.ReadToEnd());
        }

        private string[] GetBibiFullDescText()
        {
            using StreamReader reader = new(config.BibiLibFullConfig);
            return JsonSerializer.Deserialize<string[]>(reader.ReadToEnd());
        }

        [GroupCommand]
        [Description("Access the great cat bibi library.")]
        public async Task BibiLibrary(CommandContext ctx)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            List<Page> pages = new();
            for (int a = 0; a < BibiDescText.Length; a++)
            {
                var imgurl = $"https://github.com/thesilvercraft/SilverBot/blob/master/SilverBotDS/Templates/BibiLibCutout/{a + 1}.png?raw=true";
                pages.Add(new Page(embed: new DiscordEmbedBuilder().WithTitle(BibiDescText[a]).WithDescription($"{imgurl}\n{string.Format(lang.PageNuget, a + 1, BibiDescText.Length)}").WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).WithImageUrl(imgurl).WithColor(color: await ColorUtils.GetSingleAsync())));
            }
            var interactivity = ctx.Client.GetInteractivity();
            await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages, token: new System.Threading.CancellationToken());
        }

        [Command("full")]
        [Description("Access the great cat bibi library.")]
        public async Task BibiLibraryFull(CommandContext ctx)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            List<Page> pages = new();
            for (int a = 0; a < BibiFullDescText.Length; a++)
            {
                var imgurl = $"https://github.com/thesilvercraft/SilverBot/blob/master/SilverBotDS/Templates/BibiLibFull/{a + 1}.png?raw=true";
                pages.Add(new Page(embed: new DiscordEmbedBuilder().WithTitle(BibiFullDescText[a]).WithDescription($"{imgurl}\n{string.Format(lang.PageNuget, a + 1, BibiDescText.Length)}").WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).WithImageUrl(imgurl).WithColor(color: await ColorUtils.GetSingleAsync())));
            }
            var interactivity = ctx.Client.GetInteractivity();
            await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages, token: new System.Threading.CancellationToken());
        }
    }
}