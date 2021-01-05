using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CefSharp;
using System.Threading;
using System.Collections.Concurrent;

namespace SilverBotDS
{
    internal class browser
    {
        private static ChromiumWebBrowser CEFBrowser;
        private static ConcurrentDictionary<string, Image> concurrentDictionary = new();

        public static async Task<Image> ScreenshotAsync(string url)
        {
            if (CEFBrowser == null)
            {
                CEFBrowser = new ChromiumWebBrowser(url);
                while (!CEFBrowser.IsBrowserInitialized)
                {
                    await Task.Delay(200);
                }
                CEFBrowser.LoadingStateChanged += BrowserLoadingStateChanged;
            }
            else
            {
                CEFBrowser.LoadingStateChanged += BrowserLoadingStateChanged;
                while (!CEFBrowser.IsBrowserInitialized)
                {
                    await Task.Delay(200);
                }
                CEFBrowser.Load(url);
            }
            while (!concurrentDictionary.ContainsKey("e"))
            {
                await Task.Delay(200);
            }

            concurrentDictionary.TryRemove("e", out Image img);
            return img;
        }

        public static async Task<Image> ScreenshotThingAsync(string html)
        {
            try
            {
                if (CEFBrowser == null)
                {
                    CEFBrowser = new ChromiumWebBrowser();
                    CEFBrowser.LoadingStateChanged += BrowserLoadingStateChanged;
                    while (!CEFBrowser.IsBrowserInitialized)
                    {
                        await Task.Delay(200);
                    }
                    CEFBrowser.LoadHtml(html);
                }
                else
                {
                    CEFBrowser.LoadingStateChanged += BrowserLoadingStateChanged;
                    while (!CEFBrowser.IsBrowserInitialized)
                    {
                        await Task.Delay(200);
                    }
                    CEFBrowser.LoadHtml(html);
                }
                while (!concurrentDictionary.ContainsKey("e"))
                {
                    await Task.Delay(200);
                }

                concurrentDictionary.TryRemove("e", out Image img);
                return img;
            }
            catch (Exception exep)
            {
                Console.WriteLine(exep);
                throw;
            }
        }

        private static void BrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            // Check to see if loading is complete - this event is called twice, one when loading starts
            // second time when it's finished
            // (rather than an iframe within the main frame).
            if (!e.IsLoading)
            {
                // Remove the load event handler, because we only want one snapshot of the initial page.
                CEFBrowser.LoadingStateChanged -= BrowserLoadingStateChanged;
                Console.WriteLine(sender.GetType());

                //Give the browser a little time to render
                Thread.Sleep(500);
                // Wait for the screenshot to be taken.
                var task = CEFBrowser.ScreenshotAsync();
                task.ContinueWith(x =>
                {
                    concurrentDictionary.TryAdd("e", task.Result);

                    // We no longer need the Bitmap.
                    // Dispose it to avoid keeping the memory alive.  Especially important in 32-bit applications.
                    //task.Result.Dispose();
                }, TaskScheduler.Default);
            }
        }
    }
}