using DSharpPlus;
using DSharpPlus.Entities;
using SilverBot.Shared.Objects.Language;

namespace SilverBot.Shared.Pagination
{
    public class BibiPage : ILazyPage
    {
        private string[] BibiDescText;
        private Language Lang;

        public BibiPage(int pageId, string urlFormat, string[] _bibiDescText, Language lang, DiscordUser user)
        {
            BibiDescText = _bibiDescText;
            Id = pageId;
            Lang = lang;
            imgurl = string.Format(urlFormat, Id + 1);
            this.user = user;
        }

        private DiscordUser user;
        private string imgurl;
        public Optional<string> Content { get; }

        public Optional<DiscordEmbedBuilder> Embed => new DiscordEmbedBuilder().WithTitle(BibiDescText[Id])
            .WithDescription($"{imgurl}\n{string.Format(Lang.PageNuget, Id + 1, BibiDescText.Length)}")
            .WithFooter(string.Format(Lang.RequestedBy, user.Username), user.GetAvatarUrl(ImageFormat.Png))
            .WithImageUrl(imgurl);

        public int Id { get; }
        public int? PreviousId => Id - 1;
        public int? NextId => Id + 1;
    }
}