using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using ImageMagick;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace SilverBotDS.Converters;
public static class ColorConverter
{
    public static MagickColor? Convert(string value)
    {
        if (uint.TryParse(value.Replace("#", ""), NumberStyles.HexNumber, null, out var color))
        {
            byte[] intBytes = BitConverter.GetBytes(color);
            if (intBytes[3] != 0)
            {
                return MagickColor.FromRgba(intBytes[0], intBytes[1], intBytes[2], intBytes[3]);
            }
            else
            {
                return MagickColor.FromRgb(intBytes[0], intBytes[1], intBytes[2]);
            }
        }
        //TODO PARSE TEXT

        return null;
    }
}
public class SColorConverter : IArgumentConverter<MagickColor>
{
    public Task<Optional<MagickColor>> ConvertAsync(string value, CommandContext ctx)
    {
        if(uint.TryParse(value.Replace("#",""), NumberStyles.HexNumber,null, out var color))
        {
            byte[] intBytes = BitConverter.GetBytes(color);
            if (intBytes[3]!=0)
            {
                return Task.FromResult(new Optional<MagickColor>(MagickColor.FromRgba(intBytes[2], intBytes[1], intBytes[0], intBytes[3])));
            }
            else
            {
                return Task.FromResult(new Optional<MagickColor>(MagickColor.FromRgb(intBytes[2], intBytes[1], intBytes[0])));
            }

        }
        //TODO PARSE TEXT
        return Task.FromResult(Optional.FromNoValue<MagickColor>());
    }
}