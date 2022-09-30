using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Database.Classes.ReactionRole;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SilverBotDS
{
    public static class ReactionRolesHandlers
    {
        public static void AddReactionRolesHandlers(this DiscordClient discord)
        {
            discord.MessageReactionAdded += Discord_MessageReactionAdded;
            discord.MessageReactionRemoved += Discord_MessageReactionRemoved;
        }

        public static void RemoveReactionRolesHandlers(this DiscordClient discord)
        {
            discord.MessageReactionAdded -= Discord_MessageReactionAdded;
            discord.MessageReactionRemoved -= Discord_MessageReactionRemoved;
        }

        private static async Task Discord_MessageReactionAdded(DiscordClient sender, MessageReactionAddEventArgs e)
        {
            if (e.Guild == null || e.User.IsBot)
            {
                return;
            }

            var services = sender.GetExtension<CommandsNextExtension>().Services;
            await using var db = (DatabaseContext)services.GetService(typeof(DatabaseContext));
            var user = await e.Guild.GetMemberAsync(e.User.Id);
            if ((e.Guild.Permissions?.HasPermission(Permissions.ManageRoles) == true || e.Guild.CurrentMember.Roles.MaxBy(x => x.Position)!.CheckPermission(Permissions.ManageRoles) == PermissionLevel.Allowed) &&
                e.Guild.CurrentMember.Roles.MaxBy(x => x.Position)!.Position >
                user.Roles.MaxBy(x => x.Position)!.Position)
            {
                var rrrm = db?.ReactionRoleMappings.FirstOrDefault(a => a.ChannelId == e.Channel.Id && a.MessageId == e.Message.Id && a.EmojiId == e.Emoji.Id && a.Emoji == e.Emoji.Name);
                if (rrrm != null && e.Guild.Roles[rrrm.RoleId] != null)
                {
                    switch (rrrm.Mode)
                    {
                        case ReactionRoleType.Normal:
                        case ReactionRoleType.Sticky:
                            await user.GrantRoleAsync(e.Guild.GetRole(rrrm.RoleId));
                            break;

                        case ReactionRoleType.Inverse:
                            await user.RevokeRoleAsync(e.Guild.GetRole(rrrm.RoleId));
                            break;

                        case ReactionRoleType.Vanishing:
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(rrrm));
                    }
                }
            }
        }

        private static async Task Discord_MessageReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs e)
        {
            if (e.Guild == null || e.User.IsBot)
            {
                return;
            }

            var services = sender.GetExtension<CommandsNextExtension>().Services;
            await using var db = (DatabaseContext)services.GetService(typeof(DatabaseContext));
            var user = await e.Guild.GetMemberAsync(e.User.Id);
            if ((e.Guild.Permissions?.HasPermission(Permissions.ManageRoles) == true || e.Guild.CurrentMember.Roles.MaxBy(x => x.Position)!.CheckPermission(Permissions.ManageRoles) == PermissionLevel.Allowed) &&
                e.Guild.CurrentMember.Roles.MaxBy(x => x.Position)!.Position >
                user.Roles.MaxBy(x => x.Position)!.Position)
            {
                var rrrm = db?.ReactionRoleMappings.FirstOrDefault(a => a.ChannelId == e.Channel.Id && a.MessageId == e.Message.Id && a.EmojiId == e.Emoji.Id && a.Emoji == e.Emoji.Name);
                if (rrrm != null && e.Guild.Roles[rrrm.RoleId] != null)
                {
                    switch (rrrm.Mode)
                    {
                        case ReactionRoleType.Normal:
                        case ReactionRoleType.Vanishing:
                            await user.RevokeRoleAsync(e.Guild.GetRole(rrrm.RoleId));
                            break;

                        case ReactionRoleType.Inverse:
                            await user.GrantRoleAsync(e.Guild.GetRole(rrrm.RoleId));
                            break;

                        case ReactionRoleType.Sticky:
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(rrrm));
                    }
                }
            }
        }
    }
}