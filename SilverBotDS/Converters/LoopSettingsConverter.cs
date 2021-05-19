using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using SilverBotDS.Objects.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS.Converters
{
    internal class LoopSettingsConverter : IArgumentConverter<LoopSettings>
    {
        public Task<Optional<LoopSettings>> ConvertAsync(string value, CommandContext ctx)
        {
            if (Enum.TryParse(typeof(LoopSettings), value, out var boolean))
            {
                return Task.FromResult(Optional.FromValue((LoopSettings)boolean));
            }
            return value.ToLowerInvariant() switch
            {
                "queue" or "q" => Task.FromResult(Optional.FromValue(LoopSettings.LoopingQueue)),
                "n" or "no" or "none" or "off" => Task.FromResult(Optional.FromValue(LoopSettings.NotLooping)),
                "song" or "s" or "single" => Task.FromResult(Optional.FromValue(LoopSettings.LoopingSong)),
                _ => Task.FromResult(Optional.FromNoValue<LoopSettings>()),
            };
        }
    }
}