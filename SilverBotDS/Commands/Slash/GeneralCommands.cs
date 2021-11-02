using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS.Commands.Slash
{
    public class GeneralCommands : ApplicationCommandModule
    {
        public DatabaseContext DBCTX { private get; set; }
        public Config CNF { private get; set; }

        [SlashCommand("hello", "A simple hello command")]
        public async Task TestCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"hello {ctx.User.Mention}"));
        }

        public async Task WhoIsTask(BaseContext ctx, DiscordUser user)
        {
            var lang = await Language.GetLanguageFromGuildIdAsync(ctx.Guild.Id, DBCTX);
            var cultureinfo = lang.GetCultureInfo();
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder()
                .WithTitle(lang.User + user.Username)
                .WithDescription(lang.InformationAbout + user.Mention)
                .AddField(lang.Userid, Formatter.InlineCode(user.Id.ToString()), true)
                .AddField(lang.JoinedSilverCraft, StringUtils.BoolToEmoteString(await Genericcommands.IsAtSilverCraftAsync(ctx.Client, user, CNF)), true)
                .AddField(lang.IsAnOwner, StringUtils.BoolToEmoteString(ctx.Client.CurrentApplication.Owners.Contains(user)), true)
                .AddField(lang.IsABot, StringUtils.BoolToEmoteString(user.IsBot), true)
                .AddField(lang.AccountCreationDate, user.CreationTimestamp.ToString(cultureinfo), true)
                .AddField(lang.AccountJoinDate, ctx.Guild?.Members.ContainsKey(user.Id) == true ? ctx.Guild?.Members[user.Id].JoinedAt.ToString(cultureinfo) : "NA", true)
                .WithColor(await ColorUtils.GetSingleAsync())
                .WithThumbnail(user.GetAvatarUrl(ImageFormat.Png))
                .WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
                .Build()).AsEphemeral(true));
        }

        [SlashCommand("whois", "Find out the info silverbot knows about someone.")]
        public Task WhoIsCommand(InteractionContext ctx, [Option("user", "User to view info on")] DiscordUser user)
        {
            return WhoIsTask(ctx, user);
        }

        [ContextMenu(ApplicationCommandType.UserContextMenu, "Whois-like search")]
        public Task UserMenu(ContextMenuContext ctx)
        {
            return WhoIsTask(ctx, ctx.TargetMember);
        }

        [SlashCommand("version", "Find out the version info for this instance of silverbot")]
        public async Task VersionInfoCommand(InteractionContext ctx)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder()
                .WithTitle(lang.VersionInfoTitle)
                .AddField(lang.VersionNumber, Formatter.InlineCode(VersionInfo.VNumber))
                .AddField(lang.GitRepo, ThisAssembly.Git.RepositoryUrl)
                .AddField(lang.GitCommitHash, Formatter.InlineCode(ThisAssembly.Git.Commit))
                .AddField(lang.GitBranch, Formatter.InlineCode(ThisAssembly.Git.Branch))
                .AddField(lang.IsDirty, StringUtils.BoolToEmoteString(ThisAssembly.Git.IsDirty))
                .AddField(lang.CLR, Formatter.InlineCode(RuntimeInformation.FrameworkDescription))
                .AddField(lang.OS, Formatter.InlineCode(Environment.OSVersion.VersionString))
                .AddField(lang.DsharpplusVersion, Formatter.InlineCode(ctx.Client.VersionString))
                .WithAuthor($"{ctx.Client.CurrentUser.Username}#{ctx.Client.CurrentUser.Discriminator}", iconUrl: ctx.Client.CurrentUser.GetAvatarUrl(ImageFormat.Auto))
                .WithColor(await ColorUtils.GetSingleAsync())
                .Build()).AsEphemeral(true));
        }

        [SlashCommand("dukthosting", "SilverHosting:tm: best")]
        public async Task DuktHostingCommand(InteractionContext ctx)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder()
                .WithTitle(lang.SilverhostingJokeTitle)
                .WithDescription(lang.SilverhostingJokeDescription)
                .WithAuthor($"{ctx.Client.CurrentUser.Username}#{ctx.Client.CurrentUser.Discriminator}", iconUrl: ctx.Client.CurrentUser.GetAvatarUrl(ImageFormat.Auto))
                .WithColor(await ColorUtils.GetSingleAsync())
                .Build()));
        }

        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Dump message")]
        public async Task DumpCommand(ContextMenuContext ctx)
        {
            await using var outStream = new MemoryStream(Encoding.UTF8.GetBytes(ctx.TargetMessage.Content))
            {
                Position = 0
            };
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddFile("message.txt", outStream));
        }
    }
}