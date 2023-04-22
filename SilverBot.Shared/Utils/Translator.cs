/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Text;
using System.Web;

namespace SilverBot.Shared.Utils
{
    public class Translator
    {
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

        private readonly HttpClient _httpClient;

        public Translator()
        {
            var nhttpClient = new HttpClient();
            _ = nhttpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("SilverBotTranslate");
            _httpClient = nhttpClient;
        }

        public Translator(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public static IEnumerable<string> Languages => LanguageModeMap.Keys.OrderBy(p => p);

        public async Task<Tuple<string, string>> TranslateAsync
        (string sourceText,
            string sourceLanguage,
            string targetLanguage)
        {
            StringBuilder translation = new();
            var url =
                $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={LanguageEnumToIdentifier(sourceLanguage)}&tl={LanguageEnumToIdentifier(targetLanguage)}&dt=t&q={HttpUtility.UrlEncode(sourceText)}";
            using (var response = await _httpClient.GetAsync(url))
            {
                var text = await response.Content.ReadAsStringAsync();

                var index = text.IndexOf($",,\"{LanguageEnumToIdentifier(sourceLanguage)}\"",
                    StringComparison.InvariantCulture);
                if (index == -1)
                {
                    var startQuote = text.IndexOf('\"');
                    if (startQuote != -1)
                    {
                        var endQuote = text.IndexOf('\"', startQuote + 1);
                        if (endQuote != -1)
                        {
                            translation = new StringBuilder(text.Substring(startQuote + 1, endQuote - startQuote - 1));
                        }
                    }
                }
                else
                {
                    text = text.Substring(0, index);
                    text = text.Replace("],[",
                        ",").Replace("]", string.Empty).Replace("[", string.Empty).Replace("\",\"", "\"");
                    var phrases = text.Split(new[] { '\"' }, StringSplitOptions.RemoveEmptyEntries);
                    for (var i = 0; i < phrases.Length; i += 2)
                    {
                        var translatedPhrase = phrases[i];
                        if (translatedPhrase.StartsWith(",,"))
                        {
                            i--;
                            continue;
                        }

                        translation.Append(translatedPhrase);
                        translation.Append("  ");
                    }
                }
            }

            var translationstring = translation.ToString().Trim().Replace(" ?", "?").Replace(" !", "!")
                .Replace(" ,", ",")
                .Replace(" .", ".").Replace(" ;", ";").Replace("\n", Environment.NewLine)
                .Replace("\\n", Environment.NewLine);
            translation.Clear();
            return new Tuple<string, string>(translationstring,
                $"https://translate.googleapis.com/translate_tts?ie=UTF-8&q={HttpUtility.UrlEncode(translationstring)}&tl={LanguageEnumToIdentifier(targetLanguage)}&total=1&idx=0&textlen={translation.Length}&client=gtx");
        }

        public static bool ContainsKeyOrVal(string language)
        {
            return LanguageModeMap.ContainsValue(language) || LanguageModeMap.ContainsKey(language);
        }

        private static string LanguageEnumToIdentifier(string language)
        {
            if (!LanguageModeMap.ContainsValue(language.ToLowerInvariant()))
            {
                LanguageModeMap.TryGetValue(language, out var mode);
                return mode;
            }

            //no need to search for a value when it is one
            return language.ToLowerInvariant();
        }
    }
}