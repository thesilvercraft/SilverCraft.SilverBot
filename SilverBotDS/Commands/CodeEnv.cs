using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using SilverBotDS.Objects;
using SilverBotDS.Utils;

namespace SilverBotDS.Commands
{
    public class CodeEnv
    {
        public CodeEnv(CommandContext context, Config config)
        {
            ctx = context;
            Member = ctx.Member;
            User = ctx.User;
            Guild = ctx.Guild;
            VerString = VersionInfo.VNumber;
            Client = ctx.Client;
            Config = config;
            ExConfig = new Config();
        }

        public CommandContext ctx { get; init; }
        public DiscordMember Member { get; init; }
        public DiscordUser User { get; init; }
        public DiscordGuild Guild { get; init; }
        public DiscordClient Client { get; init; }
        public Config ExConfig { get; init; }
        public Config Config { get; init; }
        public string VerString { get; init; }
        public static readonly string[] Monika = new string[] { "https://static.wikia.nocookie.net/doki-doki-literature-club/images/e/ef/Monika_Illustration.png/revision/latest?cb=20190319051314", "https://static.wikia.nocookie.net/doki-doki-literature-club/images/b/b9/Mon1.png/revision/latest/scale-to-width-down/960?cb=20171209195511", "https://static.wikia.nocookie.net/doki-doki-literature-club/images/8/8d/Mon2.png/revision/latest/scale-to-width-down/960?cb=20171209195516", "https://static.wikia.nocookie.net/doki-doki-literature-club/images/a/ac/Mon3.png/revision/latest/scale-to-width-down/960?cb=20171209195521", "https://static.wikia.nocookie.net/doki-doki-literature-club/images/5/5e/Mon4.png/revision/latest/scale-to-width-down/960?cb=20171209195526", "https://static.wikia.nocookie.net/doki-doki-literature-club/images/d/de/Mon5.png/revision/latest/scale-to-width-down/960?cb=20171209195534", "https://static.wikia.nocookie.net/doki-doki-literature-club/images/5/5f/Mon6.png/revision/latest/scale-to-width-down/960?cb=20171209195540", "https://static.wikia.nocookie.net/doki-doki-literature-club/images/a/af/Mon7.png/revision/latest/scale-to-width-down/960?cb=20171209195545", "https://static.wikia.nocookie.net/doki-doki-literature-club/images/5/5f/Mon6.png/revision/latest?cb=20171209195540", "https://static.wikia.nocookie.net/doki-doki-literature-club/images/7/75/Mon9.png/revision/latest/scale-to-width-down/960?cb=20171209195558", "https://static.wikia.nocookie.net/doki-doki-literature-club/images/d/da/Mon10.png/revision/latest/scale-to-width-down/960?cb=20171209195604", "https://static.wikia.nocookie.net/doki-doki-literature-club/images/5/50/Mon11.png/revision/latest/scale-to-width-down/960?cb=20171209195609", "https://static.wikia.nocookie.net/doki-doki-literature-club/images/f/fe/Mon12.png/revision/latest/scale-to-width-down/960?cb=20171209195614", "https://static.wikia.nocookie.net/doki-doki-literature-club/images/6/65/Mon13.png/revision/latest/scale-to-width-down/960?cb=20171209195620", "https://static.wikia.nocookie.net/doki-doki-literature-club/images/f/f9/3aa.png/revision/latest/scale-to-width-down/960?cb=20171209195706", "https://static.wikia.nocookie.net/doki-doki-literature-club/images/6/6e/3bb.png/revision/latest/scale-to-width-down/960?cb=20171209195716", "https://static.wikia.nocookie.net/doki-doki-literature-club/images/7/77/Interesting.jpg/revision/latest/scale-to-width-down/540?cb=20200729023154", "https://static.wikia.nocookie.net/doki-doki-literature-club/images/7/79/34B6708A-96F2-42C1-AEAE-7B95A7D33985.jpeg/revision/latest/scale-to-width-down/259?cb=20171208161645", "https://static.wikia.nocookie.net/doki-doki-literature-club/images/3/3e/Menu_bg_m.png/revision/latest/scale-to-width-down/1000?cb=20171204054458" };
        public static readonly string[] Waifu = Monika;

        public string GetWaifuPictureLink()
        {
            return StringUtils.RandomFromArray(Monika);
        }

        public readonly bool IsMaxABitch = true;
    }
}