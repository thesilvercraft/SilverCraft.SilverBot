using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using SilverBotDS.Objects;
using SilverBotDS.Utils;

namespace SilverBotDS.Commands
{
    public class CodeEnv
    {
        public CodeEnv(CommandContext context, Config config, DatabaseContext dbctx)
        {
            ctx = context;
            Member = ctx.Member;
            User = ctx.User;
            Guild = ctx.Guild;
            VerString = VersionInfo.VNumber;
            Client = ctx.Client;
            Config = config;
            ExConfig = new Config();
            DBContext = dbctx;
        }

#pragma warning disable IDE1006 // Naming Styles
        public CommandContext ctx { get; init; }
#pragma warning restore IDE1006 // Naming Styles
        public DiscordMember Member { get; init; }
        public DiscordUser User { get; init; }
        public DiscordGuild Guild { get; init; }
        public DiscordClient Client { get; init; }
        public Config ExConfig { get; init; }
        public Config Config { get; init; }
        public string VerString { get; init; }
        public static readonly string Monika = "Just Monika.";
        public static readonly string Waifu = Monika;
        public static readonly string L = "л";
        public static readonly string Wbubbler = "вбубблер";
        public static readonly bool IsSilverAWeeb = true;
        public static readonly string DDLCPlus = "hype";
        public DatabaseContext DBContext { get; init; }
    }
}