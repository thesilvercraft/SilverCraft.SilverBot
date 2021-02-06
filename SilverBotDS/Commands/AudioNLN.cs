using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS
{
    [Group("ll")]
    internal class AudioNLN : BaseCommandModule
    {
        private static readonly Dictionary<ulong, Queue<LavalinkTrack>> queues = new Dictionary<ulong, Queue<LavalinkTrack>>();

        /// <summary>
        /// returns 1 if playing
        /// 0 if stopped
        /// 2 if paused
        /// </summary>

        private static readonly ConcurrentDictionary<ulong, ushort> isplaying = new ConcurrentDictionary<ulong, ushort>();

        /// <summary>
        /// 0 if not looping
        /// 1 if song looping
        /// 2 if queue looping
        /// </summary>
        private static readonly ConcurrentDictionary<ulong, ushort> islooped = new ConcurrentDictionary<ulong, ushort>();

        public static void Set(LavalinkExtension e)
        {
            e.ConnectedNodes.Values.First().PlaybackFinished += Audio_PlaybackFinished;
        }

        public async static Task Audio_PlaybackFinished(LavalinkGuildConnection sender, DSharpPlus.Lavalink.EventArgs.TrackFinishEventArgs e)
        {
            var o = e.Player.Channel.Users.Where(x => x.IsBot == false);

            if (islooped.ContainsKey(e.Player.Guild.Id) && islooped[e.Player.Guild.Id] == 2)
            {
                if (o.ToArray().Length != 0)
                {
                    queues[e.Player.Guild.Id].Enqueue(e.Track);
                    return;
                }
                else
                {
                    await e.Player.DisconnectAsync(); return;
                }
            }
            if (islooped.ContainsKey(e.Player.Guild.Id) && islooped[e.Player.Guild.Id] == 1)
            {
                if (o.ToArray().Length != 0)
                {
                    await e.Player.PlayAsync(e.Track);
                    return;
                }
                else
                {
                    await e.Player.DisconnectAsync();
                    return;
                }
            }
            if (queues[e.Player.Guild.Id].Count == 0)
            {
                await e.Player.DisconnectAsync();
                return;
            }

            if (o.ToArray().Length != 0)
            {
                await e.Player.PlayAsync(queues[e.Player.Guild.Id].Dequeue());
                return;
            }
            else
            {
                await e.Player.DisconnectAsync();
                return;
            }
        }

        [Command]
        public async Task Join(CommandContext ctx, DiscordChannel channel)
        {
            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel.");
                return;
            }

            await node.ConnectAsync(channel);
            await ctx.RespondAsync($"Joined {channel.Name}!");
        }

        [Command]
        public async Task Join(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }
            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();

            if (ctx.Member.VoiceState.Channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel.");
                return;
            }

            await node.ConnectAsync(ctx.Member.VoiceState.Channel);
            await ctx.RespondAsync($"Joined {ctx.Member.VoiceState.Channel.Name}!");
        }

        [Command]
        public async Task Leave(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }
            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();

            if (ctx.Member.VoiceState.Channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel.");
                return;
            }
            var conn = node.GetGuildConnection(ctx.Guild);
            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }
            await conn.DisconnectAsync();
            await ctx.RespondAsync($"Left {ctx.Member.VoiceState.Channel.Name}!");
        }

        [Command]
        public async Task Leave(CommandContext ctx, DiscordChannel channel)
        {
            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel.");
                return;
            }
            var conn = node.GetGuildConnection(channel.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            await conn.DisconnectAsync();
            await ctx.RespondAsync($"Left {channel.Name}!");
        }

        [Command("queue")]
        public async Task queue(CommandContext ctx)
        {
            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Guild);
            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }
            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("There are no tracks loaded.");
                return;
            }
            int o = 1;
            var interactivity = ctx.Client.GetInteractivity();
            List<Page> pages = new List<Page>();
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            b.WithTitle($"Now playing: {conn.CurrentState.CurrentTrack.Title} by {conn.CurrentState.CurrentTrack.Author}");
            b.WithUrl(conn.CurrentState.CurrentTrack.Uri);
            b.WithAuthor($"Page:{o}/{queues[ctx.Guild.Id].Count + 1}");
            if (islooped.ContainsKey(ctx.Guild.Id))
            {
                switch (islooped[ctx.Guild.Id])
                {
                    case 1:
                        {
                            b.AddField("Looping", "Song");
                            break;
                        }
                    case 2:
                        {
                            b.AddField("Looping", "Queue");
                            break;
                        }
                    default:
                        {
                            b.AddField("Looping", "NOPE");
                            break;
                        }
                }
            }
            else
            {
                b.AddField("Looping", "NOPE");
            }

            b.AddField("Position", conn.CurrentState.PlaybackPosition + "/" + conn.CurrentState.CurrentTrack.Length);
            b.WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            pages.Add(new Page(embed: b));
            TimeSpan timetonextsong = -(conn.CurrentState.CurrentTrack.Length - conn.CurrentState.PlaybackPosition);
            o++;
            if (queues[ctx.Guild.Id].Count != 0)
            {
                foreach (LavalinkTrack pog in queues[ctx.Guild.Id])
                {
                    b.WithTitle($"{pog.Title} by {pog.Author}");
                    b.WithUrl(pog.Uri);
                    b.RemoveFieldAt(1);
                    b.AddField("Position", timetonextsong + "/" + pog.Length);
                    b.WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
                    b.WithAuthor($"Page:{o}/{queues[ctx.Guild.Id].Count + 1}");
                    pages.Add(new Page(embed: b));
                    timetonextsong -= pog.Length;
                    o++;
                }
            }

            await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages);
        }

        [Command("skip")]
        public async Task skip(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("There are no tracks loaded.");
                return;
            }
            if (queues[ctx.Guild.Id].Count == 0)
            {
                await ctx.RespondAsync("NOTHIUGT TO DSTKIY POT:arrow_forward:");
                return;
            }

            await conn.StopAsync();
            await conn.PlayAsync(queues[ctx.Guild.Id].Dequeue());
            /*   if (isplaying.ContainsKey(ctx.Guild.Id))
               {
                   isplaying[ctx.Guild.Id] = 1;
               }
               else
               {
                   isplaying.TryAdd(ctx.Guild.Id, 1);
               }*/
        }

        [Command("play")]
        [Aliases("resume", "unpause")]
        public async Task Play(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("There are no tracks loaded.");
                return;
            }
            if (isplaying.ContainsKey(ctx.Guild.Id) && isplaying[ctx.Guild.Id] == 2)
            {
                await ctx.RespondAsync("Already playing :arrow_forward:");
                return;
            }
            await conn.ResumeAsync();
            if (isplaying.ContainsKey(ctx.Guild.Id))
            {
                isplaying[ctx.Guild.Id] = 1;
            }
            else
            {
                isplaying.TryAdd(ctx.Guild.Id, 1);
            }
        }

        [Command("play")]
        public async Task Play(CommandContext ctx, [RemainingText] string search)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
                {
                    await ctx.RespondAsync("You are not in a voice channel.");
                    return;
                }

                if (ctx.Member.VoiceState.Channel.Type != ChannelType.Voice)
                {
                    await ctx.RespondAsync("Not a valid voice channel.");
                    return;
                }

                await node.ConnectAsync(ctx.Member.VoiceState.Channel);
                await ctx.RespondAsync($"Joined {ctx.Member.VoiceState.Channel.Name}!");
                conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            }

            var loadResult = await node.Rest.GetTracksAsync(search);

            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
                || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                loadResult = await node.Rest.GetTracksAsync(new Uri(search));
                if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
               || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
                {
                    await ctx.RespondAsync($"Track search failed for {search}.");
                    return;
                }
            }

            var track = loadResult.Tracks.First();
            if (isplaying.ContainsKey(ctx.Guild.Id))
            {
            PP:
                if (queues.ContainsKey(ctx.Guild.Id))
                {
                    if (loadResult.LoadResultType == LavalinkLoadResultType.PlaylistLoaded)
                    {
                        if (conn.CurrentState.CurrentTrack == null)
                        {
                            await conn.PlayAsync(track);
                            await ctx.RespondAsync($"Now playing {track.Title}!");
                            foreach (LavalinkTrack _lt in loadResult.Tracks)
                            {
                                queues[ctx.Guild.Id].Enqueue(_lt);
                            }
                            queues[ctx.Guild.Id].Dequeue();
                            await ctx.RespondAsync($"Added {loadResult.Tracks.ToList().Count - 1} tracks to the queue!");
                        }
                        else
                        {
                            foreach (LavalinkTrack _lt in loadResult.Tracks)
                            {
                                queues[ctx.Guild.Id].Enqueue(_lt);
                            }
                            await ctx.RespondAsync($"Added {loadResult.Tracks.ToList().Count} tracks to the queue!");
                        }
                    }
                    else
                    {
                        if (conn.CurrentState.CurrentTrack == null)
                        {
                            await conn.PlayAsync(track);
                            await ctx.RespondAsync($"Now playing {track.Title}!");
                        }
                        else
                        {
                            queues[ctx.Guild.Id].Enqueue(track);
                            await ctx.RespondAsync($"Added {track.Title}!");
                        }
                    }
                }
                else
                {
                    queues.Add(ctx.Guild.Id, new Queue<LavalinkTrack>());
                    goto PP;
                }
                return;
            }

            if (isplaying.ContainsKey(ctx.Guild.Id))
            {
                isplaying[ctx.Guild.Id] = 1;
            }
            else
            {
                isplaying.TryAdd(ctx.Guild.Id, 1);
            }
        }

        [Command("loop")]
        public async Task loop(CommandContext ctx, string SongORQueue)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }
            if (SongORQueue.Contains("song"))
            {
                if (islooped.ContainsKey(ctx.Guild.Id))
                {
                    islooped[ctx.Guild.Id] = 1;
                    await ctx.RespondAsync(":repeat_one: Enabled!");
                }
                else
                {
                    islooped.TryAdd(ctx.Guild.Id, 1);
                    await ctx.RespondAsync(":repeat_one: Enabled!");
                }
            }
            if (SongORQueue.Contains("queue"))
            {
                if (islooped.ContainsKey(ctx.Guild.Id))
                {
                    islooped[ctx.Guild.Id] = 2;
                    await ctx.RespondAsync(":repeat: Queue loop enabled");
                }
                else
                {
                    islooped.TryAdd(ctx.Guild.Id, 2);
                    await ctx.RespondAsync(":repeat: Queue loop enabled");
                }
            }
            if (SongORQueue.Contains("none"))
            {
                if (islooped.ContainsKey(ctx.Guild.Id))
                {
                    islooped[ctx.Guild.Id] = 0;
                    await ctx.RespondAsync("Looping Disabled!");
                }
                else
                {
                    islooped.TryAdd(ctx.Guild.Id, 0);
                    await ctx.RespondAsync("Looping Disabled!");
                }
            }
        }

        [Command]
        public async Task Pause(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("There are no tracks loaded.");
                return;
            }
            if (isplaying.ContainsKey(ctx.Guild.Id) && isplaying[ctx.Guild.Id] == 2)
            {
                await ctx.RespondAsync("Already Paused :pause_button:");
                return;
            }
            await conn.PauseAsync();
            if (isplaying.ContainsKey(ctx.Guild.Id))
            {
                isplaying[ctx.Guild.Id] = 2;
            }
            else
            {
                isplaying.TryAdd(ctx.Guild.Id, 2);
            }
        }
    }
}