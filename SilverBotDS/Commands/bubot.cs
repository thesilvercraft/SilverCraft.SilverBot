using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;
using SilverBotDS.Exceptions;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using System.Text.Json;
using SilverBotDS.Attributes;

namespace SilverBotDS.Commands
{
    [Category("Bubot")]
    internal class Bubot : BaseCommandModule
    {
        private readonly Font BibiFont = new(SystemFonts.Find("Arial"), 30, FontStyle.Bold);
        private readonly int BibiPictureCount = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(x => x.StartsWith("SilverBotDS.Templates.Bibi.") && x.EndsWith(".png")).Count();

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
            input = "bibi is " + input;
            int randomnumber;
            using (var random = new RandomGenerator())
            {
                randomnumber = random.Next(1, BibiPictureCount);
            }
            using Image picture = await Image.LoadAsync(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    $"SilverBotDS.Templates.Bibi.{randomnumber}.png"
                )
                ??
                throw new TemplateReturningNullException(
                    $"SilverBotDS.Templates.Bibi.{randomnumber}.png"
                )
            );
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

        public static readonly string[] BibiDescText = GetBibiDescText();
        public static readonly string[] BibiFullDescText = GetBibiFullDescText();
        private static string[] GetBibiDescText()
        {
            StreamReader reader = new(
               Assembly.GetExecutingAssembly().GetManifestResourceStream(
                   "SilverBotDS.Templates.BibiLibCutout.Titles.json"
               )
               ??
               throw new TemplateReturningNullException(
                   "SilverBotDS.Templates.BibiLibCutout.Titles.json"
               )
            );
            return JsonSerializer.Deserialize<string[]>(reader.ReadToEnd());
        }
        private static string[] GetBibiFullDescText()
        {
            StreamReader reader = new(
               Assembly.GetExecutingAssembly().GetManifestResourceStream(
                   "SilverBotDS.Templates.BibiLibFull.Titles.json"
               )
               ??
               throw new TemplateReturningNullException(
                   "SilverBotDS.Templates.BibiLibFull.Titles.json"
               )
            );
            return JsonSerializer.Deserialize<string[]>(reader.ReadToEnd());
        }
        [GroupCommand]
        [Description("Access the great cat bibi library.")]
        public async Task BibiLibrary(CommandContext ctx)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            var imgurl = $"https://github.com/thesilvercraft/SilverBot/blob/master/SilverBotDS/Templates/BibiLibCutout/1.png?raw=true";
            var b = new DiscordEmbedBuilder().WithTitle(BibiDescText[0]).WithDescription($"{imgurl}\n{string.Format(lang.PageGif, 1, BibiDescText.Length)}").WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).WithImageUrl(imgurl).WithColor(color: await ColorUtils.GetSingleAsync());
            await WaitForNextMessage(ctx, await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithEmbed(b.Build()).AddComponents(new DiscordButtonComponent(ButtonStyle.Primary, "nextgif", lang.PageGifButtonText)).SendAsync(ctx.Channel), ctx.Client.GetInteractivity(), lang, 0, false, b);
        }
        [Command("full")]
        [Description("Access the great cat bibi library.")]
        public async Task BibiLibraryFull(CommandContext ctx)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            var imgurl = $"https://github.com/thesilvercraft/SilverBot/blob/master/SilverBotDS/Templates/BibiLibFull/1.png?raw=true";
            var b = new DiscordEmbedBuilder().WithTitle("bibi").WithDescription($"{imgurl}\n{string.Format(lang.PageGif, 1, BibiFullDescText.Length)}").WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).WithImageUrl(imgurl).WithColor(color: await ColorUtils.GetSingleAsync());
            await WaitForNextMessage(ctx, await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithEmbed(b.Build()).AddComponents(new DiscordButtonComponent(ButtonStyle.Primary, "nextgif", lang.PageGifButtonText)).SendAsync(ctx.Channel), ctx.Client.GetInteractivity(), lang, 0, true,b);
        }
        private async Task WaitForNextMessage(CommandContext ctx, DiscordMessage oldmessage, InteractivityExtension interactivity, Language lang, int page, bool gaming ,DiscordEmbedBuilder b = null)
        {
            b ??= new DiscordEmbedBuilder();
            var msg = await oldmessage.WaitForButtonAsync(ctx.User, TimeSpan.FromSeconds(300));
            if (msg.Result != null)
            {
                page++;
                if (page == (gaming? BibiFullDescText.Length : BibiDescText.Length))
                {
                    page = 0;
                }
                var imgurl = $"https://github.com/thesilvercraft/SilverBot/blob/master/SilverBotDS/Templates/{(gaming? "BibiLibFull" : "BibiLibCutout")}/{page + 1}.png?raw=true";
                b.WithTitle(gaming ? string.Empty : BibiDescText[page]).WithDescription($"{imgurl}\n{string.Format(lang.PageGif, page + 1, (gaming ? BibiFullDescText.Length : BibiDescText.Length))}").WithImageUrl(imgurl).WithColor(color: await ColorUtils.GetSingleAsync());
                await msg.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithEmbed(b).AddComponents(new DiscordButtonComponent(ButtonStyle.Primary, "nextgif", lang.PageGifButtonText))));
                await WaitForNextMessage(ctx, oldmessage, interactivity, lang, page, gaming,b);
            }
            else
            {
                await oldmessage.ModifyAsync(new DiscordMessageBuilder().WithEmbed(b).WithContent(lang.PeriodExpired).AddComponents(new DiscordButtonComponent(ButtonStyle.Primary, "nextgif", lang.PageGifButtonText, true)));
            }
        }
    }
    }