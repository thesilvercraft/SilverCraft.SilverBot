/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Humanizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SilverBot.Shared;
using SilverBot.Shared.Attributes;
using SilverBot.Shared.Objects.Database;
using SilverBot.Shared.Objects.Database.Classes;
using SilverBot.Shared.Objects.Language;
using SilverBot.Shared.Utils;
using SilverBotDS.ProgramExtensions;
using CategoryAttribute = SilverBot.Shared.Attributes.CategoryAttribute;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SilverBotDS.Commands;

[Category("Miscellaneous")]
[RequireModuleGuildEnabled(EnabledModules.Misc, true)]

public class MiscCommands : BaseCommandModule
{

    public LanguageService LanguageService { private get; set; }
    public DatabaseContext Database { private get; set; }
    public HttpClient HttpClient { private get; set; }
    public static DiscordEmbed VersionInfoEmbed(Language lang, dynamic ctx)
    {
        return new DiscordEmbedBuilder()
            .WithTitle(lang.VersionInfoTitle)
            .AddField(lang.VersionNumber, Formatter.InlineCode(VersionInfo.VNumber))
            .AddField(lang.GitRepo, VersionInfo.GitRepo)
            // .AddField(lang.GitCommitHash, Formatter.InlineCode(ThisAssembly.Git.Commit))
            //.AddField(lang.GitBranch, Formatter.InlineCode(ThisAssembly.Git.Branch))
            //.AddField(lang.IsDirty, StringUtils.BoolToEmoteString(ThisAssembly.Git.IsDirty))
            .AddField(lang.CLR, Formatter.InlineCode(RuntimeInformation.FrameworkDescription))
            .AddField(lang.OS, Formatter.InlineCode(Environment.OSVersion.VersionString))
            .AddField(lang.DsharpplusVersion, Formatter.InlineCode(ctx.Client.VersionString))
            .WithAuthor($"{ctx.Client.CurrentUser.Username}#{ctx.Client.CurrentUser.Discriminator}",
                iconUrl: ctx.Client.CurrentUser.GetAvatarUrl(ImageFormat.Auto))
            .Build();
    }

    [Command("version")]
    [Description("Get the version info")]
    [Aliases("ver", "verinfo", "versioninfo")]
    public async Task VersionInfoCmd(CommandContext ctx)
    {
        var lang = await LanguageService.FromCtxAsync(ctx);
        await new DiscordMessageBuilder().WithEmbed(VersionInfoEmbed(lang, ctx)).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
    }

    [Command("setlang")]
    [Description("set your language")]
    [RequireUserPermissions(Permissions.ManageGuild)]
    public async Task SetLanguage(CommandContext ctx, string langName)
    {
        if (LanguageService.LoadedLanguages().AsEnumerable()
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

            var lang = await LanguageService.FromCtxAsync(ctx);
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithContent(lang.Success)
                .SendAsync(ctx.Channel);
        }
        else
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithContent($"Unknown language. Valid choices are:```{LanguageService.LoadedLanguages().Humanize()}```")
                .SendAsync(ctx.Channel);
        }
    }

    [Command("togglerepeat")]
    [Description("toggle repeated strings")]
    [RequireUserPermissions(Permissions.ManageGuild)]
    public async Task SetLanguage(CommandContext ctx, bool enable)
    {
        var serverSettings = Database.serverSettings.FirstOrDefault(x => x.ServerId == ctx.Guild.Id);
        if (serverSettings is not null)
        {
            serverSettings.RepeatThings = enable;
            Database.serverSettings.Update(serverSettings);
        }
        else
        {
            Database.serverSettings.Add(new ServerSettings
            {
                ServerId = ctx.Guild.Id,
                RepeatThings = enable
            });
        }
        await Database.SaveChangesAsync();
        var lang = await LanguageService.FromCtxAsync(ctx);
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
        var lang = await LanguageService.FromCtxAsync(ctx);
        Translator translator = new(HttpClient);
        var translation = await translator.TranslateAsync(text, "auto", lang.LangCodeGoogleTranslate);
        if (translation.Item1.Length > 4095)
        {
            await ctx.SendStringFileWithContent($"TTS URL: <{translation.Item2}>", translation.Item1);
        }
        else
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithEmbed(new DiscordEmbedBuilder().WithDescription(translation.Item1).WithUrl(translation.Item2)
                    .WithTitle("TTS").Build())
                .WithAllowedMentions(Mentions.None)
                .SendAsync(ctx.Channel);
        }
    }

    [Command("whereis")]
    [Description("get the location of a silverbot command through the silvercraftspec.md file")]
    public async Task WhereIs(CommandContext ctx, [RemainingText] string commandname)
    {
        var lang = await LanguageService.FromCtxAsync(ctx);
        var req = await HttpClient.GetAsync(
            "https://raw.githubusercontent.com/thesilvercraft/SilverCraft.SilverBot/master/SilverCraftSpec.md");
        var text = await req.Content.ReadAsStringAsync();
        int x = text.Length - 1;
        while (x > 2)
        {
            x--;
            if (text[x] == '`' && text[x - 1] == '`' && text[x - 2] == '`')
            {
                x--;
                x--;
                break;
            }
        }

        int y = x - 3;
        while (y > 6)
        {
            y--;
            if (text[y] == 'n' && text[y - 1] == 'o' && text[y - 2] == 's' && text[y - 3] == 'j' && text[y - 4] == '`' && text[y - 5] == '`' && text[y - 6] == '`')
            {
                y++;
                break;
            }
        }

        var importantjson = text[y..x];
        var list = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, SilverCraftSpecificData>>(importantjson);
        var commandNameLower = '"' + commandname.ToLower() + '"';
        var commandMatches = list.SelectMany(x => x.Value.CommandModules.SelectMany(y => y.Commands.Where(z => z.Name == commandNameLower || z.Aliases?.Contains(commandNameLower) == true)));
        if (commandMatches.Any())
        {
            StringBuilder builder = new();
            foreach (var commandMatch in commandMatches)
            {
                builder.Append('`').Append(commandMatch.Name);
                foreach (var argument in commandMatch.Arguments)
                {
                    builder.Append(' ');
                    if (argument.Optional || argument.RemainingText)
                    {
                        builder.Append('[');
                    }
                    else
                    {
                        builder.Append('<');
                    }

                    builder.Append(argument.Name);
                    if (argument.RemainingText)
                    {
                        builder.Append("...");
                    }

                    if (argument.Optional || argument.RemainingText)
                    {
                        builder.Append(']');
                    }
                    else
                    {
                        builder.Append('>');
                    }
                }
                builder.Append('`');
                builder.AppendLine();
                builder.AppendLine($"{VersionInfo.GitRepo}/blob/master{commandMatch.Location}");
            }
            await ctx.SendMessageAsync(message: builder.ToString());
        }
        else
        {
            await ctx.SendMessageAsync(message: "command code not found, help command is a meta command");
        }
    }
    public class SilverCraftSpecificData
    {
        /// <summary>
        /// List of command modules in project (if any)
        /// </summary>
        public List<CommandModule> CommandModules { get; set; } = new();
    }
    public class CommandModule
    {
        public string Name { get; set; }
        public List<Command> Commands { get; set; } = new();

    }
    public class Command
    {
        public string Location { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string[]? Aliases { get; set; }
        public List<string> CustomAttributes { get; set; } = new();
        public List<Argument> Arguments { get; set; } = new();
    }

    public class Argument
    {
        public Argument(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public string? Description { get; set; }
        public bool RemainingText { get; set; }
        public bool Optional { get; set; }
        public List<string> CustomAttributes { get; set; } = new();

    }
    [Command("translateunknownto")]
    [Aliases("translateto")]
    [Description("translate from an unknown language to a specified one")]
    public async Task TranlateUnknown(CommandContext ctx, string languageTo, [RemainingText] string text)
    {
        var lang = await LanguageService.FromCtxAsync(ctx);
        if (!Translator.ContainsKeyOrVal(languageTo))
        {
            languageTo = languageTo.Humanize(LetterCasing.Sentence);
        }

        if (!Translator.ContainsKeyOrVal(languageTo))
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithContent(string.Format(lang.NotValidLanguage,
                        string.Join(", ", Translator.Languages.ToArray())))
                .WithAllowedMentions(Mentions.None)
                .SendAsync(ctx.Channel);
            return;
        }

        Translator translator = new(HttpClient);
        var translation = await translator.TranslateAsync(text, "auto", languageTo);
        if (translation.Item1.Length > 4095)
        {
            await ctx.SendStringFileWithContent($"TTS URL: <{translation.Item2}>", translation.Item1);
        }
        else
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithEmbed(new DiscordEmbedBuilder().WithDescription(translation.Item1).WithUrl(translation.Item2)
                    .WithTitle("TTS").Build())
                .WithAllowedMentions(Mentions.None)
                .SendAsync(ctx.Channel);
        }
    }


}