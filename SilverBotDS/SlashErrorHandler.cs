using DSharpPlus;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using SilverBotDS.Attributes;
using SilverBotDS.Converters;
using SilverBotDS.Exceptions;
using SilverBotDS.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilverBotDS
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

        private static string RemoveStringFromEnd(string a, string sub)
        {
            if (a.EndsWith(sub))
            {
                a = a[..a.LastIndexOf(sub, StringComparison.Ordinal)];
            }
            return a;
        }

        /// <summary>
        ///     Render the error message for an Attribute
        /// </summary>
        /// <param name="checkBase">The attribute</param>
        /// <param name="lang">The language</param>
        /// <param name="isinguild">Was the command executed in a guild or in direct messages</param>
        /// <param name="e">Gives the raw command error arguments</param>
        /// <returns>A <see cref="string" /> containing the error message</returns>
        private static string RenderErrorMessageForAttribute(SlashCheckBaseAttribute checkBase, Language lang,
            bool isinguild, SlashCommandErrorEventArgs e)
        {
            var type = checkBase.GetType();
            return checkBase switch
            {
                RequireGuildDatabaseValueSlashAttribute a when !a.AllowDirectMessages && isinguild => lang.AttributeDataBaseCheckNoDirectMessages,
                RequireGuildDatabaseValueSlashAttribute a when a.Variable == "AttributeDataBaseCheckWebShot" => lang.AttributeDataBaseCheckNoDirectMessages,
                _ => string.Format(lang.CheckFailed, RemoveStringFromEnd(type.Name, "Attribute").Humanize()),
            };
        }

        private static async Task Slash_SlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e)
        {
            async Task RespondWithContent(string content, bool ephermal = true)
            {
                await e.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().AsEphemeral(ephermal).WithContent(content));
            }

            var config = ServiceProvider.GetService<Config>();
            if (config is { SendErrorsThroughSegment: true })
            {
                var analytics = ServiceProvider.GetService<IAnalyse>();
                if (analytics is not null)
                {
                    await analytics.EmitEvent(e.Context.User, "SlashCommandErrored", new Dictionary<string, object>()
                    {
                        {"commandname", e.Context.CommandName},
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
                    await RespondWithContent("Command not found.");
                }
                else
                {
                    var lang = await Language.GetLanguageFromCtxAsync(e.Context);
                    switch (e.Exception)
                    {
                        case SlashExecutionChecksFailedException cfe when cfe.FailedChecks.Count is 1:
                            await RespondWithContent(RenderErrorMessageForAttribute(cfe.FailedChecks[0], lang,
                                e.Context.Guild != null, e));
                            break;

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
                                break;
                            }
                        case InvalidOverloadException:
                        case ArgumentException { Message: "Could not find a suitable overload for the command." }:
                            await RespondWithContent(string.Format(lang.InvalidOverload, e.Context.CommandName));
                            break;

                        /*case UnknownImageFormatException:
                            await RespondWithContent(lang.UnknownImageFormat);
                            break;*/

                        case AttachmentCountIncorrectException aa when aa.AttachmentCount == AttachmentCountIncorrect.TooManyAttachments:
                            await RespondWithContent(lang.WrongImageCount);
                            break;

                        case AttachmentCountIncorrectException:
                            await RespondWithContent(lang.NoImageGeneric);
                            break;

                        default:
                            await RespondWithContent(lang.GeneralException);
                            break;
                    }
                }
            }

            Log.Error(e.Exception,
                "Error `{ExceptionName}` encountered.\nGuild `{GuildId}`, channel `{ChannelId}`, user `{UserID}`\n```\nNA\n```", e.Exception.GetType().FullName, e.Context.Guild?.Id.ToString() ?? "None", e.Context.Channel?.Id.ToString(), e.Context.User?.Id.ToString() ?? "None");
        }
    }
}