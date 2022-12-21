using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SDBrowser;

[Obsolete]
public class RemoteBrowser : IBrowser
{
    private readonly HttpClient _client;

    private readonly string _urlOfRemote = "";

    public RemoteBrowser(HttpClient client)
    {
        _client = client;
    }
    public RemoteBrowser(HttpClient client, string Remote) 
    {
        _client = client;
        _urlOfRemote = Remote;
    }
    public RemoteBrowser()
    {
        _client = new HttpClient();
    }
    public RemoteBrowser(string Remote)
    {
        _client = new HttpClient();
        _urlOfRemote = Remote;
    }
    public Task<Stream> RenderHtmlAsync(string html, byte waittime = 0)
    {
        throw new NotSupportedException("Remote Browsers do not have support for rendering raw HTML yet");
    }

    public async Task<Stream> RenderUrlAsync(string url, byte waittime = 0)
    {
        return await (await _client.GetAsync(_urlOfRemote + "renderpage?url=" + HttpUtility.UrlEncode(url) +
                                             "&waittime=" + waittime)).Content.ReadAsStreamAsync();
    }
}
