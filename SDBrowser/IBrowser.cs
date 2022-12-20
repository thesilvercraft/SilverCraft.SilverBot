using System;
using System.IO;
using System.Threading.Tasks;

namespace SDBrowser;
[Obsolete]
public interface IBrowser
{
    Task<Stream> RenderUrlAsync(string url, byte waittime = 0);

    Task<Stream> RenderUrlAsync(Uri url, byte waittime = 0)
    {
        return RenderUrlAsync(url.ToString(), waittime);
    }

    Task<Stream> RenderHtmlAsync(string html, byte waittime = 0);
}
