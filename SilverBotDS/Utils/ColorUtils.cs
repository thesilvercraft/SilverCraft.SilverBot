using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SilverBotDS.Utils
{
    internal class ColorUtils
    {
        private static DiscordColor[] Internal { get; } = { DiscordColor.Red, DiscordColor.Green, DiscordColor.Blue };
        private static DiscordColor[] Cache;

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
            if (ignorecache || Cache == null)
            {
                if (File.Exists("colors.json"))
                {
                    using StreamReader reader = new("colors.json");
                    List<DiscordColor> list = new();
                    foreach (var sdcolor in JsonSerializer.Deserialize<SDColor[]>(json: await reader.ReadToEndAsync()))
                    {
                        list.Add(sdcolor.ToDiscordColor());
                    }
                    Cache = list.ToArray();
                    return Cache;
                }
                else
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    using StreamWriter writer = new("colors.json");
                    List<SDColor> list = new();
                    foreach (var dcolor in Internal)
                    {
                        list.Add(SDColor.FromDiscordColor(dcolor));
                    }
                    writer.Write(JsonSerializer.Serialize(list.ToArray(), options));
                    return Internal;
                }
            }
            else
            {
                return Cache;
            }
        }

        public class SDColor
        {
            public DiscordColor ToDiscordColor()
            {
                return new DiscordColor(R, G, B);
            }

            public static SDColor FromDiscordColor(DiscordColor color)
            {
                return new SDColor { R = color.R, G = color.G, B = color.B };
            }

            public byte R { get; set; }
            public byte G { get; set; }
            public byte B { get; set; }
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
            DiscordColor[] arr = await GetAsync(ignorecache, useinternal);
            return arr[rg.Next(0, arr.Length)];
        }
    }
}