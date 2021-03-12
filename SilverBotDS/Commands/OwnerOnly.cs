using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;
using Xabe.FFmpeg;
using System.Collections;
using System.Linq;
using Xabe.FFmpeg.Downloader;
using DSharpPlus.Interactivity.Extensions;

namespace SilverBotDS.Commands
{
    [RequireOwner]
    [Hidden]
    internal partial class OwnerOnly : BaseCommandModule
    {
#pragma warning disable CA1822 // Mark members as static

        [Command("repeat")]
        [Description("Repeats the message content")]
        public async Task Repeat(CommandContext ctx, [RemainingText()][Description("The thing to repeat")] string e) => await ctx.RespondAsync(e);

        private readonly string[] _urls = { "https://silverdimond.tk", "https://vfl.gg", "https://github.com/silverdimond" };

        [Command("riprandomframes")]
        [Description("less gooo baybae")]
        public async Task RipRandomFrames(CommandContext ctx, int times, string loc)
        {
            var info = await FFmpeg.GetMediaInfo(loc).ConfigureAwait(false);
            await ctx.RespondAsync($"its {info.Duration.Humanize()} ({info.Duration}) long");

            RandomGenerator random = new();

            string name = Path.GetFileName(loc);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.png);
            for (int i = 0; i < times; i++)
            {
                IConversionResult conversionResult = await FFmpeg.Conversions.New()
                .AddStream(videoStream)
                .ExtractNthFrame(random.Next(1, (int)(info.VideoStreams.First().Framerate * info.VideoStreams.First().Duration.TotalSeconds)), (number) => { return "Extracts\\" + name + i + ".png"; })
                .UseHardwareAcceleration(HardwareAccelerator.auto, VideoCodec.hevc, VideoCodec.png)
                .Start();
            }

            await ctx.RespondAsync($"done?");
        }

        [Command("downloadffmpeg")]
        [Description("less gooo baybae")]
        public async Task Downloadffmpeg(CommandContext ctx)
        {
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official);
            await ctx.RespondAsync($"done?");
            //await FFmpeg.GetLatestVersion();
        }

        [Command("reloadcolors")]
        [Description("reloads the colors.json")]
        public async Task ReloadColors(CommandContext ctx)
        {
            if (!Program.GetConfig().ColorConfig)
            {
                await new DiscordMessageBuilder().WithEmbed(new DiscordEmbedBuilder().WithTitle("Reloading does nothing lololol").WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).Build()).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
            }
            await ColorUtils.ReloadConfig();
            await new DiscordMessageBuilder().WithEmbed(new DiscordEmbedBuilder().WithTitle("Reloaded the colors").WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).WithColor(await ColorUtils.GetSingleAsync()).Build()).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
        }

        [Command("stress")]
        [Description("less gooo baybae")]
        public Task Stress(CommandContext ctx)
        {
            foreach (var url in _urls)
            {
                _ = Html(ctx, url);
            }
            return Task.CompletedTask;
        }

        [Command("plot")]
        [Description("plot a thingy")]
        public async Task Plot(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("send x data"));
            var msg = await interactivity.WaitForMessageAsync((a) => { return a.Author.Id == ctx.User.Id; });
            if (msg.TimedOut)
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("nvm your too slow"));
                return;
            }
            double[] xdata = Array.ConvertAll(msg.Result.Content.Split(','), double.Parse);
            await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("send y data"));
            msg = await interactivity.WaitForMessageAsync((a) => { return a.Author.Id == ctx.User.Id; });
            if (msg.TimedOut)
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("nvm your too slow"));
                return;
            }
            double[] ydata = Array.ConvertAll(msg.Result.Content.Split(','), double.Parse);
            var plt = new ScottPlot.Plot(1920, 1080);
            plt.AddScatter(xdata, ydata);
            var bitmap = plt.Render();
            await using var outStream = new MemoryStream();
            bitmap.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
            outStream.Position = 0;
            await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("plotted that").WithFile("silverbotimage.png", outStream));
        }

        [Command("setupcategory")]
        [Description("Set up a category in the silverbot dev server")]
        [RequireBotPermissions(Permissions.ManageChannels | Permissions.ManageRoles)]
        public async Task Category(CommandContext ctx, [Description("The role to set up a category for")] DiscordRole
         role)
        {
            var name = Regex.Replace(role.Name, @"[^\w]", "");
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
            DiscordMessageBuilder discordMessage = new();
            discordMessage.Content = ctx.User.Mention + " there m8 that took some time to do";

            await channel.SendMessageAsync(discordMessage);
        }

        [Command("setupcategory")]
        [Description("Set up a category in the silverbot dev server")]
        [RequireBotPermissions(Permissions.ManageChannels | Permissions.ManageRoles)]
        public async Task Category(CommandContext ctx, [Description("The person to set up a category for")] DiscordMember
             person)
        {
            var name = Regex.Replace(person.Username, @"[^\w]", "");
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

        private readonly string[] references = new string[] { "System", "System.Collections.Generic", "System.Diagnostics", "System.IO", "System.IO.Compression", "System.Text", "System.Text.RegularExpressions", "System.Threading.Tasks", "System.Linq", "Wbubbler", "Humanizer", "MathParser.org-mXparser", "ScottPlot", "SilverBotDS" };

        private readonly string[] imports = new string[] { "System", "System.Collections.Generic", "System.Diagnostics", "System.IO", "System.IO.Compression", "System.Text", "System.Text.RegularExpressions", "System.Threading.Tasks", "System.Linq", "Wbubbler", "Humanizer" };

        public static string RemoveCodeBraces(string str)
        {
            if (str.StartsWith("```csharp"))
            {
                str = str.Remove(0, 9);
            }
            if (str.StartsWith("```cs"))
            {
                str = str.Remove(0, 5);
            }
            if (str.StartsWith("```"))
            {
                str = str.Remove(0, 3);
            }
            if (str.EndsWith("```"))
            {
                str = str.Remove(str.Length - 3, 3);
            }
            return str;
        }

        private static async Task SendStringFileWithContent(CommandContext ctx, string title, string file, string filename = "message.txt")
        {
            await new DiscordMessageBuilder().WithContent(title).WithFile(filename, new MemoryStream(Encoding.UTF8.GetBytes(file))).WithAllowedMentions(Mentions.None).SendAsync(ctx.Channel);
        }

        private static string AddBraces(string a)
        {
            return "```" + a + "```";
        }

        public static async Task SendBestRepresentationAsync(object ob, CommandContext ctx)
        {
            try
            {
                string str = ob.ToString();
                if (ob.GetType() == typeof(TimeSpan))
                {
                    str = ((TimeSpan)ob).Humanize(999);
                }
                else if (ob.GetType() == typeof(DateTime))
                {
                    str = ((DateTime)ob).Humanize();
                }
                else if (ob.GetType() == typeof(string))
                {
                    str = RemoveCodeBraces((string)ob);
                }
                else if (ob.GetType().IsSerializable || ob.GetType().IsArray || ob.GetType().IsEnum || ob.GetType().FullName == ob.ToString())
                {
                    str = JsonSerializer.Serialize(ob, options);
                    if (str.Length >= 2000)
                    {
                        await SendStringFileWithContent(ctx, ob.GetType().FullName, str, "eval.txt");
                        return;
                    }
                    else
                    {
                        str = "```json\n" + str + "```";
                    }
                }
                else
                {
                    str = AddBraces(str);
                }
                if (ob.ToString().Length >= 2000)
                {
                    await SendStringFileWithContent(ctx, ob.GetType().FullName, str, "eval.txt");
                    return;
                }
                await new DiscordMessageBuilder().WithContent(ob.GetType().FullName + ' ' + str).WithAllowedMentions(Mentions.None).SendAsync(ctx.Channel);
            }
            catch (Exception e)
            {
                //abort ship
                Program.SendLog(e);
                await new DiscordMessageBuilder().WithContent($"Failed to parse {ob.GetType().FullName} as a string, using the generic ToString. " + ob.ToString()).WithAllowedMentions(Mentions.None).SendAsync(ctx.Channel);
            }
        }

        public static readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        /// <summary>
        /// Stolen idea from https://github.com/Voxel-Fox-Ltd/VoxelBotUtils/blob/master/voxelbotutils/cogs/owner_only.py#L172-L252
        /// </summary>
        [Command("evaluate")]
        [Description("UHHHHHHHHHHHHH its a secret")]
        [Aliases("eval", "ev")]
        public async Task Eval(CommandContext ctx, [RemainingText] string code)
        {
            TextWriter console = Console.Out;
            try
            {
                Program.SendLog("Evaling a peace of code, wish me luck", true);
                var timer = new Stopwatch();
                timer.Start();
                using StringWriter sw = new StringWriter();
                Console.SetOut(sw);
                var result = await CSharpScript.RunAsync(RemoveCodeBraces(code),
           ScriptOptions.Default.WithReferences(references).WithImports(imports), globals: new CodeEnv(ctx));
                timer.Stop();
                if (result.ReturnValue is not null)
                {
                    await SendBestRepresentationAsync(result.ReturnValue, ctx);
                }
                if (!string.IsNullOrEmpty(sw.ToString()))
                {
                    if (sw.ToString().Length > 1979)
                    {
                        //sending as a file to not get a 400
                        await SendStringFileWithContent(ctx, "Console Output", sw.ToString(), "console.txt");
                    }
                    else
                    {
                        await new DiscordMessageBuilder().WithContent("Console Output" + AddBraces(sw.ToString())).SendAsync(ctx.Channel);
                    }
                }
                sw.Close();
                Console.SetOut(console);
                await new DiscordMessageBuilder().WithContent($"Executed the code in {timer.Elapsed.Humanize(6)}").SendAsync(ctx.Channel);
            }
            catch (CompilationErrorException e)
            {
                Console.SetOut(console);
                Program.SendLog(e);
                if (e.Diagnostics.Humanize().Length > 1958)
                {
                    await SendStringFileWithContent(ctx, "Compilation Error occurred:", e.Diagnostics.Humanize(), "error.txt");
                }
                else
                {
                    await new DiscordMessageBuilder().WithContent($"Compilation Error occurred: ```csharp\n" + RemoveCodeBraces(e.Diagnostics.Humanize()) + "```").SendAsync(ctx.Channel);
                }
                throw;
            }
            catch (Exception e)
            {
                Console.SetOut(console);
                Program.SendLog(e);
                throw;
            }
        }

        [Command("runsql")]
        [Description("UHHHHHHHHHHHHH its a secret")]
        public async Task Runsql(CommandContext ctx, string sql)
        {
            var thing = await Program.GetDatabase().RunSqlAsync(sql);
            if (thing.Item1 != null && thing.Item2 == null)
            {
                await ctx.RespondAsync(thing.Item1);
            }
            if (thing.Item1 == null && thing.Item2 != null)
            {
                var bob = new DiscordEmbedBuilder();
                bob.WithImageUrl("attachment://html.png");
                bob.WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));

                var image = new MemoryStream();
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
            var bob = new DiscordEmbedBuilder().WithImageUrl("attachment://html.png").WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).WithColor(DiscordColor.Green);

            await using var image = new MemoryStream();
            using var e = await Browser.ScreenshotAsync(html);
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
                var lang = Language.GetLanguageFromCtx(ctx);
                if (ctx.Message.Attachments.Count == 0)
                {
                    await ctx.RespondAsync(lang.NoImageGeneric);
                    return;
                }
                if (ctx.Message.Attachments.Count > 1)
                {
                    await ctx.RespondAsync(lang.MoreThanOneImageGeneric);
                    return;
                }
                if (FileUtils.GetFileExtensionFromUrl(ctx.Message.Attachments[0].Url) != ".zip")
                {
                    await ctx.RespondAsync("please use a zip");
                    return;
                }
                var client = NetClient.Get();
                var rm = await client.GetAsync(ctx.Message.Attachments[0].Url);
                await using (var fs = new FileStream(
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
                    await using FileStream fileStream = new(file, FileMode.Open);
                    await using var stream = new MemoryStream();
                    fileStream.Position = 0;
                    await fileStream.CopyToAsync(stream);
                    if (stream.Length > 256 * 1000)
                    {
                        status.Append(Path.GetFileName(file));
                        status.Append("\t " + StringUtils.BoolToEmoteString(false) + " Bigger than 256kb");
                    }
                    else
                    {
                        var emote = await ctx.Guild.CreateEmojiAsync(name: Path.GetFileNameWithoutExtension(file),
                            image: stream, reason: "Added via silverbot by " + ctx.User.Username);
                        status.Append("\t " + emote + ' ' + StringUtils.BoolToEmoteString(true));
                    }
                }
                await ctx.RespondAsync(status.ToString());
                Directory.Delete(Environment.CurrentDirectory + "\\temp", true);
                File.Delete(Environment.CurrentDirectory + "\\temp.zip");
            }
            catch (Exception e)
            {
                Program.SendLog(e);
                throw;
            }
        }

        [Command("guilds")]
        public async Task Guilds(CommandContext ctx)
        {
            StringBuilder bob = new();
            foreach (var guild in ctx.Client.Guilds.Values)
            {
                bob.AppendLine($"{guild.Name} | {guild.Owner.DisplayName} | {guild.MemberCount} | {guild.Id}");
            }
            await ctx.RespondAsync(bob.ToString());
        }

        [Command("splashesreload")]
        [Description("reload the config for splashes")]
        public async Task Reloadsplashes(CommandContext ctx)
        {
            await Splashes.GetAsync(true);
            var bob = new DiscordEmbedBuilder();
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
            var bob = new DiscordEmbedBuilder();
            bob.WithImageUrl("attachment://html.png");
            bob.WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            var e = await Browser.ScreenshotHtmlAsync(html);
            var image = new MemoryStream();
            e.Save(image, System.Drawing.Imaging.ImageFormat.Png);
            image.Position = 0;
            await new DiscordMessageBuilder().WithEmbed(bob.Build()).WithFile("html.png", image).SendAsync(ctx.Channel);
            e.Dispose();
        }
    }
}