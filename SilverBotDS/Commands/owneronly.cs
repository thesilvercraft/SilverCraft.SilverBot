using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SilverBotDS.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    [RequireOwner]
    [Hidden]
    internal class OwnerOnly : BaseCommandModule
    {
#pragma warning disable CA1822 // Mark members as static

        [Command("repeat")]
        [Description("Repeats the message content")]
        public async Task Repeat(CommandContext ctx, [RemainingText()][Description("The thing to repeat")] string e)
        {
            await ctx.RespondAsync(e);
        }

        private readonly string[] urls = { "https://silverdimond.tk", "https://yahoo.com", "https://bing.com", "https://vfl.gg", "https://discord.com", "https://github.com", "https://github.com/silverdimond" };

        [Command("stress")]
        [Description("less gooo baybae")]
        public Task Stress(CommandContext ctx)
        {
            foreach (string url in urls)
            {
                _ = Html(ctx, url);
            }

            return Task.CompletedTask;
        }

        [Command("setupcategory")]
        [Description("Set up a category in the silverbot dev server")]
        [RequireBotPermissions(Permissions.ManageChannels | Permissions.ManageRoles)]
        public async Task Category(CommandContext ctx, [Description("The role to set up a category for")] DiscordRole
         role)
        {
            string name = Regex.Replace(role.Name, @"[^\w]", "");
            List<DiscordOverwriteBuilder> builders = new();
            var builder = new DiscordOverwriteBuilder
            {
                Allowed = Permissions.All
            };
            builder.For(ctx.Guild.CurrentMember);
            builders.Add(builder);
            builder = new DiscordOverwriteBuilder
            {
                Denied = Permissions.All
            };
            builder.For(ctx.Guild.EveryoneRole);
            builders.Add(builder);
            builder = new DiscordOverwriteBuilder
            {
                Allowed = Permissions.AccessChannels | Permissions.AddReactions | Permissions.AttachFiles | Permissions.ChangeNickname | Permissions.DeafenMembers | Permissions.EmbedLinks | Permissions.ManageChannels | Permissions.ManageMessages | Permissions.MoveMembers | Permissions.ReadMessageHistory | Permissions.SendMessages | Permissions.Speak | Permissions.Stream | Permissions.UseVoice | Permissions.UseVoiceDetection | Permissions.ManageRoles
            };
            builder.For(role);
            builders.Add(builder);
            var category = await ctx.Guild.CreateChannelCategoryAsync(name, builders, "Added by SilverBot as requested by" + ctx.User.Username);
            var channel = await ctx.Guild.CreateChannelAsync(name, ChannelType.Text, category, reason: "Added by SilverBot as requested by" + ctx.User.Username);
            _ = await ctx.Guild.CreateChannelAsync(name, ChannelType.Voice, category, reason: "Added by SilverBot as requested by" + ctx.User.Username);
            Discord​Message​Builder discordMessage = new();
            discordMessage.Content = ctx.User.Mention + " there m8 that took some time to do";

            await channel.SendMessageAsync(discordMessage);
        }

        [Command("setupcategory")]
        [Description("Set up a category in the silverbot dev server")]
        [RequireBotPermissions(Permissions.ManageChannels | Permissions.ManageRoles)]
        public async Task Category(CommandContext ctx, [Description("The person to set up a category for")] DiscordMember
             person)
        {
            string name = Regex.Replace(person.Username, @"[^\w]", "");
            List<DiscordOverwriteBuilder> builders = new();
            var builder = new DiscordOverwriteBuilder
            {
                Allowed = Permissions.All
            };
            builder.For(ctx.Guild.CurrentMember);
            builders.Add(builder);
            builder = new DiscordOverwriteBuilder
            {
                Denied = Permissions.All
            };
            builder.For(ctx.Guild.EveryoneRole);
            builders.Add(builder);
            builder = new DiscordOverwriteBuilder
            {
                Allowed = Permissions.AccessChannels | Permissions.AddReactions | Permissions.AttachFiles | Permissions.ChangeNickname | Permissions.DeafenMembers | Permissions.EmbedLinks | Permissions.ManageChannels | Permissions.ManageMessages | Permissions.MoveMembers | Permissions.ReadMessageHistory | Permissions.SendMessages | Permissions.Speak | Permissions.Stream | Permissions.UseVoice | Permissions.UseVoiceDetection | Permissions.ManageRoles
            };
            builder.For(person);
            builders.Add(builder);
            var category = await ctx.Guild.CreateChannelCategoryAsync(name, builders, "Added by SilverBot as requested by" + ctx.User.Username);
            var channel = await ctx.Guild.CreateChannelAsync(name, ChannelType.Text, category, reason: "Added by SilverBot as requested by" + ctx.User.Username);
            _ = await ctx.Guild.CreateChannelAsync(name, ChannelType.Voice, category, reason: "Added by SilverBot as requested by" + ctx.User.Username);
            await channel.SendMessageAsync(person.Mention + " there m8 that took some time to do");
        }

        [Command("runsql")]
        [Description("UHHHHHHHHHHHHH its a secret")]
        public async Task Runsql(CommandContext ctx, string sql)
        {
            var thing = await Database.RunSQLAsync(sql);
            if (thing.Item1 != null && thing.Item2 == null)
            {
                await ctx.RespondAsync(thing.Item1);
            }
            if (thing.Item1 == null && thing.Item2 != null)
            {
                DiscordEmbedBuilder bob = new DiscordEmbedBuilder();
                bob.WithImageUrl("attachment://html.png");
                bob.WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));

                MemoryStream image = new MemoryStream();
                thing.Item2.Save(image, System.Drawing.Imaging.ImageFormat.Png);
                image.Position = 0;
                await new DiscordMessageBuilder().WithEmbed(bob.Build()).WithFile("html.png", image).SendAsync(ctx.Channel);

                thing.Item2.Dispose();
            }
        }

        [Command("html")]
        [Description("UHHHHHHHHHHHHH its a secret")]
        public async Task Html(CommandContext ctx, string html)
        {
            DiscordEmbedBuilder bob = new DiscordEmbedBuilder().WithImageUrl("attachment://html.png").WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).WithColor(DiscordColor.Green);

            using MemoryStream image = new MemoryStream();
            using Image e = await Browser.ScreenshotAsync(html);
            e.Save(image, System.Drawing.Imaging.ImageFormat.Png);
            image.Position = 0;
            await new DiscordMessageBuilder().WithEmbed(bob.Build()).WithFile("html.png", image).SendAsync(ctx.Channel);
        }

        [Command("addemotes")]
        [Description("testing shiz")]
        [RequireGuild()]
        [RequirePermissions(Permissions.ManageEmojis)]
        public async Task Addemotez(CommandContext ctx)
        {
            try
            {
                Language lang = Language.GetLanguageFromCtx(ctx);
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
                if (FileUtils.GetFileExtensionFromUrl(ctx.Message.Attachments[0].Url) != ".zip")
                {
                    await ctx.RespondAsync("please use a zip");
                    return;
                }
                System.Net.Http.HttpClient client = WebClient.Get();
                HttpResponseMessage rm = await client.GetAsync(ctx.Message.Attachments[0].Url);
                using (var fs = new FileStream(
        Environment.CurrentDirectory + "\\temp.zip",
        FileMode.CreateNew))
                {
                    await rm.Content.CopyToAsync(fs);
                }
                if (!Directory.Exists(Environment.CurrentDirectory + "\\temp"))
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\temp");
                }
                else if (Directory.GetFiles(Environment.CurrentDirectory + "\\temp").Length != 0)
                {
                    Directory.Delete(Environment.CurrentDirectory + "\\temp", true);
                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\temp");
                }
                ZipFile.ExtractToDirectory(Environment.CurrentDirectory + "\\temp.zip", Environment.CurrentDirectory + "\\temp");
                StringBuilder status = new();
                foreach (var file in Directory.GetFiles(Environment.CurrentDirectory + "\\temp"))
                {
                    using FileStream fileStream = new(file, FileMode.Open);
                    using MemoryStream stream = new MemoryStream();
                    fileStream.Position = 0;
                    await fileStream.CopyToAsync(stream);
                    if (stream.Length > 256 * 1000)
                    {
                        status.Append(Path.GetFileName(file));
                        status.Append("\t " + StringUtils.BoolToEmoteString(false) + " Bigger than 256kb");
                    }
                    else
                    {
                        Console.WriteLine(Path.GetFileName(file));
                        DiscordGuildEmoji emote = await ctx.Guild.CreateEmojiAsync(name: Path.GetFileNameWithoutExtension(file), image: stream, reason: "Added via silverbot by " + ctx.User.Username);
                        status.Append("\t " + emote.ToString() + ' ' + StringUtils.BoolToEmoteString(true));
                    }
                }
                await ctx.RespondAsync(status.ToString());
                Directory.Delete(Environment.CurrentDirectory + "\\temp", true);
                File.Delete(Environment.CurrentDirectory + "\\temp.zip");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("splashesreload")]
        [Description("reload the config for splashes")]
        public async Task Reloadsplashes(CommandContext ctx)
        {
            await Splashes.GetAsync(true);
            DiscordEmbedBuilder bob = new DiscordEmbedBuilder();
            bob.WithTitle("Reloaded splashes for ya.");

            bob.WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            if (Program.GetConfig().UseSplashConfig == false)
            {
                await ctx.RespondAsync("ps that did nothing as splash config is set false", embed: bob.Build());
            }
            else
            {
                await ctx.RespondAsync(embed: bob.Build());
            }
        }

        [Command("htmle")]
        [Description("UHHHHHHHHHHHHH its a secret")]
        public async Task HtmlE(CommandContext ctx, string html)
        {
            DiscordEmbedBuilder bob = new DiscordEmbedBuilder();
            bob.WithImageUrl("attachment://html.png");
            bob.WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            Image e = await Browser.ScreenshotHtmlAsync(html);
            MemoryStream image = new MemoryStream();
            e.Save(image, System.Drawing.Imaging.ImageFormat.Png);
            image.Position = 0;
            await new DiscordMessageBuilder().WithEmbed(bob.Build()).WithFile("html.png", image).SendAsync(ctx.Channel);
            e.Dispose();
        }
    }
}