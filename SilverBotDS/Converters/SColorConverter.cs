using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using ImageMagick;
using System;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using static SilverBotDS.Utils.ColorUtils;

namespace SilverBotDS.Converters;
public static class ColorConverter
{
    public static Color? Convert(string value)
    {
        if (uint.TryParse(value.Replace("#", ""), NumberStyles.HexNumber, null, out var color))
        {
            byte[] intBytes = BitConverter.GetBytes(color);
            if (intBytes[3] != 0)
            {
                return  Color.FromArgb(intBytes[3],intBytes[2], intBytes[1], intBytes[0]);
            }
            else
            {
                return Color.FromArgb(intBytes[2], intBytes[1], intBytes[0]);
            }
        }
        //TODO PARSE TEXT

        return null;
    }
}
public class SColorConverter : IArgumentConverter<Color>
{
    public Task<Optional<Color>> ConvertAsync(string value, CommandContext ctx)
    {
        if(uint.TryParse(value.Replace("#",""), NumberStyles.HexNumber,null, out var color))
        {
            byte[] intBytes = BitConverter.GetBytes(color);
            if (intBytes[3]!=0)
            {
                return Task.FromResult(new Optional<Color>(Color.FromArgb(intBytes[3],intBytes[2], intBytes[1], intBytes[0])));
            }
            else
            {
                return Task.FromResult(new Optional<Color>(Color.FromArgb(intBytes[2], intBytes[1], intBytes[0])));
            }

        }
        //TODO PARSE TEXT
        return Task.FromResult(Optional.FromNoValue<Color>());
    }
}