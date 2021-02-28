using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace SilverBotDS.Commands
{
    public class CodeEnv
    {
        public CodeEnv(CommandContext context)
        {
            ctx = context;
            Member = ctx.Member;
            User = ctx.User;
            Guild = ctx.Guild;
            VerString = VersionInfo.VNumber;
            Client = ctx.Client;
        }

        public CommandContext ctx;
        public DiscordMember Member;
        public DiscordUser User;
        public DiscordGuild Guild;
        public DiscordClient Client;
        public string VerString;
        public string Monika = "https://static.wikia.nocookie.net/doki-doki-literature-club/images/e/ef/Monika_Illustration.png/revision/latest?cb=20190319051314";
        public bool IsMaxABitch=true;
    }
}