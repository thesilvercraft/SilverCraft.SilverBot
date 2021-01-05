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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS
{
    internal class Emotes : BaseCommandModule
    {
        [Command("allemotes")]
        [Description("Get all the emotes from the SilverSocial enabled servers")]
        public async Task allemotes(CommandContext ctx)
        {
            StringBuilder builder = new StringBuilder();

            List<Serveroptin> serverthatareoptedin = await Database.ServersoptedinAsync().ToListAsync();

            foreach (DiscordGuild a in ctx.Client.Guilds.Values)
            {
                var thing = serverthatareoptedin.Find(x => x.ServerId == a.Id);
                if (thing != null && thing.optedin == true)
                {
                    builder.AppendLine($"Server :{a.Name}");
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
            b.WithTitle("All available emotes");
            b.WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
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

        [Command("emote")]
        [Description("Get an emote from the SilverSocial enabled servers")]
        public async Task Midsdsdsadng(CommandContext ctx, [Description("Emote name like :pog:")] string emote)
        {
            List<DiscordEmoji> emotes = new List<DiscordEmoji>();
            List<Serveroptin> serverthatareoptedin = await Database.ServersoptedinAsync().ToListAsync();

            foreach (DiscordGuild a in ctx.Client.Guilds.Values)
            {
                var thing = serverthatareoptedin.Find(x => x.ServerId == a.Id);
                if (thing != null && thing.optedin == true)
                {
                    //EA SPORTS ITS IN THE GAEM
                    var cunt = a.Emojis.Values.FirstOrDefault(x => (":" + x.Name + ":") == emote);
                    if (cunt != null)
                    {
                        emotes.Add(cunt);
                    }
                }
            }

            if (emotes.Count == 0)
            {
                DiscordEmbedBuilder b = new DiscordEmbedBuilder();
                b.WithTitle("**No emote found**");
                b.WithDescription("Searched for:`" + emote + "`");
                b.WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
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
                b.WithTitle("**Multiple emotes found**");
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
                b.WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
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
                optedin = true
            };
            await Database.InsertAsync(newserverthing);
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            b.WithTitle(lang.Opted_in);
            b.WithFooter(lang.Requested_by + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            await ctx.RespondAsync(embed: b.Build());
        }

        [Command("waitforcode"), Description("Waits for a response containing a generated code.")]
        public async Task WaitForCode(CommandContext ctx)
        {
            // first retrieve the interactivity module from the client
            var interactivity = ctx.Client.GetInteractivity();

            // generate a code
            var codebytes = new byte[8];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(codebytes);

            var code = BitConverter.ToString(codebytes).ToLower().Replace("-", "");

            // announce the code
            await ctx.RespondAsync($"The first one to type the following code wins: `{code}`");

            // wait for anyone who types it
            var msg = await interactivity.WaitForMessageAsync(xm => xm.Content.Contains(code), TimeSpan.FromSeconds(60));
            if (msg.Result != null)
            {
                // announce the winner
                await ctx.RespondAsync($"And the winner is: {msg.Result.Author.Mention}");
            }
            else
            {
                await ctx.RespondAsync("Nobody? Really?");
            }
        }
    }
}