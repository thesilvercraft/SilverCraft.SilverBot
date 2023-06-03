/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SilverBot.Shared;
using SilverBot.Shared.Attributes;
using SilverBot.Shared.Objects.Database;
using SilverBot.Shared.Objects.Database.Classes;
using SilverBot.Shared.Objects.Language;
using SilverBot.Shared.Utils;
using CategoryAttribute = SilverBot.Shared.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands
{
    [RequireUserPermissions(Permissions.Administrator)]
    [Category("Utility")]
    [RequireModuleGuildEnabled(EnabledModules.Admin, false)]
    public class AdminCommands : BaseCommandModule
    {
        public static readonly DiscordEmoji YesEmoji = DiscordEmoji.FromUnicode("☑️");
        public static readonly DiscordEmoji NoEmoji = DiscordEmoji.FromUnicode("❌");
        public static readonly DiscordEmoji EntryEmoji = DiscordEmoji.FromUnicode("☑️");

        public DatabaseContext Database { private get; set; }
        public HttpClient HttpClient { private get; set; }
        public LanguageService LanguageService { private get; set; }
        public ColourService ColourService { private get; set; }

        [Command("setprefix")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task SetPrefix(CommandContext ctx, params string[] cake)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            Database.SetServerPrefixes(ctx.Guild.Id, cake);
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                .WithContent(lang.SetToProvidedStrings)
                .WithAllowedMentions(Mentions.None)
                .SendAsync(ctx.Channel);
        }


        /// <summary>
        ///     A simple emoji based yes/no poll.
        /// </summary>
        /// <param name="commandContext">CommandContext of the message that has executed this command</param>
        /// <param name="duration">Amount of time how long the poll should last.</param>
        /// <param name="question">Polls question</param>
        /// <returns></returns>
        [Command("emojipoll")]
        [Description("Start a simple emoji poll for a simple yes/no question")]
        [Cooldown(2, 30, CooldownBucketType.Guild)]
        public async Task EmojiPollAsync(CommandContext commandContext,
            [Description("How long should the poll last. (e.g. 1m = 1 minute)")]
            TimeSpan duration,
            [Description("Poll question")] [RemainingText]
            string question)
        {
            if (!string.IsNullOrEmpty(question))
            {
                var bob = new DiscordEmbedBuilder();
                bob.WithTitle(question)
                    .WithAuthor(commandContext.Member?.Nickname ?? commandContext.User.Username,
                        iconUrl: commandContext.User.GetAvatarUrl(ImageFormat.Png))
                    .WithColor(ColourService.GetSingle());
                var pollStartMessage = await commandContext.RespondAsync(bob.Build());
                await pollStartMessage.CreateReactionAsync(YesEmoji);
                await pollStartMessage.CreateReactionAsync(NoEmoji);
                await Database.plannedEvents.AddAsync(new PlannedEvent
                {
                    ChannelID = commandContext.Channel.Id,
                    EventID = RandomGenerator.RandomAbcString(20),
                    Handled = false,
                    MessageID = commandContext.Message.Id,
                    Time = DateTime.Now + duration,
                    Type = PlannedEventType.EmojiPoll,
                    UserID = commandContext.User.Id,
                    ResponseMessageID = pollStartMessage.Id
                });
                await Database.SaveChangesAsync();
            }
            else
            {
                var lang = await LanguageService.FromCtxAsync(commandContext);
                await commandContext.RespondAsync(lang.PollErrorQuestionNull);
            }
        }

        [Command("GiveAway")]
        [Description("Start a simple giveaway")]
        public async Task GiveAway(CommandContext commandContext,
            [Description("How long should the giveaway last. (e.g. 1m = 1 minute)")]
            TimeSpan duration,
            [Description("Giveaway content")] [RemainingText]
            string item)
        {
            if (!string.IsNullOrEmpty(item))
            {
                var bob = new DiscordEmbedBuilder();
                bob.WithTitle(item).WithAuthor(commandContext.Member.Nickname ?? commandContext.User.Username,
                    iconUrl: commandContext.User.GetAvatarUrl(ImageFormat.Png));
                var pollStartMessage = await commandContext.RespondAsync(bob.Build());
                await pollStartMessage.CreateReactionAsync(EntryEmoji);
                await Database.plannedEvents.AddAsync(new PlannedEvent
                {
                    ChannelID = commandContext.Channel.Id,
                    EventID = RandomGenerator.RandomAbcString(20),
                    Handled = false,
                    MessageID = commandContext.Message.Id,
                    Time = DateTime.Now + duration,
                    Type = PlannedEventType.GiveAway,
                    UserID = commandContext.User.Id,
                    ResponseMessageID = pollStartMessage.Id
                });
                await Database.SaveChangesAsync();
            }
            else
            {
                var lang = await LanguageService.FromCtxAsync(commandContext);
                await commandContext.RespondAsync(lang.GiveawayItemNull);
            }
        }

        [Command("exportemotestoguilded")]
        [Description("Exports your guild's emotes into a \"Emote Pack\" Guilded can read")]
        [RequireGuild]
        public async Task ExportEmotesToGuilded(CommandContext ctx)
        {
            var emotes = ctx.Guild.Emojis.Values.Select(emoji => new Emote { Name = emoji.Name, Url = emoji.Url })
                .ToList();
            while (emotes.Count != 0)
            {
                await ctx.SendStringFileWithContent("", JsonSerializer.Serialize(new EmojiPack
                {
                    Author = "SilverBot",
                    Name = $"{ctx.Guild.Name}'s emotes",
                    Emotes = emotes.Take(30).ToArray()
                }), "pack.json");
                if (emotes.Count >= 30)
                {
                    emotes.RemoveRange(0, 30);
                }
                else
                {
                    emotes.Clear();
                }
            }

            await new DiscordMessageBuilder().WithContent("https://support.guilded.gg/hc/en-us/articles/1500000398142")
                .WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
        }

        [Command("exportemotes")]
        [RequireGuild]
        public async Task DownloadEmotz(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            List<SourceFile> sourceFiles = new();
            foreach (var emoji in ctx.Guild.Emojis.Values)
            {
                if (emoji.IsAnimated)
                {
                    sourceFiles.Add(new SourceFile
                    {
                        Name = emoji.Name, Extension = "gif", FileBytes = await HttpClient.GetByteArrayAsync(emoji.Url)
                    });
                }
                else
                {
                    sourceFiles.Add(new SourceFile
                    {
                        Name = emoji.Name, Extension = "png", FileBytes = await HttpClient.GetByteArrayAsync(emoji.Url)
                    });
                }

                await Task.Delay(50);
            }

            await using MemoryStream memoryStream = new();
            using (ZipArchive zip = new(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var f in sourceFiles)
                {
                    var zipItem = zip.CreateEntry($"{f.Name}.{f.Extension}");
                    await using MemoryStream originalFileMemoryStream = new(f.FileBytes);
                    await using var entryStream = zipItem.Open();
                    await originalFileMemoryStream.CopyToAsync(entryStream);
                }
            }

            memoryStream.Position = 0;
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id).AddFile("emojis.zip", memoryStream)
                .SendAsync(ctx.Channel);
        }
    }
}