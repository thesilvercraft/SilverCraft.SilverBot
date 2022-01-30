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
        if (!(ctx.Guild.Permissions?.HasPermission(Permissions.ManageRoles) == true || ctx.Guild.CurrentMember.Roles.MaxBy(x => x.Position)!.CheckPermission(Permissions.ManageRoles) == PermissionLevel.Allowed))
        {
            await ctx.RespondAsync("pls give perms to manage roles or feature will not work");
        }
        var msg = await ctx.RespondAsync("Welcome to the text guided reaction role menu creator thingy. Do you want the menu to be an embed? (**y**es/**n**o)");

        bool GetFromContent(string content)
        {
            return content.ToLower() switch
            {
                "y" => true,
                "yes" => true,
                "n" => true,
                "no" => true,
                _ => false
            };
        }
        var result = await ctx.Message.GetNextMessageAsync(m =>
        {
            return m.Content.ToLower() switch
            {
                "y" => true,
                "yes" => true,
                "n" => true,
                "no" => true,
                _ => false
            };
        });

        if (!result.TimedOut)
        {
            bool useembed = GetFromContent(result.Result.Content);
            await result.Result.DeleteAsync();
            msg = await msg.ModifyAsync("ok now what do you want the title to be? (eg. `***Role Menu***`)");
            result = await ctx.Message.GetNextMessageAsync(m => m.Content.Length > 0);
            var title = result.Result.Content;
            await result.Result.DeleteAsync();
            msg = await msg.ModifyAsync("what roles do you want to add to the menu? ping the role or add its id first, add a space and add an emoji and optionally hit space again and send a type of reaction role by its name or number ```\nAdd the role when the user reacts to the emoji and remove when unreacting | Normal | 16 \nRemove the role when the user reacts to the emoji and add when unreacting | Inverse | 2\nAdd the role when the user reacts to the emoji BUT DO NOT REMOVE WHEN UNREACTING | Sticky | 4\nDO NOT ADD THE ROLE but remove it when the user unreacts | Vanishing | 8\n``` send many messages similar to this\n```\n@role 🥲 Sticky\n@anotherrole 😳\n``` (do not put things in code blocks ok thanks) say `done` youre done");
            var nmsg = await ctx.Channel.SendMessageAsync("roles and stuff added: nothing");
            //get the roles in a loop
            var roles = new Dictionary<DiscordRole, Tuple<DiscordEmoji, ReactionRoleType>>();
            while (true)
            {
                result = await ctx.Message.GetNextMessageAsync(m => m.Content.Length > 0, TimeSpan.FromMinutes(5));
                if (result.TimedOut)
                {
                    msg = await msg.ModifyAsync("you are WAY WAY TOO SLOW");
                    return;
                }
                if (string.Equals(result.Result.Content, "done", StringComparison.OrdinalIgnoreCase))
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
                StringBuilder asasasasas = new($"roles and stuff added ({roles.Count}):\n");
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
                msg = await msg.ModifyAsync("ay sorry to bother ya but what color do you want for the embed");
                result = await ctx.Message.GetNextMessageAsync(m => m.Content.Length > 0, TimeSpan.FromMinutes(2));
                DiscordColor colour = DiscordColor.Red;
                if (result.TimedOut)
                {
                    msg = await msg.ModifyAsync("nvm");
                }
                else
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
            await msg.ModifyAsync("done???");
        }
    }
}