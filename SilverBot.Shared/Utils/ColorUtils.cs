﻿using DSharpPlus.Entities;
using System.Text.Json;

namespace SilverBotDS.Utils;

public class ColorUtils
{
    private static DiscordColor[] cache;

    private static DiscordColor[] Internal { get; } =
    {
        DiscordColor.Red,
        DiscordColor.Green,
        DiscordColor.Blue,
    };

    public static ColorUtils CreateInstance()
    {
        return new ColorUtils();
    }

    /// <summary>
    ///     Gets an array of <see cref="DiscordColor" />s
    /// </summary>
    /// <param name="ignorecache">Should it reload the colors.json</param>
    /// <param name="useinternal">Should it ignore colors.json alltogether</param>
    /// <returns>an array of <see cref="DiscordColor" />s</returns>
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
                var colors = JsonSerializer.Deserialize<SdColor[]>(await reader.ReadToEndAsync());
                cache = (colors ?? Array.Empty<SdColor>()).Select(sdcolor => sdcolor.ToDiscordColor()).ToArray();
                return cache;
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            await using StreamWriter writer = new("colors.json");
            await writer.WriteAsync(JsonSerializer.Serialize(Internal.Select(SdColor.FromDiscordColor).ToArray(),
                options));
            return Internal;
        }
        return cache;
    }

    /// <summary>
    ///     Reloads the config
    /// </summary>
    /// <returns>Nothing</returns>
    public static async Task ReloadConfig()
    {
        await GetAsync(true);
    }

    /// <summary>
    ///     Gets a single (random) <see cref="DiscordColor" /> with no parameters
    /// </summary>
    /// <returns>a single (random) <see cref="DiscordColor" /></returns>
    public static async Task<DiscordColor> GetSingleAsync()
    {
        return await GetSingleAsyncInternal();
    }

    /// <summary>
    ///     Gets a single (random) <see cref="DiscordColor" />
    /// </summary>
    /// <param name="ignorecache">Should it reload the colors.json</param>
    /// <param name="useinternal">Should it ignore colors.json altogether</param>
    /// <returns>a single (random) <see cref="DiscordColor" /></returns>
    internal static async Task<DiscordColor> GetSingleAsyncInternal(bool ignorecache = false, bool useinternal = false)
    {
        var arr = await GetAsync(ignorecache, useinternal);
        return arr[RandomGenerator.Next(0, arr.Length)];
    }

    public class SdColor
    {
        public byte R { get; init; }
        public byte G { get; init; }
        public byte B { get; init; }

        public DiscordColor ToDiscordColor()
        {
            return new DiscordColor(R, G, B);
        }

        public static SdColor FromDiscordColor(DiscordColor color)
        {
            return new SdColor { R = color.R, G = color.G, B = color.B };
        }
    }
}