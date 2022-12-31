/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using SilverBot.Shared.Attributes;
using SilverBot.Shared.Objects.Language;
using SilverBot.Shared.Utils;

namespace SilverBot.Shared.Objects.Classes;

public class CoolerHelpFormatter : BaseHelpFormatter
{
    /// <summary>
    ///     Creates a new default help formatter.
    /// </summary>
    /// <param name="ctx">Context in which this formatter is being invoked.</param>
    public CoolerHelpFormatter(CommandContext ctx)
        : base(ctx)
    {
        var languageservice = ctx.Services.GetService<LanguageService>();
        Lang = languageservice?.FromCtx(ctx);
        EmbedBuilder = new DiscordEmbedBuilder()
            .WithTitle(Lang.HelpCommandHelpString).AddRequestedByFooter(ctx,Lang);
    }

    public DiscordEmbedBuilder EmbedBuilder { get; }
    private Command Command { get; set; }
    private Language.Language Lang { get; }

    /// <summary>
    ///     Sets the command this help message will be for.
    /// </summary>
    /// <param name="command">Command for which the help message is being produced.</param>
    /// <returns>This help formatter.</returns>
    public override BaseHelpFormatter WithCommand(Command command)
    {
        Command = command;
        EmbedBuilder.WithDescription(
            $"{Formatter.InlineCode(command.Name)}: {command.Description ?? Lang.HelpCommandNoDescription}");
        if (command is CommandGroup { IsExecutableWithoutSubcommands: true })
        {
            EmbedBuilder.WithDescription($"{EmbedBuilder.Description}\n{Lang.HelpCommandGroupCanBeExecuted}");
        }

        if (command.Aliases?.Any() == true)
        {
            EmbedBuilder.AddField(Lang.HelpCommandGroupAliases,
                string.Join(", ", command.Aliases.Select(Formatter.InlineCode)));
        }

        if (command.Overloads?.Any() != true)
        {
            return this;
        }

        var sb = new StringBuilder();
        foreach (var ovl in command.Overloads.OrderByDescending(x => x.Priority).Select(ovl => ovl.Arguments))
        {
            sb.Append('`').Append(command.QualifiedName);
            foreach (var arg in ovl)
            {
                sb.Append(arg.IsOptional || arg.IsCatchAll ? " [" : " <").Append(arg.Name)
                    .Append(arg.IsCatchAll ? "..." : "").Append(arg.IsOptional || arg.IsCatchAll ? ']' : '>');
            }

            sb.Append("`\n");
            foreach (var arg in ovl)
            {
                sb.Append('`').Append(arg.Name).Append(" (").Append(CommandsNext.GetUserFriendlyTypeName(arg.Type))
                    .Append(")`: ").Append(arg.Description ?? Lang.HelpCommandNoDescription).Append('\n');
            }

            sb.Append('\n');
        }

        EmbedBuilder.AddField(Lang.HelpCommandGroupArguments, sb.ToString().Trim());

        return this;
    }

    /// <summary>
    ///     Sets the subcommands for this command, if applicable. This method will be called with filtered data.
    /// </summary>
    /// <param name="subcommands">Subcommands for this command group.</param>
    /// <returns>This help formatter.</returns>
    public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
    {
        if (Command == null)
        {
            Dictionary<string, HashSet<string>> commands = new();
            foreach (var command in subcommands)
            {
                if (command.CustomAttributes.Any(x => x.GetType() == typeof(CategoryAttribute)))
                {
                    foreach (var attribute in command.CustomAttributes.Where(x =>
                                 x.GetType() == typeof(CategoryAttribute)))
                    {
                        foreach (var category in ((CategoryAttribute)attribute).Category)
                        {
                            if (!commands.ContainsKey(category))
                            {
                                commands.Add(category, new HashSet<string>());
                            }

                            commands[category].Add(command.Name);
                        }
                    }
                }

                if (command.Module != null && command.Module.ModuleType.GetCustomAttributes(true)
                        .Any(x => x.GetType() == typeof(CategoryAttribute)))
                {
                    foreach (var attribute in command.Module.ModuleType.GetCustomAttributes(true)
                                 .Where(x => x is CategoryAttribute))
                    {
                        foreach (var category in ((CategoryAttribute)attribute).Category)
                        {
                            if (!commands.ContainsKey(category))
                            {
                                commands.Add(category, new HashSet<string>());
                            }

                            commands[category].Add(command.Name);
                        }
                    }
                }
            }

            foreach (var category in commands.Keys)
            {
                EmbedBuilder.AddField(category,
                    string.Join(", ", commands[category].Select(x => Formatter.InlineCode(x))));
            }
        }
        else
        {
            EmbedBuilder.AddField(Lang.HelpCommandGroupSubcommands,
                string.Join(", ", subcommands.Select(x => Formatter.InlineCode(x.Name))));
        }

        return this;
    }

    /// <summary>
    ///     Construct the help message.
    /// </summary>
    /// <returns>Data for the help message.</returns>
    public override CommandHelpMessage Build()
    {
        if (Command == null)
        {
            EmbedBuilder.WithDescription(Lang.HelpCommandGroupListingAllCommands);
        }
        EmbedBuilder.Color = ColorUtils.GetSingle();
        return new CommandHelpMessage(embed: EmbedBuilder.Build());
    }
}