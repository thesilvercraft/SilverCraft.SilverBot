using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using static SilverBotDS.Objects.Classes.Oauth;

namespace SilverBotDS.WebHelpers;

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

    public static Guild[] GetGuildsFromSession(this ISession session, HttpClient client)
    {
        if (session.TryGetValue("GuildsCache", out _))
        {
            var (item1, item2) = session.GetObjectFromJson<Tuple<Guild[], DateTime>>("GuildsCache");
            if (DateTime.UtcNow - item2 > TimeSpan.FromSeconds(10))
            {
                return item1;
            }

            Debug.WriteLine(DateTime.UtcNow - item2);
        }
        //TODO: MIGRATE TO V9
        using var requestMessage =
            new HttpRequestMessage(HttpMethod.Get, "https://discordapp.com/api/v8/users/@me/guilds");
        requestMessage.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", GetObjectFromJson<string>(session, "accessToken"));
        var response = client.Send(requestMessage);
        StreamReader reader2 = new(response.Content.ReadAsStream());
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            return Array.Empty<Guild>();
        }
        var guilds = JsonSerializer.Deserialize<Guild[]>(reader2.ReadToEnd());
        session.SetObjectAsJson("GuildsCache", new Tuple<Guild[], DateTime>(guilds, DateTime.UtcNow));
        return guilds;
    }

    public static Oauththingy GetUserInfoFromSession(this ISession session, HttpClient client)
    {
        if (session.TryGetValue("UserCache", out _))
        {
            var (item1, item2) = session.GetObjectFromJson<Tuple<Oauththingy, DateTime>>("UserCache");
            if (DateTime.UtcNow - item2 > TimeSpan.FromSeconds(10))
            {
                return item1;
            }
        }

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://discordapp.com/api/users/@me");
        requestMessage.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", GetObjectFromJson<string>(session, "accessToken"));
        var response = client.Send(requestMessage);
        StreamReader reader1 = new(response.Content.ReadAsStream());
        var oauth = JsonSerializer.Deserialize<Oauththingy>(reader1.ReadToEnd());
        session.SetObjectAsJson("UserCache", new Tuple<Oauththingy, DateTime>(oauth, DateTime.UtcNow));
        return oauth;
    }
}