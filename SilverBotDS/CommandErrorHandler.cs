using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using SilverBotDS.Attributes;
using SilverBotDS.Converters;
using SilverBotDS.Exceptions;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilverBotDS
{
    public static class CommandErrorHandler
    {
        private static ServiceProvider ServiceProvider { get; set; }
        private static Logger Log { get; set; }
        private static bool UseSegment { get; set; }
        private static CommandsNextExtension E { get; set; }

        public static Task RegisterErrorHandler(ServiceProvider sp, Logger log, CommandsNextExtension e)
        {
            ServiceProvider = sp;
            Log = log;
            E = e;
            E.CommandErrored += Commands_CommandErrored;
            UseSegment = sp.GetRequiredService<Config>().SendErrorsThroughSegment;
            return Task.CompletedTask;
        }

        public static void Reload()
        {
            E.CommandErrored -= Commands_CommandErrored;
            UseSegment = ServiceProvider.GetRequiredService<Config>().SendErrorsThroughSegment;
            E.CommandErrored += Commands_CommandErrored;
        }

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
            var type = checkBase.GetType();
            if (type == typeof(RequireDjAttribute))
            {
                return lang.RequireDJCheckFailed;
            }
            if (type == typeof(RequireGuildAttribute))
            {
                return lang.RequireGuildCheckFailed;
            }
            if (type == typeof(RequireNsfwAttribute))
            {
                return lang.RequireNsfwCheckFailed;
            }
            if (type == typeof(RequireOwnerAttribute))
            {
                return lang.RequireOwnerCheckFailed;
            }
            return checkBase switch
            {
                RequireRolesAttribute requireRolesAttribute when requireRolesAttribute.RoleNames.Count == 1 => string.Format(lang.RequireRolesCheckFailedSG, requireRolesAttribute.RoleNames[0]),
                RequireRolesAttribute requireRolesAttribute => string.Format(lang.RequireRolesCheckFailedPL, requireRolesAttribute.RoleNames.Humanize()),
                RequireBotPermissionsAttribute requireBotPermissions when !(requireBotPermissions.IgnoreDms && isinguild) => lang.RequireGuildCheckFailed,
                RequireBotPermissionsAttribute requireBotPermissions when Enum.IsDefined(requireBotPermissions.Permissions) &&
requireBotPermissions.Permissions != Permissions.All => string.Format(lang.RequireBotPermisionsCheckFailedSG,
requireBotPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequireBotPermissionsAttribute requireBotPermissions => string.Format(lang.RequireBotPermisionsCheckFailedPL,
requireBotPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequireUserPermissionsAttribute userPermissions when !(userPermissions.IgnoreDms && isinguild) => lang.RequireGuildCheckFailed,
                RequireUserPermissionsAttribute userPermissions when Enum.IsDefined(userPermissions.Permissions) && userPermissions.Permissions != Permissions.All => string.Format(lang.RequireUserPermisionsCheckFailedSG,
userPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequireUserPermissionsAttribute userPermissions => string.Format(lang.RequireUserPermisionsCheckFailedPL,
userPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequirePermissionsAttribute userAndBotPermissions when !(userAndBotPermissions.IgnoreDms && isinguild) => lang.RequireGuildCheckFailed,
                RequirePermissionsAttribute userAndBotPermissions when Enum.IsDefined(userAndBotPermissions.Permissions) &&
userAndBotPermissions.Permissions != Permissions.All => string.Format(lang.RequireBotAndUserPermisionsCheckFailedSG,
userAndBotPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequirePermissionsAttribute userAndBotPermissions => string.Format(lang.RequireBotAndUserPermisionsCheckFailedPL,
userAndBotPermissions.Permissions.Humanize(LetterCasing.LowerCase)),
                RequireAttachmentAttribute attachmentAttribute when e.Context.Message.Attachments.Count > attachmentAttribute.AttachmentCount => (string)typeof(Language).GetProperty(attachmentAttribute.MoreThenLang)?.GetValue(lang),
                RequireAttachmentAttribute attachmentAttribute => (string)typeof(Language).GetProperty(attachmentAttribute.LessThenLang)?.GetValue(lang),
                _ => string.Format(lang.CheckFailed, type.Name.RemoveStringFromEnd("Attribute").Humanize()),
            };
        }

        private static async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            async Task RespondWithContent(string content)
            {
                await new DiscordMessageBuilder()
                    .WithReply(e.Context.Message.Id)
                    .WithContent(content)
                    .SendAsync(e.Context.Channel);
            }

            if (UseSegment)
            {
                var analytics = ServiceProvider.GetService<IAnalyse>();
                if (analytics is not null)
                {
                    await analytics.EmitEvent(e.Context.User, "SlashCommandErrored", new Dictionary<string, object>()
                    {
                        {"commandname", e.Context.Command.Name},
                        {"error", e.Exception}
                    });
                }
            }

            if (e.Context.Channel.IsPrivate || e.Context.Channel
                    .PermissionsFor(await e.Context.Guild.GetMemberAsync(sender.Client.CurrentUser.Id))
                    .HasPermission(Permissions.SendMessages))
            {
                if (e.Exception is CommandNotFoundException)
                {
                    //we do not do anything if it is a nonexistent command, i would have liked it to be a user only visible message but discord is shit
                    return;
                }
                else
                {
                    var lang = await Language.GetLanguageFromCtxAsync(e.Context);
                    switch (e.Exception)
                    {
                        case ChecksFailedException cfe when cfe.FailedChecks.Count is 1:
                            await RespondWithContent(RenderErrorMessageForAttribute(cfe.FailedChecks[0], lang,
                                e.Context.Guild != null, e));
                            break;

                        case ChecksFailedException cfe:
                            {
                                var embedBuilder = new DiscordEmbedBuilder().WithTitle(lang.ChecksFailed);
                                var pages = cfe.FailedChecks.Select((t, i) => new Page(embed: embedBuilder.WithFooter($"{i + 1} / {cfe.FailedChecks.Count}")
                                        .WithDescription(RenderErrorMessageForAttribute(t, lang, e.Context.Guild != null, e))))
                                    .ToList();

                                var interactivity = e.Context.Client.GetInteractivity();
                                await interactivity.SendPaginatedMessageAsync(e.Context.Channel, e.Context.User, pages,
                                    token: new CancellationToken());
                                break;
                            }
                        case InvalidOverloadException:
                        case ArgumentException { Message: "Could not find a suitable overload for the command." }:
                            await RespondWithContent(string.Format(lang.InvalidOverload, e.Context.Command.Name));
                            break;

                        case InvalidOperationException { Message: "No matching subcommands were found, and this group is not executable." }:
                            await RespondWithContent(lang.NoMatchingSubcommandsAndGroupNotExecutable);
                            break;

                        case UnknownImageFormatException:
                            await RespondWithContent(lang.UnknownImageFormat);
                            break;

                        case AttachmentCountIncorrectException { AttachmentCount: AttachmentCountIncorrect.TooManyAttachments }:
                            await RespondWithContent(lang.WrongImageCount);
                            break;

                        case AttachmentCountIncorrectException aa:
                            await RespondWithContent(lang.NoImageGeneric);
                            break;

                        default:
                            await RespondWithContent(lang.GeneralException);
                            break;
                    }
                }
            }

            Log.Error(e.Exception,
                "Error `{ExceptionName}` encountered.\nGuild `{GuildId}`, channel `{ChannelId}`, user `{UserId}`\n```\n{MessageContent}\n```", e.Exception.GetType().FullName, e.Context.Guild?.Id.ToString() ?? "None", e.Context.Channel?.Id.ToString(), e.Context.User?.Id.ToString() ?? "None", e.Context.Message.Content);
        }
    }
}