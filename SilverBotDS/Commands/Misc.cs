using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Humanizer;
using SilverBotDS.Attributes;
using SilverBotDS.Exceptions;
using SilverBotDS.Objects;
using SilverBotDS.Utils;

namespace SilverBotDS.Commands;

[Category("Miscellaneous")]
public class MiscCommands : BaseCommandModule
{
    private readonly Regex CsharpErrorR = new("CS[0-9][0-9][0-9][0-9]",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private readonly Regex DotNetErrorR = new("CA[0-9][0-9][0-9][0-9]",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private readonly Regex FsharpErrorR = new("FS[0-9][0-9][0-9][0-9]",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private readonly Regex IdeErrorR = new("IDE[0-9][0-9][0-9][0-9]",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private readonly Regex NuGetErrorR = new("NU[0-9][0-9][0-9][0-9]",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private readonly Regex SerilogErrorR = new("Serilog[0-9][0-9][0-9]",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private readonly Regex SonarErrorR = new("S([0-9][0-9][0-9][0-9])",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private readonly Regex VbErrorR = new("BC[0-9][0-9][0-9][0-9][0-9]",
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

    [Command("piss")]
    [Description("piss :)")]
    public Task Piss(CommandContext ctx)
    {
        throw new PissException(69); // line 69 :)
        return Task.CompletedTask;
    }

    [Command("setlang")]
    [Description("set your language")]
    [RequireUserPermissions(Permissions.ManageGuild)]
    public async Task SetLanguage(CommandContext ctx, string LangName)
    {
        if (Language.LoadedLanguages().AsEnumerable()
            .Any(x => string.Equals(x, LangName, StringComparison.InvariantCultureIgnoreCase)))
        {
            if (ctx.Channel.IsPrivate)
                Database.InserOrUpdateLangCodeUser(ctx.User.Id, LangName.ToLowerInvariant());
            else
                Database.InserOrUpdateLangCodeGuild(ctx.Guild.Id, LangName.ToLowerInvariant());
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
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithContent(lang.EnableRepeatedPhrases)
                .SendAsync(ctx.Channel);
        else
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithContent(lang.DisabledRepeatedPhrases)
                .SendAsync(ctx.Channel);
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
            await OwnerOnly.SendStringFileWithContent(ctx, $"TTS URL: <{tranlsation.Item2}>", tranlsation.Item1);
        else
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithEmbed(new DiscordEmbedBuilder().WithDescription(tranlsation.Item1).WithUrl(tranlsation.Item2)
                    .WithTitle("TTS").Build())
                .WithAllowedMentions(Mentions.None)
                .SendAsync(ctx.Channel);
    }

    [Command("translateunknownto")]
    [Aliases("translateto")]
    [Description("translate from an unknown language to a specified one")]
    public async Task TranlateUnknown(CommandContext ctx, string LanguageTo, [RemainingText] string text)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        if (!Translator.ContainsKeyOrVal(LanguageTo)) LanguageTo = LanguageTo.Humanize(LetterCasing.Sentence);
        if (!Translator.ContainsKeyOrVal(LanguageTo))
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithContent(string.Format(lang.NotValidLanguage,
                    StringUtils.ArrayToString(Translator.Languages.ToArray(), ", ")))
                .WithAllowedMentions(Mentions.None)
                .SendAsync(ctx.Channel);
            return;
        }

        Translator translator = new(HttpClient);
        var tranlsation = await translator.TranslateAsync(text, "auto", LanguageTo);
        if (tranlsation.Item1.Length > 4095)
            await OwnerOnly.SendStringFileWithContent(ctx, $"TTS URL: <{tranlsation.Item2}>", tranlsation.Item1);
        else
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithEmbed(new DiscordEmbedBuilder().WithDescription(tranlsation.Item1).WithUrl(tranlsation.Item2)
                    .WithTitle("TTS").Build())
                .WithAllowedMentions(Mentions.None)
                .SendAsync(ctx.Channel);
    }

    [Command("urbandictionary")]
    [Aliases("urbandict", "urban")]
    [RequireNsfw]
    [Description("Search up definitions for words on urban dictionary, pls dont kill me urban")]
    public async Task Urban(CommandContext ctx, [Description("the name of the package")] [RemainingText] string query)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        try
        {
            var data = await UrbanDictUtils.GetDefenition(query, HttpClient);
            var pages = new List<Page>();
            for (var i = 0; i < data.Length; i++)
            {
                Console.WriteLine(data[i].Definition.Length);
                var tempbuilder = new DiscordEmbedBuilder().WithTitle(data[i].Word).WithUrl(data[i].Permalink)
                    .WithColor(await ColorUtils.GetSingleAsync()).WithDescription(data[i].Definition.Length > 2048
                        ? data[i].Definition.Remove(2045, data[i].Definition.Length - 2045) + "..."
                        : data[i].Definition);
                if (!string.IsNullOrEmpty(data[i].Example)) tempbuilder.AddField(lang.UrbanExample, data[i].Example);

                tempbuilder.WithFooter(
                    $"{lang.RequestedBy}{ctx.User.Username} {string.Format(lang.PageNuget, i + 1, data.Length)}",
                    ctx.User.GetAvatarUrl(ImageFormat.Png));
                pages.Add(new Page(embed: tempbuilder));
            }

            var interactivity = ctx.Client.GetInteractivity();
            await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages, token: new CancellationToken());
        }
        catch (Exception)
        {
            var bob = new DiscordEmbedBuilder().WithTitle(lang.SearchFailTitle)
                .WithDescription(lang.SearchFailDescription).WithColor(await ColorUtils.GetSingleAsync());
            await ctx.RespondAsync(bob.Build());
            throw;
        }
    }

    [Command("nuget")]
    [Aliases("nu")]
    [Description("Search up packages on the NuGet")]
    public async Task Nuget(CommandContext ctx, [Description("the name of the package")] string query)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        try
        {
            var data = await NuGetUtils.SearchAsync(query, HttpClient);
            var pages = new List<Page>();
            for (var i = 0; i < data.Length; i++)
            {
                var tempbuilder = new DiscordEmbedBuilder().WithTitle(data[i].Title)
                    .WithUrl($"https://www.nuget.org/packages/{data[i].Id}")
                    .WithColor(await ColorUtils.GetSingleAsync());
                if (data[i].Authors is null)
                    tempbuilder.WithAuthor(string.Format(lang.SomethingsContributors, data[i].Title),
                        data[i].ProjectUrl);
                else
                    tempbuilder.WithAuthor(StringUtils.ArrayToString(data[i].Authors, ", "), data[i].ProjectUrl);
                tempbuilder.WithFooter(
                    lang.RequestedBy + ctx.User.Username + " " + string.Format(lang.PageNuget, i + 1, data.Length),
                    ctx.User.GetAvatarUrl(ImageFormat.Png));
                if (!string.IsNullOrEmpty(data[i].IconUrl)) tempbuilder.WithThumbnail(data[i].IconUrl);
                if (!string.IsNullOrEmpty(data[i].Description)) tempbuilder.WithDescription(data[i].Description);
                if (data[i].Verified is not null)
                    tempbuilder.AddField(lang.NuGetVerified, StringUtils.BoolToEmoteString(data[i].Verified == true),
                        true);
                if (!string.IsNullOrEmpty(data[i].Type)) tempbuilder.AddField(lang.Type, data[i].Type, true);
                if (data[i].TotalDownloads is not null)
                    tempbuilder.AddField(lang.Downloads, data[i].TotalDownloads.ToString(), true);
                if (!string.IsNullOrEmpty(data[i].Version)) tempbuilder.AddField(lang.Version, data[i].Version, true);
                pages.Add(new Page(embed: tempbuilder));
            }

            var interactivity = ctx.Client.GetInteractivity();
            await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages, token: new CancellationToken());
        }
        catch (Exception)
        {
            var bob = new DiscordEmbedBuilder().WithTitle(lang.SearchFailTitle)
                .WithDescription(lang.SearchFailDescription).WithColor(await ColorUtils.GetSingleAsync());
            await ctx.RespondAsync(bob.Build());
            throw;
        }
    }

    [Command("cserrcode")]
    public async Task Csharperror(CommandContext ctx, string error)
    {
        var NuGetError = NuGetErrorR.Match(error);
        var DotNetError = DotNetErrorR.Match(error);
        var CsharpError = CsharpErrorR.Match(error);
        var FsharpError = FsharpErrorR.Match(error);
        var VbError = VbErrorR.Match(error);
        var SerilogError = SerilogErrorR.Match(error);
        var SonarError = SonarErrorR.Match(error);
        var IdeError = IdeErrorR.Match(error);
        var link = "Not Found";
        if (NuGetError.Success)
            link = $"https://docs.microsoft.com/en-us/nuget/reference/errors-and-warnings/{NuGetError.Groups[0]}";
        else if (DotNetError.Success)
            link =
                $"https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/{DotNetError.Groups[0]}";
        else if (CsharpError.Success)
            link =
                $"https://docs.microsoft.com/en-us/dotnet/csharp/misc/{CsharpError.Groups[0]} or https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/{CsharpError.Groups[0]}";
        else if (FsharpError.Success)
            link =
                $"https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/compiler-messages/{FsharpError.Groups[0]}";
        else if (VbError.Success)
            link = $"https://docs.microsoft.com/en-us/dotnet/visual-basic/misc/{VbError.Groups[0]}";
        else if (SonarError.Success)
            link = $"https://rules.sonarsource.com/csharp/RSPEC-{SonarError.Groups[1]}";
        else if (SerilogError.Success)
            link =
                $"https://github.com/Suchiman/SerilogAnalyzer/blob/master/README.md#:~:text={SerilogError.Groups[0].Value.ToLower()} note: text fragments may not work for you if so search for `{SerilogError.Groups[0].Value.ToLower()}` in the site";
        else if (IdeError.Success)
            link =
                $"https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/{IdeError.Groups[0]}";
        await new DiscordMessageBuilder()
            .WithReply(ctx.Message.Id)
            .WithContent(link)
            .SendAsync(ctx.Channel);
    }
}