using SilverBotDS;

namespace SilverBot.Shared.Pagination;

public class PaginationRecord
{
    public PaginationRecord(IPagination pagination, int currentPage)
    {
        Pagination = pagination;
        CurrentPage = currentPage;
    }

    public IPagination Pagination { get; }
    public int CurrentPage { get; set; }
}