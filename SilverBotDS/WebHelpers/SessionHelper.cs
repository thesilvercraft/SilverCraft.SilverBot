using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net;
using static SilverBotDS.Objects.Classes.Oauth;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SilverBotDS.WebHelpers
{
    public static class SessionHelper
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

        public static Guild[] GetGuildsFromSession(this ISession session)
        {
            HttpWebRequest webRequest2 = (HttpWebRequest)WebRequest.Create("https://discordapp.com/api/v8/users/@me/guilds");
            webRequest2.Method = "Get";
            webRequest2.ContentLength = 0;
            webRequest2.Headers.Add("Authorization", $"Bearer {GetObjectFromJson<string>(session, "accessToken")}");
            webRequest2.ContentType = "application/x-www-form-urlencoded";
            using (HttpWebResponse response2 = webRequest2.GetResponse() as HttpWebResponse)
            {
                StreamReader reader2 = new(response2.GetResponseStream());
                return JsonSerializer.Deserialize<Guild[]>(reader2.ReadToEnd());
            }
        }

        public static Oauththingy GetUserInfoFromSession(this ISession session)
        {
            HttpWebRequest webRequest1 = (HttpWebRequest)WebRequest.Create("https://discordapp.com/api/users/@me");
            webRequest1.Method = "Get";
            webRequest1.ContentLength = 0;
            webRequest1.Headers.Add("Authorization", $"Bearer {GetObjectFromJson<string>(session, "accessToken")}");
            webRequest1.ContentType = "application/x-www-form-urlencoded";
            using (HttpWebResponse response1 = webRequest1.GetResponse() as HttpWebResponse)
            {
                StreamReader reader1 = new(response1.GetResponseStream());
                return JsonSerializer.Deserialize<Oauththingy>(reader1.ReadToEnd());
            }
        }
    }
}