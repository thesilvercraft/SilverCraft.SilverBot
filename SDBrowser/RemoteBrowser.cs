using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SDBrowser
{
    public class RemoteBrowser : IBrowser
    {
        private readonly HttpClient _client;

        public RemoteBrowser(HttpClient client)
        {
            _client = client;
        }

        public RemoteBrowser()
        {
            _client = new HttpClient();
        }

        private readonly string UrlOfRemote = "https://pagerendererapi.herokuapp.com/";

        public Task<Stream> RenderHtmlAsync(string html, byte waittime = 0)
        {
            throw new NotSupportedException("Remote Browsers do not have support for rendering raw HTML yet");
        }

        public async Task<Stream> RenderUrlAsync(string url, byte waittime = 0)
        {
            return await (await _client.GetAsync(UrlOfRemote + "renderpage?url=" + HttpUtility.UrlEncode(url) + "&waittime=" + waittime)).Content.ReadAsStreamAsync();
        }
    }
}