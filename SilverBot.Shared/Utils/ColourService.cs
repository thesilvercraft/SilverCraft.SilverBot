/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Text.Json;
using DSharpPlus.Entities;

namespace SilverBot.Shared.Utils
{
    public class ColourService
    {
        private DiscordColor[] cache;

        private DiscordColor[] Internal { get; } =
        {
            DiscordColor.Red,
            DiscordColor.Green,
            DiscordColor.Blue,
            new(105, 105, 255), // SAP purple #6969ff
            new(0, 237, 239) // SilverCraftBlue #00EDEF
        };

        public DiscordColor[] Get(bool ignorecache = false, bool useinternal = false)
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
                ReadConfig();
                return cache;
            }

            WriteConfig();
            return Internal;
        }

        public void ReadConfig()
        {
            if (File.Exists("colors.json"))
            {
                using StreamReader reader = new("colors.json");
                var colors = JsonSerializer.Deserialize<SdColor[]>(reader.ReadToEnd());
                cache = (colors ?? Array.Empty<SdColor>()).Select(sdcolor => sdcolor.ToDiscordColor()).ToArray();
            }
        }

        public void WriteConfig()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            using StreamWriter writer = new("colors.json");
            writer.Write(JsonSerializer.Serialize(Internal.Select(SdColor.FromDiscordColor).ToArray(),
                options));
        }

        /// <summary>
        ///     Gets a single (random) <see cref="DiscordColor" />
        /// </summary>
        /// <param name="ignorecache">Should it reload the colors.json</param>
        /// <param name="useinternal">Should it ignore colors.json altogether</param>
        /// <returns>a single (random) <see cref="DiscordColor" /></returns>
        public DiscordColor GetSingle(bool ignorecache = false, bool useinternal = false)
        {
            var arr = Get(ignorecache, useinternal);
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
}