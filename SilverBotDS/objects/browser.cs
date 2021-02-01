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
    internal class Browser
    {
        private static ChromiumWebBrowser CEFBrowser;
        private static readonly ConcurrentDictionary<string, Image> concurrentDictionary = new();

        public static async Task<Image> ScreenshotAsync(string url)
        {
            string loc = new RandomGenerator().RandomString(69);
            if (CEFBrowser == null)
            {
                CEFBrowser = new ChromiumWebBrowser(url);
                while (!CEFBrowser.IsBrowserInitialized)
                {
                    await Task.Delay(200);
                }
                CEFBrowser.LoadingStateChanged += (o, args) =>
                {
                    BrowserLoadingStateChanged(o, args, loc);
                };
            }
            else
            {
                CEFBrowser.LoadingStateChanged += (o, args) =>
                {
                    BrowserLoadingStateChanged(o, args, loc);
                };
                while (!CEFBrowser.IsBrowserInitialized)
                {
                    await Task.Delay(200);
                }
                CEFBrowser.Load(url);
            }

            while (!concurrentDictionary.ContainsKey(loc))
            {
                await Task.Delay(200);
            }

            concurrentDictionary.TryRemove(loc, out Image img);
            return img;
        }

        public static async Task<Image> ScreenshotThingAsync(string html)
        {
            try
            {
                string loc = new RandomGenerator().RandomString(69);

                if (CEFBrowser == null)
                {
                    CEFBrowser = new ChromiumWebBrowser();

                    CEFBrowser.LoadingStateChanged += (o, args) =>
                    {
                        BrowserLoadingStateChanged(o, args, loc);
                    };
                    while (!CEFBrowser.IsBrowserInitialized)
                    {
                        await Task.Delay(200);
                    }
                    CEFBrowser.LoadHtml(html);
                }
                else
                {
                    CEFBrowser.LoadingStateChanged += (o, args) =>
                    {
                        BrowserLoadingStateChanged(o, args, loc);
                    };
                    while (!CEFBrowser.IsBrowserInitialized)
                    {
                        await Task.Delay(200);
                    }
                    CEFBrowser.LoadHtml(html);
                }
                while (!concurrentDictionary.ContainsKey(loc))
                {
                    await Task.Delay(200);
                }

                concurrentDictionary.TryRemove(loc, out Image img);
                return img;
            }
            catch (Exception exep)
            {
                Console.WriteLine(exep);
                throw;
            }
        }

        private static void BrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e, string id)
        {
            // Check to see if loading is complete - this event is called twice, one when loading starts
            // second time when it's finished
            // (rather than an iframe within the main frame).
            if (!e.IsLoading)
            {
                // Remove the load event handler, because we only want one snapshot of the initial page.
                CEFBrowser.LoadingStateChanged -= (o, args) =>
                {
                    BrowserLoadingStateChanged(o, args, id);
                };
                Console.WriteLine(sender.GetType());

                //Give the browser a little time to render
                Thread.Sleep(500);
                // Wait for the screenshot to be taken.
                var task = CEFBrowser.ScreenshotAsync();
                task.ContinueWith(x =>
                {
                    Console.WriteLine("We are located at" + ((ChromiumWebBrowser)sender).Address + " and serving " + id);
                    concurrentDictionary.TryAdd(id, task.Result);
                }, TaskScheduler.Default);
            }
        }
    }
}