using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Humanizer;
using Lavalink4NET;
using Lavalink4NET.Lyrics;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using SilverBotDS.Converters;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SilverBotDS.Utils;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimeSpanParserUtil;

namespace SilverBotDS.Commands
{
    [RequireGuild]
    internal class Audio : BaseCommandModule
    {
        public LavalinkNode AudioService { private get; set; }
        public LyricsService LyricsService { private get; set; }

        public Config Config { private get; set; }

        public SpotifyClient SpotifyClient { private get; set; }

        private bool IsInVc(CommandContext ctx) => AudioService.HasPlayer(ctx.Guild.Id) && AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id) is not null && (AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id).State != PlayerState.NotConnected
                                                                                                                                                                    || AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id).State != PlayerState.Destroyed);

        private static async Task SendNowPlayingMessage(CommandContext ctx, string title = "", string message = "", string imageurl = "", string url = "", Language language = null)
        {
            language ??= await Language.GetLanguageFromCtxAsync(ctx);
            var embedBuilder = new DiscordEmbedBuilder().WithFooter(language.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto)).WithColor(await ColorUtils.GetSingleAsync());
            var messageBuilder = new DiscordMessageBuilder();
            if (!string.IsNullOrEmpty(message))
            {
                embedBuilder.WithDescription(message);
            }
            if (!string.IsNullOrEmpty(title))
            {
                embedBuilder.WithTitle(title);
            }
            if (!string.IsNullOrEmpty(imageurl))
            {
                embedBuilder.WithImageUrl(imageurl);
            }
            if (!string.IsNullOrEmpty(url))
            {
                embedBuilder.WithUrl(url);
            }
            await messageBuilder
        .WithReply(ctx.Message.Id)
        .WithEmbed(embedBuilder.Build())
        .SendAsync(ctx.Channel);
        }

        public static async Task SendSimpleMessage(CommandContext ctx, string title = "", string message = "", Language language = null)
        {
            language ??= await Language.GetLanguageFromCtxAsync(ctx);
            var embedBuilder = new DiscordEmbedBuilder().WithFooter(language.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto)).WithColor(await ColorUtils.GetSingleAsync());
            var messageBuilder = new DiscordMessageBuilder();
            if (!string.IsNullOrEmpty(message))
            {
                embedBuilder.WithDescription(message);
            }
            if (!string.IsNullOrEmpty(title))
            {
                embedBuilder.WithTitle(title);
            }
            await messageBuilder
        .WithReply(ctx.Message.Id)
        .WithEmbed(embedBuilder.Build())
        .SendAsync(ctx.Channel);
        }

        private TimeSpan TimeTillSongPlays(QueuedLavalinkPlayer player, int song)
        {
            if (player.IsLooping)
            {
                return TimeSpan.MaxValue;
            }
            TimeSpan time;
            if (player.CurrentTrack.IsLiveStream)
            {
                time = TimeSpan.FromHours(2) - player.TrackPosition;
            }
            else
            {
                time = player.CurrentTrack.Duration - player.TrackPosition;
            }

            for (int i = 0; i < song - 1; i++)
            {
                time += player.Queue[i].Duration;
            }
            return time;
        }

        [Command("playnext")]
        [Description("Tell me to play a song next in the queue")]
        [Aliases("pn")]
        public async Task PlayNext(CommandContext ctx, [RemainingText] SongORSongs song)
        {
            Language lang = await Language.GetLanguageFromCtxAsync(ctx);
            BetterVoteLavalinkPlayer player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);

            await player.PlayTopAsync(song.Song);

            await new DiscordMessageBuilder()
        .WithReply(ctx.Message.Id)
        .WithEmbed(new DiscordEmbedBuilder().WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto))
        .WithTitle(string.Format(lang.Enqueued, song.Song.Title + lang.SongByAuthor + song.Song.Author))
        .WithUrl(song.Song.Source)
        .AddField(lang.TimeTillTrackPlays, player.LoopSettings == LoopSettings.LoopingSong ? lang.SongTimeLeftSongLooping : TimeTillSongPlays(player, 1).Humanize(culture: lang.GetCultureInfo()))
        .Build())
        .SendAsync(ctx.Channel);

            if (song.GetRestOfSongs is not null)
            {
                await foreach (var t in song.GetRestOfSongs)
                {
                    await player.PlayTopAsync(t);
                }

                await SendNowPlayingMessage(ctx, string.Format(lang.AddedXAmountOfSongs, await song.GetRestOfSongs.CountAsync()));
            }
        }

        [Command("play")]
        [Description("Tell me to play a song")]
        [Aliases("p")]
        public async Task Play(CommandContext ctx)
        {
            await Resume(ctx);
        }

        [Command("play")]
        public async Task Play(CommandContext ctx, [RemainingText] SongORSongs song)
        {
            Language lang = await Language.GetLanguageFromCtxAsync(ctx);
            BetterVoteLavalinkPlayer player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);

            int pos = await player.PlayAsync(song.Song, true);
            if (pos == 0)
            {
                await SendNowPlayingMessage(ctx, string.Format(lang.NowPlaying, song.Song.Title + lang.SongByAuthor + song.Song.Author), url: song.Song.Source, language: lang);
            }
            else
            {
                await new DiscordMessageBuilder()
            .WithReply(ctx.Message.Id)
            .WithEmbed(new DiscordEmbedBuilder().WithFooter(lang.RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto))
            .WithTitle(string.Format(lang.Enqueued, song.Song.Title + lang.SongByAuthor + song.Song.Author))
            .WithUrl(song.Song.Source)
            .AddField(lang.TimeTillTrackPlays, player.LoopSettings == LoopSettings.LoopingSong ? lang.SongTimeLeftSongLooping : TimeTillSongPlays(player, pos).Humanize(culture: lang.GetCultureInfo()))
            .Build())
            .SendAsync(ctx.Channel);
            }
            if (song.GetRestOfSongs is not null)
            {
                await foreach (var t in song.GetRestOfSongs)
                {
                    await player.PlayAsync(t, true);
                }

                await SendNowPlayingMessage(ctx, string.Format(lang.AddedXAmountOfSongs, await song.GetRestOfSongs.CountAsync()));
            }
        }

        [Command("volume")]
        [Aliases("vol")]
        [Description("Change the volume 1-100%")]
        public async Task Volume(CommandContext ctx, ushort volume)
        {
            Language lang = await Language.GetLanguageFromCtxAsync(ctx);
            if (!IsInVc(ctx))
            {
                await SendSimpleMessage(ctx, lang.NotConnected, language: lang);
                return;
            }
            var channel = ctx.Member?.VoiceState?.Channel;
            if (channel == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected, language: lang);
                return;
            }
            if (volume is < 0 or > 100)
            {
                await SendSimpleMessage(ctx, lang.VolumeNotCorrect, language: lang);
                return;
            }
            BetterVoteLavalinkPlayer player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            await player.SetVolumeAsync(volume / 100f, true);
        }

        [Command("seek")]
        [RequireDJ]
        [Description("Seeks to the specified time")]
        public async Task Seek(CommandContext ctx, string time)
        {
            Language lang = await Language.GetLanguageFromCtxAsync(ctx);
            if (!IsInVc(ctx))
            {
                await SendSimpleMessage(ctx, lang.NotConnected, language: lang);
                return;
            }
            var channel = ctx.Member?.VoiceState?.Channel;
            if (channel == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected, language: lang);
                return;
            }
            var timespan = TimeSpanParser.Parse(time, new TimeSpanParserOptions { ColonedDefault = Units.Minutes, UncolonedDefault = Units.Seconds });
            BetterVoteLavalinkPlayer player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            try
            {
                await player.SeekPositionAsync(timespan);
            }
            catch (NotSupportedException)
            {
                await SendSimpleMessage(ctx, lang.TrackCanNotBeSeeked, language: lang);
            }
        }

        [Command("shuffle")]
        [RequireDJ]
        [Description("Shuffles the queue")]
        public async Task Shuffle(CommandContext ctx)
        {
            Language lang = await Language.GetLanguageFromCtxAsync(ctx);
            if (!IsInVc(ctx))
            {
                await SendSimpleMessage(ctx, lang.NotConnected, language: lang);
                return;
            }
            var channel = ctx.Member?.VoiceState?.Channel;
            if (channel == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected, language: lang);
                return;
            }
            BetterVoteLavalinkPlayer player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            player.Queue.Shuffle();
            await SendSimpleMessage(ctx, lang.ShuffledSuccess, language: lang);
        }

        [Command("remove")]
        [Description("Remove song X from the queue. 0 < index > queue length")]
        public async Task Remove(CommandContext ctx, int songindex)
        {
            Language lang = await Language.GetLanguageFromCtxAsync(ctx);
            if (!IsInVc(ctx))
            {
                await SendSimpleMessage(ctx, lang.NotConnected, language: lang);
                return;
            }
            var channel = ctx.Member?.VoiceState?.Channel;
            if (channel == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected, language: lang);
                return;
            }
            BetterVoteLavalinkPlayer player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (songindex < 0 || songindex > player.Queue.Count)
            {
                await SendSimpleMessage(ctx, lang.SongNotExist, language: lang);
                return;
            }
            var thingy = player.Queue[songindex - 1];
            player.Queue.RemoveAt(songindex - 1);
            await SendSimpleMessage(ctx, lang.RemovedFront + thingy.Title + lang.SongByAuthor + thingy.Author, language: lang);
        }

        [Command("queue")]
        [Description("check whats playing rn and whats next")]
        [Aliases("np", "nowplaying", "q")]
        public async Task Queue(CommandContext ctx)
        {
            Language lang = await Language.GetLanguageFromCtxAsync(ctx);
            if (!IsInVc(ctx))
            {
                await SendSimpleMessage(ctx, lang.NotConnected, language: lang);
                return;
            }
            var channel = ctx.Member?.VoiceState?.Channel;
            if (channel == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected, language: lang);
                return;
            }

            BetterVoteLavalinkPlayer player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (player.Queue.Count == 0 && player.State != PlayerState.Playing)
            {
                await SendSimpleMessage(ctx, lang.NothingInQueue, language: lang);
                return;
            }

            var pages = new List<Page>
                {
                    new Page(embed: new DiscordEmbedBuilder().WithTitle(player.CurrentTrack.Title).WithUrl(player.CurrentTrack.Source).WithColor(await ColorUtils.GetSingleAsync()).WithAuthor(string.Format(lang.PageNuget,1,player.Queue.Count+1)).AddField(lang.SongLength,player.CurrentTrack.Duration.ToString()).AddField(lang.SongTimePosition,player.TrackPosition.ToString()).AddField(lang.SongTimeLeft,player.LoopSettings == LoopSettings.LoopingSong  ? lang.SongTimeLeftSongLoopingCurrent:(player.CurrentTrack.Duration - player.TrackPosition).Humanize()))
                };
            for (var i = 0; i < player.Queue.Count; i++)
            {
                var timetillsongplays = TimeTillSongPlays(player, i + 1);
                pages.Add(new Page(embed: new DiscordEmbedBuilder().WithTitle(player.Queue[i].Title)
                    .WithUrl(player.Queue[i].Source).WithColor(await ColorUtils.GetSingleAsync())
                    .AddField(lang.TimeTillTrackPlays, player.LoopSettings == LoopSettings.LoopingSong ? lang.SongTimeLeftSongLooping : timetillsongplays.Humanize(culture: lang.GetCultureInfo()))
                    .WithAuthor(string.Format(lang.PageNuget, i + 2, player.Queue.Count + 1))));
            }

            await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages, timeoutoverride: new TimeSpan(0, 2, 0));
        }

        [Command("loop")]
        [Description("Loop or unloop the current song")]
        public async Task Loop(CommandContext ctx, LoopSettings settings)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            if (!IsInVc(ctx))
            {
                await SendSimpleMessage(ctx, lang.NotConnected, language: lang);
                return;
            }

            var channel = ctx.Member?.VoiceState?.Channel;
            if (channel == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected, language: lang);
                return;
            }

            BetterVoteLavalinkPlayer player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            player.LoopSettings = settings;
            switch (settings)
            {
                case LoopSettings.NotLooping:
                    await SendSimpleMessage(ctx, lang.NotLooping, language: lang);
                    break;

                case LoopSettings.LoopingSong:
                    await SendSimpleMessage(ctx, lang.LoopingSong, language: lang);
                    break;

                case LoopSettings.LoopingQueue:
                    await SendSimpleMessage(ctx, lang.LoopingQueue, language: lang);
                    break;

                default:
                    break;
            }
        }

        [Command("pause")]
        [Description("pause the current song")]
        public async Task Pause(CommandContext ctx)
        {
            Language lang = await Language.GetLanguageFromCtxAsync(ctx);

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
            BetterVoteLavalinkPlayer player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (player.State != PlayerState.Playing)
            {
                await SendSimpleMessage(ctx, lang.NotPlaying, language: lang);
                return;
            }
            await player.PauseAsync();
        }

        [Command("ovh")]
        [Description("get the lyrics from ovh")]
        public async Task OVH(CommandContext ctx, string name, string artist)
        {
            var lyrics = await LyricsService.GetLyricsAsync(artist, name);
            if (string.IsNullOrEmpty(lyrics))
            {
                await SendSimpleMessage(ctx, "lyrics go null");
                return;
            }
            await SendSimpleMessage(ctx, "Lyrics", $"```{lyrics}```");
        }

        [Command("resume")]
        [Description("resume the current song")]
        public async Task Resume(CommandContext ctx)
        {
            Language lang = await Language.GetLanguageFromCtxAsync(ctx);
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
            BetterVoteLavalinkPlayer player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (player.State != PlayerState.Paused)
            {
                await SendSimpleMessage(ctx, lang.NotPaused, language: lang);
                return;
            }
            await player.ResumeAsync();
        }

        [Command("join")]
        [Description("Tell me to join your channel of the voice type")]
        public async Task Join(CommandContext ctx)
        {
            await StaticJoin(ctx, AudioService);
        }

        public static async Task StaticJoin(CommandContext ctx, LavalinkNode audioService)
        {
            Language lang = await Language.GetLanguageFromCtxAsync(ctx);
            if (audioService.HasPlayer(ctx.Guild.Id) && (audioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id) is not null || audioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id).State is PlayerState.NotConnected or PlayerState.Destroyed))
            {
                await SendSimpleMessage(ctx, lang.AlreadyConnected, language: lang);
                return;
            }
            if ((ctx.Member?.VoiceState?.Channel) == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected, language: lang);
                return;
            }
            await audioService.JoinAsync<BetterVoteLavalinkPlayer>(ctx.Guild.Id, (ctx.Member?.VoiceState?.Channel).Id, true);
            await SendSimpleMessage(ctx, string.Format(lang.Joined, (ctx.Member?.VoiceState?.Channel).Name), language: lang);
        }

        [Command("forceskip")]
        [Description("skip a song. dj only command")]
        [RequireDJ]
        [Aliases("fs")]
        public async Task Skip(CommandContext ctx)
        {
            Language lang = await Language.GetLanguageFromCtxAsync(ctx);
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
            BetterVoteLavalinkPlayer player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);

            if (player.State != PlayerState.Playing)
            {
                await SendSimpleMessage(ctx, lang.NotPlaying, language: lang);
                return;
            }
            var trackbefore = player.CurrentTrack;
            await player.SkipAsync(1);
            var trackafter = player.CurrentTrack;
            await SendSimpleMessage(ctx, string.Format(lang.SkippedNP, trackbefore.Title, trackafter == null ? "Nothing" : trackafter?.Title), language: lang);
        }

        [Command("voteskip")]
        [Description("skip a song")]
        [Aliases("skip")]
        public async Task VoteSkip(CommandContext ctx)
        {
            Language lang = await Language.GetLanguageFromCtxAsync(ctx);
            if (!IsInVc(ctx))
            {
                await SendSimpleMessage(ctx, lang.NotConnected, language: lang);
                return;
            }
            DiscordChannel channel = ctx.Member?.VoiceState?.Channel;
            if (channel == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected, language: lang);
                return;
            }
            BetterVoteLavalinkPlayer player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (player.State != PlayerState.Playing)
            {
                await SendSimpleMessage(ctx, lang.NotPlaying, language: lang);
                return;
            }
            var trackbefore = player.CurrentTrack;
            if (await new RequireDJAttribute().ExecuteCheckAsync(ctx, false))
            {
                await SendSimpleMessage(ctx, lang.CanForceSkip, language: lang);
            }
            var thing = await player.VoteAsync(ctx.Member.Id);
            if (thing.WasSkipped)
            {
                await SendSimpleMessage(ctx, string.Format(lang.SkippedNP, trackbefore.Title, player.CurrentTrack == null ? "Nothing" : player.CurrentTrack.Title), language: lang);
            }
            else if (thing.WasAdded)
            {
                await SendSimpleMessage(ctx, lang.Voted, language: lang);
            }
            else
            {
                await SendSimpleMessage(ctx, lang.AlreadyVoted, language: lang);
            }
        }

        [Command("forcedisconnect")]
        [Description("Tell me to leave your channel of the voice type, without checking if its in a vc")]
        [Aliases("fuckoffisntworking")]
        [RequireDJ]
        public async Task ForceDisconnect(CommandContext ctx)
        {
            Language lang = await Language.GetLanguageFromCtxAsync(ctx);
            var channel = ctx.Member?.VoiceState?.Channel;
            if (channel == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected, language: lang);
                return;
            }
            BetterVoteLavalinkPlayer player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            await player.DisconnectAsync();
            await player.DestroyAsync();
            player.Dispose();
            await SendSimpleMessage(ctx, string.Format(lang.Left, channel.Name), language: lang);
        }

        [Command("disconnect")]
        [Description("Tell me to leave your channel of the voice type")]
        [Aliases("fuckoff", "minecraftbedrockisbetter", "fockoff", "leave")]
        [RequireDJ]
        public async Task Disconnect(CommandContext ctx)
        {
            Language lang = await Language.GetLanguageFromCtxAsync(ctx);
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
            BetterVoteLavalinkPlayer player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            await player.DisconnectAsync();
            await SendSimpleMessage(ctx, string.Format(lang.Left, channel.Name), language: lang);
        }
    }
}