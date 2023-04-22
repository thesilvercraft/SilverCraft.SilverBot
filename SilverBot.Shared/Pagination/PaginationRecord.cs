using DSharpPlus.Entities;

namespace SilverBot.Shared.Pagination
{
    public class PaginationRecord
    {
        public PaginationRecord(IPagination pagination, int currentPage, DiscordUser user, bool shared)
        {
            Pagination = pagination;
            CurrentPage = currentPage;
            User = user;
            Shared = shared;
        }

        public IPagination Pagination { get; }
        public int CurrentPage { get; set; }
        public DiscordUser User { get; set; }
        public bool Shared { get; set; }
    }
}