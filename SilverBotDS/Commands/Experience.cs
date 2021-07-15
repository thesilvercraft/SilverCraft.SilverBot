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
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    [Category("XP")]
    internal class Experience : BaseCommandModule
    {
        public DatabaseContext Database { private get; set; }
        public HttpClient HttpClient { private get; set; }

        [Command("xp")]
        public async Task XPCommand(CommandContext ctx)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            var b = new DiscordEmbedBuilder().WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).WithColor(color: await ColorUtils.GetSingleAsync());
            var o = await Database.userExperiences.FirstOrDefaultAsync(x => x.Id == ctx.User.Id);
            if (o is not null)
            {
                var levelcount = GetLevel(o.XP);
                b.WithDescription(string.Format(lang.XPCommandSelf, o.XP, levelcount, levelcount % 2 == 0 ? lang.XPCommandLevels : lang.XPCommandLevel));
            }
            else
            {
                b.WithDescription(lang.XPCommandFailSelf);
            }

            await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithEmbed(b.Build()).SendAsync(ctx.Channel);
        }

        [Command("xp")]
        public async Task XPCommand(CommandContext ctx, DiscordMember member)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            var b = new DiscordEmbedBuilder().WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).WithColor(color: await ColorUtils.GetSingleAsync());
            var o = await Database.userExperiences.FirstOrDefaultAsync(x => x.Id == member.Id);
            if (o is not null)
            {
                var levelcount = GetLevel(o.XP);
                b.WithDescription(string.Format(lang.XPCommandOther, o.XP, levelcount, levelcount % 2 == 0 ? lang.XPCommandLevels : lang.XPCommandLevel));
            }
            else
            {
                b.WithDescription(lang.XPCommandFailOther);
            }

            await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithEmbed(b.Build()).SendAsync(ctx.Channel);
        }

        private static readonly IEnumerable<int> range = Enumerable.Range(1900, 2000);

        [Command("xptop")]
        [RequireGuild]
        public async Task XPLeaderboard(CommandContext ctx)
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
                    if (range.Contains(stringBuilder.Length))
                    {
                        bob.WithDescription(stringBuilder.ToString());
                        pages.Add(new Page(embed: bob));
                        stringBuilder.Clear();
                    }
                    else
                    {
                        stringBuilder.Append("<@!").Append(person.Id).Append("> has ").Append(person.XP).AppendLine("XP");
                    }
                }
                bob.WithDescription(stringBuilder.ToString());
                pages.Add(new Page(embed: bob));
                Page[] pages1 = pages.ToArray();
                pages.Clear();
                for (ulong a = 0; a < (ulong)pages1.Length; a++)
                {
                    var embedbuilder = new DiscordEmbedBuilder(pages1[a].Embed);
                    embedbuilder.WithAuthor(string.Format(lang.PageNuget, a + 1, pages1.Length));
                    pages1[a].Embed = embedbuilder.Build();
                }
                await ctx.Channel.SendPaginatedMessageAsync(ctx.User, pages1);
            }
            else
            {
                await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithEmbed(new DiscordEmbedBuilder().WithDescription(lang.XPCommandGeneralFail).WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).WithColor(color: await ColorUtils.GetSingleAsync()).Build()).SendAsync(ctx.Channel);
            }
        }

        private readonly Font DiavloLight = new(SystemFonts.Find("Diavlo Light"), 30.0f);
        private readonly SolidBrush BlackBrush = new(Color.Black);

        [Command("xpcard")]
        public async Task XpCard(CommandContext ctx, DiscordUser user)
        {
            await ctx.TriggerTypingAsync();
            var outStream = new MemoryStream();
            SdImage image = new(user.GetAvatarUrl(ImageFormat.Png));
            using (var imanidiot = Image.Load((await ImageModule.ResizeAsync(await image.GetBytesAsync(HttpClient), new Size(200, 200))).Item1))
            {
                Image imge = new Image<Rgba32>(800, 240);
                imge.Mutate(async oo =>
                {
                    oo.Fill(Color.White);
                    oo.DrawImage(imanidiot, new Point(13, 20), 1);
                    using var img = ImageModule.DrawText($"{user.Username}#{user.Discriminator}", DiavloLight, Color.Black, Color.Transparent);
                    oo.DrawImage(img, new Point(229, 25), 1);
                    oo.Fill(Color.Black, new Rectangle(new(233, 83), new Size(478, 30)));
                    var o = await Database.userExperiences.FirstOrDefaultAsync(x => x.Id == user.Id);
                    if (o is not null)
                    {
                        var levelcount = GetLevel(o.XP);
                        var progress = 4.76 * GetProgressToNextLevel(o.XP);
                        oo.Fill(Color.LightGreen, new Rectangle(new(234, 84), new Size((int)progress, 28)));
                        oo.DrawText($"{o.XP}XP", DiavloLight, BlackBrush, new PointF((float)(170 + progress), 140));
                        oo.DrawText($"{GetNeededXpForNextLevel(o.XP)}XP", DiavloLight, BlackBrush, new PointF(650.95f, 120));
                        oo.DrawText($"Level: {levelcount}", DiavloLight, BlackBrush, new PointF(232, 169));
                    }
                });
                imge.Save(outStream, new PngEncoder());
            }
            outStream.Position = 0;
            await ImageModule.SendImageStream(ctx, outStream);
        }

        [Command("xpcard")]
        public async Task XpCard(CommandContext ctx)
        {
            await XpCard(ctx, ctx.User);
        }

        private BigInteger GetNeededXpForNextLevel(BigInteger xp)
        {
            BigInteger e = 69;
            int l = 1;
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
            int l = 1;
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
            int l = 1;
            while (xp / e != 0)
            {
                l++;
                e *= l;
            }
            return l;
        }
    }
}