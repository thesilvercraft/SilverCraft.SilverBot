﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using SilverBotDS.Attributes;
using SilverBotDS.Objects;
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
    [RequireTranslator(true)]
    [ModuleLifespan(ModuleLifespan.Transient)]
    [Category("Translation")]
    public class TranslatorCommands : BaseCommandModule
    {
        public DatabaseContext databaseContext { set; private get; }
        public HttpClient httpClient { get; set; }

        private readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        [Command("editlangtranslator")]
        [Description("set you're testing language")]
        public async Task Edit(CommandContext ctx)
        {
            await ctx.RespondAsync("hey friendo try using the website which can be found at https://silverbot.cf/languageeditor");
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
            else if (customlangregex.IsMatch(lang))
            {
                var mtch = customlangregex.Match(lang);
                var usrid = Convert.ToUInt64(mtch.Groups["uid"].Value);
                var langid = Convert.ToInt32(mtch.Groups["langid"].Value);
                var translator = databaseContext.translatorSettings.FirstOrDefault(x => x.Id == usrid);
                if (translator != null)
                {
                    await databaseContext.Entry(translator).Collection(b => b.CustomLanguages).LoadAsync();
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
            var t = databaseContext.translatorSettings.FirstOrDefault(x => x.Id == ctx.User.Id);
            if (t == null)
            {
                t = new() { Id = ctx.User.Id, IsTranslator = true, CustomLanguages = new List<Language>() };
                t.CurrentCustomLanguage = langobj;
                databaseContext.translatorSettings.Add(t);
            }
            else
            {
                t.CurrentCustomLanguage = langobj;
                databaseContext.translatorSettings.Update(t);
            }
            await databaseContext.SaveChangesAsync();
        }

        [Command("uploadlanguage")]
        [RequireAttachment]
        public async Task UploadCustomLanguage(CommandContext ctx)
        {
            try
            {
                var jsonstring = await (await httpClient.GetAsync(ctx.Message.Attachments[0].Url)).Content.ReadAsStringAsync();
                var t = databaseContext.translatorSettings.FirstOrDefault(x => x.Id == ctx.User.Id);
                if (t == null)
                {
                    t = new() { Id = ctx.User.Id, IsTranslator = true, CustomLanguages = new List<Language>() };
                    databaseContext.translatorSettings.Add(t);
                    await databaseContext.SaveChangesAsync();
                }
                var l = JsonSerializer.Deserialize<Language>(jsonstring);
                l.Id = Guid.NewGuid();
                t.CustomLanguages.Add(l);
                databaseContext.translatorSettings.Attach(t);
                databaseContext.translatorSettings.Update(t);
                await databaseContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                await UploadCustomLanguage(ctx);
            }
        }

        private Regex customlangregex = new("(?<uid>[0-9]*)-(?<language>.+(-?([a-z]+?))?)-(?<langid>[0-9]+)");

        [Command("generatelangtemplate")]
        [Description("make a template for translation")]
        public async Task GenerateLanguageTemplate(CommandContext ctx, string lang = null)
        {
            Language langobj;
            if (lang == null)
            {
                langobj = await Language.GetLanguageFromCtxAsync(ctx);
            }
            else if (Language.LoadedLanguages().Select(x => x.ToLowerInvariant().Trim()).Contains(lang.ToLowerInvariant().Trim()))
            {
                langobj = await Language.GetAsync(lang);
            }
            else if (customlangregex.IsMatch(lang))
            {
                var mtch = customlangregex.Match(lang);
                var usrid = Convert.ToUInt64(mtch.Groups["uid"].Value);
                var langid = Convert.ToInt32(mtch.Groups["langid"].Value);
                var translator = databaseContext.translatorSettings.FirstOrDefault(x => x.Id == usrid);
                if (translator != null)
                {
                    await databaseContext.Entry(translator).Collection(b => b.CustomLanguages).LoadAsync();
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
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(langobj, options)));
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                             .WithFile("language.json", stream)
                                             .SendAsync(ctx.Channel);
        }
    }
}