using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using SilverBotDS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SilverBotDS.Objects;

namespace SilverBotDS.Commands
{
    internal class Emotes : BaseCommandModule
    {
        public static Emotes CreateInstance()
        {
            return new Emotes();
        }
#pragma warning disable CA1822 // Mark members as static

        [Command("addemote")]
        [Description("Wanna add a emote but discord is too complicated to navigate")]
        [RequireGuild()]
        [RequirePermissions(Permissions.ManageEmojis)]
        public async Task UselessFact(CommandContext ctx, [Description("Name like `Kappa`")] string name, [Description("url of emote")] string url)
        {
            var lang = Language.GetLanguageFromCtx(ctx);
            var client = WebClient.Get();
            var rm = await client.GetAsync(url);
            var st = await rm.Content.ReadAsStreamAsync();
            if (st.Length > 256 * 1000)
            {
                await ctx.RespondAsync(string.Format(lang.EmoteWasLargerThan256K, FileSizeUtils.FormatSize(st.Length)));
            }
            var emote = await ctx.Guild.CreateEmojiAsync(name: name, image: st, reason: "Added via silverbot by " + ctx.User.Username);

            await ctx.RespondAsync(emote.ToString());
        }

        [Command("addemote")]
        [Description("Wanna add a emote but discord is too complicated to navigate. You need to add attachment here ")]
        [RequireGuild()]
        [RequirePermissions(Permissions.ManageEmojis)]
        public async Task UselessFact(CommandContext ctx, [Description("Name like `Kappa`")] string name)
        {
            var lang = Language.GetLanguageFromCtx(ctx);
            switch (ctx.Message.Attachments.Count)
            {
                case 0:
                    await ctx.RespondAsync(lang.NoImageGeneric);
                    return;
                case > 1:
                    await ctx.RespondAsync(lang.MoreThanOneImageGeneric);
                    return;
            }

            var client = WebClient.Get();
            var rm = await client.GetAsync(ctx.Message.Attachments[0].ProxyUrl);
            var st = await rm.Content.ReadAsStreamAsync();
            if (st.Length > 256 * 1000)
            {
                await ctx.RespondAsync(string.Format(lang.EmoteWasLargerThan256K, FileSizeUtils.FormatSize(st.Length)));
            }
            var emote = await ctx.Guild.CreateEmojiAsync(name: name, image: st, reason: "Added via silverbot by " + ctx.User.Username);
            await ctx.RespondAsync(emote.ToString());
        }

        [Command("allemotes")]
        [Description("Get all the emotes from the SilverSocial enabled servers")]
        public async Task Allemotes(CommandContext ctx)
        {
            var builder = new StringBuilder();
            var lang = Language.GetLanguageFromCtx(ctx);
            var serverthatareoptedin = await Database.ServersoptedinAsync();
            var pages = new List<Page>();
            var b = new DiscordEmbedBuilder();
            b.WithTitle(lang.AllAvailibleEmotes);
            b.WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            foreach (var a in ctx.Client.Guilds.Values)
            {
                var thing = serverthatareoptedin.Find(x => x.ServerId == a.Id);
                if (thing == null || thing.Optedin != true) continue;
                if (Range.Contains(builder.Length))
                {
                    b.WithDescription(builder.ToString());
                    pages.Add(new Page(embed: b));
                    builder.Clear();
                }
                builder.AppendLine(string.Format(lang.Server, a.Name));
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
            await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages);
        }

        private static readonly IEnumerable<int> Range = Enumerable.Range(1900, 2000);

        private static bool CheckIfGuildIsIn(List<Serveroptin> serverthatareoptedin, ulong id)
        {
            var thing = serverthatareoptedin.Find(x => x.ServerId == id);
            return thing != null && thing.Optedin;
        }

        [Command("emote")]
        [Description("Get an emote from the SilverSocial enabled servers")]
        public async Task GetEmotes(CommandContext ctx, [Description("Emote name like :pog: or pog")] string emote)
        {
            var lang = Language.GetLanguageFromCtx(ctx);
            var emotes = new List<DiscordEmoji>();
            var serverthatareoptedin = await Database.ServersoptedinAsync();

            foreach (var a in ctx.Client.Guilds.Values.Where(e => CheckIfGuildIsIn(serverthatareoptedin, e.Id)))
            {
                var emojis = a.Emojis.Values.Where(x => (":" + x.Name + ":") == emote || x.Name == emote || (Regex.IsMatch(emote, @"^\d+$") && x.Id == Convert.ToUInt64(emote)));
                emotes.AddRange(emojis);
            }

            switch (emotes.Count)
            {
                case 0:
                {
                    var b = new DiscordEmbedBuilder();
                    b.WithTitle(lang.NoEmotesFound);
                    b.WithDescription(string.Format(lang.SearchedFor, emote));
                    b.WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
                    await ctx.RespondAsync(embed: b.Build());
                    break;
                }
                case 1 when emotes[0].IsAnimated:
                    await ctx.RespondAsync("<a:" + emotes[0].Name + ":" + emotes[0].Id + ">");
                    break;
                case 1:
                    await ctx.RespondAsync("<:" + emotes[0].Name + ":" + emotes[0].Id + ">");
                    break;
                case > 1:
                {
                    var b = new DiscordEmbedBuilder();
                    b.WithTitle(lang.MultipleEmotesFound);
                    var builder = new StringBuilder();
                    foreach (var e in emotes)
                    {
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
                    }
                    b.WithDescription(builder.ToString());
                    b.WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
                    await ctx.RespondAsync(embed: b.Build());
                    break;
                }
            }
        }
        

        [Command("optintoemotes")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [RequireGuild()]
        public async Task Optin(CommandContext ctx)
        {
            var isoptedin = await Database.Isoptedin(ctx.Guild.Id);
            var lang = Language.GetLanguageFromCtx(ctx);

            switch (isoptedin)
            {
                case true:
                {
                    var bob = new DiscordEmbedBuilder();
                    bob.WithTitle(lang.AlreadyOptedIn);
                    bob.WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
                    await ctx.RespondAsync(embed: bob.Build());
                    return;
                }
                case false:
                {
                    var bob = new DiscordEmbedBuilder();
                    bob.WithTitle(lang.UserIsBannedFromSilversocial);
                    bob.WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
                    await ctx.RespondAsync(embed: bob.Build());
                    return;
                }
            }

            var newserverthing = new Serveroptin
            {
                ServerId = ctx.Guild.Id,
                Optedin = true
            };
            await Database.InsertAsync(newserverthing);
            var b = new DiscordEmbedBuilder();
            b.WithTitle(lang.OptedIn);
            b.WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            await ctx.RespondAsync(embed: b.Build());
        }
    }
}