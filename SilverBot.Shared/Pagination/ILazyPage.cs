using DSharpPlus.Entities;

namespace SilverBot.Shared.Pagination
{
    public interface ILazyPage
    {
        /// <summary>
        /// Plain message content
        /// </summary>
        public Optional<string> Content { get; }

        /// <summary>
        /// Embed message content
        /// </summary>
        public Optional<DiscordEmbedBuilder> Embed { get; }

        /// <summary>
        /// Numerical id of page
        /// </summary>
        public int Id { get; }

        public int? PreviousId { get; }
        public int? NextId { get; }
    }
}