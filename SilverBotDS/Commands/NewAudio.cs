using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Lavalink4NET;
using Lavalink4NET.Lyrics;
using Lavalink4NET.Player;
using SilverBotDS.Attributes;
using SilverBotDS.Converters;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SilverBotDS.Utils;
using SnowdPlayer;
using SpotifyAPI.Web;

namespace SilverBotDS.Commands;

[RequireGuild]
[Category("New Audio")]
[Group("a")]
public class NewAudio : SilverBotCommandModule
{
    public SnowService AudioService { private get; set; }
    public LavalinkNode AudioServicee { private get; set; }
    public LyricsService LyricsService { private get; set; }
    public Config Config { private get; set; }

    public SpotifyClient SpotifyClient { private get; set; }

    public override Task<bool> ExecuteRequirements(Config conf)
    {
        return Task.FromResult(conf.UseNewAudio);
    }

    private bool IsInVc(CommandContext ctx)
    {
        return AudioService.HasPlayer(ctx.Guild.Id) &&
               AudioService.GetPlayer<QueuedSnowPlayer>(ctx.Guild.Id) is not null &&
               (AudioService.GetPlayer<QueuedSnowPlayer>(ctx.Guild.Id).State != PlayerState.NotConnected ||
                AudioService.GetPlayer<QueuedSnowPlayer>(ctx.Guild.Id).State != PlayerState.Destroyed);
    }

    public static async Task SendSimpleMessage(CommandContext ctx, string title = "", string message = "",
        Language language = null)
    {
        language ??= await Language.GetLanguageFromCtxAsync(ctx);
        var embedBuilder = new DiscordEmbedBuilder()
            .WithFooter(language.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto))
            .WithColor(await ColorUtils.GetSingleAsync());
        var messageBuilder = new DiscordMessageBuilder();
        if (!string.IsNullOrEmpty(message)) embedBuilder.WithDescription(message);
        if (!string.IsNullOrEmpty(title)) embedBuilder.WithTitle(title);
        await messageBuilder
            .WithReply(ctx.Message.Id)
            .WithEmbed(embedBuilder.Build())
            .SendAsync(ctx.Channel);
    }

    [Command("stop")]
    [Description("stops playing the current song")]
    public async Task Stop(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);

        var channel = ctx.Member?.VoiceState?.Channel;

        if (!IsInVc(ctx))
        {
            await SendSimpleMessage(ctx, lang.NotConnected, language: lang);
            return;
        }

        if (channel == null)
        {
            await SendSimpleMessage(ctx, lang.UserNotConnected, language: lang);
            return;
        }

        var player = AudioService.GetPlayer<QueuedSnowPlayer>(ctx.Guild.Id);

        await player.StopAsync();
    }

    [Command("disconnect")]
    [Description("stops playing the current song and leaves")]
    [RequireDj]
    public async Task Disconnect(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);

        var channel = ctx.Member?.VoiceState?.Channel;

        if (!IsInVc(ctx))
        {
            await SendSimpleMessage(ctx, lang.NotConnected, language: lang);
            return;
        }

        if (channel == null)
        {
            await SendSimpleMessage(ctx, lang.UserNotConnected, language: lang);
            return;
        }

        var player = AudioService.GetPlayer<QueuedSnowPlayer>(ctx.Guild.Id);

        await player.DisconnectAsync();
    }

    [Command("resume")]
    [Description("resume the current song")]
    public async Task Resume(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);

        var channel = ctx.Member?.VoiceState?.Channel;

        if (!IsInVc(ctx))
        {
            await SendSimpleMessage(ctx, lang.NotConnected, language: lang);
            return;
        }

        if (channel == null)
        {
            await SendSimpleMessage(ctx, lang.UserNotConnected, language: lang);
            return;
        }

        var player = AudioService.GetPlayer<QueuedSnowPlayer>(ctx.Guild.Id);
        if (player.State != PlayerState.Paused)
        {
            await SendSimpleMessage(ctx, lang.NotPaused, language: lang);
            return;
        }

        await player.ResumeAsync();
    }

    [Command("pause")]
    [Description("pause the current song")]
    public async Task Pause(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);

        var channel = ctx.Member?.VoiceState?.Channel;

        if (!IsInVc(ctx))
        {
            await SendSimpleMessage(ctx, lang.NotConnected, language: lang);
            return;
        }

        if (channel == null)
        {
            await SendSimpleMessage(ctx, lang.UserNotConnected, language: lang);
            return;
        }

        var player = AudioService.GetPlayer<QueuedSnowPlayer>(ctx.Guild.Id);
        if (player.State != PlayerState.Playing)
        {
            await SendSimpleMessage(ctx, lang.NotPlaying, language: lang);
            return;
        }

        await player.PauseAsync();
    }

    [Command("play")]
    [Description("sponge bob sponge bob squarepants")]
    public async Task Test(CommandContext ctx, string tunez)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);

        if (ctx.Member?.VoiceState?.Channel == null)
        {
            await SendSimpleMessage(ctx, lang.UserNotConnected, language: lang);
            return;
        }

        var pl = AudioService.HasPlayer(ctx.Guild.Id)
            ? AudioService.GetPlayer<QueuedSnowPlayer>(ctx.Guild.Id)
            : await AudioService.JoinAsync<QueuedSnowPlayer>(ctx.Guild.Id, (ctx.Member?.VoiceState?.Channel).Id, true);
        await SendSimpleMessage(ctx, string.Format(lang.Joined, (ctx.Member?.VoiceState?.Channel).Name),
            language: lang);

        await pl.PlayAsync(await AudioService.GetTrackAsync(tunez));
        await SendSimpleMessage(ctx, "e", language: lang);
    }
}