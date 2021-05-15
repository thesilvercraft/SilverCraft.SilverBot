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
    internal class VoteSettingsConverter : IArgumentConverter<VoteSettings>
    {
        public Task<Optional<VoteSettings>> ConvertAsync(string value, CommandContext ctx)
        {
            if (Enum.TryParse(typeof(VoteSettings), value, out var boolean))
            {
                return Task.FromResult(Optional.FromValue((VoteSettings)boolean));
            }
            return value.ToLower() switch
            {
                "queue" or "q" => Task.FromResult(Optional.FromValue(VoteSettings.LoopingQueue)),
                "no" or "none" or "off" => Task.FromResult(Optional.FromValue(VoteSettings.NotLooping)),
                "song" or "s" or "single" => Task.FromResult(Optional.FromValue(VoteSettings.LoopingSong)),
                _ => Task.FromResult(Optional.FromNoValue<VoteSettings>()),
            };
        }
    }
}