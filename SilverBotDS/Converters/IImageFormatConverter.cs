/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System.Threading.Tasks;
namespace SilverBotDS.Converters;
/*
internal class ImageFormatConverter : IArgumentConverter<MagickFormat>
{
    public Task<Optional<MagickFormat>> ConvertAsync(string value, CommandContext ctx)
    {
        return value.ToLowerInvariant() switch
        {
            "bmp" => Task.FromResult(Optional.FromValue(MagickFormat.Bmp)),
            "gif" => Task.FromResult(Optional.FromValue(MagickFormat.Gif)),
            "ico" => Task.FromResult(Optional.FromValue(MagickFormat.Ico)),
            "jpeg" or "jpg" => Task.FromResult(Optional.FromValue(MagickFormat.Jpeg)),
            "png" => Task.FromResult(Optional.FromValue(MagickFormat.Png)),
            "wbmp" => Task.FromResult(Optional.FromValue(MagickFormat.Wbmp)),
            "webp" => Task.FromResult(Optional.FromValue(MagickFormat.WebP)),
            "dng" => Task.FromResult(Optional.FromValue(MagickFormat.Dng)),
            "tga" => Task.FromResult(Optional.FromValue(MagickFormat.Tga)),
            "tiff" or "tif" => Task.FromResult(Optional.FromValue(MagickFormat.Tiff)),
            "xcf"  => Task.FromResult(Optional.FromValue(MagickFormat.Xcf)),
            "heif" => Task.FromResult(Optional.FromValue(MagickFormat.Heif)),
            "avif" => Task.FromResult(Optional.FromValue(MagickFormat.Avif)),
            _ => Task.FromResult(Optional.FromNoValue<MagickFormat>())
        };
    }
}
*/
