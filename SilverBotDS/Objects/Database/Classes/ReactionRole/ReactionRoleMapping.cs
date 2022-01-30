using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SilverBotDS.Objects.Database.Classes.ReactionRole;

public class ReactionRoleMapping
{
    [Key]
    public Guid MappingId { get; set; }

    public ulong RoleId { get; set; }

    public ulong MessageId { get; set; }

    public ulong ChannelId { get; set; }
    public string Emoji { get; set; }
    public ulong? EmojiId { get; set; }
    public ReactionRoleType Mode { get; set; }
}

[Flags]
public enum ReactionRoleType : ushort
{
    None = 0,

    /// <summary>
    /// Add the role when the user reacts to the emoji and remove when unreacting
    /// </summary>
    Normal = 16,

    /// <summary>
    /// Remove the role when the user reacts to the emoji and add when unreacting
    /// </summary>
    Inverse = 2,

    /// <summary>
    /// Add the role when the user reacts to the emoji BUT DO NOT REMOVE WHEN UNREACTING
    /// </summary>
    Sticky = 4,

    /// <summary>
    /// DO NOT ADD THE ROLE but remove it when the user unreacts
    /// </summary>
    Vanishing = 8
}