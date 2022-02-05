using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using SilverBotDS.Attributes;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Classes;
using SilverBotDS.Objects.Database.Classes.ReactionRole;

namespace SilverBotDS.Commands;

[RequireGuild]
[Category("Reaction Roles")]
[Group("rr")]
[RequireUserPermissions(Permissions.Administrator)]
public class ReactionRoleCommands : SilverBotCommandModule
{
    public DatabaseContext dbCtx { private get; set; }

    public override Task<bool> ExecuteRequirements(Config conf)
    {
        return Task.FromResult(conf.ReactionRolesEnabled);
    }

    private static readonly Regex Emote = new("<(a)?:(?<name>.+?):(?<id>.+?)>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    [Command("addmenu")]
    public async Task ReactionRoleAdd(CommandContext ctx)
    {
        var lang = await Language.GetLanguageFromCtxAsync(ctx);
        if (!(ctx.Guild.Permissions?.HasPermission(Permissions.ManageRoles) == true || ctx.Guild.CurrentMember.Roles.MaxBy(x => x.Position)!.CheckPermission(Permissions.ManageRoles) == PermissionLevel.Allowed))
        {
            await ctx.RespondAsync(lang.ReactionRoleNoPermManageRoles);
        }
        var msg = await ctx.RespondAsync(lang.ReactionRoleIntro);

        bool GetFromContent(string content)
        {
            var ctl=content.ToLower().Trim();
            if(lang.ReactionRoleResponseYes.Contains(ctl))
            {
                return true;
            }
            else if(lang.ReactionRoleResponseNo.Contains(ctl))
            {
                return false;
            }
            return false;
        }
        var result = await ctx.Message.GetNextMessageAsync(m =>
        {
            var ctl=m.Trim().ToLower();
            return lang.ReactionRoleResponseYes.Contains(ctl) || lang.ReactionRoleResponseNo.Contains(ctl);
        });

        if (!result.TimedOut)
        {
            bool useembed = GetFromContent(result.Result.Content);
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
                    msg = await msg.DeleteAsync();
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
                        if (ulong.TryParse(splitline[0], out ulong id))
                        {
                            role = ctx.Guild.GetRole(id);
                        }
                        else
                        {
                            role = ctx.Guild.Roles.FirstOrDefault(r => r.Value.Mention == splitline[0]).Value;
                        }
                        DiscordEmoji emoji;
                        var m = Emote.Match(splitline[1]);
                        Console.WriteLine(";" + splitline[1] + ";");
                        if (m.Success)
                        {
                            emoji = DiscordEmoji.FromGuildEmote(ctx.Client, ulong.Parse(m.Groups["id"].Value));
                        }
                        else
                        {
                            emoji = DiscordEmoji.FromUnicode(ctx.Client, splitline[1]);
                        }
                        ReactionRoleType t = ReactionRoleType.Normal;
                        if (splitline.Length == 3)
                        {
                            if (ushort.TryParse(splitline[2], out ushort idoft) && Enum.GetValues<ReactionRoleType>().Select(x => ((ushort)x)).Contains(idoft))
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
                DiscordColor colour = DiscordColor.Red;
                if (!result.TimedOut)
                {                
                    colour = new DiscordColor(SixLabors.ImageSharp.Color.Parse(result.Result.Content).ToHex()[..6]);
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
                dbCtx.ReactionRoleMappings.Add(new ReactionRoleMapping { Emoji = role.Value.Item1.Name, EmojiId = role.Value.Item1.Id, RoleId = role.Key.Id, MessageId = nnmsg.Id, ChannelId = ctx.Channel.Id, MappingId = Guid.NewGuid(), Mode = role.Value.Item2 });
                _ = nnmsg.CreateReactionAsync(role.Value.Item1);
            }
            dbCtx.SaveChanges();
            await msg.DeleteAsync();
        }
    }
}