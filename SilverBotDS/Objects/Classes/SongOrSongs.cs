using DSharpPlus.Entities;
using SilverBotDS.Utils;
using System.Collections.Generic;
using Lavalink4NET.Player;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace SilverBotDS.Objects
{
    public class SongORSongs
    {
        public SongORSongs(LavalinkTrack song, string nameofplaylist, IAsyncEnumerable<LavalinkTrack>? songs)
        {
            Song = song;
            NameOfPlayList = nameofplaylist;
            GetRestOfSongs = songs;
        }

        public LavalinkTrack Song { get; set; }
        public string NameOfPlayList { get; set; }
        public IAsyncEnumerable<LavalinkTrack>? GetRestOfSongs { get; set; }
    }
}