using System.Linq;
using ImageMagick;
using SilverBot.Shared;
using SilverBot.Shared.Utils;

namespace SilverBotDS.ProgramExtensions
{
    public class FontAssetSchemeChecker :IAssetSchemeChecker
    {
        public string Scheme => "font://";
        public bool CheckForAsset(string asset)
        {
            var font = asset.RemoveStringFromStart("font://").Replace(' ','-');
            return MagickNET.FontNames.Contains(font);
        }
    }
}