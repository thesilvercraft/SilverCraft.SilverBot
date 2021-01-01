using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS
{
    class genericcommands : BaseCommandModule
    {
        [Command("hi")]
        public async Task GreetCommand(CommandContext ctx)
        {
            var lang = language.GetLanguageFromId(ctx.Guild.Id);
            await ctx.RespondAsync(string.Format(lang.Hi,ctx.Member.Mention));
        }
        [Command("time")]
        public async Task Time(CommandContext ctx)
        {
            var lang = language.GetLanguageFromId(ctx.Guild.Id);
            await ctx.RespondAsync(string.Format(lang.TimeInUtc,DateTime.UtcNow.ToString(lang.Timeformat)));
        }
        [Command("invite")]
        public async Task Invite(CommandContext ctx)
        {
            
          await ctx.RespondAsync($"https://discord.com/api/oauth2/authorize?client_id={ctx.Client.CurrentUser.Id}&permissions=2147483639&scope=bot");
            if (ctx.Client.CurrentApplication.IsPublic == false)
            {
                await ctx.RespondAsync("note to the guy that hosts the bot, set it to public lol");
            }
        }
        [Command("duckhosting"), Aliases("dukthosting", "ducthosting")]
        [Description("SilverHosting best")]
        public async Task dukt(CommandContext ctx)
        {
            DiscordEmbedBuilder bob = new DiscordEmbedBuilder();
            bob.WithTitle("SilverCraftBot sponsored by SilverHosting");
            bob.WithDescription("Use offer code [SLVR](https://www.youtube.com/watch?v=dQw4w9WgXcQ)");
            bob.WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            await ctx.RespondAsync(embed: bob.Build());
        }
        public static async Task<bool> Is_at_silvercraftAsync(CommandContext ctx, DiscordUser b)
        {
            bool is_at_silvercraft = false;

            if ((await ctx.Client.GetGuildAsync(Convert.ToUInt64(Program.GetConfig().ServerId))).Members[b.Id] != null)
            {
                is_at_silvercraft = true;
            }
            return is_at_silvercraft;
        }

        public static async Task<bool> Is_bot_adminAsync(CommandContext ctx, DiscordUser b)
        {
            bool isbotadmin = false;
            DiscordMember socketGuildus = (await ctx.Client.GetGuildAsync(Convert.ToUInt64(Program.GetConfig().ServerId))).Members[b.Id];
            foreach (DiscordRole role in socketGuildus.Roles)
            {
                if (role.Id == Convert.ToUInt64(Program.GetConfig().AdminRoleId))
                {
                    isbotadmin = true;
                }
            }
            return isbotadmin;
        }

        [Command("user")]
        [Description("Get the info the bot knows about a specified user")]
        public async Task Userinfo(CommandContext ctx, [Description("the user like duh")] DiscordUser a)
        {
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            b.WithTitle("User " + a.Username);
            b.WithDescription("Information about " + a.Mention);
            b.AddField("ID", a.Id.ToString(), true);
           // b.AddField("Has joined the SilverCraft Server", stringutils.BoolToEmoteString(await Is_at_silvercraftAsync(ctx, a)), true);
         //   b.AddField("Is a SilverCraft bot admin", stringutils.BoolToEmoteString(await Is_bot_adminAsync(ctx, a)), true);
            b.AddField("Is a SilverCraft bot owner", stringutils.BoolToEmoteString(Program.GetConfig().Botowners.Contains(a.Id)), true);
            if (a.Id == 208691453973495808)
            {
                b.AddField("Is a bot", ":white_check_mark: see https://discord.com/channels/714154158969716780/714154159590473801/767829209052872724", true);
            }
            else
            {
                b.AddField("Is a bot", stringutils.BoolToEmoteString(a.IsBot), true);
            }
            b.WithThumbnail(a.GetAvatarUrl(ImageFormat.Png));
            b.WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png));
            await ctx.RespondAsync(embed: b.Build());
        }

    }
}
