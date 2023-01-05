/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.ComponentModel.DataAnnotations;

namespace SilverBot.Shared.Objects.Database.Classes.ReactionRole;

public class ReactionRoleMapping
{
    [Key]
    public Guid MappingId { get; set; }
    public virtual Guid ServerSettingsId { get; set; }
    public ulong RoleId { get; set; }

    public ulong MessageId { get; set; }

    public ulong ChannelId { get; set; }
    public string? Emoji { get; set; }
    public ulong? EmojiId { get; set; }
    public ReactionRoleType Mode { get; set; }
}

[Flags]
public enum ReactionRoleType : ushort
{
    None = 0,

    /// <summary>
    /// Remove the role when the user reacts to the emoji and add when unreacting
    /// </summary>
    Inverse = 1 << 1,

    /// <summary>
    /// Add the role when the user reacts to the emoji BUT DO NOT REMOVE WHEN UNREACTING
    /// </summary>
    Sticky = 1 << 2,

    /// <summary>
    /// DO NOT ADD THE ROLE but remove it when the user unreacts
    /// </summary>
    Vanishing = 1 << 3,

    /// <summary>
    /// Add the role when the user reacts to the emoji and remove when unreacting
    /// </summary>
    Normal = 1 << 4
}