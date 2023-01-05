/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Text.Json;
using DSharpPlus.Entities;

namespace SilverBot.Shared.Utils;

public class ColorUtils
{
    private static DiscordColor[] cache;

    private static DiscordColor[] Internal { get; } =
    {
        DiscordColor.Red,
        DiscordColor.Green,
        DiscordColor.Blue,
        new(105, 105, 255), // SAP purple #6969ff
        new (0,237,239) // SilverCraftBlue #00EDEF
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

        if (!ignorecache && cache != null)
        {
            return cache;
        }

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
    public static DiscordColor[] Get(bool ignorecache = false, bool useinternal = false)
    {
        if (useinternal)
        {
            return Internal;
        }

        if (!ignorecache && cache != null)
        {
            return cache;
        }

        if (File.Exists("colors.json"))
        {
            using StreamReader reader = new("colors.json");
            var colors = JsonSerializer.Deserialize<SdColor[]>(reader.ReadToEnd());
            cache = (colors ?? Array.Empty<SdColor>()).Select(sdcolor => sdcolor.ToDiscordColor()).ToArray();
            return cache;
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        using StreamWriter writer = new("colors.json");
        writer.Write(JsonSerializer.Serialize(Internal.Select(SdColor.FromDiscordColor).ToArray(),
            options));
        return Internal;
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
    ///     Gets a single (random) <see cref="DiscordColor" />
    /// </summary>
    /// <param name="ignorecache">Should it reload the colors.json</param>
    /// <param name="useinternal">Should it ignore colors.json altogether</param>
    /// <returns>a single (random) <see cref="DiscordColor" /></returns>
    public static DiscordColor GetSingle(bool ignorecache = false, bool useinternal = false)
    {
        var arr = Get(ignorecache, useinternal);
        return arr[RandomGenerator.Next(0, arr.Length)];
    }

    /// <summary>
    ///     Gets a single (random) <see cref="DiscordColor" />
    /// </summary>
    /// <param name="ignorecache">Should it reload the colors.json</param>
    /// <param name="useinternal">Should it ignore colors.json altogether</param>
    /// <returns>a single (random) <see cref="DiscordColor" /></returns>
    public static async Task<DiscordColor> GetSingleAsync(bool ignorecache = false, bool useinternal = false)
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