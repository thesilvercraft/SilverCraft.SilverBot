using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SilverBotDS
{
    [RequireOwner()]
    internal class OwnerOnly : BaseCommandModule
    {
#pragma warning disable CA1822 // Mark members as static

        [Command("repeat")]
        [Description("Repeats the message content")]
        public async Task Repeat(CommandContext ctx, [RemainingText()][Description("The thing to repeat")] string e)
        {
            await ctx.RespondAsync(e);
        }

        private readonly string[] urls = { "silverdimond.tk", "yahoo.com", "bing.com", "vfl.gg", "discord.com", "github.com", "github.com/silverdimond" };

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
            await channel.SendMessageAsync(ctx.User.Mention + " there m8 that took some time to do", mentions: Mentions.None);
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
                await ctx.RespondWithFileAsync("html.png", image, embed: bob.Build());
                thing.Item2.Dispose();
            }
        }

        [Command("html")]
        [Description("UHHHHHHHHHHHHH its a secret")]
        public async Task Html(CommandContext ctx, string html)
        {
            DiscordEmbedBuilder bob = new DiscordEmbedBuilder();
            bob.WithImageUrl("attachment://html.png");
            bob.WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            using MemoryStream image = new MemoryStream();
            using Image e = await Browser.ScreenshotAsync(html);
            e.Save(image, System.Drawing.Imaging.ImageFormat.Png);
            image.Position = 0;
            await ctx.RespondWithFileAsync("html.png", image, embed: bob.Build());
        }

        [Command("addemotes")]
        [Description("testing shiz")]
        [RequireGuild()]
        [RequirePermissions(Permissions.ManageEmojis)]
        public async Task Addemotez(CommandContext ctx)
        {
            try
            {
                Language lang = Language.GetLanguageFromId(ctx.Guild?.Id);
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
                HttpClient client = Webclient.Get();
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
                        status.Append("\t " + Stringutils.BoolToEmoteString(false) + " Bigger than 256kb");
                    }
                    else
                    {
                        Console.WriteLine(Path.GetFileName(file));
                        DiscordGuildEmoji emote = await ctx.Guild.CreateEmojiAsync(name: Path.GetFileNameWithoutExtension(file), image: stream, reason: "Added via silverbot by " + ctx.User.Username);
                        status.Append("\t " + emote.ToString() + ' ' + Stringutils.BoolToEmoteString(true));
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
            Splashes.GetAsync(true);
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
            await ctx.RespondWithFileAsync("html.png", image, embed: bob.Build());
            e.Dispose();
        }

        [Command("test")]
        [Description("UHHHHHHHHHHHHH its a secret")]
        public async Task Test(CommandContext ctx, DiscordChannel chn = null, DiscordChannel chnb = null)
        {
            var vnext = ctx.Client.GetVoiceNext();
            if (vnext == null)
            {
                // not enabled
                await ctx.RespondAsync("VNext is not enabled or configured.");
                return;
            }

            // check whether we aren't already connected
            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc != null)
            {
                // already connected
                await ctx.RespondAsync("Already connected in this guild.");
                return;
            }

            // get member's voice state
            var vstat = ctx.Member?.VoiceState;
            if (vstat?.Channel == null && chn == null)
            {
                // they did not specify a channel and are not in one
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            // channel not specified, use user's
            if (chn == null)
                chn = vstat.Channel;
            if (chnb == null)
            {
                chnb = await ctx.Client.GetChannelAsync(699361202312183886);
            }
            var vncb = await vnext.ConnectAsync(chnb);
            // connect
            vnc = await vnext.ConnectAsync(chn);
            // thing = vnc.GetTransmitSink();
            vncb.UserSpeaking += Vncb_UserSpeaking;
            vncb.VoiceReceived += Vncb_VoiceReceived;
            Directory.CreateDirectory("Output");
            vnc.UserJoined += Vnc_UserJoined;
            await ctx.RespondAsync($"Connected to `{chn.Name}`");
        }

        private Task Vnc_UserJoined(VoiceNextConnection sender, DSharpPlus.VoiceNext.EventArgs.VoiceUserJoinEventArgs e)
        {
            return Task.CompletedTask;
        }

        private Task Vncb_UserSpeaking(VoiceNextConnection sender, DSharpPlus.EventArgs.UserSpeakingEventArgs e)
        {
            Console.Write("person is speaking");
            return Task.CompletedTask;
        }

        // private static VoiceTransmitSink thing;
        private int howfull = 0;

        private readonly byte[] pog = new byte[1000000];

        private async Task Vncb_VoiceReceived(VoiceNextConnection sender, DSharpPlus.VoiceNext.EventArgs.VoiceReceiveEventArgs e)
        {
            //Console.Write("pog");

            var array = e.PcmData.ToArray();
            Console.WriteLine(array.Length);
            if (howfull > 99999)
            {
                howfull = 0;
                var fileName = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                var ffmpeg = Process.Start(new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $@"-ac 2 -f s16le -ar 48000 -i pipe:0 -ac 2 -ar 44100 Output/{fileName}.wav",
                    RedirectStandardInput = true
                });

                await ffmpeg.StandardInput.BaseStream.WriteAsync(pog);
                ffmpeg.Dispose();
            }
            array.CopyTo(pog, howfull);
            howfull += array.Length;
        }
    }
}