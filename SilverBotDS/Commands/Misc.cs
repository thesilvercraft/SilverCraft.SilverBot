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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace SilverBotDS.Commands
{
    internal class MiscCommands : BaseCommandModule
    {
        public ISBDatabase Database { private get; set; }
        public Config Config { private get; set; }
        public HttpClient HttpClient { private get; set; }
        private readonly Regex _emote = new("<(a)?:(?<name>.+?):(?<id>.+?)>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private struct SDEmote : IEquatable<SDEmote>
        {
            public string Raw;
            public string Name;
            public ulong Id;
            public DateTimeOffset Timestamp;

            public bool Equals(SDEmote other)
            {
                return other.Id == Id;
            }

            public override bool Equals(object obj)
            {
                return obj is SDEmote emote && Equals(emote);
            }
        }

        [Command("emoteanalyse")]
        [Description("analyse emote usage in a specified channel")]
        [Cooldown(1, 60 * 60, CooldownBucketType.Guild)]
        public async Task EmoteAnalytics(CommandContext ctx, DiscordChannel channel, int limit = 10000)
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                             .WithContent($"starting emoji getter bot dot exe 69696969696969969696\nDownloading at most {limit} messages which will take around {new TimeSpan(0, 0, limit / 100).Humanize(2)} cause of discord ratelimit")
                                             .SendAsync(ctx.Channel);
            DateTime start = DateTime.Now;
            var messages = await channel.GetMessagesAsync(limit);
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                            .WithContent($"Downloaded {messages.Count} while expecting {limit}, estimated time was: {new TimeSpan(0, 0, limit / 100).Humanize(2)} actual time was {(DateTime.Now - start).Humanize(2)} expected time if provided correct limit would be {new TimeSpan(0, 0, messages.Count / 100).Humanize(2)}\nWell anyways processing messages")
                                            .SendAsync(ctx.Channel);
            DateTime startproc = DateTime.Now;
            StringBuilder bob = new("Name,Id,Timestamp\n");
            ulong e = 0;
            foreach (var message in messages)
            {
                foreach (var part in message.Content.Split(' '))
                {
                    var m = _emote.Matches(part);
                    if (m.Count != 0)
                    {
                        foreach (Match match in m.ToArray())
                        {
                            try
                            {
                                bob.AppendLine($"{match.Groups["name"].Value},{Convert.ToUInt64(match.Groups["id"].Value)},{ (DateTimeOffset)(message.EditedTimestamp != null ? message.EditedTimestamp : message.Timestamp):yyyy-MM-dd HH:mm:ss}");
                            }
                            catch (FormatException)
                            {
                            }
                            catch (OverflowException)
                            {
                            }
                        }
                    }
                }
                e++;
            }
            await OwnerOnly.SendStringFileWithContent(ctx, $"i went through {messages.Count}messages in {(DateTime.Now - start).Humanize(2)}(including download) {(DateTime.Now - startproc).Humanize(2)}(excluding download), now im done with collecting that time to yeet it in a csv\nEmote usage data:", bob.ToString(), "emotes.csv");
            bob.Clear();
            GC.Collect();
        }

        [Command("version")]
        [Description("Get the version info")]
        [Aliases("ver", "verinfo", "versioninfo")]
        public async Task VersionInfoCmd(CommandContext ctx)
        {
            try
            {
                var lang = (await Language.GetLanguageFromCtxAsync(ctx));
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
            catch (Exception e)
            {
                Program.SendLog(e);
            }
        }

        [Command("generatelangtemplate")]
        [Description("make a template for localisation")]
        public async Task GenerateLanguageTemplate(CommandContext ctx)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize((await Language.GetLanguageFromCtxAsync(ctx)), options)));
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                             .WithFile("language.json", stream)
                                             .SendAsync(ctx.Channel);
        }

        [Command("setlang")]
        [Description("set your language")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task SetLanguage(CommandContext ctx, string LangName)
        {
            if (Language.LoadedLanguages().ToList().FirstOrDefault(x => x.ToLower() == LangName.ToLower()) != null)
            {
                var db = Database;
                if (ctx.Channel.IsPrivate)
                {
                    await db.InserOrUpdateLangCodeUser(new DbLang { DId = ctx.User.Id, Name = LangName.ToLower() });
                }
                else
                {
                    await db.InserOrUpdateLangCodeGuild(new DbLang { DId = ctx.Guild.Id, Name = LangName.ToLower() });
                }
                var lang = (await Language.GetLanguageFromCtxAsync(ctx));
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

        [Command("translateunknown")]
        [Description("translate from an unknown language")]
        public async Task TranlateUnknown(CommandContext ctx, [RemainingText] string text)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            Translator translator = new(HttpClient);
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                             .WithContent(await translator.TranslateAsync(text, "auto", lang.LangCodeGoogleTranslate))
                                             .SendAsync(ctx.Channel);
        }

        [Command("translateunknownto")]
        [Description("translate from an unknown language to a specified one")]
        public async Task TranlateUnknown(CommandContext ctx, string LanguageTo, [RemainingText] string text)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
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
        [Description("Search up definitions for words on urban dictionary, pls dont kill me urban")]
        public async Task Urban(CommandContext ctx, [Description("the name of the package")][RemainingText] string query)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            try
            {
                var data = await UrbanDictUtils.GetDefenition(query);
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
            catch (Exception e)
            {
                var bob = new DiscordEmbedBuilder().WithTitle(lang.SearchFailTitle).WithDescription(lang.SearchFailDescription).WithColor(await ColorUtils.GetSingleAsync());
                await ctx.RespondAsync(embed: bob.Build());
                Program.SendLog(e);
                throw;
            }
        }

        [Command("nuget"), Aliases("nu")]
        [Description("Search up packages on the NuGet")]
        public async Task Nuget(CommandContext ctx, [Description("the name of the package")] string query)
        {
            var lang = (await Language.GetLanguageFromCtxAsync(ctx));
            try
            {
                var data = await NuGetUtils.SearchAsync(query);
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