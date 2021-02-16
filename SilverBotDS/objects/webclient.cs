using System;
using System.Net.Http;
using System.Text;

namespace SilverBotDS.Objects
{
    public static class WebClient
    {
        private static readonly HttpClient HttpClient = NewhttpClientWithUserAgent();

        private static HttpClient NewhttpClientWithUserAgent()
        {
            var e = new HttpClient();
            e.DefaultRequestHeaders.UserAgent.TryParseAdd(UserAgent());
            Console.Write(UserAgent());
            return e;
        }

        private static string UserAgent()
        {
            return "SilverBot";
        }

        public static HttpClient Get()
        {
            return HttpClient;
        }
    }
}