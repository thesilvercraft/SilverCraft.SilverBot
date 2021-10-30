using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using TimeSpanParserUtil;

namespace SilverBotDS.Converters
{
    internal class TimeSpanConverter : IArgumentConverter<TimeSpan>
    {
        public Task<Optional<TimeSpan>> ConvertAsync(string value, CommandContext ctx)
        {
            if (TimeSpanParser.TryParse(value, Units.Minutes, Units.Seconds, out TimeSpan gamer))
            {
                return Task.FromResult(Optional.FromValue(gamer));
            }
            return Task.FromResult(Optional.FromNoValue<TimeSpan>());
        }
    }
}