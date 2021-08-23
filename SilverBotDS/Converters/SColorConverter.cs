using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using SColor = SixLabors.ImageSharp.Color;

namespace SilverBotDS.Converters
{
    public class SColorConverter : IArgumentConverter<SColor>
    {
        public Task<Optional<SColor>> ConvertAsync(string value, CommandContext ctx)
        {
            if (SColor.TryParse(value, out SColor tintcolor))
            {
                return Task.FromResult(Optional.FromValue(tintcolor));
            }
            return Task.FromResult(Optional.FromNoValue<SColor>());
        }
    }
}