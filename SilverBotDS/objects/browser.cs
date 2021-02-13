using SDBrowser;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace SilverBotDS
{
    internal class Browser
    {
        private static IBrowser iBrowser;

        public static void SetBrowser(IBrowser browser)
        {
            iBrowser = browser;
        }

        internal static async Task<Image> ScreenshotHtmlAsync(string html)
        {
            return await iBrowser.RenderHtmlAsync(html);
        }

        internal static async Task<Image> ScreenshotAsync(string url)
        {
            return await iBrowser.RenderUrlAsync(url);
        }
    }
}