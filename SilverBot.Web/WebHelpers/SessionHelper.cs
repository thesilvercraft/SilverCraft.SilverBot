/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Caching.Memory;
using SilverBot.Shared.Objects.Classes.OAuth;

namespace SilverBot.Web.WebHelpers;

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

    public static OAuthGuild[] GetGuilds(this ClaimsPrincipal user, HttpClient client, IMemoryCache cache)
    {
        if (cache.TryGetValue(user.UID(), out Tuple<OAuthGuild[], DateTime> ch))
        {
            if (DateTime.UtcNow - ch.Item2 > TimeSpan.FromSeconds(10))
            {
                Debug.Assert( ch.Item1!=null);
                Debug.Assert( ch.Item1.Length!=0);
                return ch.Item1;
            }
        }
        //TODO: MIGRATE TO V9
        using var requestMessage =
            new HttpRequestMessage(HttpMethod.Get, "https://discordapp.com/api/v8/users/@me/guilds");
        
        requestMessage.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", user.FindFirstValue("DiscordToken"));
        var response = client.Send(requestMessage);
        StreamReader reader2 = new(response.Content.ReadAsStream());
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            return Array.Empty<OAuthGuild>();
        }
        var guilds = JsonSerializer.Deserialize<OAuthGuild[]>(reader2.ReadToEnd());
        cache.Set(user.UID(), new Tuple<OAuthGuild[], DateTime>(guilds,DateTime.UtcNow));
        Debug.Assert(guilds!=null);
        Debug.Assert(guilds.Length!=0);
        return guilds;
    }

    public static AuthenticationState AuthState(this AuthenticationStateProvider provider)
    {
        return provider.GetAuthenticationStateAsync().GetAwaiter().GetResult();
    }
    public static string Username(this ClaimsPrincipal user)
    {
        return user.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
    }
    public static string Discriminator(this ClaimsPrincipal user)
    {
        return user.FindFirstValue("urn:discord:user:discriminator");
    }
    public static string UID(this ClaimsPrincipal user)
    {
        return user.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
    }
    public static ulong PUID(this ClaimsPrincipal user)
    {
        return ulong.Parse(user.UID());
    }
    public static string AvatarHash(this ClaimsPrincipal user)
    {
        return user.FindFirstValue("urn:discord:avatar:hash");
    }
    public static OAuthUser GetUserInfoFromSession(this ISession session, HttpClient client)
    {
        if (session.TryGetValue("UserCache", out _))
        {
            var (item1, item2) = session.GetObjectFromJson<Tuple<OAuthUser, DateTime>>("UserCache");
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
        var oauth = JsonSerializer.Deserialize<OAuthUser>(reader1.ReadToEnd());
        session.SetObjectAsJson("UserCache", new Tuple<OAuthUser, DateTime>(oauth, DateTime.UtcNow));
        return oauth;
    }

}
