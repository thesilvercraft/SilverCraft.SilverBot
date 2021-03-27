using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using PostSharp.Patterns.Caching;

namespace SilverBotDS.Utils
{
    public class Translator
    {
        public Translator()
        {
            var httpClient = new HttpClient();
            _ = httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("SilverBotTranslate");
            this.httpClient = httpClient;
        }

        public Translator(HttpClient httpClient) => this.httpClient = httpClient;

        public static IEnumerable<string> Languages => LanguageModeMap.Keys.OrderBy(p => p);

        public string TranslationSpeechUrl
        {
            get;
            private set;
        }

        private readonly HttpClient httpClient;

        [Cache]
        public async System.Threading.Tasks.Task<string> TranslateAsync
            (string sourceText,
             string sourceLanguage,
             string targetLanguage)
        {
            string translation = string.Empty;

            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={LanguageEnumToIdentifier(sourceLanguage)}&tl={LanguageEnumToIdentifier(targetLanguage)}&dt=t&q={HttpUtility.UrlEncode(sourceText)}";
            using (var response = await httpClient.GetAsync(url))
            {
                string text = await response.Content.ReadAsStringAsync();

                int index = text.IndexOf($",,\"{LanguageEnumToIdentifier(sourceLanguage)}\"",
                                         comparisonType: StringComparison.InvariantCulture);
                if (index == -1)
                {
                    int startQuote = text.IndexOf('\"');
                    if (startQuote != -1)
                    {
                        int endQuote = text.IndexOf('\"', startQuote + 1);
                        if (endQuote != -1)
                        {
                            translation = text.Substring(startQuote + 1, endQuote - startQuote - 1);
                        }
                    }
                }
                else
                {
                    text = text.Substring(0, index);
                    text = text.Replace("],[",
                                        ",").Replace("]", string.Empty).Replace("[", string.Empty).Replace("\",\"", "\"");
                    string[] phrases = text.Split(new[] { '\"' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; (i < phrases.Length); i += 2)
                    {
                        string translatedPhrase = phrases[i];
                        if (translatedPhrase.StartsWith(",,"))
                        {
                            i--;
                            continue;
                        }
                        translation += translatedPhrase + "  ";
                    }
                }
            }
            translation = translation.Trim().Replace(" ?", "?").Replace(" !", "!").Replace(" ,", ",").Replace(" .", ".").Replace(" ;", ";");
            TranslationSpeechUrl = string.Format("https://translate.googleapis.com/translate_tts?ie=UTF-8&q={0}&tl={1}&total=1&idx=0&textlen={2}&client=gtx",
            HttpUtility.UrlEncode(translation), LanguageEnumToIdentifier(targetLanguage), translation.Length);
            return translation;
        }

        public static bool ContainsKeyOrVal(string language) => LanguageModeMap.ContainsValue(language) || LanguageModeMap.ContainsKey(language);

        private static string LanguageEnumToIdentifier(string language)
        {
            if (!LanguageModeMap.ContainsValue(language))
            {
                LanguageModeMap.TryGetValue(language, out string mode);
                return mode;
            }
            //no need to search for a value when it is one
            return language;
        }

        private static readonly Dictionary<string, string> LanguageModeMap = new()
        {
            { "Auto", "auto" },
            { "Afrikaans", "af" },
            { "Albanian", "sq" },
            { "Arabic", "ar" },
            { "Armenian", "hy" },
            { "Azerbaijani", "az" },
            { "Basque", "eu" },
            { "Belarusian", "be" },
            { "Bengali", "bn" },
            { "Bulgarian", "bg" },
            { "Catalan", "ca" },
            { "Chinese", "zh-CN" },
            { "Croatian", "hr" },
            { "Czech", "cs" },
            { "Danish", "da" },
            { "Dutch", "nl" },
            { "English", "en" },
            { "Esperanto", "eo" },
            { "Estonian", "et" },
            { "Filipino", "tl" },
            { "Finnish", "fi" },
            { "French", "fr" },
            { "Galician", "gl" },
            { "German", "de" },
            { "Georgian", "ka" },
            { "Greek", "el" },
            { "Haitian Creole", "ht" },
            { "Hebrew", "iw" },
            { "Hindi", "hi" },
            { "Hungarian", "hu" },
            { "Icelandic", "is" },
            { "Indonesian", "id" },
            { "Irish", "ga" },
            { "Italian", "it" },
            { "Japanese", "ja" },
            { "Korean", "ko" },
            { "Lao", "lo" },
            { "Latin", "la" },
            { "Latvian", "lv" },
            { "Lithuanian", "lt" },
            { "Macedonian", "mk" },
            { "Malay", "ms" },
            { "Maltese", "mt" },
            { "Norwegian", "no" },
            { "Persian", "fa" },
            { "Polish", "pl" },
            { "Portuguese", "pt" },
            { "Romanian", "ro" },
            { "Russian", "ru" },
            { "Serbian", "sr" },
            { "Slovak", "sk" },
            { "Slovenian", "sl" },
            { "Spanish", "es" },
            { "Swahili", "sw" },
            { "Swedish", "sv" },
            { "Tamil", "ta" },
            { "Telugu", "te" },
            { "Thai", "th" },
            { "Turkish", "tr" },
            { "Ukrainian", "uk" },
            { "Urdu", "ur" },
            { "Vietnamese", "vi" },
            { "Welsh", "cy" },
            { "Yiddish", "yi" }
        };
    }
}