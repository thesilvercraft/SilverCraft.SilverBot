using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using SilverAudioPlayer.Shared;
using HttpClient = System.Net.Http.HttpClient;

namespace SilverCraft.SnowdPlayer
{
    public interface ISnowdPlayerService
    {
        public Task<IPlay> GetPlayer(CommandContext ctx, DiscordChannel channel=null);
    }


    public class SnowdPlayerService :ISnowdPlayerService
    {
        public Dictionary<ulong, AudioPlayer> Players = new();
        private HttpClient _client;
        public SnowdPlayerService(HttpClient client)
        {
            _client = client;
        }
        public async Task<IPlay> GetPlayer(CommandContext ctx, DiscordChannel channel=null)
        {
            if (Players.TryGetValue(ctx.Guild.Id, out var player))
            {
                
                return player;
            }
            channel ??= ctx.Member.VoiceState?.Channel;
           var connection= await channel.ConnectAsync();
           var nplayer = new AudioPlayer(connection, _client);
           Players[ctx.Guild.Id] = nplayer;
           return nplayer;
        }
    }
}