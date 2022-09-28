using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using SilverBotDS.Attributes;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SilverBotDS.Utils;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands;

[Category("Bubot")]
public class Bubot : BaseCommandModule
{
    [Command("silveryeet")]
    [Description("Sends SilverYeet.gif")]
    public async Task Silveryeet(CommandContext ctx)
    {
        await new DiscordMessageBuilder()
            .WithContent(
                "https://cdn.discordapp.com/attachments/751246248102592567/823475242822533120/SilverYeet.gif")
            .WithReply(ctx.Message.Id).WithAllowedMentions(Mentions.None).SendAsync(ctx.Channel);
    }
}

[Category("Bubot")]
internal class BibiCommands : SilverBotCommandModule, IRequireFonts
{
    private readonly Font _bibiFont = new(SystemFonts.Get("Arial"), 30, FontStyle.Bold);
    public static string[] RequiredFontFamilies => new[] { "Arial" };
    public Config Config { private get; set; }

    private int BibiPictureCount
    {
        get { return Directory.EnumerateFiles(Config.LocalBibiPictures).Count(x => x.EndsWith(".png")); }
    }

    public override Task<bool> ExecuteRequirements(Config conf)
    {
        if (!Directory.Exists(conf.LocalBibiPictures))
        {
            return Task.FromResult(false);
        }

        if (!File.Exists(conf.BibiLibCutOutConfig))
        {
            return Task.FromResult(false);
        }

        if (!File.Exists(conf.BibiLibFullConfig))
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    [Command("bibi")]
    [Description("Makes a image with the great cat Bibi.")]
    [Cooldown(1, 2, CooldownBucketType.User)]
    public async Task Bibi(CommandContext ctx, [RemainingText][Description("Bibi is")] string input)
    {
        await ctx.TriggerTypingAsync();
        input = $"bibi is {input}";
        var randomnumber = RandomGenerator.Next(1, BibiPictureCount);
        using var picture = await Image.LoadAsync($"{Config.LocalBibiPictures}{randomnumber}.png");
        var size = _bibiFont.Size;
        while (TextMeasurer.Measure(input, new TextOptions(new Font(_bibiFont.Family, size, FontStyle.Bold))).Width >
               picture.Width)
        {
            size -= 0.05f;
        }

        picture.Mutate(
            x => x.DrawText(
                input, new Font(_bibiFont.Family, size, FontStyle.Bold),
                randomnumber is 10 or 9 ? Color.Gray : Color.White, new PointF(4, 230)
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
internal class BibiLib : SilverBotCommandModule
{
    private string[] _bibiDescText;
    private string[] _bibiFullDescText;
    public Config Config { private get; set; }

    public override Task<bool> ExecuteRequirements(Config conf)
    {
        if (!Directory.Exists(conf.LocalBibiPictures))
        {
            return Task.FromResult(false);
        }

        if (!File.Exists(conf.BibiLibCutOutConfig))
        {
            return Task.FromResult(false);
        }

        if (!File.Exists(conf.BibiLibFullConfig))
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    private void EnsureCreated()
    {
        if (_bibiDescText == null)
        {
            _bibiDescText = GetBibiDescText();
        }

        if (_bibiFullDescText == null)
        {
            _bibiFullDescText = GetBibiFullDescText();
        }
    }

    private string[] GetBibiDescText()
    {
        using StreamReader reader = new(Config.BibiLibCutOutConfig);
        return JsonSerializer.Deserialize<string[]>(reader.ReadToEnd());
    }

    private string[] GetBibiFullDescText()
    {
        using StreamReader reader = new(Config.BibiLibFullConfig);
        return JsonSerializer.Deserialize<string[]>(reader.ReadToEnd());
    }

    [GroupCommand]
    [Description("Access the great cat bibi library.")]
    public async Task BibiLibrary(CommandContext ctx)
    {
        EnsureCreated();
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        List<Page> pages = new();
        for (var a = 0; a < _bibiDescText.Length; a++)
        {
            var imgurl =
                $"https://github.com/thesilvercraft/SilverBot/blob/master/SilverBotDS/Templates/BibiLibCutout/{a + 1}.png?raw=true";
            pages.Add(new Page(embed: new DiscordEmbedBuilder().WithTitle(_bibiDescText[a])
                .WithDescription($"{imgurl}\n{string.Format(lang.PageNuget, a + 1, _bibiDescText.Length)}")
                .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
                .WithImageUrl(imgurl).WithColor(await ColorUtils.GetSingleAsync())));
        }

        var interactivity = ctx.Client.GetInteractivity();
        await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages, token: new CancellationToken());
    }

    [Command("full")]
    [Description("Access the great cat bibi library.")]
    public async Task BibiLibraryFull(CommandContext ctx)
    {
        EnsureCreated();
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        List<Page> pages = new();
        for (var a = 0; a < _bibiFullDescText.Length; a++)
        {
            var imgurl =
                $"https://github.com/thesilvercraft/SilverBot/blob/master/SilverBotDS/Templates/BibiLibFull/{a + 1}.png?raw=true";
            pages.Add(new Page(embed: new DiscordEmbedBuilder().WithTitle(_bibiFullDescText[a])
                .WithDescription($"{imgurl}\n{string.Format(lang.PageNuget, a + 1, _bibiDescText.Length)}")
                .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
                .WithImageUrl(imgurl).WithColor(await ColorUtils.GetSingleAsync())));
        }

        var interactivity = ctx.Client.GetInteractivity();
        await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages, token: new CancellationToken());
    }
}