using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using SilverBotDS.Attributes;
using SilverBotDS.Utils;
using System.Text;

namespace SilverBotDS.Objects.Classes;

public class CustomHelpFormatter : BaseHelpFormatter
{
    /// <summary>
    ///     Creates a new default help formatter.
    /// </summary>
    /// <param name="ctx">Context in which this formatter is being invoked.</param>
    public CustomHelpFormatter(CommandContext ctx)
        : base(ctx)
    {
        var langtask = Language.GetLanguageFromCtxAsync(ctx);
        langtask.Wait();
        Lang = langtask.Result;
        EmbedBuilder = new DiscordEmbedBuilder()
            .WithTitle(Lang.HelpCommandHelpString).WithFooter(Lang.RequestedBy + ctx.User.Username,
                ctx.User.GetAvatarUrl(ImageFormat.Auto));
    }

    public DiscordEmbedBuilder EmbedBuilder { get; }
    private Command Command { get; set; }
    private Language Lang { get; }

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
        if (command is CommandGroup cgroup && cgroup.IsExecutableWithoutSubcommands)
        {
            EmbedBuilder.WithDescription($"{EmbedBuilder.Description}\n{Lang.HelpCommandGroupCanBeExecuted}");
        }

        if (command.Aliases?.Any() == true)
        {
            EmbedBuilder.AddField(Lang.HelpCommandGroupAliases,
                string.Join(", ", command.Aliases.Select(Formatter.InlineCode)));
        }

        if (command.Overloads?.Any() == true)
        {
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
        }

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

                if (command.Module.ModuleType.GetCustomAttributes(true)
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

        var colortask = ColorUtils.GetSingleAsync();
        colortask.Wait();
        EmbedBuilder.Color = colortask.Result;
        return new CommandHelpMessage(embed: EmbedBuilder.Build());
    }
}