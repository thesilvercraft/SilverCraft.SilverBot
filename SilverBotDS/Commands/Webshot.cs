using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SDBrowser;
using SilverBotDS.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    public class Webshot : BaseCommandModule
    {
        public IBrowser Browser { private get; set; }

        [Command("webshot")]
        [Description("screenshots a webpage")]
        [RequireConfigVariable("AllowPublicWebshot", true)]
        public async Task WebshotCommand(CommandContext ctx, string html)
        {
            var bob = new DiscordEmbedBuilder().WithImageUrl("attachment://html.png").WithFooter($"Requested by {ctx.User.Username}", ctx.User.GetAvatarUrl(ImageFormat.Png)).WithColor(DiscordColor.Green);
            using var e = await Browser.RenderUrlAsync(html);
            await new DiscordMessageBuilder().WithEmbed(bob.Build()).WithReply(ctx.Message.Id).WithFile("html.png", e).SendAsync(ctx.Channel);
        }
    }
}