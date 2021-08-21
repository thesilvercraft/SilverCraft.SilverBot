using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverBotDS.Converters
{
    
    internal class IImageFormatConverter : IArgumentConverter<IImageFormat>
    {
        public Task<Optional<IImageFormat>> ConvertAsync(string value, CommandContext ctx)
        {
            return value.ToLowerInvariant() switch
            {
                "png" => Task.FromResult(Optional.FromValue<IImageFormat>(PngFormat.Instance)),
                "jpeg" or "jpg" => Task.FromResult(Optional.FromValue<IImageFormat>(JpegFormat.Instance)),
                "gif" => Task.FromResult(Optional.FromValue<IImageFormat>(GifFormat.Instance)),
                "tga" => Task.FromResult(Optional.FromValue<IImageFormat>(TgaFormat.Instance)),
                "bmp" => Task.FromResult(Optional.FromValue<IImageFormat>(BmpFormat.Instance)),
                _ => Task.FromResult(Optional.FromNoValue<IImageFormat>()),
            };
        }
    }
}
