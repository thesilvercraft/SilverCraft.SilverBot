using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Humanizer;
using Lavalink4NET;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using SilverBotDS.Converters;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    [RequireGuild]
    internal class Audio : BaseCommandModule
    {
        private static LavalinkNode audioService;

        public static void SetLavaLinkNode(LavalinkNode _audioService)
        {
            audioService = _audioService;
        }

        private static async Task SendNowPlayingMessage(CommandContext ctx, string title = "", string message = "", string imageurl = "", string url = "")
        {
            var embedBuilder = new DiscordEmbedBuilder().WithFooter(Language.GetLanguageFromCtx(ctx).RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto)).WithColor(await ColorUtils.GetSingleAsync());
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

        private static async Task SendSimpleMessage(CommandContext ctx, string title = "", string message = "")
        {
            var embedBuilder = new DiscordEmbedBuilder().WithFooter(Language.GetLanguageFromCtx(ctx).RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto)).WithColor(await ColorUtils.GetSingleAsync());
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

        [Command("play")]
        [Description("Tell me to play a song")]
        [Aliases("p")]
        public async Task Play(CommandContext ctx, [RemainingText] string song)
        {
            if (string.IsNullOrEmpty(song))
            {
                await Resume(ctx);
                return;
            }
            try
            {
                Language lang = Language.GetLanguageFromCtx(ctx);

                if (!audioService.HasPlayer(ctx.Guild.Id))
                {
                    if (ctx.Member?.VoiceState?.Channel == null)
                    {
                        await SendSimpleMessage(ctx, lang.UserNotConnected);
                        return;
                    }
                    else
                    {
                        await Join(ctx);
                    }
                }
                VoteLavalinkPlayer player = audioService.GetPlayer<VoteLavalinkPlayer>(ctx.Guild.Id);
                if (song.Contains("velimirs") || song.Contains("velimir's"))
                {
                    song = "i got bitches";
                }

                var track = await audioService.GetTrackAsync(song, SearchMode.YouTube);
                if (track is null)
                {
                    await SendSimpleMessage(ctx, string.Format(lang.NoResults, song));
                    return;
                }
                else
                {
                    int pos = await player.PlayAsync(track, true);
                    if (pos == 0)
                    {
                        await SendNowPlayingMessage(ctx, string.Format(lang.NowPlaying, track.Title + lang.SongByAuthor + track.Author), url: track.Source);
                    }
                    else
                    {
                        var embedBuilder = new DiscordEmbedBuilder().WithFooter(Language.GetLanguageFromCtx(ctx).RequestedBy + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Auto)).WithTitle(string.Format(lang.Enqueued, track.Title + lang.SongByAuthor + track.Author)).WithUrl(track.Source).AddField(lang.TimeTillTrackPlays, TimeTillSongPlays(player, pos).Humanize(culture: lang.GetCultureInfo()));
                        var messageBuilder = new DiscordMessageBuilder();

                        await messageBuilder
                    .WithReply(ctx.Message.Id)
                    .WithEmbed(embedBuilder.Build())
                    .SendAsync(ctx.Channel);
                    }
                }
            }
            catch (Exception e)
            {
                Program.SendLog(e);
                throw;
            }
        }

        [Command("volume")]
        [Description("Change the volume 1-100%")]
        public async Task Volume(CommandContext ctx, ushort volume)
        {
            Language lang = Language.GetLanguageFromCtx(ctx);
            if (!audioService.HasPlayer(ctx.Guild.Id))
            {
                await SendSimpleMessage(ctx, lang.NotConnected);
                return;
            }
            var channel = ctx.Member?.VoiceState?.Channel;
            if (channel == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected);
                return;
            }
            if (volume is < 0 or > 100)
            {
                await SendSimpleMessage(ctx, lang.VolumeNotCorrect);
                return;
            }
            VoteLavalinkPlayer player = audioService.GetPlayer<VoteLavalinkPlayer>(ctx.Guild.Id);
            await player.SetVolumeAsync(volume / 100f, true);
        }

        [Command("remove")]
        [Description("Remove song X from the queue. 0 < songindex > 10000000000")]
        public async Task Remove(CommandContext ctx, int songindex)
        {
            Language lang = Language.GetLanguageFromCtx(ctx);
            if (!audioService.HasPlayer(ctx.Guild.Id))
            {
                await SendSimpleMessage(ctx, lang.NotConnected);
                return;
            }
            var channel = ctx.Member?.VoiceState?.Channel;
            if (channel == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected);
                return;
            }
            VoteLavalinkPlayer player = audioService.GetPlayer<VoteLavalinkPlayer>(ctx.Guild.Id);
            if (songindex < 0 || songindex > player.Queue.Count)
            {
                await SendSimpleMessage(ctx, lang.SongNotExist);
                return;
            }
            var thingy = player.Queue[songindex - 1];
            player.Queue.RemoveAt(songindex - 1);
            await SendSimpleMessage(ctx, lang.RemovedFront + thingy.Title + lang.SongByAuthor + thingy.Author);
        }

        [Command("queue")]
        [Description("check whats playing rn and whats next")]
        [Aliases("np", "nowplaying")]
        public async Task Queue(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromCtx(ctx);
            if (!audioService.HasPlayer(ctx.Guild.Id))
            {
                await SendSimpleMessage(ctx, lang.NotConnected);
                return;
            }
            var channel = ctx.Member?.VoiceState?.Channel;
            if (channel == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected);
                return;
            }

            VoteLavalinkPlayer player = audioService.GetPlayer<VoteLavalinkPlayer>(ctx.Guild.Id);
            if (player.Queue.Count == 0 && player.State != PlayerState.Playing)
            {
                await SendSimpleMessage(ctx, lang.NothingInQueue);
                return;
            }

            try
            {
                var pages = new List<Page>
                {
                    new Page(embed: new DiscordEmbedBuilder().WithTitle(player.CurrentTrack.Title).WithUrl(player.CurrentTrack.Source).WithColor(await ColorUtils.GetSingleAsync()).WithAuthor(string.Format(lang.PageNuget,1,player.Queue.Count+1)).AddField(lang.SongLength,player.CurrentTrack.Duration.ToString()).AddField(lang.SongTimePosition,player.TrackPosition.ToString()).AddField(lang.SongTimeLeft,(player.CurrentTrack.Duration - player.TrackPosition).Humanize()))
                };
                for (var i = 0; i < player.Queue.Count; i++)
                {
                    pages.Add(new Page(embed: new DiscordEmbedBuilder().WithTitle(player.Queue[i].Title).WithUrl(player.Queue[i].Source).WithColor(await ColorUtils.GetSingleAsync()).AddField(lang.TimeTillTrackPlays, TimeTillSongPlays(player, i + 1).Humanize(culture: lang.GetCultureInfo())).WithAuthor(string.Format(lang.PageNuget, i + 2, player.Queue.Count + 1))));
                }

                await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages, timeoutoverride: new TimeSpan(0, 2, 0));
            }
            catch (Exception e)
            {
                var bob = new DiscordEmbedBuilder().WithTitle("Something went fucky wucky on my side").WithDescription("Try again a little later?").WithColor(await ColorUtils.GetSingleAsync());
                await ctx.RespondAsync(embed: bob.Build());
                Program.SendLog(e);
                throw;
            }
        }

        [Command("loop")]
        [Description("Loop or unloop the current song")]
        public async Task Loop(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromCtx(ctx);
            if (!audioService.HasPlayer(ctx.Guild.Id))
            {
                await SendSimpleMessage(ctx, lang.NotConnected);
                return;
            }

            var channel = ctx.Member?.VoiceState?.Channel;
            if (channel == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected);
                return;
            }

            VoteLavalinkPlayer player = audioService.GetPlayer<VoteLavalinkPlayer>(ctx.Guild.Id);
            if (player.IsLooping)
            {
                player.IsLooping = false;
                await SendSimpleMessage(ctx, lang.NotLooping);
            }
            else
            {
                player.IsLooping = true;
                await SendSimpleMessage(ctx, lang.LoopingSong);
            }
        }

        [Command("pause")]
        [Description("pause the current song")]
        public async Task Pause(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromCtx(ctx);

            var channel = ctx.Member?.VoiceState?.Channel;

            if (!audioService.HasPlayer(ctx.Guild.Id))
            {
                await SendSimpleMessage(ctx, lang.NotConnected);
                return;
            }
            if (channel == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected);
                return;
            }
            VoteLavalinkPlayer player = audioService.GetPlayer<VoteLavalinkPlayer>(ctx.Guild.Id);
            if (player.State != PlayerState.Playing)
            {
                await SendSimpleMessage(ctx, lang.NotPlaying);
                return;
            }

            await player.PauseAsync();
        }

        [Command("resume")]
        [Description("resume the current song")]
        public async Task Resume(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromCtx(ctx);

            var channel = ctx.Member?.VoiceState?.Channel;

            if (!audioService.HasPlayer(ctx.Guild.Id))
            {
                await SendSimpleMessage(ctx, lang.NotConnected);
                return;
            }
            if (channel == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected);
                return;
            }
            VoteLavalinkPlayer player = audioService.GetPlayer<VoteLavalinkPlayer>(ctx.Guild.Id);
            if (player.State != PlayerState.Paused)
            {
                await SendSimpleMessage(ctx, lang.NotPaused);
                return;
            }

            await player.ResumeAsync();
        }

        [Command("join")]
        [Description("Tell me to join your channel of the voice type")]
        public async Task Join(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromCtx(ctx);
            if (audioService.HasPlayer(ctx.Guild.Id))
            {
                await SendSimpleMessage(ctx, lang.AlreadyConnected);
                return;
            }
            if ((ctx.Member?.VoiceState?.Channel) == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected);
                return;
            }
            await audioService.JoinAsync<VoteLavalinkPlayer>(ctx.Guild.Id, (ctx.Member?.VoiceState?.Channel).Id, true);
            await SendSimpleMessage(ctx, string.Format(lang.Joined, (ctx.Member?.VoiceState?.Channel).Name));
        }

        [Command("forceskip")]
        [Description("skip a song. dj only command")]
        [RequireDJ]
        [Aliases("fs")]
        public async Task Skip(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromCtx(ctx);

            var channel = ctx.Member?.VoiceState?.Channel;

            if (!audioService.HasPlayer(ctx.Guild.Id))
            {
                await SendSimpleMessage(ctx, lang.NotConnected);
                return;
            }
            if (channel == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected);
                return;
            }
            VoteLavalinkPlayer player = audioService.GetPlayer<VoteLavalinkPlayer>(ctx.Guild.Id);
            if (player.State != PlayerState.Playing)
            {
                await SendSimpleMessage(ctx, lang.NotPlaying);
                return;
            }
            var trackbefore = player.CurrentTrack;
            if (player.Queue.Count == 0)
            {
                await SendSimpleMessage(ctx, lang.NothingInQueue);
                return;
            }
            await player.SkipAsync();
            player.ClearVotes();
            var trackafter = player.CurrentTrack;
            await SendSimpleMessage(ctx, string.Format(lang.SkippedNP, trackbefore.Title, trackafter.Title));
        }

        [Command("voteskip")]
        [Description("skip a song")]
        [Aliases("skip")]
        public async Task VoteSkip(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromCtx(ctx);
            if (!audioService.HasPlayer(ctx.Guild.Id))
            {
                await SendSimpleMessage(ctx, lang.NotConnected);
                return;
            }

            DiscordChannel channel = ctx.Member?.VoiceState?.Channel;
            if (channel == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected);
                return;
            }
            VoteLavalinkPlayer player = audioService.GetPlayer<VoteLavalinkPlayer>(ctx.Guild.Id);
            if (player.State != PlayerState.Playing)
            {
                await SendSimpleMessage(ctx, lang.NotPlaying);
                return;
            }
            var trackbefore = player.CurrentTrack;
            if (player.Queue.Count == 0)
            {
                await SendSimpleMessage(ctx, lang.NothingInQueue);
                return;
            }
            if (await new RequireDJAttribute().ExecuteCheckAsync(ctx, false))
            {
                await SendSimpleMessage(ctx, lang.CanForceSkip);
            }
            var thing = await player.VoteAsync(ctx.Member.Id);
            if (thing.WasSkipped)
            {
                await SendSimpleMessage(ctx, string.Format(lang.SkippedNP, trackbefore.Title, player.CurrentTrack.Title));
            }
            else if (thing.WasAdded)
            {
                await SendSimpleMessage(ctx, lang.Voted);
            }
            else
            {
                await SendSimpleMessage(ctx, lang.AlreadyVoted);
            }
        }

        [Command("disconnect")]
        [Description("Tell me to leave your channel of the voice type")]
        [Aliases("fuckoff", "minecraftbedrockisbetter", "fockoff", "leave")]
        [RequireDJ]
        public async Task Disconnect(CommandContext ctx)
        {
            Language lang = Language.GetLanguageFromCtx(ctx);

            var channel = ctx.Member?.VoiceState?.Channel;

            if (!audioService.HasPlayer(ctx.Guild.Id))
            {
                await SendSimpleMessage(ctx, lang.NotConnected);
                return;
            }
            if (channel == null)
            {
                await SendSimpleMessage(ctx, lang.UserNotConnected);
                return;
            }

            VoteLavalinkPlayer player = audioService.GetPlayer<VoteLavalinkPlayer>(ctx.Guild.Id);
            await player.DisconnectAsync();

            await SendSimpleMessage(ctx, string.Format(lang.Left, channel.Name), "Goodbye!👋");
        }
    }
}