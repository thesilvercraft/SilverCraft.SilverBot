﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using SilverBotDS.Attributes;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SilverBotDS.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands;

[RequireOwner]
[Category("Owner")]
public class OwnerOnly : SilverBotCommandModule
{
    public DatabaseContext Database { private get; set; }
    public Config Config { private get; set; }
    public HttpClient HttpClient { private get; set; }

    private readonly string[] _urls =
    {
        "https://silverdiamond.cf", "https://vfl.gg", "https://github.com/silverdimond", "https://cmpc.live",
        "https://silverbot.cf", "https://oscie.net"
    };

    [Command("reloadcolors")]
    [Description("reloads the colors.json")]
    public async Task ReloadColors(CommandContext ctx)
    {
        if (!Config.ColorConfig)
        {
            await new DiscordMessageBuilder()
                .WithEmbed(new DiscordEmbedBuilder().WithTitle("Reloading does nothing lololol")
                    .WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)).Build())
                .WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
        }

        await ColorUtils.ReloadConfig();
        await new DiscordMessageBuilder()
            .WithEmbed(new DiscordEmbedBuilder().WithTitle("Reloaded the colors")
                .WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
                .WithColor(await ColorUtils.GetSingleAsync()).Build()).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
    }

  
    [Command("UnRegisterCommand")]
    public Task UnRegCmd(CommandContext ctx, [RemainingText] string cmdwithparm)
    {
        ctx.CommandsNext.UnregisterCommands(ctx.CommandsNext.FindCommand(cmdwithparm, out _));
        return Task.CompletedTask;
    }

    [Command("RegisterModule")]
    public async Task RegMod(CommandContext ctx, string mod, bool skipcheck = false)
    {
        var type = Type.GetType(mod);
        if (!skipcheck)
        {
            if (type.GetInterfaces().Contains(typeof(IRequireFonts)))
            {
                var fonts = (string[])type.GetProperty("RequiredFontFamilies").GetValue(null);
                if (!Program.CheckIfAllFontsAreHere(fonts))
                {
                    await ctx.RespondAsync($"Module {mod} won't be loaded as its requirements weren't met: the font/fonts {string.Join(',', fonts)} is/are missing");
                }
            }
            if (type.IsSubclassOf(typeof(SilverBotCommandModule)))
            {
                var n = (SilverBotCommandModule)Activator.CreateInstance(type);
                if (await n.ExecuteRequirements(Config))
                {
                    ctx.CommandsNext.RegisterCommands(type);
                }
                else
                {
                    await ctx.RespondAsync($"Module {mod} won't be loaded as its requirements weren't met");
                }
            }
            else
            {
                ctx.CommandsNext.RegisterCommands(type);
            }
        }
        else
        {
            ctx.CommandsNext.RegisterCommands(type);
        }
    }

    [Command("sudo")]
    [Description("Executes a command as another user.")]
    [Hidden]
    [RequireOwner]
    public async Task Sudo(CommandContext ctx, [Description("Member to execute as.")] DiscordMember member,
        [RemainingText][Description("Command text to execute.")] string command)
    {
        await ctx.TriggerTypingAsync();
        var cmd = ctx.CommandsNext.FindCommand(command, out var customArgs);
        var fakeContext = ctx.CommandsNext.CreateFakeContext(member, ctx.Channel, command, ctx.Prefix, cmd, customArgs);
        await ctx.CommandsNext.ExecuteCommandAsync(fakeContext);
    }

    [Command("setupcategory")]
    [Description("Set up a category in the silverbot dev server")]
    [RequireBotPermissions(Permissions.ManageChannels | Permissions.ManageRoles)]
    public async Task Category(CommandContext ctx, [Description("The role to set up a category for")] DiscordRole
        role)
    {
        var name = Regex.Replace(role.Name, @"[^\w]", "");
        DiscordOverwriteBuilder[] builders =
        {
            new(ctx.Guild.CurrentMember)
            {
                Allowed = Permissions.All
            },
            new(ctx.Guild.EveryoneRole)
            {
                Denied = Permissions.All
            },
            new(role)
            {
                Allowed = Permissions.AccessChannels | Permissions.AddReactions | Permissions.AttachFiles |
                          Permissions.ChangeNickname | Permissions.DeafenMembers | Permissions.EmbedLinks |
                          Permissions.ManageChannels | Permissions.ManageMessages | Permissions.MoveMembers |
                          Permissions.ReadMessageHistory | Permissions.SendMessages | Permissions.Speak |
                          Permissions.Stream | Permissions.UseVoice | Permissions.UseVoiceDetection |
                          Permissions.ManageRoles
            }
        };
        var category = await ctx.Guild.CreateChannelCategoryAsync(name, builders,
            reason: $"Added by SilverBot as requested by {ctx.User.Username}");
        var channel = await ctx.Guild.CreateChannelAsync(name, ChannelType.Text, category,
            reason: $"Added by SilverBot as requested by {ctx.User.Username}");
        _ = await ctx.Guild.CreateChannelAsync(name, ChannelType.Voice, category,
            reason: $"Added by SilverBot as requested by {ctx.User.Username}");
        DiscordMessageBuilder discordMessage = new();
        discordMessage.Content = $"{ctx.User.Mention} there m8 that took some time to do";
        await channel.SendMessageAsync(discordMessage);
    }

    [Command("setupcategory")]
    [Description("Set up a category in the silverbot dev server")]
    [RequireBotPermissions(Permissions.ManageChannels | Permissions.ManageRoles)]
    public async Task Category(CommandContext ctx, [Description("The person to set up a category for")] DiscordMember
        person)
    {
        var name = Regex.Replace(person.Username, @"[^\w]", "");
        DiscordOverwriteBuilder[] builders =
        {
            new(ctx.Guild.CurrentMember)
            {
                Allowed = Permissions.All
            },
            new(ctx.Guild.EveryoneRole)
            {
                Denied = Permissions.All
            },
            new(person)
            {
                Allowed = Permissions.AccessChannels | Permissions.AddReactions | Permissions.AttachFiles |
                          Permissions.ChangeNickname | Permissions.DeafenMembers | Permissions.EmbedLinks |
                          Permissions.ManageChannels | Permissions.ManageMessages | Permissions.MoveMembers |
                          Permissions.ReadMessageHistory | Permissions.SendMessages | Permissions.Speak |
                          Permissions.Stream | Permissions.UseVoice | Permissions.UseVoiceDetection |
                          Permissions.ManageRoles
            }
        };
        var category = await ctx.Guild.CreateChannelCategoryAsync(name, builders,
            reason: $"Added by SilverBot as requested by {ctx.User.Username}");
        var channel = await ctx.Guild.CreateChannelAsync(name, ChannelType.Text, category,
            reason: $"Added by SilverBot as requested by {ctx.User.Username}");
        _ = await ctx.Guild.CreateChannelAsync(name, ChannelType.Voice, category,
            reason: $"Added by SilverBot as requested by {ctx.User.Username}");
        await channel.SendMessageAsync($"{person.Mention} there m8 that took some time to do");
    }

    private readonly string[] _imports =
    {
        "System", "System.Collections.Generic", "System.Diagnostics", "System.IO", "System.IO.Compression",
        "System.Text", "System.Text.RegularExpressions", "System.Threading.Tasks", "System.Linq", "Humanizer",
        "System.Globalization", "System.Text.Json","Serilog",
    };

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

        if (str.StartsWith("```js"))
        {
            str = str.Remove(0, 5);
        }

        if (str.StartsWith("```javascript"))
        {
            str = str.Remove(0, 14);
        }

        if (str.StartsWith("```"))
        {
            str = str.Remove(0, 3);
        }

        if (str.StartsWith("``"))
        {
            str = str.Remove(0, 2);
        }

        if (str.StartsWith("`"))
        {
            str = str.Remove(0, 1);
        }

        if (str.EndsWith("```"))
        {
            str = str.Remove(str.Length - 3, 3);
        }

        if (str.EndsWith("``"))
        {
            str = str.Remove(str.Length - 2, 2);
        }

        if (str.EndsWith("`"))
        {
            str = str.Remove(str.Length - 1, 1);
        }

        return str;
    }

    public static async Task SendStringFileWithContent(CommandContext ctx, string title, string file,
        string filename = "message.txt")
    {
        await new DiscordMessageBuilder().WithContent(title)
            .WithFile(filename, new MemoryStream(Encoding.UTF8.GetBytes(file))).WithAllowedMentions(Mentions.None)
            .SendAsync(ctx.Channel);
    }

    public static async Task SendBestRepresentationAsync(object ob, CommandContext ctx)
    {
        try
        {
            var str = ob.ToString();
            if (ob is TimeSpan span)
            {
                str = span.Humanize(20);
            }
            else if (ob is DateTime time)
            {
                str = Formatter.Timestamp(time);
            }
            else if (ob is string @string)
            {
                str = Formatter.BlockCode(@string);
            }
            else if (ob.GetType().IsSerializable || ob.GetType().IsArray || ob.GetType().IsEnum ||
                     ob.GetType().FullName == ob.ToString())
            {
                str = JsonSerializer.Serialize(ob, Options);
                if (str.Length >= 2000)
                {
                    await SendStringFileWithContent(ctx, ob.GetType().FullName, str, "eval.txt");
                    return;
                }

                str = Formatter.BlockCode(str, "json");
            }
            else
            {
                str = Formatter.BlockCode(str);
            }

            if (ob.ToString().Length >= 2000)
            {
                await SendStringFileWithContent(ctx, ob.GetType().FullName, str, "eval.txt");
                return;
            }

            await new DiscordMessageBuilder().WithContent($"{ob.GetType().FullName} {str}")
                .WithAllowedMentions(Mentions.None).SendAsync(ctx.Channel);
        }
        catch (Exception e)
        {
            Program.SendLog(e);
            await new DiscordMessageBuilder()
                .WithContent($"Failed to parse `{ob.GetType().FullName}` as a string, using the generic ToString. {ob}")
                .WithAllowedMentions(Mentions.None).SendAsync(ctx.Channel);
        }
    }

    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    [Command("dependencies")]
    [Description("get the dependencies used")]
    public async Task Dependencies(CommandContext ctx)
    {
        StringBuilder sb = new();
        foreach (var dependancy in AppDomain.CurrentDomain.GetAssemblies())
        {
            sb.AppendLine(dependancy.FullName);
        }

        if (sb.Length > 1958)
        {
            await SendStringFileWithContent(ctx, "stuff", sb.ToString(), "stuff.txt");
        }
        else
        {
            await new DiscordMessageBuilder().WithContent(Formatter.BlockCode(RemoveCodeBraces(sb.ToString())))
                .SendAsync(ctx.Channel);
        }
    }

    /// <summary>
    ///     Stolen idea from
    ///     https://github.com/Voxel-Fox-Ltd/VoxelBotUtils/blob/master/voxelbotutils/cogs/owner_only.py#L172-L252
    /// </summary>
    [Command("evaluate")]
    [Description("EVALUATE SOME C# CODE")]
    [Aliases("eval", "ev")]
    public async Task Eval(CommandContext ctx, [RemainingText] string code)
    {
        var console = Console.Out;
        try
        {
            using var sw = new StringWriter();
            Console.SetOut(sw);
            var sw1 = Stopwatch.StartNew();
            var script = CSharpScript.Create(RemoveCodeBraces(code),
                ScriptOptions.Default
                    .WithReferences(AppDomain.CurrentDomain.GetAssemblies()
                        .Where(xa => !xa.IsDynamic && !string.IsNullOrWhiteSpace(xa.Location))).WithImports(_imports),
                typeof(CodeEnv));
            var diag = script.Compile();
            sw1.Stop();
            if (diag.Length != 0)
            {
                if (diag.Humanize().Length > 1958)
                {
                    await SendStringFileWithContent(ctx, "Compilation Diagnostics showed up:", diag.Humanize(),
                        "diag.txt");
                }
                else
                {
                    await new DiscordMessageBuilder()
                        .WithContent(
                            $"Compilation Diagnostics showed up: {Formatter.BlockCode(RemoveCodeBraces(diag.Humanize()), "cs")}")
                        .SendAsync(ctx.Channel);
                }

                var errcount = diag.LongCount(x => x.Severity == DiagnosticSeverity.Error);
                if (errcount != 0)
                {
                    await new DiscordMessageBuilder()
                        .WithContent(
                            $"I found {errcount} {(errcount == 1 ? "error" : "errors")}. I will **NOT** attempt to run this code.")
                        .SendAsync(ctx.Channel);
                    Console.SetOut(console);
                    return;
                }
            }

            await new DiscordMessageBuilder().WithContent($"Compiled the code in {sw1.Elapsed.Humanize(6)}")
                .SendAsync(ctx.Channel);
            sw1.Start();
            var sw2 = Stopwatch.StartNew();
            var result = await script.RunAsync(new CodeEnv(ctx, Config, Database));
            if (result.ReturnValue is not null)
            {
                await SendBestRepresentationAsync(result.ReturnValue, ctx);
            }
            else
            {
                await new DiscordMessageBuilder().WithContent("The evaluated code returned a `null`.")
                    .SendAsync(ctx.Channel);
            }

            if (!string.IsNullOrEmpty(sw.ToString()))
            {
                if (sw.ToString().Length > 1978)
                {
                    await SendStringFileWithContent(ctx, "Console Output:", sw.ToString(), "console.txt");
                }
                else
                {
                    await new DiscordMessageBuilder()
                        .WithContent($"Console Output: {Formatter.BlockCode(sw.ToString())}").SendAsync(ctx.Channel);
                }
            }

            sw.Close();
            Console.SetOut(console);
            await new DiscordMessageBuilder()
                .WithContent(
                    $"Executed the code in {Formatter.Bold(sw2.Elapsed.Humanize(6))} excluding compile time, or {Formatter.Bold(sw1.Elapsed.Humanize(6))} including it.")
                .SendAsync(ctx.Channel);
            sw1.Stop();
            sw2.Stop();
            result = null;
            script = null;
        }
        catch (CompilationErrorException e)
        {
            Console.SetOut(console);
            if (e.Diagnostics.Humanize().Length > 1958)
            {
                await SendStringFileWithContent(ctx, "Compilation Error occurred:", e.Diagnostics.Humanize(),
                    "error.txt");
            }
            else
            {
                await new DiscordMessageBuilder()
                    .WithContent(
                        $"Compilation Error occurred: {Formatter.BlockCode(RemoveCodeBraces(e.Diagnostics.Humanize()), "cs")}")
                    .SendAsync(ctx.Channel);
            }

            throw;
        }
        catch (Exception)
        {
            Console.SetOut(console);
            throw;
        }
    }


    [Command("sh")]
    [Description("runs some commands")]
    public async Task RunConsole(CommandContext ctx, [RemainingText] string command)
    {
        Process main = new();
        main.StartInfo.FileName = Environment.OSVersion.Platform == PlatformID.Win32NT ? "CMD.exe" : "/bin/bash";
        main.StartInfo.Arguments = $"{(Environment.OSVersion.Platform == PlatformID.Win32NT ? "/c" : "-c")} {command}";
        main.StartInfo.RedirectStandardOutput = true;
        main.StartInfo.RedirectStandardError = true;
        main.StartInfo.RedirectStandardInput = true;
        DiscordMessage msg = null;
        main.Start();

        StringBuilder content = new();

        while (main.StandardOutput.Peek() != -1 || main.StandardError.Peek() != -1)
        {
            var readline = await main.StandardOutput.ReadToEndAsync() + await main.StandardError.ReadToEndAsync();
            if (msg is null || msg.Content.Length + readline.Length + 7 >= 2000)
            {
                foreach (var part in readline.SplitInParts(1991))
                {
                    msg = await ctx.Channel.SendMessageAsync(Formatter.BlockCode(part));
                    content.Clear();
                    content.AppendLine(part);
                }
            }
            else
            {
                await msg.ModifyAsync(Formatter.BlockCode($"{content}{readline}"));
                content.AppendLine(readline);
            }

            await Task.Delay(2000);
        }
    }

    [Command("runsql")]
    [Description("runs some sql")]
    public async Task Runsql(CommandContext ctx, [RemainingText] string sql)
    {
        var thing = await Database.RunSqlAsync(sql);
        
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithContent(thing).SendAsync(ctx.Channel);
       
    }

   

   

    public class Rootobject
    {
        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("author")] public string Author { get; set; }

        [JsonPropertyName("emotes")] public Emote[] Emotes { get; set; }
    }

    public class Emote
    {
        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("url")] public string Url { get; set; }
    }

    public class SourceFile
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public byte[] FileBytes { get; set; }
    }

    [Command("addemotes")]
    [Description("testing shiz")]
    [RequireGuild]
    [RequirePermissions(Permissions.ManageEmojis)]
    public async Task Addemotez(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
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

        if (ctx.Message.Attachments[0].Url.GetFileExtensionFromUrl() != ".zip")
        {
            await ctx.RespondAsync("please use a zip");
            return;
        }

        var client = HttpClient;
        var ziploc = Path.Combine(Environment.CurrentDirectory,"temp.zip");
        var rm = await client.GetAsync(ctx.Message.Attachments[0].Url);
        await using (var fs = new FileStream(
                         ziploc,
                         FileMode.CreateNew))
        {
            await rm.Content.CopyToAsync(fs);
        }

        var foldername = Path.Combine(Environment.CurrentDirectory,"temp");
        if (!Directory.Exists(foldername))
        {
            Directory.CreateDirectory(foldername);
        }
        else if (Directory.GetFiles(foldername).Length != 0)
        {
            Directory.Delete(foldername, true);
            Directory.CreateDirectory(foldername);
        }

        ZipFile.ExtractToDirectory(ziploc, foldername);
        StringBuilder status = new();
        foreach (var file in Directory.GetFiles(foldername))
        {
            await using FileStream fileStream = new(file, FileMode.Open);
            await using var stream = new MemoryStream();
            fileStream.Position = 0;
            await fileStream.CopyToAsync(stream);
            if (stream.Length > 256000)
            {
                status.Append(Path.GetFileName(file));
                status.Append("\t ").Append(StringUtils.BoolToEmoteString(false)).Append(" Bigger than 256kb");
            }
            else
            {
                var emote = await ctx.Guild.CreateEmojiAsync(Path.GetFileNameWithoutExtension(file),
                    stream, reason: $"Added by SilverBot as requested by {ctx.User.Username}");
                status.Append("\t ").Append(emote).Append(' ').Append(StringUtils.BoolToEmoteString(true));
            }
        }

        await ctx.RespondAsync(status.ToString());
        Directory.Delete(foldername, true);
        File.Delete(ziploc);
    }

    [Command("guilds")]
    public async Task Guilds(CommandContext ctx)
    {
        StringBuilder bob = new();
        foreach (var guild in ctx.Client.Guilds.Values)
        {
            bob.Append(guild.Name).Append(" | ").Append(guild.Owner.DisplayName).Append(" | ").Append(guild.MemberCount)
                .Append(" | ").Append(guild.Id).AppendLine();
        }

        await ctx.RespondAsync(bob.ToString());
    }

    [Command("togglebanuser")]
    public async Task ToggleBanUser(CommandContext ctx, DiscordUser userid, bool ban = true)
    {
        Database.ToggleBanUser(userid.Id, ban);
        await Database.SaveChangesAsync();
        await ctx.RespondAsync($"{userid.Id} has been {(ban ? "banned" : "unbanned")}");
    }

    [Command("shutdown")]
    [Description("kill the bot")]
    public async Task Reloadsplashes(CommandContext ctx)
    {
        await ctx.RespondAsync("bye and try using shutdowngrace next time");
        Environment.Exit(469);
    }

    [Command("shutdowngrace")]
    [Description("kill the bot but shutdown the tasks first")]
    public async Task Reloadsplashesas(CommandContext ctx)
    {
        Program.CancelTasks();
        await ctx.RespondAsync("bye");
        Environment.Exit(469);
    }

    [Command("removeuser")]
    public async Task RemoveUser(CommandContext ctx, DiscordUser userid)
    {
        await Database.RemoveUser(userid.Id);
        await Database.SaveChangesAsync();
        await ctx.RespondAsync($"All traces of {userid.Id} have been removed from the database.");
    }

   
}