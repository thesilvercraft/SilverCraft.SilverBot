using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using SilverBotDS.Objects;

namespace SilverBotDS.Commands;

public class CodeEnv
{
    public static readonly string Monika = "Just Monika.";
    public static readonly string Waifu = Monika;
    public static readonly string L = "л";
    public static readonly string Wbubbler = "вбубблер";
    public static readonly bool IsSilverAWeeb = true;
    public static readonly string DdlcPlus = "hype";

    public CodeEnv(CommandContext context, Config config, DatabaseContext dbctx)
    {
        Ctx = context;
        Member = Ctx.Member;
        User = Ctx.User;
        Guild = Ctx.Guild;
        VerString = VersionInfo.VNumber;
        Client = Ctx.Client;
        Config = config;
        ExConfig = new Config();
        DbContext = dbctx;
    }

#pragma warning disable IDE1006 // Naming Styles
    public CommandContext Ctx { get; init; }
#pragma warning restore IDE1006 // Naming Styles
    public DiscordMember Member { get; init; }
    public DiscordUser User { get; init; }
    public DiscordGuild Guild { get; init; }
    public DiscordClient Client { get; init; }
    public Config ExConfig { get; init; }
    public Config Config { get; init; }
    public string VerString { get; init; }
    public DatabaseContext DbContext { get; init; }
}