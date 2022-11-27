/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
namespace SilverBot.SysAdminModule
{
    internal class ScoopPackage : IPackage
    {
        public ScoopPackage(string name, string version)
        {
            Name = name;
            Version = version;
        }
        public ScoopPackage(string name, string version, string newversion):this(name,version)
        {
            NewVersion = newversion;
        }
        public ScoopPackage(string name, string version, string bucket, string date, string time)
        {
            Name = name;
            Version = version;
            Bucket = bucket;
            Date = date;
            Time = time;
        }
        public string Date { get; }
        public string Time { get; }

        public string Name { get; }

        public string Version { get; }
        public string? NewVersion { get; }

        public string Bucket { get; }
        public string? Source => Bucket;

        public string? Description => null;

        public string? FullDescription => throw new NotImplementedException();

    }
}