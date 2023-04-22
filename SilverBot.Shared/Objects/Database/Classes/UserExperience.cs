/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Numerics;

namespace SilverBot.Shared.Objects.Database.Classes
{
    public class UserExperience
    {
        [Key] public ulong Id { get; init; }


        public ulong XP { get; set; }

        public void Increase()
        {
            XP++;
        }

        public void Increase(ulong count)
        {
            XP += count;
        }

        public void Decrease()
        {
            XP--;
        }

        public void Decrease(ulong count)
        {
            XP -= count;
        }
    }
}