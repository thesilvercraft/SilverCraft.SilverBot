/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using SilverBot.Shared.Objects;

namespace SilverBotDS.ProgramExtensions
{
    public static class SlashErrorHandler
    {
        private static ServiceProvider ServiceProvider { get; set; }
        private static Logger Log { get; set; }

        public static Task RegisterErrorHandler(ServiceProvider sp, Logger log, SlashCommandsExtension e)
        {
            ServiceProvider = sp;
            Log = log;
            e.SlashCommandErrored += Slash_SlashCommandErrored;
            return Task.CompletedTask;
        }


        private static async Task Slash_SlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e)
        {
            async Task RespondWithContent(string content, bool ephermal = true)
            {
                try
                {
                    await e.Context.DeferAsync(ephermal);
                }
                catch
                {
                    //Try deferring if allowed, if not continue on
                }

                await e.Context.FollowUpAsync(new DiscordFollowupMessageBuilder().AsEphemeral(ephermal)
                    .WithContent(content));
            }

            var config = ServiceProvider.GetService<Config>();
            if (config is { UseAnalytics: true })
            {
                var analytics = ServiceProvider.GetService<IAnalyse>();
                if (analytics is not null)
                {
                    await analytics.EmitEvent(e.Context.User, "SlashCommandErrored", new Dictionary<string, object>()
                    {
                        { "commandname", e.Context.CommandName },
                        { "error", e.Exception }
                    });
                }
            }

            if (e.Context.Channel.IsPrivate || e.Context.Channel
                    .PermissionsFor(await e.Context.Guild.GetMemberAsync(sender.Client.CurrentUser.Id))
                    .HasPermission(Permissions.SendMessages))
            {
                if (e.Exception is CommandNotFoundException)
                {
                    await RespondWithContent("Command not found.");
                }
                else
                {
                    await CommandErrorHandler.ExceptionFormatAndHandle(e,
                        async (x) => { await RespondWithContent(x); });
                }
            }

            Log.Error(e.Exception,
                "Error `{ExceptionName}` encountered.\nGuild `{GuildId}`, channel `{ChannelId}`, user `{UserID}`\n```\nNA\n```",
                e.Exception.GetType().FullName, e.Context.Guild?.Id.ToString() ?? "None",
                e.Context.Channel?.Id.ToString(), e.Context.User?.Id.ToString() ?? "None");
        }
    }
}