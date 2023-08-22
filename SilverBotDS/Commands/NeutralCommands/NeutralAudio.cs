/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
#if NoAudio
#else
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Humanizer;
using Lavalink4NET;
using Lavalink4NET.Artwork;
using Lavalink4NET.Lyrics;
using Lavalink4NET.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SilverBot.Shared.Attributes;
using SilverBot.Shared.Objects;
using SilverBot.Shared.Objects.Classes;
using SilverBot.Shared.Objects.Database.Classes;
using SilverBot.Shared.Objects.Language;
using SilverBot.Shared.Utils;
using CategoryAttribute = SilverBot.Shared.Attributes.CategoryAttribute;
using SilverBot.Shared.Exceptions;
using SilverBot.Shared;

namespace SilverBotDS.Commands
{
    public class MusicController
    {
        public BetterVoteLavalinkPlayer Player { get; set; }
        public DiscordGuild Guild { get; set; }
        public DiscordMessage Message { get; set; }
    }

    public class MusicControllerService
    {
        public MusicController GetControllerViaMessage(DiscordMessage message)
        {
            return Controllers[message.Id];
        }

        public async Task<DiscordEmbed> GetEmbedFromController(MusicController controller,
            ArtworkService ArtworkService, Language lang)
        {
            var player = controller.Player;
            var currentTrack = player.CurrentTrack;
            var e = new DiscordEmbedBuilder().WithTitle(currentTrack.Title)
                .WithUrl(currentTrack.Uri?.ToString())
                .AddField(lang.SongLength, player.CurrentTrack.Duration.ToString())
                .AddField(lang.SongTimePosition, player.Position.Position.ToString()).AddField(lang.SongTimeLeft,
                    player.LoopSettings == LoopSettings.LoopingSong
                        ? lang.SongTimeLeftSongLoopingCurrent
                        : (currentTrack.Duration - player.Position.Position).Humanize(
                            culture: lang.GetCultureInfo()));
            var artwrk = await ArtworkService.ResolveAsync(currentTrack);
            if (artwrk != null)
            {
                e.WithThumbnail(artwrk);
            }

            return e.Build();
        }

        public Dictionary<ulong, MusicController> Controllers = new();

        public IEnumerable<DiscordButtonComponent> GetButtons(BetterVoteLavalinkPlayer player)
        {
            switch (player.State)
            {
                case PlayerState.Paused:
                    yield return new DiscordButtonComponent(ButtonStyle.Primary, "MCP" + player.VoiceChannelId, null,
                        false,
                        new DiscordComponentEmoji("▶️"));
                    break;
                case PlayerState.Playing:
                    yield return new DiscordButtonComponent(ButtonStyle.Primary, "MCP" + player.VoiceChannelId, null,
                        false,
                        new DiscordComponentEmoji("⏸️"));
                    break;
            }

            if (player.Queue.Any())
            {
                yield return new DiscordButtonComponent(ButtonStyle.Secondary, "MCR" + player.VoiceChannelId, null,
                    false,
                    new DiscordComponentEmoji("⏭️"));
                yield return new DiscordButtonComponent(ButtonStyle.Danger, "MCF" + player.VoiceChannelId, null, false,
                    new DiscordComponentEmoji("💣"));
            }

            yield return new DiscordButtonComponent(ButtonStyle.Success, "MCZ" + player.VoiceChannelId, null, false,
                new DiscordComponentEmoji("🔄"));
        }

        public async Task SendMessageController(DiscordChannel channel, BetterVoteLavalinkPlayer player,
            ArtworkService artworkService, LanguageService languageService)
        {
            var controller = new MusicController() { Guild = channel.Guild, Player = player };
            controller.Message = await channel.SendMessageAsync(new DiscordMessageBuilder()
                .AddEmbed(await GetEmbedFromController(controller, artworkService,
                    await languageService.GetDefaultAsync())).AddComponents(GetButtons(controller.Player)));
            if (Controllers.ContainsKey((ulong)controller.Player.VoiceChannelId!))
            {
                Controllers[(ulong)controller.Player.VoiceChannelId!] = controller;
            }
            else
            {
                Controllers.Add((ulong)controller.Player.VoiceChannelId!, controller);
            }

            controller.Player.OnChangedState += (sender, data) =>
            {
                Task.Run(async () =>
                {
                    await controller.Message.ModifyAsync(new DiscordMessageBuilder()
                        .AddEmbed(await GetEmbedFromController(controller, artworkService,
                            await languageService.GetDefaultAsync())).AddComponents(GetButtons(controller.Player)));
                });
            };
        }

        public MusicControllerService(DiscordClient discord, ArtworkService artworkService,
            LanguageService languageService)
        {
            discord.ComponentInteractionCreated += async (sender, args) =>
            {
                if (args.Id.StartsWith("MC") && ulong.TryParse(args.Id[3..], out var id) &&
                    Controllers.TryGetValue(id, out var controller))
                {
                    switch (args.Id[2])
                    {
                        case 'P':
                            if (controller.Player.State == PlayerState.Paused)
                            {
                                await controller.Player.ResumeAsync();
                            }
                            else if (controller.Player.State == PlayerState.Playing)
                            {
                                await controller.Player.PauseAsync();
                            }

                            break;
                        case 'R':
                            await controller.Player?.VoteAsync(args.User.Id)!;
                            break;
                        case 'F':
                            if (args.User is DiscordMember m && (m.Roles.Any(e =>
                                                                     e.CheckPermission(Permissions
                                                                         .ManageChannels) ==
                                                                     PermissionLevel.Allowed ||
                                                                     e.Name.Contains("dj",
                                                                         StringComparison.OrdinalIgnoreCase)) ||
                                                                 m.VoiceState?.Channel?.Users?.LongCount(x =>
                                                                     !x.IsBot) == 1))
                            {
                                await controller.Player?.SkipAsync();
                            }

                            break;
                        case 'Z':
                            break;
                    }

                    await args.Interaction.CreateResponseAsync(
                        InteractionResponseType.UpdateMessage,
                        new DiscordInteractionResponseBuilder()
                            .AddEmbed(await GetEmbedFromController(controller, artworkService,
                                await languageService.GetDefaultAsync())).AddComponents(GetButtons(controller.Player)));
                }
            };
        }
    }

    [RequireGuild]
    [Category("Audio")]
    [RequireModuleGuildEnabled(EnabledModules.Audio, false)]
    public class NeutralAudio
    {
        public LavalinkNode AudioService { private get; set; }
        public LyricsService LyricsService { private get; set; }
        public Config Config { private get; set; }
        public ArtworkService ArtworkService { private get; set; }
        public LanguageService LanguageService { private get; set; }
        public ColourService ColourService { private get; set; }
        public MusicControllerService MusicControllerService { private get; set; }

        private bool IsInVc(ISilverBotContext ctx)
        {
            return IsInVc(ctx, AudioService);
        }

        private static bool IsInVc(ISilverBotContext ctx, LavalinkNode lavalinkNode)
        {
            return lavalinkNode.HasPlayer(ctx.Guild.Id) &&
                   lavalinkNode.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id) is not null and not
                       { State: PlayerState.NotConnected } and not { State: PlayerState.Destroyed };
        }

        [Command("musiccontroller")]
        [Description("Controls music playback")]
        public async Task MusicController(ISilverBotContext ctx)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id) ??
                         throw new PlayerIsNullException();
            await MusicControllerService.SendMessageController(ctx.Channel, player, ArtworkService, LanguageService);
        }

        public static TimeSpan TimeTillSongPlays(QueuedLavalinkPlayer player, int song)
        {
            if (player.LoopMode == PlayerLoopMode.Track)
            {
                return TimeSpan.MaxValue;
            }

            var time = TimeSpan.FromHours(20);
            if (player.CurrentTrack is { IsLiveStream: true })
            {
                if (player.Position.Position > TimeSpan.FromHours(20))
                {
                    time = player.Position.Position + TimeSpan.FromHours(20);
                }
                else
                {
                    time = TimeSpan.FromHours(20) - player.Position.Position;
                }
            }
            else
            {
                if (player.CurrentTrack != null)
                {
                    time = player.CurrentTrack.Duration - player.Position.Position;
                }
            }

            for (var i = 0; i < song - 1; i++)
            {
                time += player.Queue[i].Duration;
            }

            return time;
        }

        [Command("playnext")]
        [Description("Tell me to play a song next in the queue")]
        [Aliases("pn")]
        public async Task PlayNext(ISilverBotContext ctx, [RemainingText] SongORSongs song)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (song.Song != null)
            {
                await player?.PlayTopAsync(song.Song)!;
                var dmb = new DiscordEmbedBuilder()
                    .AddRequestedByFooter(ctx, lang)
                    .WithTitle(lang.GetEnqueuedTitle(song.Song.Title, song.Song.Author))
                    .WithUrl(song.Song.Uri?.ToString())
                    .AddField(lang.TimeTillTrackPlays,
                        player.LoopSettings == LoopSettings.LoopingSong
                            ? lang.SongTimeLeftSongLooping
                            : TimeTillSongPlays(player, 1).Humanize(culture: lang.GetCultureInfo()));
                var art = await ArtworkService.ResolveAsync(song.Song);
                if (art != null)
                {
                    dmb.WithThumbnail(art);
                }

                await ctx.RespondAsync(dmb);
            }

            if (song.GetRestOfSongs is not null)
            {
                ulong songCount = 0;
                await foreach (var t in song.GetRestOfSongs)
                {
                    await player?.PlayTopAsync(t)!;
                    songCount++;
                }

                if (songCount != 0)
                {
                    await ctx.SendMessageAsync(string.Format(lang.AddedXAmountOfSongs, songCount));
                }
            }
        }

        [Command("play")]
        [Description("Tell me to play a song")]
        [Aliases("p")]
        public async Task Play(ISilverBotContext ctx)
        {
            if (ctx is TextNeutralCommandContext bsdctx)
            {
                if (bsdctx.Message.Attachments.Count == 1)
                {
                    await Play(ctx,
                        (SongORSongs)await ctx.CommandsNext.ConvertArgument(bsdctx.Message.Attachments[0].Url, bsdctx,
                            typeof(SongORSongs)));
                }
                else if (bsdctx.Message.ReferencedMessage?.Attachments.Count == 1)
                {
                    await Play(ctx,
                        (SongORSongs)await ctx.CommandsNext.ConvertArgument(
                            bsdctx.Message.ReferencedMessage.Attachments[0].Url,
                            bsdctx, typeof(SongORSongs)));
                }
                else
                {
                    await Resume(ctx);
                }
            }
        }

        [Command("play")]
        public async Task Play(ISilverBotContext ctx, [RemainingText] SongORSongs song)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (song.Song != null)
            {
                var pos = await player.PlayAsync(song.Song, true, song.SongStartTime);
                if (pos == 0)
                {
                    var artworkUri = await ArtworkService.ResolveAsync(song.Song);
                    await ctx.SendMessageAsync(
                        lang.GetNowPlaying(song.Song.Title, song.Song.Author),
                        url: song.Song.Uri.ToString(), imageUrl: artworkUri?.ToString(), language: lang);
                }
                else
                {
                    var emb = new DiscordEmbedBuilder()
                        .AddRequestedByFooter(ctx, lang)
                        .WithTitle(lang.GetEnqueuedTitle(song.Song.Title, song.Song.Author))
                        .WithUrl(song.Song.Uri.ToString())
                        .AddField(lang.TimeTillTrackPlays,
                            player.LoopSettings == LoopSettings.LoopingSong
                                ? lang.SongTimeLeftSongLooping
                                : TimeTillSongPlays(player, pos).Humanize(culture: lang.GetCultureInfo()));
                    var artwork = await ArtworkService.ResolveAsync(song.Song);
                    if (artwork != null)
                    {
                        emb.WithThumbnail(artwork);
                    }

                    await ctx.RespondAsync(emb);
                }
            }

            if (song.GetRestOfSongs is not null)
            {
                ulong countofsongs = 0;
                await foreach (var t in song.GetRestOfSongs)
                {
                    await player?.PlayAsync(t, true)!;
                    countofsongs++;
                }

                if (countofsongs != 0)
                {
                    await ctx.SendMessageAsync(string.Format(lang.AddedXAmountOfSongs, countofsongs));
                }
            }
        }

        [Command("volume")]
        [Aliases("vol")]
        [RequireDj]
        [Description("Change the volume 1-100%")]
        public async Task Volume(ISilverBotContext ctx, ushort volume)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            if (!IsInVc(ctx))
            {
                await ctx.SendMessageAsync(lang.NotConnected, language: lang);
                return;
            }

            if (ctx.Member?.VoiceState?.Channel == null)
            {
                await ctx.SendMessageAsync(lang.UserNotConnected, language: lang);
                return;
            }

            if (volume is < 0 or > 100)
            {
                await ctx.SendMessageAsync(lang.VolumeNotCorrect, language: lang);
                return;
            }

            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            await player.SetVolumeAsync(volume / 100f, true);
        }

        [Command("seek")]
        [RequireDj]
        [Description("Seeks to the specified time")]
        public async Task Seek(ISilverBotContext ctx, TimeSpan time)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            try
            {
                await player?.SeekPositionAsync(time)!;
            }
            catch (NotSupportedException)
            {
                await ctx.SendMessageAsync(lang.TrackCanNotBeSeeked, language: lang);
            }
        }

        public async Task MakeSureBotIsInVC(ISilverBotContext ctx, Language lang)
        {
            if (!IsInVc(ctx))
            {
                await ctx.SendMessageAsync(lang.NotConnected, language: lang);
                throw new BotNotInVCException("bot not in voice chat");
            }
        }

        public static async Task MakeSureUserIsInVC(ISilverBotContext ctx, Language lang)
        {
            if (ctx.Member?.VoiceState?.Channel == null)
            {
                await ctx.SendMessageAsync(lang.UserNotConnected, language: lang);
                throw new UserNotInVCException("user not in voice chat");
            }
        }

        public async Task MakeSureBothAreInVC(ISilverBotContext ctx, Language lang)
        {
            await MakeSureBotIsInVC(ctx, lang);
            await MakeSureUserIsInVC(ctx, lang);
        }

        public async Task MakeSurePlayerIsntNull(ISilverBotContext ctx, Language lang, BetterVoteLavalinkPlayer? player)
        {
            if (player == null)
            {
                await ctx.SendMessageAsync(lang.UnknownError, language: lang);
                throw new PlayerIsNullException("player returned null");
            }
        }

        [Command("clearqueue")]
        [RequireDj]
        [Description("Clears the queue")]
        public async Task ClearQueue(ISilverBotContext ctx)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            await MakeSurePlayerIsntNull(ctx, lang, player);
            if (player.Queue.Count == 0 && player.State != PlayerState.NotPlaying)
            {
                await ctx.SendMessageAsync(lang.NothingInQueueToRemove, language: lang);
                return;
            }

            var cnt = player.Queue.Clear();
            await ctx.SendMessageAsync(
                string.Format(lang.RemovedXSongOrSongs, cnt, cnt == 1 ? lang.RemovedSong : lang.RemovedSongs),
                language: lang);
        }

        [Command("shuffle")]
        [RequireDj]
        [Description("Shuffles the queue")]
        public async Task Shuffle(ISilverBotContext ctx)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            player?.Queue.Shuffle();
            await ctx.SendMessageAsync(lang.ShuffledSuccess, language: lang);
        }

        [Command("export")]
        [Description("Export the queue")]
        public async Task ExportQueue(ISilverBotContext ctx, string? playlistName = null)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            var queue = player?.Queue.Select(x => x.Identifier).ToList();
            if (player?.CurrentTrack?.Identifier != null)
            {
                queue?.Insert(0, player?.CurrentTrack?.Identifier);
            }

            if (player != null)
            {
                await ctx.SendStringFileWithContent("", JsonSerializer.Serialize(new SilverBotPlaylist
                {
                    Identifiers = queue?.ToArray() ?? throw new InvalidOperationException(),
                    CurrentSongTimems = player.Position.Position.TotalMilliseconds,
                    PlaylistTitle = playlistName
                }), "queue.json");
            }
        }

        [Command("remove")]
        [Description("Remove song X from the queue. 0 < index > queue length")]
        public async Task Remove(ISilverBotContext ctx, int songindex)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id) ?? throw new PlayerIsNullException();
            if (songindex < 0 || songindex > player.Queue.Count)
            {
                await ctx.SendMessageAsync(lang.SongNotExist, language: lang);
                return;
            }

            var thingy = player.Queue[songindex - 1];
            player.Queue.RemoveAt(songindex - 1);
            await ctx.SendMessageAsync(lang.GetRemovedTitle(thingy.Title, thingy.Author),
                language: lang);
        }

        [Command("queuehistory")]
        [Description("check what was playing")]
        [Aliases("qh", "prevplaying", "pq")]
        public async Task QueueHistory(ISilverBotContext ctx)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id) ??
                         throw new PlayerIsNullException();
            if (player.QueueHistory.Count == 0)
            {
                await ctx.SendMessageAsync(lang.NothingInQueueHistory, language: lang);
                return;
            }

            var pages = new List<Page>();
            var a = 1;
            foreach (var qh in player.QueueHistory)
            {
                pages.Add(new Page(embed: new DiscordEmbedBuilder().WithTitle(qh.Value.Track.Title)
                    .WithUrl(qh.Value.Track.Uri?.ToString()).WithColor(ColourService.GetSingle())
                    .AddField("Time added", Formatter.Timestamp(qh.Value.TimeAdded))
                    .AddField("Times played/skipped", qh.Value.Playbacks.Count.ToString())
                    .WithAuthor(string.Format(lang.PageNuget, a++, player.QueueHistory.Count))));
            }

            var interactivity = ctx.Client.GetInteractivity();
            await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages);
        }

        [Command("queue")]
        [Description("check whats playing rn and whats next")]
        [Aliases("np", "nowplaying", "q")]
        public async Task Queue(ISilverBotContext ctx)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (player != null && player.Queue.Count == 0 && player.State != PlayerState.Playing)
            {
                await ctx.SendMessageAsync(lang.NothingInQueue, language: lang);
                return;
            }

            var e = new DiscordEmbedBuilder().WithTitle(player.CurrentTrack.Title)
                .WithUrl(player.CurrentTrack.Uri?.ToString()).WithColor(ColourService.GetSingle())
                .WithAuthor(string.Format(lang.PageNuget, 1, player.Queue.Count + 1))
                .AddField(lang.SongLength, player.CurrentTrack.Duration.ToString())
                .AddField(lang.SongTimePosition, player.Position.Position.ToString()).AddField(lang.SongTimeLeft,
                    player.LoopSettings == LoopSettings.LoopingSong
                        ? lang.SongTimeLeftSongLoopingCurrent
                        : (player.CurrentTrack.Duration - player.Position.Position).Humanize(
                            culture: lang.GetCultureInfo()));
            var url = await ArtworkService.ResolveAsync(player.CurrentTrack);
            if (url != null)
            {
                e.WithThumbnail(url);
            }

            if (player.LoopTimes > 0)
            {
                e.AddField(lang.TimesTrackLooped, player.LoopTimes + (player.LoopTimes != 1 ? " times" : " time"));
            }

            var pages = new List<Page>
            {
                new(embed: e)
            };
            for (var i = 0; i < player.Queue.Count; i++)
            {
                var timetillsongplays = TimeTillSongPlays(player, i + 1);
                var em = new DiscordEmbedBuilder().WithTitle(player.Queue[i].Title)
                    .WithUrl(player.Queue[i].Uri.ToString()).WithColor(ColourService.GetSingle())
                    .AddField(lang.TimeTillTrackPlays,
                        player.LoopSettings == LoopSettings.LoopingSong
                            ? lang.SongTimeLeftSongLooping
                            : timetillsongplays.Humanize(culture: lang.GetCultureInfo()))
                    .WithAuthor(string.Format(lang.PageNuget, i + 2, player.Queue.Count + 1));
                var artwrk = await ArtworkService.ResolveAsync(player.Queue[i]);
                if (artwrk != null)
                {
                    em.WithThumbnail(artwrk);
                }

                pages.Add(new Page(embed: em));
            }

            var interactivity = ctx.Client.GetInteractivity();
            await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages);
        }

        [Command("loop")]
        [Aliases("repeat")]
        [Description("Loops nothing/the queue/the currently playing song")]
        public async Task Loop(ISilverBotContext ctx,
            [Description("Has to be none, song or queue")] LoopSettings settings)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            player!.LoopSettings = settings;
            await ctx.SendMessageAsync(GetMessageOfLoopSetting(lang, settings), language: lang);
        }

        public string GetMessageOfLoopSetting(Language lang, LoopSettings setting)
        {
            return setting switch
            {
                LoopSettings.LoopingQueue => lang.LoopingQueue,
                LoopSettings.LoopingSong => lang.LoopingSong,
                LoopSettings.NotLooping => lang.NotLooping,
                _ => throw new ArgumentOutOfRangeException(nameof(setting), setting, null)
            };
        }

        [Command("pause")]
        [Description("pause the current song")]
        public async Task Pause(ISilverBotContext ctx)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (player != null && player.State != PlayerState.Playing)
            {
                await ctx.SendMessageAsync(lang.NotPlaying, language: lang);
                return;
            }

            await player?.PauseAsync()!;
        }

        [Command("ovh")]
        [Description("get the lyrics from ovh")]
        public async Task Ovh(ISilverBotContext ctx, string name, string artist)
        {
            var lyrics = await LyricsService.GetLyricsAsync(artist, name);
            if (string.IsNullOrEmpty(lyrics))
            {
                await ctx.SendMessageAsync("Lyrics not found");
                return;
            }

            await ctx.SendMessageAsync("Lyrics", $"```{lyrics}```");
        }

        [Command("songaliases")]
        [Description("Get the hardcoded aliases in silverbots code")]
        public async Task Aliases(ISilverBotContext ctx)
        {
            StringBuilder bob = new();
            foreach (var song in Config.SongAliases)
            {
                bob.Append(Formatter.InlineCode(song.Key)).Append(" - ").AppendLine(Formatter.InlineCode(song.Value));
            }

            await ctx.SendMessageAsync(message: bob.ToString());
        }

        [Command("resume")]
        [Description("resume the current song")]
        public async Task Resume(ISilverBotContext ctx)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (player != null && player.State != PlayerState.Paused)
            {
                await ctx.SendMessageAsync(lang.NotPaused, language: lang);
                return;
            }

            await player?.ResumeAsync()!;
        }

        [Command("join")]
        [Description("to join the voice chat you're in")]
        public async Task Join(ISilverBotContext ctx)
        {
            await StaticJoin(ctx, AudioService);
        }

        public static async Task StaticJoin(ISilverBotContext ctx, LavalinkNode audioService, Language? language = null)
        {
            language ??= await ctx.Services.GetService<LanguageService>().FromCtxAsync(ctx);
            if (IsInVc(ctx, audioService))
            {
                await ctx.SendMessageAsync(language.AlreadyConnected, language: language);
                return;
            }

            await MakeSureUserIsInVC(ctx, language);
            if (ctx.Member?.VoiceState?.Channel != null)
            {
                await audioService.JoinAsync<BetterVoteLavalinkPlayer>(ctx.Guild!.Id,  ctx.Member!.VoiceState!.Channel!.Id, true);
                await ctx.SendMessageAsync(string.Format(language.Joined, ctx.Member?.VoiceState?.Channel?.Name),
                    language: language);
            }
        }

        [Command("forceskip")]
        [Description("skip a song. dj only command")]
        [RequireDj]
        [Aliases("fs")]
        public async Task Skip(ISilverBotContext ctx)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (player != null && player.State != PlayerState.Playing)
            {
                await ctx.SendMessageAsync(lang.NotPlaying, language: lang);
                return;
            }

            var trackbefore = player?.CurrentTrack;
            await player?.SkipAsync()!;
            var trackafter = player.CurrentTrack;
            await ctx.SendMessageAsync(
                string.Format(lang.SkippedNP, trackbefore?.Title,
                    trackafter == null ? lang.QueueNothing : trackafter?.Title),
                language: lang,
                imageUrl: trackafter == null ? null : (await ArtworkService.ResolveAsync(trackafter))?.ToString());
        }

        [Command("voteskip")]
        [Description("skip a song")]
        [Aliases("skip")]
        public async Task VoteSkip(ISilverBotContext ctx)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (player != null && player.State != PlayerState.Playing)
            {
                await ctx.SendMessageAsync(lang.NotPlaying, language: lang);
                return;
            }

            var trackbefore = player?.CurrentTrack;

            if (ctx is TextNeutralCommandContext bcc && await new RequireDjAttribute().ExecuteCheckAsync(bcc, false))
            {
                await ctx.SendMessageAsync(lang.CanForceSkip, language: lang);
            }

            if (ctx is SlashNeutralCommandContext ubcc && await new RequireDjSlashAttribute().ExecuteChecksAsync(ubcc))
            {
                await ctx.SendMessageAsync(lang.CanForceSkip, language: lang);
            }

            if (ctx.Member != null)
            {
                var thing = await player?.VoteAsync(ctx.Member.Id)!;
                if (thing.WasSkipped)
                {
                    await ctx.SendMessageAsync(
                        string.Format(lang.SkippedNP, trackbefore?.Title,
                            player.CurrentTrack == null ? lang.QueueNothing : player.CurrentTrack.Title),
                        imageUrl: (await ArtworkService.ResolveAsync(player.CurrentTrack))?.ToString(), language: lang);
                }
                else if (thing.WasAdded)
                {
                    await ctx.SendMessageAsync(lang.Voted, language: lang);
                }
                else
                {
                    await ctx.SendMessageAsync(lang.AlreadyVoted, language: lang);
                }
            }
        }

        [Command("forcedisconnect")]
        [Description("Tell me to leave your channel of the voice type, without checking if its in a vc")]
        [Aliases("fuckoffisntworking")]
        [RequireDj]
        public async Task ForceDisconnect(ISilverBotContext ctx)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureUserIsInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            if (player is not null)
            {
                await player.DisconnectAsync();
                await player.DestroyAsync();
                await player.DisposeAsync();
            }
        }

        [Command("disconnect")]
        [Description("Tell me to leave your channel of the voice type")]
        [Aliases("fuckoff", "minecraftbedrockisbetter", "fockoff", "leave")]
        [RequireDj]
        public async Task Disconnect(ISilverBotContext ctx)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await MakeSureBothAreInVC(ctx, lang);
            var player = AudioService.GetPlayer<BetterVoteLavalinkPlayer>(ctx.Guild.Id);
            await player?.DisconnectAsync()!;
            await ctx.SendMessageAsync(string.Format(lang.Left, ctx.Member?.VoiceState?.Channel.Name), language: lang);
        }
    }
}
#endif