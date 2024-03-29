﻿/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Humanizer;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SilverBot.Shared;
using SilverBot.Shared.Attributes;
using SilverBot.Shared.Objects.Database;
using SilverBot.Shared.Objects.Database.Classes;
using SilverBot.Shared.Objects.Language;
using CategoryAttribute = SilverBot.Shared.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands
{
    [Category("Server statistics")]
    [RequireModuleGuildEnabled(EnabledModules.ServerStats, true)]
    public class ServerStatsCommands : BaseCommandModule
    {
        private readonly Regex _emote = new("<(a)?:(?<name>.+?):(?<id>.+?)>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public DatabaseContext Database { private get; set; }
        public LanguageService LanguageService { private get; set; }

        [Command("emoteanalyse")]
        [Description("analyse emote usage in a specified channel")]
        [Cooldown(1, 60 * 60, CooldownBucketType.Guild)]
        public async Task EmoteAnalytics(CommandContext ctx, DiscordChannel channel, int limit = 10000)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithContent(string.Format(lang.EmojiMessageDownloadStart, limit,
                    new TimeSpan(0, 0, limit / 100).Humanize(2, lang.GetCultureInfo())))
                .SendAsync(ctx.Channel);
            var start = DateTime.Now;
            var messages = await channel.GetMessagesAsync(limit);
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithContent(string.Format(lang.EmojiMessageDownloadEnd, messages.Count, limit,
                    new TimeSpan(0, 0, limit / 100).Humanize(2, lang.GetCultureInfo()),
                    (DateTime.Now - start).Humanize(2, lang.GetCultureInfo()),
                    new TimeSpan(0, 0, messages.Count / 100).Humanize(2, lang.GetCultureInfo())))
                .SendAsync(ctx.Channel);
            var startTime = DateTime.Now;
            StringBuilder bob = new("Name,Id,Timestamp\n");
            foreach (var message in messages)
            {
                foreach (var part in message.Content.Split(' '))
                {
                    var m = _emote.Matches(part);
                    if (m.Count == 0)
                    {
                        continue;
                    }

                    foreach (var gr in m.Select(ovl => ovl.Groups))
                    {
                        bob.Append(gr["name"].Value).Append(',').Append(Convert.ToUInt64(gr["id"].Value)).Append(',')
                            .Append($"{message.EditedTimestamp ?? message.Timestamp:yyyy-MM-dd HH:mm:ss}")
                            .AppendLine();
                    }
                }

            }

            await ctx.SendStringFileWithContent(
                string.Format(lang.EmojiEnd, messages.Count, (DateTime.Now - start).Humanize(2, lang.GetCultureInfo()),
                    (DateTime.Now - startTime).Humanize(2, lang.GetCultureInfo())), bob.ToString(), "emotes.csv");
            bob.Clear();
        }

        [Command("setcategory")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [RequireBotPermissions(Permissions.ManageChannels)]
        public async Task SetCategory(CommandContext ctx, DiscordChannel category)
        {
            if (category.Type == ChannelType.Category)
            {
                var lang = await LanguageService.FromCtxAsync(ctx);
                if (category.PermissionsFor(ctx.Guild.CurrentMember).HasPermission(Permissions.ManageChannels))
                {
                    Database.SetServerStatsCategory(ctx.Guild.Id, category.Id);
                    await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                        .WithContent(string.Format(lang.CategorySetSuccess, category.Name))
                        .WithAllowedMentions(Mentions.None)
                        .SendAsync(ctx.Channel);
                }
                else
                {
                    await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                        .WithContent(lang.NoPerm)
                        .WithAllowedMentions(Mentions.None)
                        .SendAsync(ctx.Channel);
                }
            }
        }

        [Command("setstatstrings")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [RequireBotPermissions(Permissions.ManageChannels)]
        public async Task SetStatisticStrings(CommandContext ctx)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            Database.SetServerStatStrings(ctx.Guild.Id, null);
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithContent(lang.SetToDefaultStrings)
                .WithAllowedMentions(Mentions.None)
                .SendAsync(ctx.Channel);
        }

        [Command("setstatstrings")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [RequireBotPermissions(Permissions.ManageChannels)]
        public async Task SetStatisticStrings(CommandContext ctx, params string[] cake)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            Database.SetServerStatStrings(ctx.Guild.Id, cake.Select(x => new ServerStatString(x)).ToArray());
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithContent(lang.SetToProvidedStrings)
                .WithAllowedMentions(Mentions.None)
                .SendAsync(ctx.Channel);
        }
    }
}