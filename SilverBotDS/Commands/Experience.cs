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
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using NetVips;
using SilverBot.Shared.Attributes;
using SilverBot.Shared.Objects.Database;
using SilverBot.Shared.Objects.Database.Classes;
using SilverBot.Shared.Objects.Language;
using SilverBot.Shared.Utils;
using CategoryAttribute = SilverBot.Shared.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands;

using CategoryAttribute = CategoryAttribute;

[Category("XP")]
[RequireModuleGuildEnabled(EnabledModules.Experience, true)]

public class Experience : BaseCommandModule, IRequireAssets
{
    private static readonly IEnumerable<int> Range = Enumerable.Range(700, 2000);

    public static string[] RequiredAssets => new[]
    {
        "font://Diavlo Light",
    };

    public DatabaseContext Database { private get; set; }
    public HttpClient HttpClient { private get; set; }
    public LanguageService LanguageService { private get; set; }


    [Command("givexpbecausedowntimepercent")]
    [RequireOwner]
    public async Task BonusXp(CommandContext ctx, byte percent)
    {
        foreach (var person in Database.userExperiences)
        {
            person.XP += person.XP / 100 * percent;
            Database.userExperiences.Update(person);
        }
        Console.WriteLine("aa");
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
        var lang = await LanguageService.FromCtxAsync(ctx);
        var b = new DiscordEmbedBuilder()
            .AddRequestedByFooter(ctx,lang)
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
        var lang = await LanguageService.FromCtxAsync(ctx);
        var b = new DiscordEmbedBuilder()
            .AddRequestedByFooter(ctx,lang)
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
        var lang = await LanguageService.FromCtxAsync(ctx);
        var o = Database.userExperiences.AsEnumerable().OrderByDescending(x => x.XP);
        if (o != null)
        {
            DiscordEmbedBuilder bob = new();
            StringBuilder stringBuilder = new();
            List<Page> pages = new();
            bob.WithTitle(lang.XPCommandLeaderBoardTitle);
            bob.AddRequestedByFooter(ctx,lang);
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
                .AddRequestedByFooter(ctx,lang)
                .WithColor(await ColorUtils.GetSingleAsync()).Build()).SendAsync(ctx.Channel);
        }
    }

  

    [Command("xpcard")]
    public async Task XpCard(CommandContext ctx, DiscordUser user)
    {
        await ctx.TriggerTypingAsync();
        var outStream = new MemoryStream();
        var imanidiot =
            await ImageModule.GetProfilePictureAsyncStatic(user, HttpClient, 256);
        if (!imanidiot.HasAlpha())
        {
            imanidiot = imanidiot.Bandjoin(255);
        }
        var background = DateTime.UtcNow.Month switch
        {
            10 when DateTime.UtcNow.Day == 31 => new double[] { 0xFF, 0x78, 0x00, 0xFF },
            6 => new double[] { 0x00, 0x00, 0x00, 0x00 },
            _ => new double[] { 0x2E, 0xC2, 0x7E, 0xFF }
        };

        var picture = imanidiot.Embed(14, 17, 900, 286, Enums.Extend.Background, background);

        if (DateTime.UtcNow.Month == 6)
        {
            var bg = Image.Black(900, 286, 3);
            bg = bg.SRGB2HSV();
                bg = bg.Mutate(y =>
                    {
                        byte p = 0;
                        for (var x = 0; x < 900; x++)
                        {
                        y.DrawLine(new double[] { p, 255,255  }, x, 0, x, 286);
                        if (p == 255)
                        {
                            p = 0;
                        }
                        else
                        {
                            p++;
                        }
                        }
                    }
                    );
                bg=bg.HSV2sRGB();
            picture = bg.Composite(picture, Enums.BlendMode.Over, 0, 0);
        }

        var picturetext = Image.Text(HttpUtility.HtmlEncode($"{user.Username}#{user.Discriminator}"), "Twemoji Color Emoji, Diavlo Light", 596,
            42, Enums.Align.Low,
            rgba: true);
        picture = picture.Composite(picturetext, Enums.BlendMode.Over, 287, 19);
        picture = picture.Mutate(x => { x.DrawRect(new double[] { 0, 0, 0, 255 }, 280, 76, 570, 52, true); });
        var o = await Database.userExperiences.FirstOrDefaultAsync(x => x.Id == user.Id);
        if (o is not null)
        {
            var levelcount = GetLevel(o.XP);
            var progress = (int)(5.69d * GetProgressToNextLevel(o.XP));
            picture = picture.Mutate(x =>
            {
                x.DrawRect(new double[] { 69, 69, 255, 255 }, 281, 77, progress, 50, true);
            });
            using var lvlText = Image.Text(HttpUtility.HtmlEncode($"Level: {levelcount}"), "Twemoji Color Emoji, Diavlo Light", 433,
                38, Enums.Align.Centre,
                rgba: true);
            picture = picture.Composite(lvlText, Enums.BlendMode.Over, 287, 224);
            using var xpText = Image.Text(HttpUtility.HtmlEncode($"{o.XP}XP"), "Twemoji Color Emoji, Diavlo Light", 229,
                22, Enums.Align.Centre,
                rgba: true);
            picture = picture.Composite(xpText, Enums.BlendMode.Over, (280+progress)-(xpText.Width/2), 132);
            using var nextLvlText = Image.Text(HttpUtility.HtmlEncode($"{GetNeededXpForNextLevel(o.XP)}XP"), "Twemoji Color Emoji, Diavlo Light", 229,
                22, Enums.Align.Low,
                rgba: true);
            picture = picture.Composite(nextLvlText, Enums.BlendMode.Over, (570+280)-(xpText.Width/2), 132);
        }
        ImageModule.WriteImageToStream(picture, outStream, ".png");
        outStream.Position = 0;
        var lang = await LanguageService.FromCtxAsync(ctx);
        await ImageModule.SendImageStream(ctx, outStream, content: lang.XPCommandCardSuccess);
    }

    [Command("xpcard")]
    public async Task XpCard(CommandContext ctx)
    {
        await XpCard(ctx, ctx.User);
    }

    private BigInteger GetNeededXpForNextLevel(ulong xp)
    {
        ulong e = 69;
        ulong l = 1;
        while (xp / e != 0)
        {
            l++;
            e *= l;
        }

        return e;
    }

    private double GetProgressToNextLevel(ulong xp)
    {
        ulong e = 69;
        ulong l = 1;
        while (xp / e != 0)
        {
            l++;
            e *= l;
        }

        return (double)xp / (double)e * 100d;
    }

    private ulong GetLevel(ulong xp)
    {
        ulong e = 69;
        ulong l = 1;
        while (xp / e != 0)
        {
            l++;
            e *= l;
        }

        return l;
    }
}