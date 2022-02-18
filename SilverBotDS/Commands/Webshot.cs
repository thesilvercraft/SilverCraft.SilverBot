using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SDBrowser;
using SilverBotDS.Attributes;
using SilverBotDS.Objects;
using System.Threading.Tasks;

namespace SilverBotDS.Commands;

[Category("WebShot")]
public class Webshot : BaseCommandModule
{
    public IBrowser Browser { private get; set; }
    public DatabaseContext Database { private get; set; }

    [Command("webshot")]
    [Description("screenshots a webpage")]
    [RequireConfigVariableAttribute("AllowPublicWebshot", true)]
    [RequireGuildDatabaseValueAttribute("WebShot", true, true)]
    public async Task WebshotCommand(CommandContext ctx, string html, byte waittime = 0)
    {
        var bob = new DiscordEmbedBuilder().WithImageUrl("attachment://html.png")
            .WithFooter($"Requested by {ctx.User.Username}", ctx.User.GetAvatarUrl(ImageFormat.Png))
            .WithColor(DiscordColor.Green);
        using var e = await Browser.RenderUrlAsync(html, waittime);
        await new DiscordMessageBuilder().WithEmbed(bob.Build()).WithReply(ctx.Message.Id).WithFile("html.png", e)
            .SendAsync(ctx.Channel);
    }

    [Command("togglewebshot")]
    [RequireUserPermissions(Permissions.ManageGuild)]
    [RequireGuild]
    public async Task Optin(CommandContext ctx, bool toggle)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);

        if (Database.IsBanned(ctx.User.Id) || Database.IsBanned(ctx.Guild.OwnerId))
        {
            var bob = new DiscordEmbedBuilder();
            bob.WithTitle(lang.UserIsBannedFromSilversocial);
            bob.WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            await ctx.RespondAsync(bob.Build());
            return;
        }

        var e = Database.GetServerSettings(ctx.Guild.Id);
        e.WebShot = toggle;
        var b = new DiscordEmbedBuilder();
        b.WithTitle(toggle ? lang.OptedInWebshot : lang.OptedOutWebshot)
            .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
        await ctx.RespondAsync(b.Build());
    }
}