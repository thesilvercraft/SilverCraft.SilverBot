using System.Net.Http;

namespace SilverBotDS
{
    public static class Webclient
    {
        private static readonly HttpClient HttpClient = NewhttpClientwithstrign();

        public static HttpClient NewhttpClientwithstrign()
        {
            HttpClient e = new HttpClient();
            e.DefaultRequestHeaders.UserAgent.TryParseAdd($"SilverBotDS");
            return e;
        }

        public static HttpClient Get()
        {
            return HttpClient;
        }
    }
}