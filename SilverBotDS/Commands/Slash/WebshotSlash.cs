using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SDBrowser;
using SilverBotDS.Attributes;
using System.Threading.Tasks;

namespace SilverBotDS.Commands.Slash;

public class WebshotSlash : ApplicationCommandModule
{
    public IBrowser Browser { private get; set; }

    [SlashCommand("webshot", "A command to take a screenshot of a website")]
    [RequireGuildDatabaseValueSlashAttribute("WebShot", true, true)]
    public async Task WebShot(InteractionContext ctx, [Option("url", "The url to take a screenshot of")] string url,
        [Choice("10s", 10)]
        [Choice("7s", 7)]
        [Choice("5s", 5)]
        [Choice("Don't wait", 0)]
        [Option("waittime", "How long to wait after it takes a screenshot")]
        long waittime = 0)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        var bob = new DiscordEmbedBuilder().WithImageUrl("attachment://html.png")
            .WithFooter($"Requested by {ctx.User.Username}", ctx.User.GetAvatarUrl(ImageFormat.Png))
            .WithColor(DiscordColor.Green);
        using var e = await Browser.RenderUrlAsync(url, (byte)waittime);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddFile("html.png", e).AddEmbed(bob.Build()));
    }
}