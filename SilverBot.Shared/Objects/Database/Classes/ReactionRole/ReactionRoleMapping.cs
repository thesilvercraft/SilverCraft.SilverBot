using System.ComponentModel.DataAnnotations;

namespace SilverBotDS.Objects.Database.Classes.ReactionRole;

public class ReactionRoleMapping
{
    [Key]
    public Guid MappingId { get; set; }
    public virtual Guid ServerSettingsId { get; set; }
    public virtual ServerSettings ServerSettings { get; set; }
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