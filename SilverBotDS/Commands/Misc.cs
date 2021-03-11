using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Humanizer;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    internal class MiscCommands : BaseCommandModule
    {
        [Command("ver")]
        [Description("Get the version info")]
        public async Task Userinfo(CommandContext ctx)
        {
            try
            {
                var lang = Language.GetLanguageFromCtx(ctx);
                await new DiscordMessageBuilder().WithEmbed(new DiscordEmbedBuilder()
                    .WithTitle(lang.VersionInfoTitle)
                    .AddField("Version number", "`" + VersionInfo.VNumber + "`")
                    .AddField("Git repo", ThisAssembly.Git.RepositoryUrl)
                    .AddField("Git Commit hash", "`" + ThisAssembly.Git.Commit + "`")
                    .AddField("Git Branch", "`" + ThisAssembly.Git.Branch + "`")
                    .AddField("Is dirty", StringUtils.BoolToEmoteString(ThisAssembly.Git.IsDirty))
                    .WithAuthor(ctx.Client.CurrentUser.Username + "#" + ctx.Client.CurrentUser.Discriminator, iconUrl: ctx.Client.CurrentUser.GetAvatarUrl(ImageFormat.Auto))
                    .WithColor(await ColorUtils.GetSingleAsync())
                    .Build()).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
            }
            catch (Exception e)
            {
                Program.SendLog(e);
            }
        }

        [Command("translateunknown")]
        [Description("translate from an unknown language")]
        public async Task TranlateUnknown(CommandContext ctx, [RemainingText] string text)
        {
            var lang = Language.GetLanguageFromCtx(ctx);
            Translator translator = new(NetClient.Get());
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                             .WithContent(await translator.TranslateAsync(text, "auto", lang.LangCodeGoogleTranslate))
                                             .SendAsync(ctx.Channel);
        }

        [Command("translateunknownto")]
        [Description("translate from an unknown language to a specified one")]
        public async Task TranlateUnknown(CommandContext ctx, string LanguageTo, [RemainingText] string text)
        {
            var lang = Language.GetLanguageFromCtx(ctx);
            LanguageTo = LanguageTo.Humanize(casing: LetterCasing.Sentence);
            Translator translator = new(NetClient.Get());

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

        //TODO make it better
        [Command("urbandictionary"), Aliases("urbandict", "urban")]
        [Description("Search up definitions for words on urban dictionary, pls dont kill me urban")]
        public async Task Urban(CommandContext ctx, [Description("the name of the package")][RemainingText] string query)
        {
            var lang = Language.GetLanguageFromCtx(ctx);
            try
            {
                var data = await UrbanDictUtils.GetDefenition(query);
                var pages = new List<Page>();
                for (var i = 0; i < data.Length; i++)
                {
                    var tempbuilder = new DiscordEmbedBuilder().WithTitle(data[i].Word).WithUrl(data[i].Permalink).WithColor(await ColorUtils.GetSingleAsync()).WithDescription(data[i].Definition);
                    if (!string.IsNullOrEmpty(data[i].Example))
                    {
                        tempbuilder.AddField("Example", data[i].Example);
                    }

                    tempbuilder.WithFooter(lang.RequestedBy + ctx.User.Username + " " + string.Format(lang.PageNuget, i + 1, data.Length), ctx.User.GetAvatarUrl(ImageFormat.Png));
                    pages.Add(new Page(embed: tempbuilder));
                }

                await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages, timeoutoverride: new TimeSpan(0, 2, 0));
            }
            catch (Exception e)
            {
                var bob = new DiscordEmbedBuilder().WithTitle(lang.SearchFailTitle).WithDescription(lang.SearchFailDescription).WithColor(await ColorUtils.GetSingleAsync());
                await ctx.RespondAsync(embed: bob.Build());
                Program.SendLog(e);
                throw;
            }
        }

        //TODO make it better
        [Command("nuget"), Aliases("nu")]
        [Description("Search up packages on the NuGet")]
        public async Task Nuget(CommandContext ctx, [Description("the name of the package")] string query)
        {
            var lang = Language.GetLanguageFromCtx(ctx);
            try
            {
                var data = await NuGetUtils.SearchAsync(query);
                var pages = new List<Page>();
                for (var i = 0; i < data.Length; i++)
                {
                    var tempbuilder = new DiscordEmbedBuilder().WithTitle(data[i].Title).WithUrl($"https://www.nuget.org/packages/{data[i].Id}").WithColor(await ColorUtils.GetSingleAsync());
                    if (data[i].Authors is null)
                    {
                        tempbuilder.WithAuthor(data[i].Title + "'s contributors", data[i].ProjectUrl);
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
                        tempbuilder.AddField("NuGet verified", StringUtils.BoolToEmoteString(data[i].Verified == true), true);
                    }

                    if (!string.IsNullOrEmpty(data[i].Type))
                    {
                        tempbuilder.AddField("Type", data[i].Type, true);
                    }
                    if (data[i].TotalDownloads is not null)
                    {
                        tempbuilder.AddField("<:green_download_icon:805051604797227038>", data[i].TotalDownloads.ToString(), true);
                    }
                    if (!string.IsNullOrEmpty(data[i].Version))
                    {
                        tempbuilder.AddField("V", data[i].Version, true);
                    }

                    pages.Add(new Page(embed: tempbuilder));
                }

                await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages, timeoutoverride: new TimeSpan(0, 2, 0));
            }
            catch (Exception e)
            {
                var bob = new DiscordEmbedBuilder().WithTitle(lang.SearchFailTitle).WithDescription(lang.SearchFailDescription).WithColor(await ColorUtils.GetSingleAsync());
                await ctx.RespondAsync(embed: bob.Build());
                Program.SendLog(e);
                throw;
            }
        }

        [Command("cserrcode")]
        public async Task Csharperror(CommandContext ctx, string error)
        {
            bool NuGetError = false;
            bool DotNetError = false;
            bool CsharpError = false;
            bool FsharpError = false;
            bool VbError = false;
            try
            {
                NuGetError = Regex.IsMatch(error, @"NU[0-9][0-9][0-9][0-9]");
            }
            catch (RegexMatchTimeoutException)
            {
            }
            try
            {
                DotNetError = Regex.IsMatch(error, @"CA[0-9][0-9][0-9][0-9]");
            }
            catch (RegexMatchTimeoutException)
            {
            }
            try
            {
                CsharpError = Regex.IsMatch(error, @"CS[0-9][0-9][0-9][0-9]");
            }
            catch (RegexMatchTimeoutException)
            {
            }
            try
            {
                FsharpError = Regex.IsMatch(error, @"FS[0-9][0-9][0-9][0-9]");
            }
            catch (RegexMatchTimeoutException)
            {
            }
            try
            {
                VbError = Regex.IsMatch(error, @"BC[0-9][0-9][0-9][0-9][0-9]");
            }
            catch (RegexMatchTimeoutException)
            {
            }

            string link = "Not Found";
            if (NuGetError)
            {
                link = "https://docs.microsoft.com/en-us/nuget/reference/errors-and-warnings/" + error;
            }
            if (DotNetError)
            {
                link = "https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/" + error;
            }
            if (CsharpError)
            {
                link = $"https://docs.microsoft.com/en-us/dotnet/csharp/misc/{error} or https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/{error}";
            }
            if (FsharpError)
            {
                link = "https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/compiler-messages/" + error;
            }
            if (VbError)
            {
                link = "https://docs.microsoft.com/en-us/dotnet/visual-basic/misc/" + error;
            }
            await new DiscordMessageBuilder()
                                            .WithReply(ctx.Message.Id)
                                            .WithContent(link)
                                            .SendAsync(ctx.Channel);
        }
    }
}