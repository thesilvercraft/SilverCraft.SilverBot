using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Humanizer;
using SilverBotDS.Objects;
using SilverBotDS.Objects.Database.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SilverBotDS.Commands
{
    internal class ServerStatsCommands : BaseCommandModule
    {
        private readonly Regex _emote = new("<(a)?:(?<name>.+?):(?<id>.+?)>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        public DatabaseContext Database { private get; set; }

        [Command("emoteanalyse")]
        [Description("analyse emote usage in a specified channel")]
        [Cooldown(1, 60 * 60, CooldownBucketType.Guild)]
        public async Task EmoteAnalytics(CommandContext ctx, DiscordChannel channel, int limit = 10000)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                             .WithContent(string.Format(lang.StatisticCommand.EmojiMessageDownloadStart, limit, new TimeSpan(0, 0, limit / 100).Humanize(2)))
                                             .SendAsync(ctx.Channel);
            DateTime start = DateTime.Now;
            var messages = await channel.GetMessagesAsync(limit);
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                            .WithContent(string.Format(lang.StatisticCommand.EmojiMessageDownloadEnd, messages.Count, limit, new TimeSpan(0, 0, limit / 100).Humanize(2), (DateTime.Now - start).Humanize(2), new TimeSpan(0, 0, messages.Count / 100).Humanize(2)))
                                            .SendAsync(ctx.Channel);
            DateTime startproc = DateTime.Now;
            StringBuilder bob = new("Name,Id,Timestamp\n");
            ulong e = 0;
            foreach (var message in messages)
            {
                foreach (var part in message.Content.Split(' '))
                {
                    var m = _emote.Matches(part);
                    if (m.Count != 0)
                    {
                        foreach (Match match in m.ToArray())
                        {
                            bob.AppendLine($"{match.Groups["name"].Value},{Convert.ToUInt64(match.Groups["id"].Value)},{(DateTimeOffset)(message.EditedTimestamp != null ? message.EditedTimestamp : message.Timestamp):yyyy-MM-dd HH:mm:ss}");
                        }
                    }
                }
                e++;
            }
            await OwnerOnly.SendStringFileWithContent(ctx, string.Format(lang.StatisticCommand.EmojiEnd, messages.Count, (DateTime.Now - start).Humanize(2), (DateTime.Now - startproc).Humanize(2)), bob.ToString(), "emotes.csv");
            bob.Clear();
            GC.Collect();
        }

        [Command("setcategory")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [RequireBotPermissions(Permissions.ManageChannels)]
        public async Task SetCategory(CommandContext ctx, DiscordChannel category)
        {
            if (category.Type == ChannelType.Category)
            {
                var lang = await Language.GetLanguageFromCtxAsync(ctx);
                if (category.PermissionsFor(ctx.Guild.CurrentMember).HasPermission(Permissions.ManageChannels))
                {
                    Database.SetServerStatsCategory(ctx.Guild.Id, category.Id);
                    await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                              .WithContent(string.Format(lang.StatisticCommand.CategorySetSuccess, category.Name))
                                              .WithAllowedMentions(Mentions.None)
                                              .SendAsync(ctx.Channel);
                }
                else
                {
                    await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                           .WithContent(lang.StatisticCommand.NoPerm)
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
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            Database.SetServerStatStrings(ctx.Guild.Id, null);
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                      .WithContent(lang.StatisticCommand.SetToDefaultStrings)
                                      .WithAllowedMentions(Mentions.None)
                                      .SendAsync(ctx.Channel);
        }

        [Command("setstatstrings")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [RequireBotPermissions(Permissions.ManageChannels)]
        public async Task SetStatisticStrings(CommandContext ctx, params string[] cake)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            List<ServerStatString> e = new();
            foreach (var peace in cake)
            {
                e.Add(new(peace));
            }
            Database.SetServerStatStrings(ctx.Guild.Id, e);
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                      .WithContent(lang.StatisticCommand.SetToProvidedStrings)
                                      .WithAllowedMentions(Mentions.None)
                                      .SendAsync(ctx.Channel);
        }
    }
}