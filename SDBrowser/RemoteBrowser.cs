using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SDBrowser
{
    class RemoteBrowser : IBrowser
    {
        string UrlOfRemote = "https://pagerendererapi.herokuapp.com/";
        public Task<Stream> RenderHtmlAsync(string html)
        {
            throw new NotSupportedException("Remote Browsers do not have support for rendering raw HTML yet");
        }

        public Task<Stream> RenderUrlAsync(string url)
        {
            throw new NotImplementedException();
        }

       
    }
}
