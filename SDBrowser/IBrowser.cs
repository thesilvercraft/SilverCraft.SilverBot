using System;
using System.IO;
using System.Threading.Tasks;

namespace SDBrowser
{
    public interface IBrowser
    {
        Task<MemoryStream> RenderUrlAsync(string url);

        Task<MemoryStream> RenderUrlAsync(Uri url);

        Task<MemoryStream> RenderHtmlAsync(string html);
    }
}