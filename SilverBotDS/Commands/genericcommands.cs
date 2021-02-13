using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using SilverBotDS.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace SilverBotDS.Commands
{
    internal class Genericcommands : BaseCommandModule
    {
#pragma warning disable CA1822 // Mark members as static

        [Command("hi")]
        [Description("Hello fellow human! beep boop")]
        public async Task GreetCommand(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromCtx(ctx);
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                             .WithContent(string.Format(lang.Hi, ctx.Member.Mention))
                                             .SendAsync(ctx.Channel);
        }

        [Command("meme")]
        public async Task Kindsffeefdfdfergergrgfdfdsgfdfg(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromCtx(ctx);
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            System.Net.Http.HttpClient client = WebClient.Get();
            HttpResponseMessage rm = await client.GetAsync("https://meme-api.herokuapp.com/gimme");
            if (rm.StatusCode == HttpStatusCode.OK)
            {
                Meme asdf = JsonSerializer.Deserialize<Meme>(await rm.Content.ReadAsStringAsync());
                b.WithTitle(lang.Meme + asdf.Title)
                .WithUrl(asdf.PostLink)
                .WithAuthor("👍 " + asdf.Ups + " | r/" + asdf.Subreddit)
                .WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
                .AddField("NSFW", asdf.Nsfw.ToString(), true)
                .AddField("Spoiler", asdf.Spoiler.ToString(), true)
                .AddField("Author", asdf.Author, true)
                .WithImageUrl(asdf.Url);
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
            Language lang = Language.GetLanguageFromCtx(ctx);
            await new DiscordMessageBuilder()
                                             .WithReply(ctx.Message.Id)
                                             .WithContent(string.Format(lang.Time_In_Utc, DateTime.UtcNow.ToString(lang.Time_format)))
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

        [Command("dump")]
        [Description("Dump a messages raw content (useful for emote walls)")]
        public async Task DumpMessage(CommandContext ctx, DiscordMessage message)
        {
            using MemoryStream outStream = new MemoryStream(Encoding.UTF8.GetBytes(message.Content))
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
            Language lang = Language.GetLanguageFromCtx(ctx);
            await new DiscordMessageBuilder()
                                            .WithReply(ctx.Message.Id)
                                            .WithEmbed(new DiscordEmbedBuilder()
                                                                                .WithTitle(lang.Silverhosting_joke_title)
                                                                                .WithDescription(lang.Silverhosting_joke_description)
                                                                                .WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
                                                                                .Build())
                                            .SendAsync(ctx.Channel);
        }

        private async Task SimpleImageMeme(CommandContext ctx, string imageurl, string title = null, string content = null)
        {
            var EmbedBuilder = new DiscordEmbedBuilder().WithFooter(Language.GetLanguageFromCtx(ctx).Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto)).WithImageUrl(imageurl);
            var MessageBuilder = new DiscordMessageBuilder();
            if (title != null)
            {
                EmbedBuilder.WithTitle(title);
            }
            if (content != null)
            {
                MessageBuilder.WithContent(content);
            }
            await MessageBuilder
        .WithReply(ctx.Message.Id)
        .WithEmbed(EmbedBuilder.Build())
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
            Language lang = Language.GetLanguageFromCtx(ctx);
            System.Net.Http.HttpClient client = WebClient.Get();
            HttpResponseMessage rm = await client.GetAsync("https://uselessfacts.jsph.pl/random.md?language=" + lang.Lang_code_for_useless_facts);
            await new DiscordMessageBuilder()
                                                 .WithReply(ctx.Message.Id)
                                                 .WithEmbed(new DiscordEmbedBuilder()
                                                                                     .WithTitle(lang.Useless_fact)
                                                                                     .WithDescription(await rm.Content.ReadAsStringAsync())
                                                                                     .WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto))
                                                                                     .Build())
                                                 .SendAsync(ctx.Channel);
        }

        //TODO make it better
        [Command("nuget"), Aliases("nu")]
        [Description("Search up packages on the NuGet")]
        public async Task Nuget(CommandContext ctx, [Description("the name of the package")] string query)
        {
            Language lang = Language.GetLanguageFromCtx(ctx);
            try
            {
                NuGetUtils.Datum[] data = await NuGetUtils.SearchAsync(query);
                InteractivityExtension interactivity = ctx.Client.GetInteractivity();
                List<Page> pages = new List<Page>();
                DiscordEmbedBuilder tempbuilder;
                for (int i = 0; i < data.Length; i++)
                {
                    tempbuilder = new DiscordEmbedBuilder();
                    tempbuilder.WithTitle(data[i].Title);
                    tempbuilder.WithUrl($"https://www.nuget.org/packages/{data[i].Id}");
                    tempbuilder.WithAuthor(StringUtils.ArrayToString(data[i].Authors, ","), data[i].ProjectUrl);
                    tempbuilder.WithFooter(lang.Requested_by + ctx.User.Username + " " + string.Format(lang.Page_Nuget, i + 1, data.Length), ctx.User.GetAvatarUrl(ImageFormat.Png));
                    if (!string.IsNullOrEmpty(data[i].IconUrl))
                    {
                        tempbuilder.WithThumbnail(data[i].IconUrl);
                    }
                    tempbuilder.WithDescription(data[i].Description);
                    tempbuilder.AddField("NuGet verified", StringUtils.BoolToEmoteString(data[i].Verified), true);
                    if (!string.IsNullOrEmpty(data[i].Type))
                    {
                        tempbuilder.AddField("Type", data[i].Type, true);
                    }

                    tempbuilder.AddField("<:green_download_icon:805051604797227038>", data[i].TotalDownloads.ToString(), true);
                    tempbuilder.AddField("V", data[i].Version, true);
                    pages.Add(new Page(embed: tempbuilder));
                }

                await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages, timeoutoverride: new TimeSpan(0, 2, 0));
            }
            catch (Exception e)
            {
                DiscordEmbedBuilder bob = new DiscordEmbedBuilder();
                bob.WithTitle("Something went fucky wucky on my side");
                bob.WithDescription("Try again a little later?");
                await ctx.RespondAsync(embed: bob.Build());
                await Program.SendLogAsync(e.Message, new());
                throw;
            }
        }

        public static async Task<bool> Is_at_silvercraftAsync(CommandContext ctx, DiscordUser b)
        {
            bool is_at_silvercraft = false;

            if ((await ctx.Client.GetGuildAsync(Convert.ToUInt64(Program.GetConfig().ServerId))).Members[b.Id] != null)
            {
                is_at_silvercraft = true;
            }
            return is_at_silvercraft;
        }

        public static async Task<bool> Is_bot_adminAsync(CommandContext ctx, DiscordUser b)
        {
            bool isbotadmin = false;
            DiscordMember socketGuildus = (await ctx.Client.GetGuildAsync(Convert.ToUInt64(Program.GetConfig().ServerId))).Members[b.Id];
            foreach (DiscordRole role in socketGuildus.Roles)
            {
                if (role.Id == Convert.ToUInt64(Program.GetConfig().AdminRoleId))
                {
                    isbotadmin = true;
                }
            }
            return isbotadmin;
        }

        [Command("bot")]
        [Description("Don't know what some new bot your friends invited here but you dont want to google? just ask me ")]
        public async Task WhoIsBot(CommandContext ctx, [Description("the bot")] DiscordUser user)
        {
            Language lang = Language.GetLanguageFromCtx(ctx);
            if (string.IsNullOrEmpty(Program.GetConfig().Topgg_Sid_Token) || Program.GetConfig().Topgg_Sid_Token.ToLower() == "none")
            {
                await new DiscordMessageBuilder()
                                                 .WithReply(ctx.Message.Id)
                                                 .WithEmbed(new DiscordEmbedBuilder()
                                                                                     .WithTitle(lang.Command_Is_Disabled)
                                                                                     .WithColor(color: await ColorUtils.GetSingleAsync()))
                                                 .SendAsync(ctx.Channel);
                return;
            }
            if (!user.IsBot)
            {
                await new DiscordMessageBuilder()
                                                  .WithReply(ctx.Message.Id)
                                                  .WithEmbed(new DiscordEmbedBuilder()
                                                                                      .WithTitle(lang.User_Isnt_Bot)
                                                                                      .WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto))
                                                                                      .Build())
                                                  .SendAsync(ctx.Channel);
                return;
            }

            DBLA.Client client = new(Program.GetConfig().Topgg_Sid_Token, true);
            var bot = await client.GetViaIdAsync(user.Id);

            if (bot == null)
            {
                await new DiscordMessageBuilder()
                                                  .WithReply(ctx.Message.Id)
                                                  .WithEmbed(new DiscordEmbedBuilder()
                                                                                      .WithTitle(lang.DBLA_Returned_Null)
                                                                                      .WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto))
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
                                                                                    .AddField(lang.Prefix_Used_Topgg, "`" + bot.Prefix + "`")
                                                                                    .WithUrl(bot.Website)
                                                                                    .WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto))
                                                                                    .Build())
                                                .SendAsync(ctx.Channel);
            }
        }

        //TODO make strings in language file
        [Command("user")]
        [Description("Get the info I know about a specified user")]
        public async Task Userinfo(CommandContext ctx, [Description("the user like duh")] DiscordUser a)
        {
            Language lang = Language.GetLanguageFromCtx(ctx);
            await new DiscordMessageBuilder().WithEmbed(new DiscordEmbedBuilder()
                .WithTitle(lang.User + a.Username).WithDescription(lang.Information_About + a.Mention)
                .AddField(lang.Userid, a.Id.ToString(), true)
                .AddField(lang.Joined_SilverCraft, StringUtils.BoolToEmoteString(await Is_at_silvercraftAsync(ctx, a)), true)
                .AddField(lang.Is_SilverBot_Admin, StringUtils.BoolToEmoteString(await Is_bot_adminAsync(ctx, a)), true)
                .AddField(lang.Is_an_owner, StringUtils.BoolToEmoteString(Program.GetConfig().Botowners.Contains(a.Id)), true)
                .AddField(lang.Is_a_bot, StringUtils.BoolToEmoteString(a.IsBot), true)
                .WithColor(await ColorUtils.GetSingleAsync()).WithThumbnail(a.GetAvatarUrl(ImageFormat.Png))
                .WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).Build()).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
        }

#pragma warning restore CA1822 // Mark members as static
    }
}