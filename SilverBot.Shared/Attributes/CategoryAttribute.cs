/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
namespace SilverBotDS.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class CategoryAttribute : Attribute
{
    public string[] Category;

    public CategoryAttribute(params string[] thing)
    {
        Category = thing;
    }
}