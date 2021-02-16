using DSharpPlus.Entities;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SilverBotDS.Utils
{
    public class ColorUtils
    {
        public static ColorUtils CreateInstance()
        {
            return new ColorUtils();
        }

        private static DiscordColor[] Internal { get; } = { DiscordColor.Red, DiscordColor.Green, DiscordColor.Blue };
        private static DiscordColor[] cache;

        /// <summary>
        /// Gets an array of <see cref="DiscordColor"/>s
        /// </summary>
        /// <param name="ignorecache">Should it reload the colors.json</param>
        /// <param name="useinternal">Should it ignore colors.json alltogether</param>
        /// <returns>an array of <see cref="DiscordColor"/>s</returns>
        public static async Task<DiscordColor[]> GetAsync(bool ignorecache = false, bool useinternal = false)
        {
            if (useinternal)
            {
                return Internal;
            }
            if (ignorecache || cache == null)
            {
                if (File.Exists("colors.json"))
                {
                    using StreamReader reader = new("colors.json");
                    var colors = JsonSerializer.Deserialize<SdColor[]>(json: await reader.ReadToEndAsync());

                    cache = (colors ?? Array.Empty<SdColor>()).Select(sdcolor => sdcolor.ToDiscordColor()).ToArray();
                    return cache;
                }
                else
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    await using StreamWriter writer = new("colors.json");
                    await writer.WriteAsync(JsonSerializer.Serialize(Internal.Select(SdColor.FromDiscordColor).ToArray(), options));
                    return Internal;
                }
            }
            else
            {
                return cache;
            }
        }

        public class SdColor
        {
            public DiscordColor ToDiscordColor()
            {
                return new DiscordColor(R, G, B);
            }

            public static SdColor FromDiscordColor(DiscordColor color)
            {
                return new SdColor { R = color.R, G = color.G, B = color.B };
            }

            private byte R { get; init; }
            private byte G { get; init; }
            private byte B { get; init; }
        }

        /// <summary>
        /// Gets a single (random) <see cref="DiscordColor"/>
        /// </summary>
        /// <param name="ignorecache">Should it reload the colors.json</param>
        /// <param name="useinternal">Should it ignore colors.json alltogether</param>
        /// <returns>a single (random) <see cref="DiscordColor"/></returns>
        public static async Task<DiscordColor> GetSingleAsync(bool ignorecache = false, bool useinternal = false)
        {
            RandomGenerator rg = new();
            var arr = await GetAsync(ignorecache, useinternal);
            return arr[rg.Next(0, arr.Length)];
        }
    }
}