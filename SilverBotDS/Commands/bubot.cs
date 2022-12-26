/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using ImageMagick;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SilverBotDS.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SilverBotDS.Attributes;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands;

[Category("Bubot")]
[RequireModuleGuildEnabled(EnabledModules.Bubot, true)]

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
[RequireModuleGuildEnabled(EnabledModules.Bubot, true)]
internal class BibiCommands : BaseCommandModule, IHaveExecutableRequirements, IRequireAssets
{
    public static string[] RequiredAssets => new[]
    {
        "font://Arial",
    };

    public Config Config { private get; set; }
    public LanguageService LanguageService { private get; set; }

    private int BibiPictureCount
    {
        get { return Directory.EnumerateFiles(Config.LocalBibiPictures).Count(x => x.EndsWith(".png")); }
    }

    public Task<bool> ExecuteRequirements(Config conf)
    {
        return Task.FromResult(Directory.Exists(conf.LocalBibiPictures));
    }
    
    [Command("bibi")]
    [Description("Makes a image with the great cat Bibi.")]
    [Cooldown(1, 2, CooldownBucketType.User)]
    public async Task Bibi(CommandContext ctx, [RemainingText][Description("Bibi is")] string input)
    {
        //await ctx.TriggerTypingAsync();
        input = $"bibi is {input}";
        var randomnumber = RandomGenerator.Next(1, BibiPictureCount);
        await using var file = File.OpenRead(Path.Combine(Config.LocalBibiPictures,$"{randomnumber}.png"));
        using var picture = new MagickImage(file);
        MagickReadSettings settings = new()
        {
            FillColor = randomnumber is 10 or 9 ? MagickColors.Gray : MagickColors.White,
            Font = "Arial",
            FontPointsize = picture.Width / 14,
            BackgroundColor = MagickColors.Transparent,
            Width = picture.Width,
        };
        using var label = new MagickImage($"caption:{input}", settings);
        picture.Composite(label, 4, 230, CompositeOperator.Over);
        await using var outStream = new MemoryStream();
        await picture.WriteAsync(outStream, MagickFormat.Png);
        outStream.Position = 0;
        await ImageModule.SendImageStreamIfAllowed(ctx, outStream, DisposeOfStream:true, content: input);
    }
}

[Group("bibiLibrary")]
[Aliases("bibilib")]
[Description("Access the great cat bibi library.")]
[Category("Bubot")]
[RequireModuleGuildEnabled(EnabledModules.Bubot, true)]
internal class BibiLib : BaseCommandModule, IHaveExecutableRequirements
{
    private string[] _bibiDescText;
    private string[] _bibiFullDescText;
    public Config Config { private get; set; }
    public LanguageService LanguageService { private get; set; }


    public Task<bool> ExecuteRequirements(Config conf)
    {
        return Task.FromResult(Directory.Exists(conf.LocalBibiPictures) && File.Exists(conf.BibiLibCutOutConfig) && File.Exists(conf.BibiLibFullConfig));
    }

    private void EnsureCreated()
    {
        _bibiDescText ??= GetBibiDescText();
        _bibiFullDescText ??= GetBibiFullDescText();
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
        var lang = await LanguageService.FromCtxAsync(ctx);
        List<Page> pages = new();
        for (var a = 0; a < _bibiDescText.Length; a++)
        {
            var imgurl =
                $"https://github.com/thesilvercraft/SilverBotAssets/blob/main/BibiLibCutout/{a + 1}.png?raw=true";
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
        var lang = await LanguageService.FromCtxAsync(ctx);
        List<Page> pages = new();
        for (var a = 0; a < _bibiFullDescText.Length; a++)
        {
            var imgurl =
                $"https://github.com/thesilvercraft/SilverBotAssets/blob/main/BibiLibFull/{a + 1}.png?raw=true";
            pages.Add(new Page(embed: new DiscordEmbedBuilder().WithTitle(_bibiFullDescText[a])
                .WithDescription($"{imgurl}\n{string.Format(lang.PageNuget, a + 1, _bibiDescText.Length)}")
                .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
                .WithImageUrl(imgurl).WithColor(await ColorUtils.GetSingleAsync())));
        }

        var interactivity = ctx.Client.GetInteractivity();
        await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages, token: new CancellationToken());
    }
}