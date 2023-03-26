using System.Resources;
using System.Text.Json;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SilverBot.Shared.Attributes;
using SilverBot.Shared.Objects.Database;

namespace SilverBot.Shared.Objects.Language
{
    public class LanguageService
    {
        private static readonly Dictionary<string, Language> CachedLanguages = new();

        private static readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };
        public  Dictionary<string, Language> GetLoadedLanguages()
        {
            if (CachedLanguages.Count != 0)
            {
                return CachedLanguages;
            }
            Get("en");
            return CachedLanguages;
        }

        public IEnumerable<string> LoadedLanguages()
        {
            if (CachedLanguages.Count == 0)
            {
                Get("en");
            }
            return CachedLanguages.Keys.ToArray();
        }

        public Language Get(string languageName)
        {
            return Task.Run(async ()=> await GetAsync(languageName)).GetAwaiter().GetResult();
        }
        ///<summary>
        ///a last resort for a non null <seealso cref="Language"/>
        ///</summary>
        public async Task<Language> GetDefaultAsync()
        {
            Log.Warning("GetDefaultAsync was called in {StackTrace}", Environment.StackTrace);
            return await GetAsync("en") ?? new Language();
        }
         ///<summary>
        ///a last resort for a non null <seealso cref="Language"/>
        ///</summary>
        public  Language GetDefault()
        {
            Log.Warning("GetDefault was called in {StackTrace}", Environment.StackTrace);
            return Get("en") ?? new Language();
        }
        
        public async Task<Language> GetAsync(string languageName)
        {
            languageName = languageName.Trim();
            if (CachedLanguages.Count != 0)
            {
                return CachedLanguages.TryGetValue(languageName, out var value) ? value : new Language();
            }
            var folloc = Path.Combine(Environment.CurrentDirectory, "Languages");
            if (Directory.Exists(folloc))
            {
                foreach (var folder in Directory.GetDirectories(folloc))
                {
                    foreach (var u in Directory.GetFiles(folder))
                    {
                        await using Stream stream = File.OpenRead(u);
                        using var reader = new StreamReader(stream);
                        var content = await reader.ReadToEndAsync();
                        CachedLanguages.Add(Path.GetFileNameWithoutExtension(u), JsonSerializer.Deserialize<Language>(content)!);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(folloc, "en"));
                await SerialiseDefaultAsync(
                    Path.Combine(Environment.CurrentDirectory, "languages", "en", "en.json"));
            }
            return await GetAsync(languageName);
        }

        public static async Task SerialiseDefaultAsync(string loc)
        {
            await using var streamWriter = new StreamWriter(loc);
            await streamWriter.WriteAsync(JsonSerializer.Serialize(new Language(), options));
        }

        public static void SerialiseDefault(string loc)
        {
            using var streamWriter = new StreamWriter(loc);
            streamWriter.Write(JsonSerializer.Serialize(new Language(), options));
        }

        public async Task<Language> GetLanguageFromGuildIdAsync(ulong id, DatabaseContext db)
        {
            return await GetAsync(db.GetLangCodeGuild(id));
        }

        public Language FromCtx(CommandContext ctx)
        {
          return Task.Run(async ()=> await FromCtxAsync(ctx)).GetAwaiter().GetResult();
        }
        public Language FromCtx(ISilverBotContext ctx)
        {
            return Task.Run(async ()=> await FromCtxAsync(ctx)).GetAwaiter().GetResult();
        }
        public async Task<Language> FromCtxAsync(CommandContext ctx)
        {
            await using var databaseContext = ctx.Services.GetService<DatabaseContext>();
            var config = ctx.Services.GetService<Config>();
            return await FromCtxAsync(ctx, config ?? throw new InvalidOperationException(), databaseContext ?? throw new InvalidOperationException());
        }
        public async Task<Language> FromCtxAsync(BaseContext ctx)
        {
            await using var databaseContext = ctx.Services.GetService<DatabaseContext>();
            var config = ctx.Services.GetService<Config>();
            return await FromCtxAsync(ctx, config ?? throw new InvalidOperationException(), databaseContext ?? throw new InvalidOperationException());
        }
        public async Task<Language> FromCtxAsync(ISilverBotContext ctx)
        {
            await using var databaseContext = ctx.Services.GetService<DatabaseContext>();
            var config = ctx.Services.GetService<Config>();
            return await FromCtxAsync(ctx, config ?? throw new InvalidOperationException(), databaseContext ?? throw new InvalidOperationException());
        }
        public Language FromUserId(ulong userId,DatabaseContext databaseContext)
        {
            return Task.Run(async ()=> await FromUserIdAsync(userId,databaseContext)).GetAwaiter().GetResult();
        }
        public async Task<Language> FromUserIdAsync(ulong userId,DatabaseContext databaseContext)
        {
            return await GetAsync(databaseContext.GetLangCodeUser(userId));
        }
        private async Task<Language> FromCtxAsync(dynamic ctx, Config config, DatabaseContext databaseContext)
        {
            if (!await RequireTranslatorAttribute.IsTranslator(config, ctx.Client, ctx.User.Id, ctx.Channel.Id))
            {
                return await GetAsync(ctx.Channel.IsPrivate
                    ? databaseContext.GetLangCodeUser(ctx.User.Id)
                    : databaseContext.GetLangCodeGuild(ctx.Guild.Id));
            }
            ulong userid=ctx.User.Id;
            var t = databaseContext.translatorSettings.FirstOrDefault(x => x.Id == userid);
            if (t?.IsTranslator != true || t.CurrentCustomLanguage == null)
            {
                return await GetAsync(ctx.Channel.IsPrivate
                    ? databaseContext.GetLangCodeUser(ctx.User.Id)
                    : databaseContext.GetLangCodeGuild(ctx.Guild.Id));
            }
            await databaseContext.Entry(t).ReloadAsync();
            return t.CurrentCustomLanguage;
        }

   
    }
}