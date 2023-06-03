/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

#nullable enable
#if NoAudio
#else
using Lavalink4NET.Player;

namespace SilverBot.Shared.Objects.Classes
{
    public class SongORSongs
    {
        public SongORSongs(LavalinkTrack song, string? nameofplaylist, IAsyncEnumerable<LavalinkTrack>? songs)
        {
            Song = song;
            NameOfPlayList = nameofplaylist;
            GetRestOfSongs = songs;
        }

        public SongORSongs(LavalinkTrack song, string? nameofplaylist, IAsyncEnumerable<LavalinkTrack>? songs,
            TimeSpan startime)
        {
            Song = song;
            NameOfPlayList = nameofplaylist;
            GetRestOfSongs = songs;
            SongStartTime = startime;
        }

        public LavalinkTrack? Song { get; set; }
        public TimeSpan? SongStartTime { get; set; }
        public string? NameOfPlayList { get; set; }
        public IAsyncEnumerable<LavalinkTrack>? GetRestOfSongs { get; set; }
    }
}
#endif