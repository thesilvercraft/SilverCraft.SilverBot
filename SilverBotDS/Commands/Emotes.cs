using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using SilverBotDS.Converters;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    internal class Emotes : BaseCommandModule
    {
#pragma warning disable CA1822 // Mark members as static

        [Command("addemote")]
        [Description("Wanna add a emote but discord is too complicated to navigate")]
        [RequireGuild()]
        [RequirePermissions(Permissions.ManageEmojis)]
        public async Task UselessFact(CommandContext ctx, [Description("Name like `Kappa`")] string name, [Description("url of emote")] string url)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            var client = NetClient.Get();
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
        public async Task AddEmote(CommandContext ctx, [Description("Name like `Kappa`")] string name)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));

            try
            {
                var image = SdImage.FromContext(ctx);
                await AddEmote(ctx, image, name);
            }
            catch (AttachmentCountIncorrectException acie)
            {
                switch (acie.AttachmentCount)
                {
                    case AttachmentCountIncorrect.TooLittleAttachments:
                        await ctx.RespondAsync(lang.NoImageGeneric);
                        return;

                    case AttachmentCountIncorrect.TooMuchAttachments:
                        await ctx.RespondAsync(lang.MoreThanOneImageGeneric);
                        return;
                }
            }
        }

        [Command("addemote")]
        [Description("Wanna add a emote but discord is too complicated to navigate. You need to add attachment here ")]
        [RequireGuild()]
        [RequirePermissions(Permissions.ManageEmojis)]
        public async Task AddEmote(CommandContext ctx, [Description("Url of the thing")] SdImage url, [Description("Name like `Kappa`")] string name)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            await using var st = new MemoryStream(await url.GetBytesAsync());
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
            try
            {
                var builder = new StringBuilder();
                var lang = (await Language.GetLanguageFromCtxAsync(ctx));
                var serverthatareoptedin = await Program.GetDatabase().ServersOptedInEmotesAsync();
                var pages = new List<Page>();
                var b = new DiscordEmbedBuilder();
                b.WithTitle(lang.AllAvailibleEmotes);
                b.WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
                foreach (var a in ctx.Client.Guilds.Values)
                {
                    var thing = serverthatareoptedin.Find(x => x.ServerId == a.Id);
                    if (thing == null || thing.Optedin != true) continue;

                    Console.WriteLine(builder.Length);
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
                await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages);
            }
            catch (Exception e)
            {
                Program.SendLog(e);
                throw;
            }
        }

        private static bool CheckIfGuildIsIn(List<ServerOptin> serverthatareoptedin, ulong id)
        {
            var thing = serverthatareoptedin.Find(x => x.ServerId == id);
            return thing != null && thing.Optedin;
        }

        [Command("emote")]
        [Description("Get an emote from the SilverSocial enabled servers")]
        public async Task GetEmotes(CommandContext ctx, [Description("Emote name like :pog: or pog")] string emote)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            var emotes = new List<DiscordEmoji>();
            var serverthatareoptedin = await Program.GetDatabase().ServersOptedInEmotesAsync();

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
            var isoptedin = await Program.GetDatabase().IsOptedInEmotes(ctx.Guild.Id);
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));

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

            var newserverthing = new ServerOptin
            {
                ServerId = ctx.Guild.Id,
                Optedin = true
            };
            await Program.GetDatabase().InsertEmoteOptinAsync(newserverthing);
            var b = new DiscordEmbedBuilder();
            b.WithTitle(lang.OptedIn);
            b.WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            await ctx.RespondAsync(embed: b.Build());
        }
    }
}