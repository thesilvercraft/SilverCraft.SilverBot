using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SDBrowser;

public class RemoteBrowser : IBrowser
{
    private readonly HttpClient _client;

    private readonly string UrlOfRemote = "https://pagerendererapi.herokuapp.com/";

    public RemoteBrowser(HttpClient client)
    {
        _client = client;
    }

    public RemoteBrowser()
    {
        _client = new HttpClient();
    }

    public Task<Stream> RenderHtmlAsync(string html, byte waittime = 0)
    {
        throw new NotSupportedException("Remote Browsers do not have support for rendering raw HTML yet");
    }

    public async Task<Stream> RenderUrlAsync(string url, byte waittime = 0)
    {
        return await (await _client.GetAsync(UrlOfRemote + "renderpage?url=" + HttpUtility.UrlEncode(url) +
                                             "&waittime=" + waittime)).Content.ReadAsStreamAsync();
    }
}