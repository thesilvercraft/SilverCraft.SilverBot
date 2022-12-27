using System;
using DSharpPlus.Entities;
using SilverBot.Shared.Pagination;
using SilverBotDS.Objects;

namespace SilverBotDS
{
    public class BibiPagination : IPagination
    {
        public BibiPagination(string urlFormat, string[] _bibiDescText, Language lang, DiscordUser user)
        {
            UrlFormat = urlFormat;
            BibiDescText = _bibiDescText;
            Lang = lang;
            User = user;
            AllowedRange= new Range(0,_bibiDescText.Length);
        }
        private string[] BibiDescText;
        private Language Lang;
        private DiscordUser User;

        private string UrlFormat;
        public int DefaultId { get; } = 0;
        public ILazyPage GetPageAtId(int id)
        {
            return new BibiPage(id, UrlFormat, BibiDescText, Lang, User);
        }

        public Range AllowedRange { get; }
    }
}