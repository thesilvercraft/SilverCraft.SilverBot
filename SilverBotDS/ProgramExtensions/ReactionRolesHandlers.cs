/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using SilverBot.Shared.Objects.Database;
using SilverBot.Shared.Objects.Database.Classes.ReactionRole;

namespace SilverBotDS.ProgramExtensions
{
    public interface IMessageReactionChangeEventArgs
    {
        public DiscordMessage Message { get; }
        public DiscordChannel Channel => Message.Channel;
        public DiscordGuild? Guild => Channel.Guild;
        public DiscordUser User { get; }
        public DiscordEmoji Emoji { get; }
    }

    public class MessageReactionAddedEventArgs : IMessageReactionChangeEventArgs
    {
        private MessageReactionAddEventArgs _args;

        public MessageReactionAddedEventArgs(MessageReactionAddEventArgs args)
        {
            _args = args;
        }

        public DiscordMessage Message => _args.Message;

        public DiscordUser User => _args.User;

        public DiscordEmoji Emoji => _args.Emoji;
    }

    public class MessageReactionRemovedEventArgs : IMessageReactionChangeEventArgs
    {
        private MessageReactionRemoveEventArgs _args;

        public MessageReactionRemovedEventArgs(MessageReactionRemoveEventArgs args)
        {
            _args = args;
        }

        public DiscordMessage Message => _args.Message;


        public DiscordUser User => _args.User;

        public DiscordEmoji Emoji => _args.Emoji;
    }

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
            await ReactionGeneric(sender, new MessageReactionAddedEventArgs(e), async (mapping, user) =>
            {
                switch (mapping.Mode)
                {
                    case ReactionRoleType.Normal or ReactionRoleType.Sticky:
                        await user.GrantRoleAsync(e.Guild.GetRole(mapping.RoleId));
                        break;
                    case ReactionRoleType.Inverse:
                        await user.RevokeRoleAsync(e.Guild.GetRole(mapping.RoleId));
                        break;
                }
            });
        }

        private static async Task ReactionGeneric(DiscordClient sender, IMessageReactionChangeEventArgs e,
            Func<ReactionRoleMapping, DiscordMember, Task> logic)
        {
            if (e.Guild == null || e.User.IsBot)
            {
                return;
            }

            var services = sender.GetExtension<CommandsNextExtension>().Services;
            await using var db = services.GetRequiredService<DatabaseContext>();
            var reactionRoleMapping = db?.ReactionRoleMappings.FirstOrDefault(a =>
                a.ChannelId == e.Channel.Id && a.MessageId == e.Message.Id && a.EmojiId == e.Emoji.Id &&
                a.Emoji == e.Emoji.Name);
            if (reactionRoleMapping != null && e.Guild.Roles[reactionRoleMapping.RoleId] != null)
            {
                var user = await e.Guild.GetMemberAsync(e.User.Id);
                await logic.Invoke(reactionRoleMapping, user);
            }
        }

        private static async Task Discord_MessageReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs e)
        {
            await ReactionGeneric(sender, new MessageReactionRemovedEventArgs(e), async (mapping, user) =>
            {
                switch (mapping.Mode)
                {
                    case ReactionRoleType.Normal or ReactionRoleType.Vanishing:
                        await user.RevokeRoleAsync(e.Guild.GetRole(mapping.RoleId));
                        break;
                    case ReactionRoleType.Inverse:
                        await user.GrantRoleAsync(e.Guild.GetRole(mapping.RoleId));
                        break;
                }
            });
        }
    }
}