/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using DSharpPlus.SlashCommands;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using DSharpPlus.SlashCommands.Attributes;
using NetVips;
using SilverBot.Shared.Objects;
using SilverBot.Shared.Objects.Database.Classes;
using SilverBot.Shared.Utils;

namespace SilverBotDS.Commands.Slash
{
    public class BubotSlash : ApplicationCommandModule, IHaveExecutableRequirements,IRequireAssets
    {
        private string[] _bibiDescText;
        private string[] _bibiFullDescText;
        public Config Config { private get; set; }

        public Task<bool> ExecuteRequirements(Config conf) => Task.FromResult(Directory.Exists(conf.LocalBibiPictures));

        private void EnsureCreated()
        {
            _bibiDescText ??= GetBibiDescText();
            _bibiFullDescText ??= GetBibiFullDescText();
        }

        private string[] GetBibiDescText()
        {
            using StreamReader reader = new(Config.BibiLibCutOutConfig);
            return JsonSerializer.Deserialize<string[]>(reader.ReadToEnd());
        }

        private string[] GetBibiFullDescText()
        {
            using StreamReader reader = new(Config.BibiLibFullConfig);
            return JsonSerializer.Deserialize<string[]>(reader.ReadToEnd());
        }
        public static string[] RequiredAssets => new[]
        {
            "font://Arial",
        };


        private int BibiPictureCount
        {
            get { return Directory.EnumerateFiles(Config.LocalBibiPictures).Count(x => x.EndsWith(".png")); }
        }

      

        [SlashCommand("bibi", "Makes a image with the great cat Bibi.")]
        [SlashCooldown(1,2,SlashCooldownBucketType.User)]
        public async Task Bibi(InteractionContext ctx, [Option("text", "Bibi is **text**")]  string input)
        {
            input = $"bibi is {input}";
            var randomnumber = RandomGenerator.Next(1, BibiPictureCount);
            using var picture = Image.NewFromFile(Path.Combine(Config.LocalBibiPictures,$"{randomnumber}.png"));
            using var textImage = Image.Text($"<span foreground=\"{(randomnumber is 10 or 9 ? "#808080ff" : "#ffffffff")}\">" + HttpUtility.HtmlEncode(input) + "</span>",
                "Noto Color Emoji, Arial normal " + 30,
                rgba: true,
                align: Enums.Align.Low, width: picture.Width);
            using var compositedImage = picture.Composite(textImage, Enums.BlendMode.Over, x: 4, y: 230); // overlay text image
            await using var outStream = new MemoryStream();
            compositedImage.WriteToStream(outStream, ".png");
            outStream.Position = 0;
            await ImageModule.SendImageStreamIfAllowed(ctx, outStream, DisposeOfStream:true, content: input);
        }

    }
}
