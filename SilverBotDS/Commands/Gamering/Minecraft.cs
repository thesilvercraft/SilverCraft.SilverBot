using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SilverBotDS.Attributes;
using SilverBotDS.Utils;
using System.Net.Http;
using System.Threading.Tasks;

namespace SilverBotDS.Commands.Gamering
{
    [Category("Gaming")]
    [Group("minecraft")]
    [Aliases("mc")]
    public class MinecraftModule : BaseCommandModule
    {
        //HEY LOIS IM PLAYING MINECRAFT
        public HttpClient HttpClient { private get; set; }

        [Command("getfromusername")]
        [Description("Get a minecraft players UUID from their username")]
        [Aliases("username")]
        public async Task Calculate(CommandContext ctx, string input)
        {
            var player = await MinecraftUtils.GetPlayerAsync(input, HttpClient);
            var b = new DiscordEmbedBuilder();
            b.WithThumbnail(player.GetAvatarUrl())
                  .WithImageUrl(player.GetBodyUrl())
                  .AddField("uuid", "`" + player.Id + "`")
                    .WithColor(await ColorUtils.GetSingleAsync());
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                         .WithEmbed(b.Build())
                                         .SendAsync(ctx.Channel);
        }
    }
}