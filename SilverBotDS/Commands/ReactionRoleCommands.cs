/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Humanizer;
using SilverBotDS.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SilverBot.Shared.Attributes;
using SilverBot.Shared.Objects;
using SilverBot.Shared.Objects.Database;
using SilverBot.Shared.Objects.Database.Classes;
using SilverBot.Shared.Objects.Database.Classes.ReactionRole;
using SilverBot.Shared.Objects.Language;
using CategoryAttribute = SilverBot.Shared.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands;

[RequireGuild]
[Category("Reaction Roles")]
[Group("rr")]
[RequireUserPermissions(Permissions.Administrator)]
[RequireModuleGuildEnabled(EnabledModules.ReactionRole,true)]
public partial class ReactionRoleCommands : BaseCommandModule, IHaveExecutableRequirements
{
    public DatabaseContext DbCtx { private get; set; }

    public Task<bool> ExecuteRequirements(Config conf)
    {
        return Task.FromResult(conf.ReactionRolesEnabled);
    }
    [GeneratedRegex("<(a)?:(?<name>.+?):(?<id>.+?)>", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex Emote();
    public LanguageService LanguageService { private get; set; }

    [Command("addmenu")]
    public async Task ReactionRoleAdd(CommandContext ctx)
    {
        var lang = await LanguageService.FromCtxAsync(ctx);
        var serverSettings = DbCtx.GetServerSettings(ctx.Guild.Id);
        if (!(ctx.Guild.Permissions?.HasPermission(Permissions.ManageRoles) == true || ctx.Guild.CurrentMember.Roles.MaxBy(x => x.Position)!.CheckPermission(Permissions.ManageRoles) == PermissionLevel.Allowed))
        {
            await ctx.RespondAsync(lang.ReactionRoleNoPermManageRoles);
        }
        var msg = await ctx.RespondAsync(lang.ReactionRoleIntro);

        bool GetFromContent(string content, bool def = false)
        {
            var ctl = content.ToLowerInvariant().Trim();
            if (lang.ReactionRoleResponseYes == ctl || lang.ReactionRoleResponseYes2 == ctl || lang.ReactionRoleResponseYes3 == ctl)
            {
                return true;
            }
            if (lang.ReactionRoleResponseNo == ctl || lang.ReactionRoleResponseNo2 == ctl || lang.ReactionRoleResponseNo3 == ctl)
            {
                return false;
            }
            return def;
        }
        var result = await ctx.Message.GetNextMessageAsync(m =>
        {
            var ctl = m.Content.Trim().ToLower();
            return lang.ReactionRoleResponseYes == ctl || lang.ReactionRoleResponseYes2 == ctl || lang.ReactionRoleResponseYes3 == ctl || lang.ReactionRoleResponseNo == ctl || lang.ReactionRoleResponseNo2 == ctl || lang.ReactionRoleResponseNo3 == ctl;
        });

        if (!result.TimedOut)
        {
            var useembed = GetFromContent(result.Result.Content);
            await result.Result.DeleteAsync();
            msg = await msg.ModifyAsync(lang.ReactionRoleTitle);
            result = await ctx.Message.GetNextMessageAsync(m => m.Content.Length > 0);
            var title = result.Result.Content;
            await result.Result.DeleteAsync();
            msg = await msg.ModifyAsync(lang.ReactionRoleMainLoop);
            var nmsg = await ctx.Channel.SendMessageAsync($"{lang.ReactionRoleRolesAdded} {lang.ReactionRoleNone}");
            //get the roles in a loop
            var roles = new Dictionary<DiscordRole, Tuple<DiscordEmoji, ReactionRoleType>>();
            while (true)
            {
                result = await ctx.Message.GetNextMessageAsync(m => m.Content.Length > 0, TimeSpan.FromMinutes(5));
                if (result.TimedOut)
                {
                    await msg.DeleteAsync();
                    return;
                }
                if (string.Equals(result.Result.Content, lang.ReactionRoleDone, StringComparison.OrdinalIgnoreCase))
                {
                    await result.Result.DeleteAsync();
                    break;
                }

                var lines = result.Result.Content.Split('\n');
                try
                {
                    foreach (var line in lines)
                    {
                        var splitLine = line.Split(' ');
                        var role = ulong.TryParse(splitLine[0], out var id) ? ctx.Guild.GetRole(id) : ctx.Guild.Roles.FirstOrDefault(r => r.Value.Mention == splitLine[0]).Value;
                        var m = Emote().Match(splitLine[1]);
                        var emoji = m.Success ? DiscordEmoji.FromGuildEmote(ctx.Client, ulong.Parse(m.Groups["id"].Value)) : DiscordEmoji.FromUnicode(ctx.Client, splitLine[1]);
                        var t = ReactionRoleType.Normal;
                        if (splitLine.Length == 3)
                        {
                            if (ushort.TryParse(splitLine[2], out var idOfType) && Enum.GetValues<ReactionRoleType>().Select(x => (ushort)x).Contains(idOfType))
                            {
                                t = (ReactionRoleType)idOfType;
                            }
                            else if (!Enum.TryParse(splitLine[2], out t))
                            {
                                t = ReactionRoleType.Normal;
                            }
                        }
                        roles.Add(role, new(emoji, t));
                    }
                }
                catch (Exception e)
                {
                    msg = await msg.ModifyAsync(e.Message);
                }
                await result.Result.DeleteAsync();
                StringBuilder rolesAlreadyAddedMsgContent = new($"{lang.ReactionRoleRolesAdded} ({roles.Count}):\n");
                foreach (var role in roles)
                {
                    rolesAlreadyAddedMsgContent.Append(role.Value.Item1).Append(' ').Append(role.Key.Name).Append(' ').AppendLine(role.Value.Item2.Humanize());
                }
                nmsg = await nmsg.ModifyAsync(rolesAlreadyAddedMsgContent.ToString());
            }
            await nmsg.DeleteAsync();
            StringBuilder roleMenuBodyText = new();
            if (useembed)
            {
                foreach (var role in roles)
                {
                    roleMenuBodyText.Append(role.Value.Item1).Append(": ").AppendLine(role.Key.Mention);
                }
            }
            else
            {
                roleMenuBodyText.AppendLine(title);
                foreach (var role in roles)
                {
                    roleMenuBodyText.Append(role.Value.Item1).Append(": `").Append(role.Key.Name).AppendLine("`");
                }
            }
            var mb = new DiscordMessageBuilder();
            if (useembed)
            {
                msg = await msg.ModifyAsync(lang.ReactionRoleEmbedColour);
                result = await ctx.Message.GetNextMessageAsync(m => m.Content.Length > 0, TimeSpan.FromMinutes(2));
                var colour = DiscordColor.Red;
                if (!result.TimedOut)
                {
                    var c = ColorConverter.Convert(result.Result.Content);
                    colour = new DiscordColor(c.Value.R, c.Value.G,c.Value.B);
                }
                mb = mb.WithEmbed(new DiscordEmbedBuilder().WithTitle(title).WithDescription(roleMenuBodyText.ToString())
                    .WithColor(colour));
            }
            else
            {
                mb = mb.WithContent(roleMenuBodyText.ToString());
            }
            var menuMessage = await ctx.Channel.SendMessageAsync(mb);
            
            foreach (var role in roles)
            {
                serverSettings.ReactionRoleMappings.Add(new ReactionRoleMapping { Emoji = role.Value.Item1.Name, EmojiId = role.Value.Item1.Id, RoleId = role.Key.Id, MessageId = menuMessage.Id, ChannelId = ctx.Channel.Id, Mode = role.Value.Item2 });
                _ = menuMessage.CreateReactionAsync(role.Value.Item1);
            }
            await DbCtx.SaveChangesAsync();
            await msg.DeleteAsync();
        }
    }


}