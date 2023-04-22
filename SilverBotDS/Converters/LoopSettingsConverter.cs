/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using SilverBot.Shared.Objects.Classes;

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
                _ => Task.FromResult(Optional.FromNoValue<LoopSettings>())
            };
        }
    }
}