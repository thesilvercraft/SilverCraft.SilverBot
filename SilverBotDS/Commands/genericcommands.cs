using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static SilverBotDS.Utils.StringUtils;

namespace SilverBotDS.Commands
{
    internal sealed class Genericcommands : BaseCommandModule
    {
#pragma warning disable CA1822 // Mark members as static
        public ISBDatabase Database { private get; set; }
        public Config Config { private get; set; }
        public HttpClient HttpClient { private get; set; }

        [Command("hi")]
        [Description("Hello fellow human! beep boop")]
        public async Task GreetCommand(CommandContext ctx)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                             .WithContent(string.Format(lang.Hi, ctx.Member.Mention))
                                             .SendAsync(ctx.Channel);
        }

        [Command("spinningfox")]
        [Description("fox go brrrrrrrr")]
        public async Task Fox(CommandContext ctx)
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                             .WithContent("https://media.discordapp.net/attachments/811583810264629252/824266450818695168/image0-1.gif")
                                             .SendAsync(ctx.Channel);
        }

        [Command("meme")]
        public async Task Kindsffeefdfdfergergrgfdfdsgfdfg(CommandContext ctx)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            var b = new DiscordEmbedBuilder();
            var client = HttpClient;
            var rm = await client.GetAsync("https://meme-api.herokuapp.com/gimme");
            if (rm.StatusCode == HttpStatusCode.OK)
            {
                var asdf = JsonSerializer.Deserialize<Meme>(await rm.Content.ReadAsStringAsync());
                if (asdf != null && !asdf.Nsfw)
                {
                    b.WithTitle(lang.Meme + asdf.Title)
                        .WithUrl(asdf.PostLink)
                        .WithAuthor("👍 " + asdf.Ups + " | r/" + asdf.Subreddit)
                        .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
                        .AddField("NSFW", asdf.Nsfw.ToString(), true)
                        .AddField("Spoiler", asdf.Spoiler.ToString(), true)
                        .AddField("Author", asdf.Author, true)
                        .WithImageUrl(asdf.Url)
                        .WithColor(await ColorUtils.GetSingleAsync());
                }
                else
                {
                    b.WithTitle("meme returned null")
                        .WithColor(await ColorUtils.GetSingleAsync());
                }

                await new DiscordMessageBuilder()
                                                 .WithReply(ctx.Message.Id)
                                                 .WithEmbed(b.Build())
                                                 .SendAsync(ctx.Channel);
            }
            else
            {
                b.WithDescription(rm.StatusCode.ToString());
                await new DiscordMessageBuilder()
                                                 .WithReply(ctx.Message.Id)
                                                 .WithEmbed(b.Build())
                                                 .SendAsync(ctx.Channel);
            }
        }

        [Command("time")]
        [Description("Get the time in UTC")]
        public async Task Time(CommandContext ctx)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            await new DiscordMessageBuilder()
                                             .WithReply(ctx.Message.Id)
                                             .WithContent(string.Format(lang.TimeInUtc, DateTime.UtcNow.ToString(lang.TimeFormat)))
                                             .SendAsync(ctx.Channel);
        }

        [Command("invite")]
        [Description("Invite me to your place")]
        public async Task Invite(CommandContext ctx)
        {
            await new DiscordMessageBuilder()
                                             .WithReply(ctx.Message.Id)
                                             .WithContent($"https://discord.com/api/oauth2/authorize?client_id={ctx.Client.CurrentUser.Id}&permissions=2147483639&scope=bot")
                                             .SendAsync(ctx.Channel);
        }

        [Command("reality")]
        [Description("JUST MONIKA")]
        [Hidden]
        public async Task Reality(CommandContext ctx)
        {
            await new DiscordMessageBuilder()
                                             .WithReply(ctx.Message.Id)
                                             .WithContent($"Every day, I imagine a future where I can be with you\nIn my hand is a pen that will write a poem of me and you\nThe ink flows down into a dark puddle.\nJust move your hand - write the way into his heart!\nBut in this world of infinite choices,\nWhat will it take just to find that special day?\nHave I found everybody a fun assignment to do today?\nWhen you're here, everything that we do is fun for them anyway.\nWhen I can't even read my own feelings\nWhat good are words when a smile says it all?\nAnd if this world won't write me an ending\nWhat will it take just for me to end it all?\nDoes my pen only write bitter words for those who are dear to me?\nIs it love if I take you, or is it love if I set you free?\nThe ink flows down into a dark puddle\nHow can I write love into reality?\nAnd in your reality, if I don't know how to love you\nI'll leave you be")
                                             .SendAsync(ctx.Channel);
        }

        [Command("monika")]
        [Description("JUST MONIKA")]
        [Hidden]
        public async Task Monika(CommandContext ctx)
        {
            await SimpleImageMeme(ctx, "https://static.wikia.nocookie.net/doki-doki-literature-club/images/e/ef/Monika_Illustration.png/revision/latest?cb=20190319051314", "Monika", "Just monika");
        }

        [Command("ping")]
        public async Task Ping(CommandContext ctx) => await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithContent($"🏓 Pong! {ctx.Client.Ping}ms").SendAsync(ctx.Channel);

        [Command("dump")]
        [Description("Dump a messages raw content (useful for emote walls)")]
        public async Task DumpMessage(CommandContext ctx, DiscordMessage message)
        {
            await using var outStream = new MemoryStream(Encoding.UTF8.GetBytes(message.Content))
            {
                Position = 0
            };
            await new DiscordMessageBuilder()
                                             .WithReply(ctx.Message.Id)
                                             .WithFile("message.txt", outStream)
                                             .SendAsync(ctx.Channel);
        }

        [Command("duckhosting"), Aliases("dukthosting", "ducthosting", ":duckhosting:", "<:duckhosting:797225115837792367>")]
        [Description("SilverHosting best")]
        public async Task Dukt(CommandContext ctx)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            await new DiscordMessageBuilder()
                                            .WithReply(ctx.Message.Id)
                                            .WithEmbed(new DiscordEmbedBuilder()
                                                                                .WithTitle(lang.SilverhostingJokeTitle)
                                                                                .WithDescription(lang.SilverhostingJokeDescription)
                                                                                .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
                                                                                .WithColor(await ColorUtils.GetSingleAsync())
                                                                                .Build())
                                            .SendAsync(ctx.Channel);
        }

        private static async Task SimpleImageMeme(CommandContext ctx, string imageurl, string title = null, string content = null)
        {
            var embedBuilder = new DiscordEmbedBuilder().WithFooter((await Language.GetLanguageFromCtxAsync(ctx)).RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto)).WithImageUrl(imageurl).WithColor(await ColorUtils.GetSingleAsync());
            var messageBuilder = new DiscordMessageBuilder();
            if (title != null)
            {
                embedBuilder.WithTitle(title);
            }
            if (content != null)
            {
                messageBuilder.WithContent(content);
            }
            await messageBuilder
        .WithReply(ctx.Message.Id)
        .WithEmbed(embedBuilder.Build())
        .SendAsync(ctx.Channel);
        }

        [Command("nou")]
        public async Task No_U(CommandContext ctx)
        {
            await SimpleImageMeme(ctx, "https://i.kym-cdn.com/photos/images/facebook/001/441/633/3c5.jpg");
        }

        [Command("nou")]
        public async Task No_U(CommandContext ctx, DiscordMember member)
        {
            await SimpleImageMeme(ctx, "https://i.kym-cdn.com/photos/images/facebook/001/441/633/3c5.jpg", content: member.Mention);
        }

        [Command("ejectmax")]
        public async Task EjectMax(CommandContext ctx)
        {
            await SimpleImageMeme(ctx, "https://cdn.discordapp.com/attachments/736664393630220289/786297198404304987/eject.gif");
        }

        [Command("booty")]
        [Hidden]
        public async Task Booty(CommandContext ctx)
        {
            await SimpleImageMeme(ctx, "https://media1.tenor.com/images/ce2e9a0a24f384a9486acfac9bf7f5c1/tenor.gif?itemid=17561816", content: "picture📸my🙋‍♂️booty🍑up🆙in 3D🤩i'll 🙋‍♂️shake🤝 my🙋‍♂️ booty🍑in my🙋‍♂️own movie");
        }

        [Command("monke"), Aliases(":monkey:", "🐒", "🐵", ":monkey_face:")]
        [Description("Reject humanity return to monke")]
        public async Task Monke(CommandContext ctx)
        {
            await SimpleImageMeme(ctx, "https://i.kym-cdn.com/photos/images/newsfeed/001/867/677/40d.jpg");
        }

        [Command("uselessfact")]
        [Description("Wanna hear some useless fact? Just ask me")]
        public async Task UselessFact(CommandContext ctx)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            var client = HttpClient;
            var rm = await client.GetAsync("https://uselessfacts.jsph.pl/random.md?language=" + lang.LangCodeForUselessFacts);
            await new DiscordMessageBuilder()
                                                 .WithReply(ctx.Message.Id)
                                                 .WithEmbed(new DiscordEmbedBuilder()
                                                                                     .WithTitle(lang.UselessFact)
                                                                                     .WithDescription(await rm.Content.ReadAsStringAsync())
                                                                                     .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto))
                                                                                     .WithColor(await ColorUtils.GetSingleAsync())
                                                                                     .Build())
                                                 .SendAsync(ctx.Channel);
        }

        private async Task<bool> IsAtSilverCraftAsync(CommandContext ctx, DiscordUser b)
        {
            return (await ctx.Client.GetGuildAsync(Config.ServerId)).Members.ContainsKey(b.Id);
        }

        [Command("bot")]
        [Description("Don't know what some new bot your friends invited here but you dont want to google? just ask me ")]
        public async Task WhoIsBot(CommandContext ctx, [Description("the bot")] DiscordUser user)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            if (string.IsNullOrEmpty(Config.TopggSidToken) || Config.TopggSidToken.ToLower() == "none")
            {
                await new DiscordMessageBuilder()
                                                 .WithReply(ctx.Message.Id)
                                                 .WithEmbed(new DiscordEmbedBuilder()
                                                                                     .WithTitle(lang.CommandIsDisabled)
                                                                                     .WithColor(color: await ColorUtils.GetSingleAsync()))
                                                 .SendAsync(ctx.Channel);
                return;
            }
            if (!user.IsBot)
            {
                await new DiscordMessageBuilder()
                                                  .WithReply(ctx.Message.Id)
                                                  .WithEmbed(new DiscordEmbedBuilder()
                                                                                      .WithTitle(lang.UserIsntBot)
                                                                                      .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto))
                                                                                      .WithColor(color: await ColorUtils.GetSingleAsync())
                                                                                      .Build())
                                                  .SendAsync(ctx.Channel);
                return;
            }

            Dbla.Client client = new(Config.TopggSidToken, Config.TopggIsSelfbot);
            var bot = await client.GetViaIdAsync(user.Id);

            if (bot == null)
            {
                await new DiscordMessageBuilder()
                                                  .WithReply(ctx.Message.Id)
                                                  .WithEmbed(new DiscordEmbedBuilder()
                                                                                      .WithTitle(lang.DblaReturnedNull)
                                                                                      .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto))
                                                                                      .WithColor(color: await ColorUtils.GetSingleAsync())
                                                                                      .Build())
                                                  .SendAsync(ctx.Channel);
            }
            else
            {
                await new DiscordMessageBuilder()
                                                .WithReply(ctx.Message.Id)
                                                .WithEmbed(new DiscordEmbedBuilder()
                                                                                    .WithAuthor(user.Username, bot.Website, user.GetAvatarUrl(ImageFormat.Auto))
                                                                                    .WithThumbnail(user.GetAvatarUrl(ImageFormat.Auto))
                                                                                    .WithDescription(bot.Shortdesc)
                                                                                    .AddField(lang.PrefixUsedTopgg, "`" + bot.Prefix + "`")
                                                                                    .WithUrl(bot.Website)
                                                                                    .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto))
                                                                                    .WithColor(color: await ColorUtils.GetSingleAsync())
                                                                                    .Build())
                                                .SendAsync(ctx.Channel);
            }
        }

        //TODO make strings in language file
        [Command("user")]
        [Description("Get the info I know about a specified user")]
        public async Task Userinfo(CommandContext ctx, [Description("the user like duh")] DiscordUser a)
        {
            try
            {
                var lang = (await Language.GetLanguageFromCtxAsync(ctx));
                await new DiscordMessageBuilder().WithEmbed(new DiscordEmbedBuilder()
                    .WithTitle(lang.User + a.Username)
                    .WithDescription(lang.InformationAbout + a.Mention)
                    .AddField(lang.Userid, a.Id.ToString(), true)
                    .AddField(lang.JoinedSilverCraft, BoolToEmoteString(await IsAtSilverCraftAsync(ctx, a)), true)
                    .AddField(lang.IsAnOwner, BoolToEmoteString(ctx.Client.CurrentApplication.Owners.Contains(a)), true)
                    .AddField(lang.IsABot, BoolToEmoteString(a.IsBot), true)
                    .WithColor(await ColorUtils.GetSingleAsync())
                    .WithThumbnail(a.GetAvatarUrl(ImageFormat.Png))
                    .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
                    .Build()).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
            }
            catch (Exception e)
            {
                Program.SendLog(e);
            }
        }

#pragma warning restore CA1822 // Mark members as static
    }
}