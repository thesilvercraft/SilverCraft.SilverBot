using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Humanizer;
using SilverBotDS.Exceptions;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    internal class MiscCommands : BaseCommandModule
    {
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
                .AddField(lang.VersionInfoCommand.VersionNumber, "`" + VersionInfo.VNumber + "`")
                .AddField(lang.VersionInfoCommand.GitRepo, ThisAssembly.Git.RepositoryUrl)
                .AddField(lang.VersionInfoCommand.GitCommitHash, "`" + ThisAssembly.Git.Commit + "`")
                .AddField(lang.VersionInfoCommand.GitBranch, "`" + ThisAssembly.Git.Branch + "`")
                .AddField(lang.VersionInfoCommand.IsDirty, StringUtils.BoolToEmoteString(ThisAssembly.Git.IsDirty))
                .AddField(lang.VersionInfoCommand.CLR, "`" + (ctx.Client.CurrentApplication.Owners.Contains(ctx.User) && ctx.Channel.IsPrivate ? System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription : "REDACTED") + "`")
                .WithAuthor(ctx.Client.CurrentUser.Username + "#" + ctx.Client.CurrentUser.Discriminator, iconUrl: ctx.Client.CurrentUser.GetAvatarUrl(ImageFormat.Auto))
                .WithColor(await ColorUtils.GetSingleAsync())
                .Build()).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
        }

        [Command("generatelangtemplate")]
        [Description("make a template for translation")]
        public async Task GenerateLanguageTemplate(CommandContext ctx)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(await Language.GetLanguageFromCtxAsync(ctx), options)));
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                             .WithFile("language.json", stream)
                                             .SendAsync(ctx.Channel);
        }

        [Command("piss")]
        [Description("piss :)")]
        public async Task Piss(CommandContext ctx)
        {
            // line 67
            // line 68
            throw new PissException(69); // line 69 :)
        }

        [Command("setlang")]
        [Description("set your language")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task SetLanguage(CommandContext ctx, string LangName)
        {
            if (Language.LoadedLanguages().AsEnumerable().FirstOrDefault(x => x.ToLowerInvariant() == LangName.ToLowerInvariant()) != null)
            {
                if (ctx.Channel.IsPrivate)
                {
                    Database.InserOrUpdateLangCodeUser(ctx.User.Id, LangName.ToLowerInvariant());
                }
                else
                {
                    Database.InserOrUpdateLangCodeGuild(ctx.Guild.Id, LangName.ToLowerInvariant());
                }
                var lang = await Language.GetLanguageFromCtxAsync(ctx);
                await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                         .WithContent(lang.Success)
                                         .SendAsync(ctx.Channel);
            }
            else
            {
                await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                          .WithContent("Unknown language. Valid choices are:```" + Language.LoadedLanguages().Humanize() + "```")
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
                Database.serverSettings.Add(new()
                {
                    EmotesOptin = false,
                    LangName = "en",
                    ServerId = ctx.Guild.Id,
                    ServerStatsCategoryId = null,
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

        [Command("button")]
        public async Task ButtonTest(CommandContext ctx)
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                             .WithContent("gaming")
                                             .AddComponents(new DiscordButtonComponent(ButtonStyle.Primary,"e","",false,new DiscordComponentEmoji(783096213875851304)))
                                             .SendAsync(ctx.Channel);
        }

        [Command("translateunknown")]
        [Aliases("translate")]
        [Description("translate from an unknown language")]
        public async Task TranlateUnknown(CommandContext ctx, [RemainingText] string text)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            Translator translator = new(HttpClient);
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                             .WithContent(await translator.TranslateAsync(text, "auto", lang.LangCodeGoogleTranslate))
                                             .SendAsync(ctx.Channel);
        }

        [Command("translateunknownto")]
        [Aliases("translateto")]
        [Description("translate from an unknown language to a specified one")]
        public async Task TranlateUnknown(CommandContext ctx, string LanguageTo, [RemainingText] string text)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            LanguageTo = LanguageTo.Humanize(casing: LetterCasing.Sentence);
            Translator translator = new(HttpClient);

            if (!Translator.ContainsKeyOrVal(LanguageTo))
            {
                await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                           .WithContent(string.Format(lang.NotValidLanguage, StringUtils.ArrayToString(Translator.Languages.ToArray(), ", ")))
                                           .SendAsync(ctx.Channel);
                return;
            }
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                         .WithContent(await translator.TranslateAsync(text, "auto", LanguageTo))
                                         .SendAsync(ctx.Channel);
        }

        [Command("urbandictionary"), Aliases("urbandict", "urban")]
        [RequireNsfw]
        [Description("Search up definitions for words on urban dictionary, pls dont kill me urban")]
        public async Task Urban(CommandContext ctx, [Description("the name of the package")][RemainingText] string query)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            try
            {
                var data = await UrbanDictUtils.GetDefenition(query, HttpClient);
                var pages = new List<Page>();
                for (var i = 0; i < data.Length; i++)
                {
                    Console.WriteLine(data[i].Definition.Length);
                    var tempbuilder = new DiscordEmbedBuilder().WithTitle(data[i].Word).WithUrl(data[i].Permalink).WithColor(await ColorUtils.GetSingleAsync()).WithDescription(data[i].Definition.Length > 2048 ? data[i].Definition.Remove(2045, data[i].Definition.Length - 2045) + "..." : data[i].Definition);
                    if (!string.IsNullOrEmpty(data[i].Example))
                    {
                        tempbuilder.AddField(lang.UrbanExample, data[i].Example);
                    }

                    tempbuilder.WithFooter(lang.RequestedBy + ctx.User.Username + " " + string.Format(lang.PageNuget, i + 1, data.Length), ctx.User.GetAvatarUrl(ImageFormat.Png));
                    pages.Add(new Page(embed: tempbuilder));
                }

                await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages, timeoutoverride: new TimeSpan(0, 2, 0));
            }
            catch (Exception)
            {
                var bob = new DiscordEmbedBuilder().WithTitle(lang.SearchFailTitle).WithDescription(lang.SearchFailDescription).WithColor(await ColorUtils.GetSingleAsync());
                await ctx.RespondAsync(embed: bob.Build());
                throw;
            }
        }

        [Command("nuget"), Aliases("nu")]
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
                    var tempbuilder = new DiscordEmbedBuilder().WithTitle(data[i].Title).WithUrl($"https://www.nuget.org/packages/{data[i].Id}").WithColor(await ColorUtils.GetSingleAsync());
                    if (data[i].Authors is null)
                    {
                        tempbuilder.WithAuthor(string.Format(lang.NuGetCommand.SomethingsContributors, data[i].Title), data[i].ProjectUrl);
                    }
                    else
                    {
                        tempbuilder.WithAuthor(StringUtils.ArrayToString(data[i].Authors, seperator: ", "), data[i].ProjectUrl);
                    }
                    tempbuilder.WithFooter(lang.RequestedBy + ctx.User.Username + " " + string.Format(lang.PageNuget, i + 1, data.Length), ctx.User.GetAvatarUrl(ImageFormat.Png));
                    if (!string.IsNullOrEmpty(data[i].IconUrl))
                    {
                        tempbuilder.WithThumbnail(data[i].IconUrl);
                    }
                    if (!string.IsNullOrEmpty(data[i].Description))
                    {
                        tempbuilder.WithDescription(data[i].Description);
                    }

                    if (data[i].Verified is not null)
                    {
                        tempbuilder.AddField(lang.NuGetCommand.NuGetVerified, StringUtils.BoolToEmoteString(data[i].Verified == true), true);
                    }

                    if (!string.IsNullOrEmpty(data[i].Type))
                    {
                        tempbuilder.AddField(lang.NuGetCommand.Type, data[i].Type, true);
                    }
                    if (data[i].TotalDownloads is not null)
                    {
                        tempbuilder.AddField(lang.NuGetCommand.Downloads, data[i].TotalDownloads.ToString(), true);
                    }
                    if (!string.IsNullOrEmpty(data[i].Version))
                    {
                        tempbuilder.AddField(lang.NuGetCommand.Version, data[i].Version, true);
                    }

                    pages.Add(new Page(embed: tempbuilder));
                }

                await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages, timeoutoverride: new TimeSpan(0, 2, 0));
            }
            catch (Exception)
            {
                var bob = new DiscordEmbedBuilder().WithTitle(lang.SearchFailTitle).WithDescription(lang.SearchFailDescription).WithColor(await ColorUtils.GetSingleAsync());
                await ctx.RespondAsync(embed: bob.Build());
                throw;
            }
        }

        private readonly Regex NuGetErrorR = new("NU[0-9][0-9][0-9][0-9]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private readonly Regex DotNetErrorR = new("CA[0-9][0-9][0-9][0-9]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private readonly Regex CsharpErrorR = new("CS[0-9][0-9][0-9][0-9]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private readonly Regex FsharpErrorR = new("FS[0-9][0-9][0-9][0-9]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private readonly Regex VbErrorR = new("BC[0-9][0-9][0-9][0-9][0-9]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private readonly Regex SerilogErrorR = new("Serilog[0-9][0-9][0-9]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private readonly Regex SonarErrorR = new("S([0-9][0-9][0-9][0-9])", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private readonly Regex IdeErrorR = new("IDE[0-9][0-9][0-9][0-9]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

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
            string link = "Not Found";
            if (NuGetError.Success)
            {
                link = $"https://docs.microsoft.com/en-us/nuget/reference/errors-and-warnings/{NuGetError.Groups[0]}";
            }
            else if (DotNetError.Success)
            {
                link = $"https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/{DotNetError.Groups[0]}";
            }
            else if (CsharpError.Success)
            {
                link = $"https://docs.microsoft.com/en-us/dotnet/csharp/misc/{CsharpError.Groups[0]} or https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/{CsharpError.Groups[0]}";
            }
            else if (FsharpError.Success)
            {
                link = $"https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/compiler-messages/{FsharpError.Groups[0]}";
            }
            else if (VbError.Success)
            {
                link = $"https://docs.microsoft.com/en-us/dotnet/visual-basic/misc/{VbError.Groups[0]}";
            }
            else if (SonarError.Success)
            {
                link = $"https://rules.sonarsource.com/csharp/RSPEC-{SonarError.Groups[1]}";
            }
            else if (SerilogError.Success)
            {
                link = $"https://github.com/Suchiman/SerilogAnalyzer/blob/master/README.md#:~:text={(SerilogError.Groups[0]).Value.ToLower()} note: text fragments may not work for you if so search for `{(SerilogError.Groups[0]).Value.ToLower()}` in the site";
            }
            else if (IdeError.Success)
            {
                link = $"https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/{IdeError.Groups[0]}";
            }
            await new DiscordMessageBuilder()
                                            .WithReply(ctx.Message.Id)
                                            .WithContent(link)
                                            .SendAsync(ctx.Channel);
        }
    }
}