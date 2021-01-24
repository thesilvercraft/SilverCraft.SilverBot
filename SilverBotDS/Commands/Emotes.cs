using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.Interactivity.Extensions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SilverBotDS
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
            Language lang = Language.GetLanguageFromId(ctx.Guild.Id);
            HttpClient client = Webclient.Get();
            HttpResponseMessage rm = await client.GetAsync(url);
            Stream st = await rm.Content.ReadAsStreamAsync();
            if (st.Length > 256 * 1000)
            {
                await ctx.RespondAsync(string.Format(lang.Emote_Was_Larger_Than_256k, FileSizeUtils.FormatSize(st.Length)));
            }
            DiscordGuildEmoji emote = await ctx.Guild.CreateEmojiAsync(name: name, image: st, reason: "Added via silverbot by " + ctx.User.Username);

            await ctx.RespondAsync(emote.ToString());
        }

        [Command("addemote")]
        [Description("Wanna add a emote but discord is too complicated to navigate. You need to add attachment here ")]
        [RequireGuild()]
        [RequirePermissions(Permissions.ManageEmojis)]
        public async Task UselessFact(CommandContext ctx, [Description("Name like `Kappa`")] string name)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild.Id);
            if (ctx.Message.Attachments.Count == 0)
            {
                await ctx.RespondAsync(lang.No_Image_Generic);
                return;
            }
            if (ctx.Message.Attachments.Count > 1)
            {
                await ctx.RespondAsync(lang.More_Than_One_Image_Generic);
                return;
            }
            HttpClient client = Webclient.Get();
            HttpResponseMessage rm = await client.GetAsync(ctx.Message.Attachments[0].ProxyUrl);
            Stream st = await rm.Content.ReadAsStreamAsync();
            if (st.Length > 256 * 1000)
            {
                await ctx.RespondAsync(string.Format(lang.Emote_Was_Larger_Than_256k, FileSizeUtils.FormatSize(st.Length)));
            }
            DiscordGuildEmoji emote = await ctx.Guild.CreateEmojiAsync(name: name, image: st, reason: "Added via silverbot by " + ctx.User.Username); ;
            await ctx.RespondAsync(emote.ToString());
        }

        [Command("allemotes")]
        [Description("Get all the emotes from the SilverSocial enabled servers")]
        public async Task Allemotes(CommandContext ctx)
        {
            StringBuilder builder = new StringBuilder();
            Language lang = Language.GetLanguageFromId(ctx.Guild.Id);
            List<Serveroptin> serverthatareoptedin = await Database.ServersoptedinAsync().ToListAsync();

            foreach (DiscordGuild a in ctx.Client.Guilds.Values)
            {
                var thing = serverthatareoptedin.Find(x => x.ServerId == a.Id);
                if (thing != null && thing.Optedin == true)
                {
                    builder.AppendLine(string.Format(lang.Server, a.Name));
                    foreach (DiscordEmoji emote in a.Emojis.Values.ToList())
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
                        builder.AppendLine();
                    }
                }
            }
            var interactivity = ctx.Client.GetInteractivity();
            List<Page> pages = new List<Page>();
            var splitemojis = builder.ToString().Split('\n');
            var stringBuilder = new StringBuilder();
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            b.WithTitle(lang.All_availible_emotes);
            b.WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            foreach (var line in splitemojis)
            {
                if (range.Contains(stringBuilder.Length))
                {
                    b.WithDescription(stringBuilder.ToString());
                    pages.Add(new Page(embed: b));
                    stringBuilder.Clear();
                }
                else
                {
                    stringBuilder.Append(line);
                }
            }
            b.WithDescription(stringBuilder.ToString());
            pages.Add(new Page(embed: b));
            await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages);
        }

        private static readonly IEnumerable<int> range = Enumerable.Range(1900, 2000);

        private bool CheckIfGuildIsIn(List<Serveroptin> serverthatareoptedin, ulong id)
        {
            var thing = serverthatareoptedin.Find(x => x.ServerId == id);
            if (thing == null)
            {
                return false;
            }
            return thing.Optedin;
        }

        [Command("emoteinfo")]
        [Description("get info about an emote")]
        public async Task EmoteInfo(CommandContext ctx, [Description("Emote")] DiscordGuildEmoji emote)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild.Id);
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            b.WithTitle(emote.Name);
            b.WithImageUrl(emote.Url);
            b.AddField("date", emote.CreationTimestamp.ToUniversalTime().ToString(), true);
            b.AddField("guild", emote.Guild.Name, true);

            b.WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            await ctx.RespondAsync(embed: b.Build());
        }

        [Command("emote")]
        [Description("Get an emote from the SilverSocial enabled servers")]
        public async Task Midsdsdsadng(CommandContext ctx, [Description("Emote name like :pog: or pog")] string emote)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild.Id);
            List<DiscordEmoji> emotes = new List<DiscordEmoji>();
            List<Serveroptin> serverthatareoptedin = await Database.ServersoptedinAsync().ToListAsync();

            foreach (DiscordGuild a in ctx.Client.Guilds.Values.Where(e => CheckIfGuildIsIn(serverthatareoptedin, e.Id)))
            {
                var cunt = a.Emojis.Values.Where(x => (":" + x.Name + ":") == emote || x.Name == emote || (Regex.IsMatch(emote, @"^\d+$") && x.Id == Convert.ToUInt64(emote)));
                if (cunt != null)
                {
                    foreach (var discordemote in cunt)
                    {
                        emotes.Add(discordemote);
                    }
                }
            }

            if (emotes.Count == 0)
            {
                DiscordEmbedBuilder b = new DiscordEmbedBuilder();
                b.WithTitle(lang.No_emotes_found);
                b.WithDescription(string.Format(lang.Searched_for, emote));
                b.WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
                await ctx.RespondAsync(embed: b.Build());
            }
            else if (emotes.Count == 1)
            {
                if (emotes[0].IsAnimated)
                {
                    await ctx.RespondAsync("<a:" + emotes[0].Name + ":" + emotes[0].Id + ">");
                }
                else
                {
                    await ctx.RespondAsync("<:" + emotes[0].Name + ":" + emotes[0].Id + ">");
                }
            }
            else if (emotes.Count > 1)
            {
                DiscordEmbedBuilder b = new DiscordEmbedBuilder();
                b.WithTitle(lang.Multiple_emotes_found);
                StringBuilder builder = new StringBuilder();
                foreach (DiscordEmoji e in emotes)
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
                b.WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
                await ctx.RespondAsync(embed: b.Build());
            }
        }

        [Command("reactwithemote")]
        [Description("Get an emote from the SilverSocial enabled servers and react with it")]
        [RequireBotPermissions(Permissions.AddReactions | Permissions.UseExternalEmojis)]
        [RequireUserPermissions(Permissions.AddReactions | Permissions.UseExternalEmojis)]
        public async Task ReactWithEmote(CommandContext ctx, [Description("Emote name like :pog: or pog")] string emote, ulong? idofmessage = null)
        {
            Language lang = Language.GetLanguageFromId(ctx.Guild.Id);
            List<DiscordEmoji> emotes = new List<DiscordEmoji>();
            List<Serveroptin> serverthatareoptedin = await Database.ServersoptedinAsync().ToListAsync();

            foreach (DiscordGuild a in ctx.Client.Guilds.Values.Where(e => CheckIfGuildIsIn(serverthatareoptedin, e.Id)))
            {
                var cunt = a.Emojis.Values.Where(x => (":" + x.Name + ":") == emote || x.Name == emote || (Regex.IsMatch(emote, @"^\d+$") && x.Id == Convert.ToUInt64(emote)));
                if (cunt != null)
                {
                    foreach (var discordemote in cunt)
                    {
                        emotes.Add(discordemote);
                    }
                }
            }

            if (emotes.Count == 0)
            {
                DiscordEmbedBuilder b = new DiscordEmbedBuilder();
                b.WithTitle(lang.No_emotes_found);
                b.WithDescription(string.Format(lang.Searched_for, emote));
                b.WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
                await ctx.RespondAsync(embed: b.Build());
            }
            else if (emotes.Count == 1)
            {
                await (await ctx.Channel.GetMessageAsync(idofmessage ?? ctx.Message.Id)).CreateReactionAsync(emotes[0]);
            }
            else if (emotes.Count > 1)
            {
                DiscordEmbedBuilder b = new DiscordEmbedBuilder();
                b.WithTitle(lang.Multiple_emotes_found);
                StringBuilder builder = new StringBuilder();
                foreach (DiscordEmoji e in emotes)
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
                b.WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
                await ctx.RespondAsync(embed: b.Build());
            }
        }

        [Command("optintoemotes")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [RequireGuild()]
        public async Task Optin(CommandContext ctx)
        {
            bool? isoptedin = await Database.Isoptedin(ctx.Guild.Id);
            Language lang = Language.GetLanguageFromId(ctx.Guild.Id);

            if (isoptedin == true)
            {
                DiscordEmbedBuilder bob = new DiscordEmbedBuilder();
                bob.WithTitle(lang.Already_opted_in);
                bob.WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
                await ctx.RespondAsync(embed: bob.Build());
                return;
            }
            var newserverthing = new Serveroptin
            {
                ServerId = ctx.Guild.Id,
                Optedin = true
            };
            await Database.InsertAsync(newserverthing);
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            b.WithTitle(lang.Opted_in);
            b.WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            await ctx.RespondAsync(embed: b.Build());
        }
    }
}