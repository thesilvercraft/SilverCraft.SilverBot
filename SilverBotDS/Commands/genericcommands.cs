using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SilverBotDS
{
    internal class Genericcommands : BaseCommandModule
    {
#pragma warning disable CA1822 // Mark members as static

        [Command("hi")]
        [Description("Hello fellow human! beep boop")]
        public async Task GreetCommand(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild.Id);
            await ctx.RespondAsync(string.Format(lang.Hi, ctx.Member.Mention));
        }

        [Command("meme")]
        public async Task Kindsffeefdfdfergergrgfdfdsgfdfg(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild.Id);
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            HttpClient client = Webclient.Get();
            HttpResponseMessage rm = await client.GetAsync("https://meme-api.herokuapp.com/gimme");
            if (rm.StatusCode == HttpStatusCode.OK)
            {
                meme asdf = System.Text.Json.JsonSerializer.Deserialize<meme>(await rm.Content.ReadAsStringAsync());
                b.WithTitle(lang.Meme + asdf.title);
                b.WithUrl(asdf.postLink);
                b.WithAuthor("👍 " + asdf.ups + " | r/" + asdf.subreddit);
                b.WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
                b.AddField("NSFW", asdf.nsfw.ToString(), true);
                b.AddField("Spoiler", asdf.spoiler.ToString(), true);
                b.AddField("Author", asdf.author, true);
                b.WithImageUrl(asdf.url);
                await ctx.RespondAsync(embed: b.Build());
            }
            else
            {
                b.WithDescription(rm.StatusCode.ToString());
                await ctx.RespondAsync(embed: b.Build());
            }
        }

        [Command("time")]
        [Description("Get the time in UTC")]
        public async Task Time(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild.Id);
            await ctx.RespondAsync(string.Format(lang.Time_In_Utc, DateTime.UtcNow.ToString(lang.Time_format)));
        }

        [Command("invite")]
        [Description("Invite me to your place")]
        public async Task Invite(CommandContext ctx)
        {
            await ctx.RespondAsync($"https://discord.com/api/oauth2/authorize?client_id={ctx.Client.CurrentUser.Id}&permissions=2147483639&scope=bot");
        }

        //TODO if not removed make strings in language file
        [Obsolete("May be removed in future release")]
        [Command("duckhosting"), Aliases("dukthosting", "ducthosting", ":duckhosting:", "<:duckhosting:797225115837792367>")]
        [Description("SilverHosting best")]
        public async Task Dukt(CommandContext ctx)
        {
            DiscordEmbedBuilder bob = new DiscordEmbedBuilder();
            bob.WithTitle("SilverCraftBot sponsored by SilverHosting");
            bob.WithDescription("Use offer code [SLVR](https://www.youtube.com/watch?v=dQw4w9WgXcQ)");
            bob.WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            await ctx.RespondAsync(embed: bob.Build());
        }

        [Command("monke")]
        [Description("Reject humanity return to monke")]
        public async Task Monke(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild.Id);
            DiscordEmbedBuilder bob = new DiscordEmbedBuilder();
            bob.WithImageUrl("https://i.kym-cdn.com/photos/images/newsfeed/001/867/677/40d.jpg");
            bob.WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            await ctx.RespondAsync(embed: bob.Build());
        }

        [Command("uselessfact")]
        [Description("Wanna hear some useless fact? Just ask me")]
        public async Task UselessFact(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild.Id);
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            HttpClient client = Webclient.Get();
            HttpResponseMessage rm = await client.GetAsync("https://uselessfacts.jsph.pl/random.md?language=" + lang.Lang_code_for_useless_facts);
            b.WithTitle(lang.Useless_fact);
            b.WithDescription(await rm.Content.ReadAsStringAsync());
            b.WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            await ctx.RespondAsync(embed: b.Build());
        }

        //TODO make strings in language file
        [Command("nuget"), Aliases("nu")]
        [Description("Search up packages on the NuGet")]
        public async Task Nuget(CommandContext ctx, [Description("the name of the package")] string query)
        {
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
                    tempbuilder.WithAuthor(Stringutils.ArrayToString(data[i].Authors, ","), data[i].ProjectUrl);
                    tempbuilder.WithFooter("Requested by " + ctx.User.Username + $" Page: {i}/{data.Length}", ctx.User.GetAvatarUrl(ImageFormat.Png));
                    if (!string.IsNullOrEmpty(data[i].IconUrl))
                    {
                        tempbuilder.WithThumbnail(data[i].IconUrl);
                    }
                    tempbuilder.WithDescription(data[i].Description);
                    tempbuilder.AddField("nuget verified", Stringutils.BoolToEmoteString(data[i].Verified), true);
                    if (!string.IsNullOrEmpty(data[i].Type))
                    {
                        tempbuilder.AddField("type", data[i].Type, true);
                    }

                    tempbuilder.AddField("downloads", data[i].TotalDownloads.ToString(), true);
                    tempbuilder.AddField("version", data[i].Version, true);
                    pages.Add(new Page(embed: tempbuilder));
                }

                await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages, timeoutoverride: new TimeSpan(0, 2, 0));
            }
            catch (Exception e)
            {
                DiscordEmbedBuilder bob = new DiscordEmbedBuilder();
                bob.WithTitle("Something went fucky wucky on my side");
                bob.WithDescription("Try again a little later?\n" + e.Message);
                await ctx.RespondAsync(embed: bob.Build());
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
            Language lang = Language.GetLanguageFromId(ctx.Guild.Id);
            DiscordEmbedBuilder b = new();
            try
            {
                HttpClient client = Webclient.Get();
                HttpResponseMessage rm = await client.GetAsync("https://silverdimond.tk/silvercraftbot/bots/" + user.Id);
                b.WithAuthor(user.Username, iconUrl: user.GetAvatarUrl(ImageFormat.Auto));
                b.WithThumbnail(user.GetAvatarUrl(ImageFormat.Auto));
                if (rm.StatusCode == HttpStatusCode.OK)
                {
                    b.WithDescription(await rm.Content.ReadAsStringAsync());
                }
                else
                {
                    b.WithDescription(rm.StatusCode.ToString());
                }
                if (user.Id == 159985870458322944)
                {
                    b.WithThumbnail("https://media.discordapp.net/attachments/728360861483401240/775156721206558741/scary_mee6.png");
                }
                b.WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto));
                await ctx.RespondAsync(embed: b.Build());
            }
            catch (HttpRequestException e)
            {
                b = new();
                b.WithTitle("WebException");
                b.WithDescription(e.Message);
                b.WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto));
                await ctx.RespondAsync(embed: b.Build());
            }
        }

        //TODO make strings in language file
        [Command("user")]
        [Description("Get the info I know about a specified user")]
        public async Task Userinfo(CommandContext ctx, [Description("the user like duh")] DiscordUser a)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild.Id);
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();

            b.WithTitle(lang.User + a.Username);
            b.WithDescription("Information about " + a.Mention);
            b.AddField(lang.Userid, a.Id.ToString(), true);
            // b.AddField("Has joined the SilverCraft Server", stringutils.BoolToEmoteString(await Is_at_silvercraftAsync(ctx, a)), true);
            // b.AddField("Is a SilverCraft bot admin", stringutils.BoolToEmoteString(await Is_bot_adminAsync(ctx, a)), true);
            b.AddField(lang.Is_an_owner, Stringutils.BoolToEmoteString(Program.GetConfig().Botowners.Contains(a.Id)), true);
            if (a.Id == 208691453973495808)
            {
                b.AddField(lang.Is_a_bot, ":white_check_mark: see https://discord.com/channels/714154158969716780/714154159590473801/767829209052872724", true);
            }
            else
            {
                b.AddField(lang.Is_a_bot, Stringutils.BoolToEmoteString(a.IsBot), true);
            }
            b.WithThumbnail(a.GetAvatarUrl(ImageFormat.Png));
            b.WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            await ctx.RespondAsync(embed: b.Build());
        }

#pragma warning restore CA1822 // Mark members as static
    }
}