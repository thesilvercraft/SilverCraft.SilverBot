using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.EntityFrameworkCore;
using SilverBotDS.Attributes;
using SilverBotDS.Converters;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SilverBotDS.Utils;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilverBotDS.Commands;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

[Category("XP")]
public class Experience : SilverBotCommandModule, IRequireFonts
{
    private static readonly IEnumerable<int> Range = Enumerable.Range(700, 2000);
    private readonly SolidBrush _blackBrush = new(Color.Black);

    private readonly Font _diavloLight = new(SystemFonts.Get("Diavlo Light"), 30.0f);
    public DatabaseContext Database { private get; set; }
    public HttpClient HttpClient { private get; set; }

    public static string[] RequiredFontFamilies => new[] { "Diavlo Light" };

    [Command("givexpbecausedowntimepercent")]
    [RequireOwner]
    public async Task BonusXp(CommandContext ctx, byte percent)
    {
        foreach (var person in Database.userExperiences)
        {
            person.XP += person.XP / 100 * percent;
            Database.userExperiences.Update(person);
        }

        await Database.SaveChangesAsync();
    }

    [Command("givexpbecausedowntime")]
    [RequireOwner]
    public async Task BonusXpPerperson(CommandContext ctx, ulong xp)
    {
        foreach (var person in Database.userExperiences)
        {
            person.XP += xp;
            Database.userExperiences.Update(person);
        }

        await Database.SaveChangesAsync();
    }

    [Command("xp")]
    public async Task XpCommand(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        var b = new DiscordEmbedBuilder()
            .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
            .WithColor(await ColorUtils.GetSingleAsync());
        var o = await Database.userExperiences.FirstOrDefaultAsync(x => x.Id == ctx.User.Id);
        if (o is not null)
        {
            var levelcount = GetLevel(o.XP);
            Console.WriteLine(lang.XPCommandSelf);
            b.WithDescription(string.Format(lang.XPCommandSelf, o.XP, levelcount));
        }
        else
        {
            b.WithDescription(lang.XPCommandFailSelf);
        }

        await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithEmbed(b.Build()).SendAsync(ctx.Channel);
    }

    [Command("xp")]
    public async Task XpCommand(CommandContext ctx, DiscordMember member)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        var b = new DiscordEmbedBuilder()
            .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
            .WithColor(await ColorUtils.GetSingleAsync());
        var o = await Database.userExperiences.FirstOrDefaultAsync(x => x.Id == member.Id);
        if (o is not null)
        {
            var levelcount = GetLevel(o.XP);
            b.WithDescription(string.Format(lang.XPCommandOther, o.XP, levelcount));
        }
        else
        {
            b.WithDescription(lang.XPCommandFailOther);
        }

        await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithEmbed(b.Build()).SendAsync(ctx.Channel);
    }

    [Command("xptop")]
    [RequireGuild]
    public async Task XpLeaderboard(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        var o = Database.userExperiences.AsEnumerable().OrderByDescending(x => x.XP);
        if (o is not null)
        {
            DiscordEmbedBuilder bob = new();
            StringBuilder stringBuilder = new();
            List<Page> pages = new();
            bob.WithTitle(lang.XPCommandLeaderBoardTitle);
            bob.WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            foreach (var person in o)
            {
                if (Range.Contains(stringBuilder.Length))
                {
                    bob.WithDescription(stringBuilder.ToString());
                    pages.Add(new Page(embed: bob));
                    stringBuilder.Clear();
                }
                else
                {
                    stringBuilder.Append("<@!").Append(person.Id).Append("> Lvl ").Append(GetLevel(person.XP))
                        .Append(' ').Append(person.XP).AppendLine("XP");
                }
            }

            bob.WithDescription(stringBuilder.ToString());
            pages.Add(new Page(embed: bob));
            var pages1 = pages.ToArray();
            pages.Clear();
            for (ulong a = 0; a < (ulong)pages1.Length; a++)
            {
                var embedbuilder = new DiscordEmbedBuilder(pages1[a].Embed);
                embedbuilder.WithAuthor(string.Format(lang.PageNuget, a + 1, pages1.Length));
                pages1[a].Embed = embedbuilder.Build();
            }

            var interactivity = ctx.Client.GetInteractivity();
            await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages1,
                token: new CancellationToken());
        }
        else
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithEmbed(new DiscordEmbedBuilder()
                .WithDescription(lang.XPCommandGeneralFail)
                .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
                .WithColor(await ColorUtils.GetSingleAsync()).Build()).SendAsync(ctx.Channel);
        }
    }

    [Command("xpcard")]
    public async Task XpCard(CommandContext ctx, DiscordUser user)
    {
        await ctx.TriggerTypingAsync();
        var outStream = new MemoryStream();
        SdImage image = new(user.GetAvatarUrl(ImageFormat.Png));
        using (var imanidiot =
               Image.Load((await ImageModule.ResizeAsync(await image.GetBytesAsync(HttpClient), new Size(200, 200)))
                   .Item1))
        {
            Image imge = new Image<Rgba32>(800, 240);
            imge.Mutate(async oo =>
            {
                oo.Fill(Color.White);
                oo.DrawImage(imanidiot, new Point(13, 20), 1);
                using var img = ImageModule.DrawText($"{user.Username}#{user.Discriminator}", _diavloLight, Color.Black,
                    Color.Transparent);
                oo.DrawImage(img, new Point(229, 25), 1);
                oo.Fill(Color.Black, new Rectangle(new Point(233, 83), new Size(478, 30)));
                var o = await Database.userExperiences.FirstOrDefaultAsync(x => x.Id == user.Id);
                if (o is not null)
                {
                    var levelcount = GetLevel(o.XP);
                    var progress = 4.76 * GetProgressToNextLevel(o.XP);
                    oo.Fill(Color.LightGreen, new Rectangle(new Point(234, 84), new Size((int)progress, 28)));
                    oo.DrawText($"{o.XP}XP", _diavloLight, _blackBrush, new PointF((float)(170 + progress), 140));
                    oo.DrawText($"{GetNeededXpForNextLevel(o.XP)}XP", _diavloLight, _blackBrush,
                        new PointF(650.95f, 120));
                    oo.DrawText($"Level: {levelcount}", _diavloLight, _blackBrush, new PointF(232, 169));
                }
            });
            imge.Save(outStream, new PngEncoder());
        }

        outStream.Position = 0;
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await ImageModule.SendImageStream(ctx, outStream, content: lang.XPCommandCardSuccess);
    }

    [Command("xpcard")]
    public async Task XpCard(CommandContext ctx)
    {
        await XpCard(ctx, ctx.User);
    }

    private BigInteger GetNeededXpForNextLevel(BigInteger xp)
    {
        BigInteger e = 69;
        BigInteger l = 1;
        while (xp / e != 0)
        {
            l++;
            e *= l;
        }

        return e;
    }

    private double GetProgressToNextLevel(BigInteger xp)
    {
        BigInteger e = 69;
        BigInteger l = 1;
        while (xp / e != 0)
        {
            l++;
            e *= l;
        }

        return (double)xp / (double)e * 100d;
    }

    private BigInteger GetLevel(BigInteger xp)
    {
        BigInteger e = 69;
        BigInteger l = 1;
        while (xp / e != 0)
        {
            l++;
            e *= l;
        }

        return l;
    }
}