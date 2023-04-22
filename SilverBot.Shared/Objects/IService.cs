/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

namespace SilverBot.Shared.Objects
{
    /// <summary>
    /// A service that does stuff, basically has all of the access to silverbots interfaces while doing something by itself (example would be a health monitor, pixels like archiver)
    /// Services should preferably use their own config stored in a different folder.
    /// </summary>
    public interface IService
    {
        public Task Start();

        public Task Stop();
    }
}