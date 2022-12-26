﻿/* This file is part of SilverBot.
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
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SilverBotDS.Objects.Database.Classes.ReactionRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SilverBotDS.Attributes;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands;

[RequireGuild]
[Category("Reaction Roles")]
[Group("rr")]
[RequireUserPermissions(Permissions.Administrator)]
[RequireModuleGuildEnabled(EnabledModules.ReactionRole,true)]
public class ReactionRoleCommands : BaseCommandModule, IHaveExecutableRequirements
{
    public DatabaseContext DbCtx { private get; set; }

    public Task<bool> ExecuteRequirements(Config conf)
    {
        return Task.FromResult(conf.ReactionRolesEnabled);
    }

    private static readonly Regex Emote = new("<(a)?:(?<name>.+?):(?<id>.+?)>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
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

        bool GetFromContent(string content)
        {
            var ctl = content.ToLowerInvariant().Trim();
            if (lang.ReactionRoleResponseYes == ctl || lang.ReactionRoleResponseYes2 == ctl || lang.ReactionRoleResponseYes3 == ctl)
            {
                return true;
            }
            else if (lang.ReactionRoleResponseNo == ctl || lang.ReactionRoleResponseNo2 == ctl || lang.ReactionRoleResponseNo3 == ctl)
            {
                return false;
            }
            return false;
        }
        var result = await ctx.Message.GetNextMessageAsync(m =>
        {
            var ctl = m.Content.Trim().ToLower();
            return lang.ReactionRoleResponseYes == (ctl) || lang.ReactionRoleResponseYes2 == (ctl) || lang.ReactionRoleResponseYes3 == (ctl) || lang.ReactionRoleResponseNo == (ctl) || lang.ReactionRoleResponseNo2 == (ctl) || lang.ReactionRoleResponseNo3 == (ctl);
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
                        var splitline = line.Split(' ');
                        DiscordRole role;
                        if (ulong.TryParse(splitline[0], out var id))
                        {
                            role = ctx.Guild.GetRole(id);
                        }
                        else
                        {
                            role = ctx.Guild.Roles.FirstOrDefault(r => r.Value.Mention == splitline[0]).Value;
                        }
                        DiscordEmoji emoji;
                        var m = Emote.Match(splitline[1]);
                        emoji = m.Success ? DiscordEmoji.FromGuildEmote(ctx.Client, ulong.Parse(m.Groups["id"].Value)) : DiscordEmoji.FromUnicode(ctx.Client, splitline[1]);
                        var t = ReactionRoleType.Normal;
                        if (splitline.Length == 3)
                        {
                            if (ushort.TryParse(splitline[2], out var idoft) && Enum.GetValues<ReactionRoleType>().Select(x => ((ushort)x)).Contains(idoft))
                            {
                                t = (ReactionRoleType)idoft;
                            }
                            else if (!Enum.TryParse(splitline[2], out t))
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
                StringBuilder asasasasas = new($"{lang.ReactionRoleRolesAdded} ({roles.Count}):\n");
                foreach (var role in roles)
                {
                    asasasasas.Append(role.Value.Item1).Append(' ').Append(role.Key.Name).Append(' ').AppendLine(role.Value.Item2.Humanize());
                }

                nmsg = await nmsg.ModifyAsync(asasasasas.ToString());
            }
            await nmsg.DeleteAsync();
            StringBuilder asasadsadsasas = new();
            if (useembed)
            {
                foreach (var role in roles)
                {
                    asasadsadsasas.Append(role.Value.Item1).Append(": ").AppendLine(role.Key.Mention);
                }
            }
            else
            {
                asasadsadsasas.AppendLine(title);
                foreach (var role in roles)
                {
                    asasadsadsasas.Append(role.Value.Item1).Append(": `").Append(role.Key.Name).AppendLine("`");
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
                mb = mb.WithEmbed(new DiscordEmbedBuilder().WithTitle(title).WithDescription(asasadsadsasas.ToString())
                    .WithColor(colour));
            }
            else
            {
                mb = mb.WithContent(asasadsadsasas.ToString());
            }
            var nnmsg = await ctx.Channel.SendMessageAsync(mb);

            foreach (var role in roles)
            {
                serverSettings.ReactionRoleMappings.Add(new ReactionRoleMapping { Emoji = role.Value.Item1.Name, EmojiId = role.Value.Item1.Id, RoleId = role.Key.Id, MessageId = nnmsg.Id, ChannelId = ctx.Channel.Id, Mode = role.Value.Item2 });
                _ = nnmsg.CreateReactionAsync(role.Value.Item1);
            }
            DbCtx.SaveChanges();
            await msg.DeleteAsync();
        }
    }
}