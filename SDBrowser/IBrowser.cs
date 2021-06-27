using System;
using System.IO;
using System.Threading.Tasks;

namespace SDBrowser
{
    public interface IBrowser
    {
        Task<Stream> RenderUrlAsync(string url);

        Task<Stream> RenderUrlAsync(Uri url)
        {
            return RenderUrlAsync(url.ToString());
        }

        Task<Stream> RenderHtmlAsync(string html);
    }
}