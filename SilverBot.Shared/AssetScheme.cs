using SilverBotDS.Utils;

namespace SilverBot.Shared
{
    public interface IAssetSchemeChecker
    {
        public string Scheme { get; }

        public bool CheckForAsset(string asset);
    }

    public class FileAssetSchemeChecker :IAssetSchemeChecker
    {
        public string Scheme => "file://";
        public bool CheckForAsset(string asset)
        {
            var file = asset.RemoveStringFromStart("file://");
            return File.Exists(file);
        }
    }
    
}