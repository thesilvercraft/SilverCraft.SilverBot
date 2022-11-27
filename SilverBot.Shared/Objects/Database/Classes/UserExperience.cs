/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Numerics;

namespace SilverBotDS.Objects.Database.Classes;

public class UserExperience
{
    [Key] public ulong Id { get; init; }

    public string XPString
    {
        get => XP.ToString();
        set => XP = BigInteger.Parse(value, CultureInfo.InvariantCulture);
    }

    [NotMapped] public BigInteger XP { get; set; }

    public void Increase()
    {
        try
        {
            XP++;
        }
        catch (OutOfMemoryException)
        {
            //we ran out of memory
        }
    }

    public void Increase(ulong count)
    {
        try
        {
            XP += count;
        }
        catch (OutOfMemoryException)
        {
            //we ran out of memory
        }
    }

    public void Decrease()
    {
        try
        {
            XP--;
        }
        catch (OutOfMemoryException)
        {
            //we ran out of memory
        }
    }

    public void Decrease(ulong count)
    {
        try
        {
            XP -= count;
        }
        catch (OutOfMemoryException)
        {
            //we ran out of memory
        }
    }
}