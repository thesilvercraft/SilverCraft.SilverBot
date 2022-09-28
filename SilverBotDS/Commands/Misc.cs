using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Humanizer;
using SilverBotDS.Attributes;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands;

[Category("Miscellaneous")]
public class MiscCommands : BaseCommandModule
{
    private readonly Regex _csharpErrorR = new("CS[0-9][0-9][0-9][0-9]",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private readonly Regex _dotNetErrorR = new("CA[0-9][0-9][0-9][0-9]",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private readonly Regex _fsharpErrorR = new("FS[0-9][0-9][0-9][0-9]",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private readonly Regex _ideErrorR = new("IDE[0-9][0-9][0-9][0-9]",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private readonly Regex _nuGetErrorR = new("NU[0-9][0-9][0-9][0-9]",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private readonly Regex _serilogErrorR = new("Serilog[0-9][0-9][0-9]",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private readonly Regex _sonarErrorR = new("S([0-9][0-9][0-9][0-9])",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private readonly Regex _vbErrorR = new("BC[0-9][0-9][0-9][0-9][0-9]",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public DatabaseContext Database { private get; set; }
    public Config Config { private get; set; }
    public HttpClient HttpClient { private get; set; }

    [Command("version")]
    [Description("Get the version info")]
    [Aliases("ver", "verinfo", "versioninfo")]
    public async Task VersionInfoCmd(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        await new DiscordMessageBuilder().WithEmbed(new DiscordEmbedBuilder()
            .WithTitle(lang.VersionInfoTitle)
            .AddField(lang.VersionNumber, Formatter.InlineCode(VersionInfo.VNumber))
            .AddField(lang.GitRepo, ThisAssembly.Git.RepositoryUrl)
            .AddField(lang.GitCommitHash, Formatter.InlineCode(ThisAssembly.Git.Commit))
            .AddField(lang.GitBranch, Formatter.InlineCode(ThisAssembly.Git.Branch))
            .AddField(lang.IsDirty, StringUtils.BoolToEmoteString(ThisAssembly.Git.IsDirty))
            .AddField(lang.CLR, Formatter.InlineCode(RuntimeInformation.FrameworkDescription))
            .AddField(lang.OS, Formatter.InlineCode(Environment.OSVersion.VersionString))
            .AddField(lang.DsharpplusVersion, Formatter.InlineCode(ctx.Client.VersionString))
            .WithAuthor($"{ctx.Client.CurrentUser.Username}#{ctx.Client.CurrentUser.Discriminator}",
                iconUrl: ctx.Client.CurrentUser.GetAvatarUrl(ImageFormat.Auto))
            .WithColor(await ColorUtils.GetSingleAsync())
            .Build()).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
    }

    [Command("setlang")]
    [Description("set your language")]
    [RequireUserPermissions(Permissions.ManageGuild)]
    public async Task SetLanguage(CommandContext ctx, string langName)
    {
        if (Language.LoadedLanguages().AsEnumerable()
            .Any(x => string.Equals(x, langName, StringComparison.InvariantCultureIgnoreCase)))
        {
            if (ctx.Channel.IsPrivate)
            {
                Database.InserOrUpdateLangCodeUser(ctx.User.Id, langName.ToLowerInvariant());
            }
            else
            {
                Database.InserOrUpdateLangCodeGuild(ctx.Guild.Id, langName.ToLowerInvariant());
            }

            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithContent(lang.Success)
                .SendAsync(ctx.Channel);
        }
        else
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithContent($"Unknown language. Valid choices are:```{Language.LoadedLanguages().Humanize()}```")
                .SendAsync(ctx.Channel);
        }
    }

    [Command("togglerepeat")]
    [Description("toggle repeated strings")]
    [RequireUserPermissions(Permissions.ManageGuild)]
    public async Task SetLanguage(CommandContext ctx, bool enable)
    {
        var serversettings = Database.serverSettings.FirstOrDefault(x => x.ServerId == ctx.Guild.Id);
        if (serversettings is not null)
        {
            serversettings.RepeatThings = enable;
            Database.serverSettings.Update(serversettings);
        }
        else
        {
            Database.serverSettings.Add(new ServerSettings
            {
                ServerId = ctx.Guild.Id,
                RepeatThings = enable
            });
        }

        Database.SaveChanges();
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        if (enable)
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithContent(lang.EnableRepeatedPhrases)
                .SendAsync(ctx.Channel);
        }
        else
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithContent(lang.DisabledRepeatedPhrases)
                .SendAsync(ctx.Channel);
        }
    }

    [Command("translateunknown")]
    [Aliases("translate")]
    [Description("translate from an unknown language")]
    public async Task TranlateUnknown(CommandContext ctx, [RemainingText] string text)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        Translator translator = new(HttpClient);
        var tranlsation = await translator.TranslateAsync(text, "auto", lang.LangCodeGoogleTranslate);
        if (tranlsation.Item1.Length > 4095)
        {
            await OwnerOnly.SendStringFileWithContent(ctx, $"TTS URL: <{tranlsation.Item2}>", tranlsation.Item1);
        }
        else
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithEmbed(new DiscordEmbedBuilder().WithDescription(tranlsation.Item1).WithUrl(tranlsation.Item2)
                    .WithTitle("TTS").Build())
                .WithAllowedMentions(Mentions.None)
                .SendAsync(ctx.Channel);
        }
    }

    [Command("translateunknownto")]
    [Aliases("translateto")]
    [Description("translate from an unknown language to a specified one")]
    public async Task TranlateUnknown(CommandContext ctx, string languageTo, [RemainingText] string text)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        if (!Translator.ContainsKeyOrVal(languageTo))
        {
            languageTo = languageTo.Humanize(LetterCasing.Sentence);
        }

        if (!Translator.ContainsKeyOrVal(languageTo))
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithContent(string.Format(lang.NotValidLanguage,
                    StringUtils.ArrayToString(Translator.Languages.ToArray(), ", ")))
                .WithAllowedMentions(Mentions.None)
                .SendAsync(ctx.Channel);
            return;
        }

        Translator translator = new(HttpClient);
        var tranlsation = await translator.TranslateAsync(text, "auto", languageTo);
        if (tranlsation.Item1.Length > 4095)
        {
            await OwnerOnly.SendStringFileWithContent(ctx, $"TTS URL: <{tranlsation.Item2}>", tranlsation.Item1);
        }
        else
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithEmbed(new DiscordEmbedBuilder().WithDescription(tranlsation.Item1).WithUrl(tranlsation.Item2)
                    .WithTitle("TTS").Build())
                .WithAllowedMentions(Mentions.None)
                .SendAsync(ctx.Channel);
        }
    }

    

  

    [Command("cserrcode")]
    public async Task Csharperror(CommandContext ctx, string error)
    {
        var nuGetError = _nuGetErrorR.Match(error);
        var dotNetError = _dotNetErrorR.Match(error);
        var csharpError = _csharpErrorR.Match(error);
        var fsharpError = _fsharpErrorR.Match(error);
        var vbError = _vbErrorR.Match(error);
        var serilogError = _serilogErrorR.Match(error);
        var sonarError = _sonarErrorR.Match(error);
        var ideError = _ideErrorR.Match(error);
        var link = "Not Found";
        if (nuGetError.Success)
        {
            link = $"https://docs.microsoft.com/en-us/nuget/reference/errors-and-warnings/{nuGetError.Groups[0]}";
        }
        else if (dotNetError.Success)
        {
            link =
                $"https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/{dotNetError.Groups[0]}";
        }
        else if (csharpError.Success)
        {
            link =
                $"https://docs.microsoft.com/en-us/dotnet/csharp/misc/{csharpError.Groups[0]} or https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/{csharpError.Groups[0]}";
        }
        else if (fsharpError.Success)
        {
            link =
                $"https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/compiler-messages/{fsharpError.Groups[0]}";
        }
        else if (vbError.Success)
        {
            link = $"https://docs.microsoft.com/en-us/dotnet/visual-basic/misc/{vbError.Groups[0]}";
        }
        else if (sonarError.Success)
        {
            link = $"https://rules.sonarsource.com/csharp/RSPEC-{sonarError.Groups[1]}";
        }
        else if (serilogError.Success)
        {
            link =
                $"https://github.com/Suchiman/SerilogAnalyzer/blob/master/README.md#:~:text={serilogError.Groups[0].Value.ToLower()} note: text fragments may not work for you if so search for `{serilogError.Groups[0].Value.ToLower()}` in the site";
        }
        else if (ideError.Success)
        {
            link =
                $"https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/{ideError.Groups[0]}";
        }

        await new DiscordMessageBuilder()
            .WithReply(ctx.Message.Id)
            .WithContent(link)
            .SendAsync(ctx.Channel);
    }
}