using System;
using System.Drawing;
using System.Globalization;

namespace SilverBotDS.Converters
{
    public static class ColorConverter
    {
        public static Color? Convert(string value)
        {
            if (uint.TryParse(value.Replace("#", ""), NumberStyles.HexNumber, null, out var color))
            {
                var intBytes = BitConverter.GetBytes(color);
                return intBytes[3] != 0
                    ? Color.FromArgb(intBytes[3], intBytes[2], intBytes[1], intBytes[0])
                    : Color.FromArgb(intBytes[2], intBytes[1], intBytes[0]);
            }

            if (!Enum.TryParse(typeof(KnownColor), value, out var result))
            {
                return null;
            }

            if (result is KnownColor kc)
            {
                return Color.FromKnownColor(kc);
            }

            return null;
        }
    }
}