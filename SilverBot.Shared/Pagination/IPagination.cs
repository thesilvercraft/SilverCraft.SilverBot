namespace SilverBot.Shared.Pagination
{
    public interface IPagination
    {
        public int DefaultId { get; }
        public ILazyPage GetPageAtId(int id);
        public Range AllowedRange { get; }
    }
}