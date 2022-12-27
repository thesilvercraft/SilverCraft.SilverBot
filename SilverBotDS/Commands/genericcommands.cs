/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Humanizer;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SilverBotDS.Attributes;
using static SilverBotDS.Utils.StringUtils;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands;

[Category("General")]
[RequireModuleGuildEnabled(EnabledModules.Generic, true)]

public sealed class Genericcommands : BaseCommandModule
{
    public Config Config { private get; set; }
    public HttpClient HttpClient { private get; set; }
    public LanguageService LanguageService { private get; set; }

    [Command("hi")]
    [Description("Hello fellow human! beep boop")]
    public async Task GreetCommand(CommandContext ctx)
    {
        var lang = await LanguageService.FromCtxAsync(ctx);
        await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
            .WithContent(string.Format(lang.Hi, ctx.User.Mention))
            .SendAsync(ctx.Channel);
    }
    
    [Command("time")]
    [Description("Get the time in UTC")]
    public async Task Time(CommandContext ctx)
    {
        var lang = await LanguageService.FromCtxAsync(ctx);
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
       var msg= await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithContent($"🏓 Pong! {ctx.Client.Ping}ms")
            .SendAsync(ctx.Channel);
        await msg.ModifyAsync($"🏓 Pong! {ctx.Client.Ping.Milliseconds().Humanize()} ({( msg.Timestamp - ctx.Message.Timestamp).Humanize()} to send, {(DateTime.UtcNow - ctx.Message.Timestamp).Humanize()} round trip)");
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
            .WithContent(ctx.User.Mention)
            .WithAllowedMentions(Mentions.None)
            .AddFile("message.txt", outStream)
            .SendAsync(ctx.Channel);
    }
    [Command("archive")]
    [RequirePermissions(Permissions.ReadMessageHistory | Permissions.SendMessages | Permissions.AttachFiles)]
    [Description("Archive a message (and its attachments)")]
    public async Task ArchiveMessage(CommandContext ctx, DiscordMessage message)
    {
        await using MemoryStream memoryStream = new();
        using (ZipArchive zip = new(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var f in message.Attachments)
            {
                var zipItem = zip.CreateEntry($"{f.FileName}");
                await using MemoryStream originalFileMemoryStream = new(await HttpClient.GetByteArrayAsync(f.ProxyUrl));
                await using var entryStream = zipItem.Open();
                await originalFileMemoryStream.CopyToAsync(entryStream);
            }
            var zipItemmsg = zip.CreateEntry($"message.txt");
            await using MemoryStream zipItemmsgfs = new(Encoding.UTF8.GetBytes(message.Content)) { Position=0};
            await using var entryStreammsg = zipItemmsg.Open();
            await zipItemmsgfs.CopyToAsync(entryStreammsg);
        }

        memoryStream.Position = 0;
        await new DiscordMessageBuilder()
            .WithReply(ctx.Message.Id)
            .WithContent($"{ctx.User.Mention}")
            .WithAllowedMentions(Mentions.None)
            .AddFile("message.zip", memoryStream)
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
        var lang = await LanguageService.FromCtxAsync(ctx);
        await new DiscordMessageBuilder()
            .WithReply(ctx.Message.Id)
            .WithEmbed(new DiscordEmbedBuilder()
                .WithTitle(lang.SilverhostingJokeTitle)
                .WithDescription(lang.SilverhostingJokeDescription)
                .AddRequestedByFooter(ctx,lang)
                .Build())
            .SendAsync(ctx.Channel);
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
        var lang = await LanguageService.FromCtxAsync(ctx);
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
            .AddRequestedByFooter(ctx,lang)
            .Build()).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
    }

    [Command("user")]
    [Description("Get the info I know about a specified user")]
    public async Task Userinfo(CommandContext ctx)
    {
        await Userinfo(ctx, ctx.User);
    }
}