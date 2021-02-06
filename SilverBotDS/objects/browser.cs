using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace SilverBotDS
{
    internal class Browser
    {
        private static ChromiumWebBrowser CEFBrowser;

        private static bool isLocked = false;

        public static async Task<Image> ScreenshotAsync(string url)
        {
            while (isLocked)
            {
                await Task.Delay(200);
            }
            isLocked = true;
            Console.WriteLine("[Browser] Rendering " + url);
            if (CEFBrowser == null)
            {
                CEFBrowser = new ChromiumWebBrowser(url);
                while (!CEFBrowser.IsBrowserInitialized)
                {
                    await Task.Delay(200);
                }
            }
            else
            {
                while (!CEFBrowser.IsBrowserInitialized)
                {
                    await Task.Delay(200);
                }
                CEFBrowser.Load(url);
            }
            ushort timestried = 0;
            do
            {
                await Task.Delay(200);
                timestried++;
            } while (!CEFBrowser.IsLoading && timestried < 10);

            Console.WriteLine($"[Browser] {url} started loading after {timestried * 200}ms");
            timestried = 0;
            while (CEFBrowser.IsLoading)
            {
                await Task.Delay(200);
                timestried++;
            }
            Console.WriteLine($"[Browser] loaded {url} after {timestried * 200}ms, Now taking a screenshot");

            Stopwatch sw = new Stopwatch();

            sw.Start();

            var image = await CEFBrowser.ScreenshotAsync();
            Console.WriteLine($"[Browser] Took a screenshot and we are returning it");
            sw.Stop();

            Console.WriteLine("Took {0} to screenshot", sw.Elapsed);
            isLocked = false;
            return image;
        }

        public static async Task<Image> ScreenshotHtmlAsync(string html)
        {
            while (isLocked)
            {
                await Task.Delay(200);
            }
            isLocked = true;
            Console.WriteLine("[Browser] Rendering html");
            if (CEFBrowser == null)
            {
                CEFBrowser = new ChromiumWebBrowser();
                while (!CEFBrowser.IsBrowserInitialized)
                {
                    await Task.Delay(200);
                }
                CEFBrowser.LoadHtml(html);
            }
            else
            {
                while (!CEFBrowser.IsBrowserInitialized)
                {
                    await Task.Delay(200);
                }
                CEFBrowser.LoadHtml(html);
            }
            ushort timestried = 0;
            do
            {
                await Task.Delay(200);
                timestried++;
            } while (!CEFBrowser.IsLoading && timestried < 10);

            Console.WriteLine($"[Browser] html started loading after {timestried * 200}ms");
            timestried = 0;
            while (CEFBrowser.IsLoading)
            {
                await Task.Delay(200);
                timestried++;
            }
            Console.WriteLine($"[Browser] loaded html after {timestried * 200}ms, Now taking a screenshot");
            var image = await CEFBrowser.ScreenshotAsync(true);
            Console.WriteLine($"[Browser] Took a screenshot and we are returning it");
            isLocked = false;
            return image;
        }
    }
}