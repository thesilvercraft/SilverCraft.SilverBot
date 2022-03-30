using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SilverBotDS.Attributes;
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

namespace SilverBotDS.Commands;

[Category("General")]
public sealed class Genericcommands : BaseCommandModule
{
    public Config Config { private get; set; }
    public HttpClient HttpClient { private get; set; }

    [Command("hi")]
    [Description("Hello fellow human! beep boop")]
    public async Task GreetCommand(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
            .WithContent(string.Format(lang.Hi, ctx.User.Mention))
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
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        var b = new DiscordEmbedBuilder();
        var client = HttpClient;
        var rm = await client.GetAsync("https://meme-api.herokuapp.com/gimme");
        if (rm.StatusCode == HttpStatusCode.OK)
        {
            var asdf = JsonSerializer.Deserialize<Meme>(await rm.Content.ReadAsStringAsync());
            if (asdf?.Nsfw == false)
            {
                b.WithTitle(lang.Meme + asdf.Title)
                    .WithUrl(asdf.PostLink)
                    .WithAuthor($"👍 {asdf.Ups} | r/{asdf.Subreddit}")
                    .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
                    .AddField("NSFW", BoolToEmoteString(asdf.Nsfw), true)
                    .AddField("Spoiler", BoolToEmoteString(asdf.Spoiler), true)
                    .AddField("Author", asdf.Author, true)
                    .WithImageUrl(asdf.Url)
                    .WithColor(await ColorUtils.GetSingleAsync());
            }
            else
            {
                b.WithTitle("Meme returned null")
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
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await new DiscordMessageBuilder()
            .WithReply(ctx.Message.Id)
            .WithContent(string.Format(lang.TimeInUtc, DateTime.UtcNow.DateTimeToTimeStamp(TimestampFormat.LongDateTime)))
            .SendAsync(ctx.Channel);
    }

    [Command("invite")]
    [Description("Invite me to your server")]
    public async Task Invite(CommandContext ctx)
    {
        await new DiscordMessageBuilder()
            .WithReply(ctx.Message.Id)
            .WithContent(
                $"{ctx.User.Mention} https://discord.com/api/oauth2/authorize?client_id={ctx.Client.CurrentUser.Id}&permissions=1278602326&scope=bot%20applications.commands")
            .WithAllowedMentions(Mentions.None)
            .SendAsync(ctx.Channel);
    }

    [Command("ping")]
    public async Task Ping(CommandContext ctx)
    {
        await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithContent($"🏓 Pong! {ctx.Client.Ping}ms")
            .SendAsync(ctx.Channel);
    }

    [Command("dump")]
    [RequirePermissions(Permissions.ReadMessageHistory | Permissions.SendMessages)]
    [Description("Dump a messages raw content (useful for emote walls)")]
    public async Task DumpMessage(CommandContext ctx, DiscordMessage message)
    {
        await using var outStream = new MemoryStream(Encoding.UTF8.GetBytes(message.Content))
        {
            Position = 0
        };
        await new DiscordMessageBuilder()
            .WithReply(ctx.Message.Id)
            .WithContent($"{ctx.User.Mention}")
            .WithAllowedMentions(Mentions.None)
            .WithFile("message.txt", outStream)
            .SendAsync(ctx.Channel);
    }

    [Command("dump")]
    [Description("Dump a messages raw content (useful for emote walls)")]
    public async Task DumpMessage(CommandContext ctx)
    {
        if (ctx.Message.ReferencedMessage != null)
        {
            await DumpMessage(ctx, ctx.Message.ReferencedMessage);
        }
        else
        {
            await DumpMessage(ctx, (await ctx.Channel.GetMessagesBeforeAsync(ctx.Message.Id, 1))[0]);
        }
    }

    [Command("duckhosting")]
    [Aliases("dukthosting", "ducthosting", ":duckhosting:", "<:duckhosting:797225115837792367>")]
    [Description("SilverHosting best")]
    public async Task Dukt(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
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

    private static async Task SimpleImageMeme(CommandContext ctx, string imageurl, string title = null,
        string content = null, Language language = null)
    {
        language ??= await Language.GetLanguageFromCtxAsync(ctx);
        var embedBuilder = new DiscordEmbedBuilder()
            .WithFooter(language.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto))
            .WithImageUrl(imageurl).WithColor(await ColorUtils.GetSingleAsync());
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

    [Command("monke")]
    [Aliases(":monkey:", "🐒", "🐵", ":monkey_face:")]
    [Description("Reject humanity return to monke")]
    public async Task Monke(CommandContext ctx)
    {
        await SimpleImageMeme(ctx, "https://i.kym-cdn.com/photos/images/newsfeed/001/867/677/40d.jpg");
    }

    public static async Task<bool> IsAtSilverCraftAsync(DiscordClient discord, DiscordUser b, Config cnf)
    {
        return (await discord.GetGuildAsync(cnf.ServerId)).Members.ContainsKey(b.Id);
    }

    //TODO reimplement "bot" command
    [Command("user")]
    [Description("Get the info I know about a specified user")]
    public async Task Userinfo(CommandContext ctx, [Description("the user like duh")] DiscordUser a)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await new DiscordMessageBuilder().WithEmbed(new DiscordEmbedBuilder()
            .WithTitle(lang.User + a.Username)
            .WithDescription(lang.InformationAbout + a.Mention)
            .AddField(lang.Userid, a.Id.ToString(), true)
            .AddField(lang.JoinedSilverCraft, BoolToEmoteString(await IsAtSilverCraftAsync(ctx.Client, a, Config)),
                true)
            .AddField(lang.IsAnOwner, BoolToEmoteString(ctx.Client.CurrentApplication.Owners.Contains(a)), true)
            .AddField(lang.IsABot, BoolToEmoteString(a.IsBot), true)
            .AddField(lang.AccountCreationDate, a.CreationTimestamp.UtcDateTime.DateTimeToTimeStamp(TimestampFormat.LongDateTime), true)
            .AddField(lang.AccountJoinDate,
                ctx.Guild?.Members.ContainsKey(a.Id) == true
                    ? ctx.Guild?.Members[a.Id].JoinedAt.UtcDateTime.DateTimeToTimeStamp(TimestampFormat.LongDateTime)
                    : "NA", true)
            .WithColor(await ColorUtils.GetSingleAsync())
            .WithThumbnail(a.GetAvatarUrl(ImageFormat.Png))
            .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
            .Build()).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
    }

    [Command("user")]
    [Description("Get the info I know about a specified user")]
    public async Task Userinfo(CommandContext ctx)
    {
        await Userinfo(ctx, ctx.User);
    }
}