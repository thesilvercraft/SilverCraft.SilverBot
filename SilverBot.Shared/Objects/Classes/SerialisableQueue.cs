/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

namespace SilverBot.Shared.Objects.Classes
{
    public struct SilverBotPlaylist : IEquatable<SilverBotPlaylist>
    {
        public string[] Identifiers { get; set; }
        public double CurrentSongTimems { get; set; }
        public string? PlaylistTitle { get; set; }

        public bool Equals(SilverBotPlaylist other)
        {
            return Identifiers.Equals(other.Identifiers) && CurrentSongTimems.Equals(other.CurrentSongTimems) &&
                   PlaylistTitle == other.PlaylistTitle;
        }

        public override bool Equals(object? obj)
        {
            return obj is SilverBotPlaylist other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Identifiers, CurrentSongTimems, PlaylistTitle);
        }
    }
}