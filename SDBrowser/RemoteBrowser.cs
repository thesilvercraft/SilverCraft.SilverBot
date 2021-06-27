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
        private HttpClient _client;
       
        public RemoteBrowser(HttpClient client)
        {
            _client = client;
           
        }
        public RemoteBrowser()
        {
            _client = new HttpClient();
        
        }

        private string UrlOfRemote = "https://pagerendererapi.herokuapp.com/";
        public Task<Stream> RenderHtmlAsync(string html)
        {
            throw new NotSupportedException("Remote Browsers do not have support for rendering raw HTML yet");
        }

        public async Task<Stream> RenderUrlAsync(string url)
        {
          return await (await _client.GetAsync(UrlOfRemote + "renderpage?url="+ HttpUtility.UrlEncode(url))).Content.ReadAsStreamAsync();
        }

       
    }
}
