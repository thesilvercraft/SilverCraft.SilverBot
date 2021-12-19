using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using SilverBotDS.Attributes;
using SilverBotDS.Converters;
using SilverBotDS.Exceptions;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using SixLabors.ImageSharp;

namespace SilverBotDS.Commands;

[Category("Emotes")]
public class Emotes : BaseCommandModule
{
    public DatabaseContext Database { private get; set; }
    public Config Config { private get; set; }
    public HttpClient HttpClient { private get; set; }

    [Command("addemote")]
    [Aliases("addemoji")]
    [Description("Wanna add a emote but discord is too complicated to navigate. You need to add attachment here ")]
    [RequireGuild]
    [RequirePermissions(Permissions.ManageEmojis)]
    public async Task AddEmote(CommandContext ctx, [Description("Name like `Kappa`")] string name,
        [Description("Url of the thing")] SdImage url)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        var bytes = await url.GetBytesAsync(HttpClient);
        var st = new MemoryStream(bytes);
        try
        {
            if (bytes.Length > 256000)
            {
                await ctx.RespondAsync(string.Format(lang.EmoteWasLargerThan256K, FileSizeUtils.FormatSize(st.Length)));
                st = (await ImageModule.ResizeAsync(bytes, new Size(128, 128))).Item1;
            }

            var emote = await ctx.Guild.CreateEmojiAsync(name, st,
                reason: "Added via silverbot by " + ctx.User.Username);

            await ctx.RespondAsync(emote.ToString());
        }
        finally
        {
            await st.DisposeAsync();
        }
    }

    [Command("addemote")]
    [Description("Wanna add a emote but discord is too complicated to navigate. You need to add attachment here ")]
    [RequireGuild]
    [RequirePermissions(Permissions.ManageEmojis)]
    public async Task AddEmote(CommandContext ctx, [Description("Name like `Kappa`")] string name)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        try
        {
            var image = SdImage.FromContext(ctx);
            await AddEmote(ctx, name, image);
        }
        catch (AttachmentCountIncorrectException acie)
        {
            if (acie.AttachmentCount is AttachmentCountIncorrect.TooLittleAttachments)
                await ctx.RespondAsync(lang.NoImageGeneric);
            else if (acie.AttachmentCount is AttachmentCountIncorrect.TooManyAttachments)
                await ctx.RespondAsync(lang.MoreThanOneImageGeneric);
        }
    }

    [Command("allemotes")]
    [Aliases("allemoji", "allemojis")]
    [Description("Get all the emotes from the SilverSocial enabled servers")]
    [RequireGuildDatabaseValueAttribute("EmotesOptin", true, true)]
    public async Task Allemotes(CommandContext ctx)
    {
        var builder = new StringBuilder();
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        var serverthatareoptedin = Database.GetIdsOfEmoteOptedInServers();
        var pages = new List<Page>();
        var b = new DiscordEmbedBuilder();
        b.WithTitle(lang.AllAvailibleEmotes);
        b.WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
        foreach (var a in ctx.Client.Guilds.Values)
        {
            var thing = serverthatareoptedin.Contains(a.Id);
            if (!thing) continue;
            builder.AppendFormat(lang.Server, a.Name).AppendLine();
            foreach (var emote in a.Emojis.Values.ToList())
            {
                if (emote.IsAnimated)
                {
                    builder.Append("<a:");
                    builder.Append(emote.Name);
                    builder.Append(':');
                    builder.Append(emote.Id);
                    builder.Append('>');
                }
                else
                {
                    builder.Append("<:");
                    builder.Append(emote.Name);
                    builder.Append(':');
                    builder.Append(emote.Id);
                    builder.Append('>');
                }

                builder.Append('\t');
                if (builder.Length is > 1900 and < 2000)
                {
                    b.WithDescription(builder.ToString());
                    pages.Add(new Page(embed: b));
                    builder.Clear();
                }
            }

            builder.AppendLine();
        }

        if (builder.Length != 0)
        {
            b.WithDescription(builder.ToString());
            pages.Add(new Page(embed: b));
            builder.Clear();
        }

        for (var indx = 0; indx < pages.Count; indx++)
        {
            var page = pages[indx];
            var discordEmbed = new DiscordEmbedBuilder(page.Embed);
            discordEmbed.WithAuthor(string.Format(lang.PageNuget, indx + 1, pages.Count));
            pages[indx].Embed = discordEmbed.Build();
        }

        var interactivity = ctx.Client.GetInteractivity();
        await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages, token: new CancellationToken());
    }

    [Command("emote")]
    [Aliases("emoji")]
    [Description("Get an emote from the SilverSocial enabled servers")]
    [RequireGuildDatabaseValueAttribute("EmotesOptin", true, true)]
    public async Task GetEmotes(CommandContext ctx, [Description("Emote name like :pog: or pog")] string emote)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        var emotes = new List<DiscordEmoji>();
        var serverthatareoptedin = Database.GetIdsOfEmoteOptedInServers();
        foreach (var emojis in from a in ctx.Client.Guilds.Values.Where(e => serverthatareoptedin.Contains(e.Id))
                 let emojis = a.Emojis.Values.Where(x =>
                     ":" + x.Name + ":" == emote || x.Name == emote ||
                     Regex.IsMatch(emote, @"^\d+$") && x.Id == Convert.ToUInt64(emote))
                 select emojis)
            emotes.AddRange(emojis);
        switch (emotes.Count)
        {
            case 0:
            {
                await ctx.RespondAsync(new DiscordEmbedBuilder().WithTitle(lang.NoEmotesFound)
                    .WithDescription(string.Format(lang.SearchedFor, emote))
                    .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).Build());
                break;
            }
            case 1 when emotes[0].IsAnimated:
            {
                await ctx.RespondAsync("<a:" + emotes[0].Name + ":" + emotes[0].Id + ">");
                break;
            }

            case 1:
            {
                await ctx.RespondAsync("<:" + emotes[0].Name + ":" + emotes[0].Id + ">");
                break;
            }

            case > 1:
            {
                var b = new DiscordEmbedBuilder();
                var builder = new StringBuilder();
                foreach (var e in emotes)
                    if (e.IsAnimated)
                    {
                        builder.Append("<a:");
                        builder.Append(e.Name);
                        builder.Append(':');
                        builder.Append(e.Id);
                        builder.Append('>');
                        builder.AppendLine();
                    }
                    else
                    {
                        builder.Append("<:");
                        builder.Append(e.Name);
                        builder.Append(':');
                        builder.Append(e.Id);
                        builder.Append('>');
                        builder.AppendLine();
                    }

                b.WithTitle(lang.MultipleEmotesFound).WithDescription(builder.ToString())
                    .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
                await ctx.RespondAsync(b.Build());
                break;
            }
            default:
            {
                throw new InvalidOperationException("emote switch statement went to default");
            }
        }
    }

    [Command("optintoemotes")]
    [Aliases("optintoemoji", "optintoemojis")]
    [RequireUserPermissions(Permissions.ManageGuild)]
    [RequireGuild]
    public async Task Optin(CommandContext ctx)
    {
        var isoptedin = Database.IsOptedInEmotes(ctx.Guild.Id);
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        if (isoptedin)
        {
            var bob = new DiscordEmbedBuilder();
            bob.WithTitle(lang.AlreadyOptedIn);
            bob.WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            await ctx.RespondAsync(bob.Build());
            return;
        }

        if (Database.IsBanned(ctx.User.Id) || Database.IsBanned(ctx.Guild.OwnerId))
        {
            var bob = new DiscordEmbedBuilder();
            bob.WithTitle(lang.UserIsBannedFromSilversocial);
            bob.WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            await ctx.RespondAsync(bob.Build());
            return;
        }

        Database.OptIntoEmotes(ctx.Guild.Id);
        var b = new DiscordEmbedBuilder();
        b.WithTitle(lang.OptedIn)
            .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
        await ctx.RespondAsync(b.Build());
    }
}