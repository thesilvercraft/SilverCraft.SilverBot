/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using SilverBot.Shared;
using SilverBot.Shared.Attributes;
using SilverBot.Shared.Exceptions;
using SilverBot.Shared.Objects;
using SilverBot.Shared.Objects.Language;
using SilverBot.Shared.Utils;

namespace SilverBotDS.ProgramExtensions
{
    public class CommandErrorHandler : IProgramExtension
    {
        private static ServiceProvider ServiceProvider { get; set; }
        private static Logger Log { get; set; }
        private static bool UseAnalytics { get; set; }
        private static CommandsNextExtension CommandsNext { get; set; }

        public Task Register(ServiceProvider sp, Logger log, params object[] additionalContext)
        {
            if (additionalContext[0] is not CommandsNextExtension extension)
            {
                throw new ArgumentException(
                    "The additionalContext array must contain one element and that element must be the CommandsNextExtension",
                    nameof(additionalContext));
            }
            if (_isLoaded)
            {
                throw new ProgramExtensionAlreadyLoadedException(nameof(CommandErrorHandler));
            }
            ServiceProvider = sp;
            Log = log;
            CommandsNext = extension;
            CommandsNext.CommandErrored += Commands_CommandErrored;
            UseAnalytics = sp.GetRequiredService<Config>().UseAnalytics;
            _isLoaded = true;
            return Task.CompletedTask;
        }

        public Task Reload()
        {
            if (!_isLoaded)
            {
                throw new ProgramExtensionNotLoadedException(nameof(CommandErrorHandler));
            }
            CommandsNext.CommandErrored -= Commands_CommandErrored;
            UseAnalytics = ServiceProvider.GetRequiredService<Config>().UseAnalytics;
            CommandsNext.CommandErrored += Commands_CommandErrored;
            return Task.CompletedTask;
        }

        public Task Unregister(ServiceProvider sp, Logger log, params object[] additionalContext)
        {
            if (additionalContext[0] is not CommandsNextExtension extension)
            {
                throw new ArgumentException(
                    "The additionalContext array must contain one element and that element must be the CommandsNextExtension",
                    nameof(additionalContext));
            }

            if (!_isLoaded)
            {
                throw new ProgramExtensionNotLoadedException(nameof(CommandErrorHandler));
            }
            CommandsNext.CommandErrored -= Commands_CommandErrored;
            CommandsNext = null;
            Log = null;
            ServiceProvider = null;
            _isLoaded = false;
            return Task.CompletedTask;
        }

        public bool IsLoaded => _isLoaded;

        public static readonly List<string> WaysToPissOffUser = new()
        {
            "https://media.discordapp.net/attachments/1040561549556854875/1046410074878390384/sbfail.gif",
            "📸",
            "think you have outsmarted me? well you haven't"
        };

        private bool _isLoaded;

        /// <summary>
        ///     Render the error message for an Attribute
        /// </summary>
        /// <param name="checkBase">The attribute</param>
        /// <param name="lang">The language</param>
        /// <param name="isinguild">Was the command executed in a guild or in direct messages</param>
        /// <param name="e">Gives the raw command error arguments</param>
        /// <returns>A <see cref="string" /> containing the error message</returns>
        private static string RenderErrorMessageForAttribute(CheckBaseAttribute checkBase, Language lang,
            bool isinguild, CommandErrorEventArgs e)
        {
            string DoOwnerOnly()
            {
                if (e is not { Command.Name: "evaluate" } ||
                    (!e.Context.RawArgumentString.Contains(lang.RequireOwnerCheckFailed) &&
                     !WaysToPissOffUser.Any(x => e.Context.RawArgumentString.Contains(x))))
                {
                    return lang.RequireOwnerCheckFailed;
                }

                var s = WaysToPissOffUser.Where(x => !e.Context.RawArgumentString.Contains(x)).ToArray();
                return s.Any() ? s.ElementAt(RandomGenerator.Next(0, s.Count())) : lang.RequireOwnerCheckFailed;
            }

            return checkBase switch
            {
                RequireOwnerAttribute => DoOwnerOnly(),
                RequireDjAttribute => lang.RequireDJCheckFailed,
                RequireGuildAttribute => lang.RequireGuildCheckFailed,
                RequireNsfwAttribute => lang.RequireNsfwCheckFailed,
                RequireRolesAttribute { RoleNames.Count: 1 } requireRolesAttribute => string.Format(
                    lang.RequireRolesCheckFailedSG, requireRolesAttribute.RoleNames[0]),
                RequireRolesAttribute requireRolesAttribute => string.Format(lang.RequireRolesCheckFailedPL,
                    requireRolesAttribute.RoleNames.Humanize()),
                RequireBotPermissionsAttribute requireBotPermissions when
                    !(requireBotPermissions.IgnoreDms && isinguild) => lang.RequireGuildCheckFailed,
                RequireBotPermissionsAttribute requireBotPermissions when
                    Enum.IsDefined(requireBotPermissions.Permissions) &&
                    requireBotPermissions.Permissions != Permissions.All => string.Format(
                        lang.RequireBotPermisionsCheckFailedSG,
                        requireBotPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequireBotPermissionsAttribute requireBotPermissions => string.Format(
                    lang.RequireBotPermisionsCheckFailedPL,
                    requireBotPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequireUserPermissionsAttribute userPermissions when !(userPermissions.IgnoreDms && isinguild) => lang
                    .RequireGuildCheckFailed,
                RequireUserPermissionsAttribute userPermissions when Enum.IsDefined(userPermissions.Permissions) &&
                                                                     userPermissions.Permissions != Permissions.All =>
                    string.Format(lang.RequireUserPermisionsCheckFailedSG,
                        userPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequireUserPermissionsAttribute userPermissions => string.Format(
                    lang.RequireUserPermisionsCheckFailedPL,
                    userPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequirePermissionsAttribute userAndBotPermissions when !(userAndBotPermissions.IgnoreDms && isinguild)
                    => lang.RequireGuildCheckFailed,
                RequirePermissionsAttribute userAndBotPermissions when
                    Enum.IsDefined(userAndBotPermissions.Permissions) &&
                    userAndBotPermissions.Permissions != Permissions.All => string.Format(
                        lang.RequireBotAndUserPermisionsCheckFailedSG,
                        userAndBotPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequirePermissionsAttribute userAndBotPermissions => string.Format(
                    lang.RequireBotAndUserPermisionsCheckFailedPL,
                    userAndBotPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequireAttachmentAttribute attachmentAttribute when
                    e.Context.Message.Attachments.Count > attachmentAttribute.AttachmentCount =>
                    (string?)typeof(Language).GetProperty(attachmentAttribute.MoreThenLang)?.GetValue(lang) ??
                    "Too many attachments (minerror 21d74757)",
                RequireAttachmentAttribute attachmentAttribute => (string?)typeof(Language)
                                                                      .GetProperty(attachmentAttribute.LessThenLang)
                                                                      ?.GetValue(lang) ??
                                                                  "Not enough attachments (minerror 80d5e4d1)",
                _ => string.Format(lang.CheckFailed,
                    checkBase.GetType().Name.RemoveStringFromEnd("Attribute").Humanize())
            };
        }

        private static string RenderErrorMessageForAttribute(SlashCheckBaseAttribute checkBase, Language lang,
            bool isinguild, SlashCommandErrorEventArgs e)
        {
            var type = checkBase.GetType();
            return checkBase switch
            {
                _ => string.Format(lang.CheckFailed, type.Name.RemoveStringFromEnd("Attribute").Humanize())
            };
        }

        public static async Task ExceptionFormatAndHandle(dynamic e, Func<string, Task> respondWithContent)
        {
            var languageService = ServiceProvider.GetService<LanguageService>();
            var lang = await languageService?.FromCtxAsync(e.Context)!;
            switch (e)
            {
                case CommandErrorEventArgs a:
                    switch (e.Exception)
                    {
                        case InvalidOverloadException:
                        case ArgumentException { Message: "Could not find a suitable overload for the command." }:

                            await respondWithContent(string.Format(lang.InvalidOverload, a.Context.Command?.Name));
                            return;
                        case ChecksFailedException { FailedChecks.Count: 1 } cfe:
                            await respondWithContent(RenderErrorMessageForAttribute(cfe.FailedChecks[0], lang,
                                e.Context.Guild != null, a));
                            return;
                        case ChecksFailedException cfe:
                        {
                            var embedBuilder = new DiscordEmbedBuilder().WithTitle(lang.ChecksFailed);
                            var pages = cfe.FailedChecks.Select((t, i) => new Page(embed: embedBuilder
                                    .WithFooter($"{i + 1} / {cfe.FailedChecks.Count}")
                                    .WithDescription(
                                        RenderErrorMessageForAttribute(t, lang, e.Context.Guild != null, a))))
                                .ToList();

                            var interactivity = e.Context.Client.GetInteractivity();
                            await interactivity.SendPaginatedMessageAsync(e.Context.Channel, e.Context.User, pages,
                                token: new CancellationToken());
                            return;
                        }
                    }

                    break;
                case SlashCommandErrorEventArgs b:
                    switch (e.Exception)
                    {
                        case SlashExecutionChecksFailedException { FailedChecks.Count: 1 } cfe:
                            await respondWithContent(RenderErrorMessageForAttribute(cfe.FailedChecks[0], lang,
                                e.Context.Guild != null, e));
                            return;
                        case SlashExecutionChecksFailedException cfe:
                        {
                            var builder = new DiscordEmbedBuilder().WithTitle(lang.ChecksFailed);
                            var pages = cfe.FailedChecks.Select((t, i) => new Page(embed: builder
                                    .WithFooter($"{i + 1} / {cfe.FailedChecks.Count}")
                                    .WithDescription(
                                        RenderErrorMessageForAttribute(t, lang, e.Context.Guild != null, e))))
                                .ToList();
                            var interactivity = e.Context.Client.GetInteractivity();
                            await interactivity.SendPaginatedMessageAsync(e.Context.Channel, e.Context.User, pages,
                                token: new CancellationToken());
                            return;
                        }
                        case InvalidOverloadException:
                        case ArgumentException { Message: "Could not find a suitable overload for the command." }:
                            await respondWithContent(string.Format(lang.InvalidOverload, e.Context.CommandName));
                            return;
                    }

                    break;
            }

            switch (e.Exception)
            {
                case InvalidOperationException
                {
                    Message: "No matching subcommands were found, and this group is not executable."
                }:
                    await respondWithContent(lang.NoMatchingSubcommandsAndGroupNotExecutable);
                    break;
                case InvalidOperationException
                {
                    Message:
                    "The node socket has not been initialized. Call 'InitializeAsync()' before sending payloads."
                }:
                    await respondWithContent(lang.LavalinkNotSetup);
                    break;
                case NetVips.VipsException
                {
                    Message:
                    "unable to load from source\nVipsForeignLoad: buffer is not in a known format\n"
                }:
                    await respondWithContent(lang.UnknownImageFormat);
                    break;
                case AttachmentCountIncorrectException
                {
                    AttachmentCount: AttachmentCountIncorrect.TooManyAttachments
                }:
                    await respondWithContent(lang.WrongImageCount);
                    break;
                case AttachmentCountIncorrectException:
                    await respondWithContent(lang.NoImageGeneric);
                    break;
                case OutOfMemoryException:
                    await respondWithContent("bot OOM");
                    break;
                default:
                    await respondWithContent(lang.GeneralException);
                    break;
            }
        }

        private static async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            if (UseAnalytics)
            {
                var analytics = ServiceProvider.GetService<IAnalyse>();
                if (analytics is not null)
                {
                    await analytics.EmitEvent(e.Context.User, "CommandErrored", new Dictionary<string, object>()
                    {
                        { "commandname", e.Context?.Command.Name },
                        { "error", e.Exception }
                    });
                }
            }

            if ((e.Context.Channel.IsPrivate || e.Context.Channel
                    .PermissionsFor(await e.Context.Guild.GetMemberAsync(sender.Client.CurrentUser.Id))
                    .HasPermission(Permissions.SendMessages)) && e.Exception is not CommandNotFoundException)
            {
                await ExceptionFormatAndHandle(e, async (x) =>
                {
                    await new DiscordMessageBuilder()
                        .WithReply(e.Context.Message.Id)
                        .WithContent(x)
                        .SendAsync(e.Context.Channel);
                });
            }

            Log.Error(e.Exception,
                "Error `{ExceptionName}` encountered.\nGuild `{GuildId}`, channel `{ChannelId}`, user `{UserId}`\n```\n{MessageContent}\n```",
                e.Exception.GetType().FullName, e.Context.Guild?.Id.ToString() ?? "None",
                e.Context.Channel?.Id.ToString(), e.Context.User?.Id.ToString() ?? "None", e.Context.Message.Content);
        }
    }
}