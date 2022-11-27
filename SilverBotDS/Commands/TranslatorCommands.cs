/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using SilverBotDS.Attributes;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SilverBotDS.Objects.Database.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands;

[RequireTranslator(true)]
//[ModuleLifespan(ModuleLifespan.Transient)]
[Category("Translation")]
public class TranslatorCommands : BaseCommandModule
{
    private readonly Regex _customlangregex = new("(?<uid>[0-9]*)-(?<language>.+(-?([a-z]+?))?)-(?<langid>[0-9]+)");

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public DatabaseContext DatabaseContext { set; private get; }
    public HttpClient HttpClient { get; set; }

    [Command("editlangtranslator")]
    [Description("set you're testing language")]
    public async Task Edit(CommandContext ctx)
    {
        await ctx.RespondAsync(
            "hey friendo try using the website which can be found at https://silverbot.cf/languageeditor");
    }

    [Command("getobjectfromcurrentlanguage")]
    [Description("yayayayayyayaya")]
    public async Task Get(CommandContext ctx, string name)
    {
        var langobj = await Language.GetLanguageFromCtxAsync(ctx);
        await ctx.RespondAsync(typeof(Language).GetProperty(name).GetValue(langobj).ToString());
    }

    [Command("setlangtranslator")]
    [Description("set you're testing language")]
    public async Task SetLanguage(CommandContext ctx, string lang)
    {
        Language langobj;
        if (lang == null)
        {
            langobj = await Language.GetLanguageFromCtxAsync(ctx);
        }
        else if (Language.LoadedLanguages().Select(x => x.ToLowerInvariant()).Contains(lang.ToLowerInvariant()))
        {
            langobj = await Language.GetAsync(lang);
        }
        else if (_customlangregex.IsMatch(lang))
        {
            var mtch = _customlangregex.Match(lang);
            var usrid = Convert.ToUInt64(mtch.Groups["uid"].Value);
            var langid = Convert.ToInt32(mtch.Groups["langid"].Value);
            var translator = DatabaseContext.translatorSettings.FirstOrDefault(x => x.Id == usrid);
            if (translator != null)
            {
                await DatabaseContext.Entry(translator).Collection(b => b.CustomLanguages).LoadAsync();
                if (translator.CustomLanguages.Count >= langid)
                {
                    langobj = translator.CustomLanguages.ElementAt(langid);
                }
                else
                {
                    langobj = await Language.GetLanguageFromCtxAsync(ctx);
                }
            }
            else
            {
                await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                    .WithContent("invalid uid using default")
                    .SendAsync(ctx.Channel);
                langobj = await Language.GetLanguageFromCtxAsync(ctx);
            }
        }
        else
        {
            langobj = await Language.GetLanguageFromCtxAsync(ctx);
        }

        var t = DatabaseContext.translatorSettings.FirstOrDefault(x => x.Id == ctx.User.Id);
        if (t == null)
        {
            t = new TranslatorSettings
            {
                Id = ctx.User.Id,
                IsTranslator = true,
                CustomLanguages = new List<Language>(),
                CurrentCustomLanguage = langobj
            };
            DatabaseContext.translatorSettings.Add(t);
        }
        else
        {
            t.CurrentCustomLanguage = langobj;
            DatabaseContext.translatorSettings.Update(t);
        }

        await DatabaseContext.SaveChangesAsync();
    }

    [Command("uploadlanguage")]
    [RequireAttachment]
    public async Task UploadCustomLanguage(CommandContext ctx)
    {
        try
        {
            var jsonstring =
                await (await HttpClient.GetAsync(ctx.Message.Attachments[0].Url)).Content.ReadAsStringAsync();
            var t = DatabaseContext.translatorSettings.FirstOrDefault(x => x.Id == ctx.User.Id);
            if (t == null)
            {
                t = new TranslatorSettings
                { Id = ctx.User.Id, IsTranslator = true, CustomLanguages = new List<Language>() };
                DatabaseContext.translatorSettings.Add(t);
                await DatabaseContext.SaveChangesAsync();
            }

            var l = JsonSerializer.Deserialize<Language>(jsonstring);
            l.Id = Guid.NewGuid();
            t.CustomLanguages.Add(l);
            DatabaseContext.translatorSettings.Attach(t);
            DatabaseContext.translatorSettings.Update(t);
            await DatabaseContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            await UploadCustomLanguage(ctx);
        }
    }

    [Command("generatelangtemplate")]
    [Description("make a template for translation")]
    public async Task GenerateLanguageTemplate(CommandContext ctx, string? lang = null)
    {
        Language langobj;
        if (lang == null)
        {
            langobj = await Language.GetLanguageFromCtxAsync(ctx);
        }
        else if (Language.LoadedLanguages().Select(x => x.ToLowerInvariant().Trim())
                 .Contains(lang.ToLowerInvariant().Trim()))
        {
            langobj = await Language.GetAsync(lang);
        }
        else if (_customlangregex.IsMatch(lang))
        {
            var mtch = _customlangregex.Match(lang);
            var usrid = Convert.ToUInt64(mtch.Groups["uid"].Value);
            var langid = Convert.ToInt32(mtch.Groups["langid"].Value);
            var translator = DatabaseContext.translatorSettings.FirstOrDefault(x => x.Id == usrid);
            if (translator != null)
            {
                await DatabaseContext.Entry(translator).Collection(b => b.CustomLanguages).LoadAsync();
                if (translator.CustomLanguages.Count > langid)
                {
                    langobj = translator.CustomLanguages.ToList()[langid];
                    await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                        .WithContent($"using custom language of <@!{usrid}> with language id `{langid}`")
                        .SendAsync(ctx.Channel);
                }
                else
                {
                    await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                        .WithContent("invalid language using default")
                        .SendAsync(ctx.Channel);
                    langobj = await Language.GetLanguageFromCtxAsync(ctx);
                }
            }
            else
            {
                await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                    .WithContent("invalid uid using default")
                    .SendAsync(ctx.Channel);
                langobj = await Language.GetLanguageFromCtxAsync(ctx);
            }
        }
        else
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithContent("invalid language using default")
                .SendAsync(ctx.Channel);
            langobj = await Language.GetLanguageFromCtxAsync(ctx);
        }

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(langobj, _options)));
        await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
            .WithFile("language.json", stream)
            .SendAsync(ctx.Channel);
    }
}