using Lavalink4NET.Player;

#nullable enable

namespace SilverBotDS.Objects;

public class SongORSongs
{
    public SongORSongs(LavalinkTrack song, string nameofplaylist, IAsyncEnumerable<LavalinkTrack>? songs)
    {
        Song = song;
        NameOfPlayList = nameofplaylist;
        GetRestOfSongs = songs;
    }

    public SongORSongs(LavalinkTrack song, string nameofplaylist, IAsyncEnumerable<LavalinkTrack>? songs,
        TimeSpan startime)
    {
        Song = song;
        NameOfPlayList = nameofplaylist;
        GetRestOfSongs = songs;
        SongStartTime = startime;
    }

    public LavalinkTrack? Song { get; set; }
    public TimeSpan? SongStartTime { get; set; }
    public string NameOfPlayList { get; set; }
    public IAsyncEnumerable<LavalinkTrack>? GetRestOfSongs { get; set; }
}