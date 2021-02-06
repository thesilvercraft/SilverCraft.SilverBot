using System.Linq;

namespace SilverBotDS
{
    internal class FileUtils
    {
        /// <summary>
        /// https://stackoverflow.com/questions/23228378/is-there-any-way-to-get-the-file-extension-from-a-url
        /// </summary>
        /// <param name="url">the url</param>
        /// <returns>the extension</returns>
        public static string GetFileExtensionFromUrl(string url)
        {
            url = url.Split('?')[0];
            url = url.Split('/').Last();
            return url.Contains('.') ? url[url.LastIndexOf('.')..] : "";
        }
    }
}