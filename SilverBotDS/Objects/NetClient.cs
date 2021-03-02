using System.Net.Http;

namespace SilverBotDS.Objects
{
    public static class NetClient
    {
        private static readonly HttpClient HttpClient = NewhttpClientWithUserAgent();

        private static HttpClient NewhttpClientWithUserAgent()
        {
            var e = new HttpClient();
            e.DefaultRequestHeaders.UserAgent.TryParseAdd(UserAgent());
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