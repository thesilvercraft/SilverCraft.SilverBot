using DSharpPlus.Entities;

namespace SilverBot.Shared.Pagination
{
    public interface IPaginator
    {
        public Task<Guid> SendPaginatedMessage(DiscordChannel channel, DiscordUser user,
            bool allowOtherUsersToStartTheirOwn, IPagination pagination);
    }
}