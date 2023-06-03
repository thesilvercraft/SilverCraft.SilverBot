using DSharpPlus.Entities;

namespace SilverBot.Shared.Utils
{
    public static class UserUtils
    {
        public static string GetNickOrUsernameInOldCmd(this DiscordUser user)
        {
            return user is DiscordMember member and not {Nickname: null } ? member.Nickname : user.Username;
        }
    }
}