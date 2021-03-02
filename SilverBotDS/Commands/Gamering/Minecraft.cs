﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SilverBotDS.Utils;
using System;
using System.Threading.Tasks;

namespace SilverBotDS.Commands.Gamering
{
    [Group("minecraft")]
    internal class MinecraftModule : BaseCommandModule
    {
        //HEY LOIS IM PLAYING MINECRAFT
        [Command("getfromusername")]
        [Description("Get a minecraft players UUID from their username")]
        public async Task Calculate(CommandContext ctx, string input)
        {
            try
            {
                var player = await MinecraftUtils.GetPlayerAsync(input);
                var b = new DiscordEmbedBuilder();
                b.WithThumbnail(player.GetAvatarUrl())
                      .WithImageUrl(player.GetBodyUrl())
                      .AddField("uuid", "`" + player.Id + "`")
                        .WithColor(await ColorUtils.GetSingleAsync());
                await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                             .WithEmbed(b.Build())
                                             .SendAsync(ctx.Channel);
            }
            catch (Exception e)
            {
                Program.SendLog(e);
                await new DiscordMessageBuilder().WithContent("Error occured: ```csharp\n" + e.Message + "```").SendAsync(ctx.Channel);
                throw;
            }
        }
    }
}