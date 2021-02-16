using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace SDBrowser
{
    public sealed class SeleniumBrowser : IBrowser
    {
        private readonly IWebDriver _webDriver;
        private bool _isLocked = false;

        public SeleniumBrowser(IWebDriver driver)
        {
            _webDriver = driver;
        }

        public static SeleniumBrowser FromBrowserType(Browsertype browsertype)
        {
            return new SeleniumBrowser(browsertype, Environment.CurrentDirectory);
        }

        public SeleniumBrowser(Browsertype browsertype, string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                location = Environment.CurrentDirectory;
            }
            switch (browsertype)
            {
                case Browsertype.Chrome:
                    {
                        var chromeOptions = new ChromeOptions();
                        chromeOptions.AddArguments("headless");
                        _webDriver = new ChromeDriver(location, chromeOptions);
                        _webDriver.Manage().Window.Size = new Size(1920, 1080);
                        break;
                    }
                case Browsertype.Firefox:
                    {
                        var firefoxOptions = new FirefoxOptions();
                        firefoxOptions.AddArguments("--headless");
                        _webDriver = new FirefoxDriver(location, firefoxOptions);
                        _webDriver.Manage().Window.Size = new Size(1920, 1080);
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<Bitmap> RenderHtmlAsync(string html)
        {
            while (_isLocked)
            {
                await Task.Delay(500);
            }
            _isLocked = true;
            var basehtml = Convert.ToBase64String(Encoding.UTF8.GetBytes(html));
            _webDriver.Url = "data:html;base64," + html;
            _webDriver.Navigate();
            IWait<IWebDriver> wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(30.00));

            wait.Until(driver1 => ((IJavaScriptExecutor)_webDriver).ExecuteScript("return document.readyState").Equals("complete")); var ss = ((ITakesScreenshot)_webDriver).GetScreenshot();
            _isLocked = false;
            return Utils.ByteArrayToImage(ss.AsByteArray);
        }

        public async Task<Bitmap> RenderUrlAsync(string url)
        {
            while (_isLocked)
            {
                await Task.Delay(500);
            }
            _isLocked = true;
            try
            {
                _webDriver.Url = url;
                _webDriver.Navigate();
                IWait<IWebDriver> wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(30.00));

                wait.Until(driver1 => ((IJavaScriptExecutor)_webDriver).ExecuteScript("return document.readyState").Equals("complete")); var ss = ((ITakesScreenshot)_webDriver).GetScreenshot();
                _isLocked = false;
                return Utils.ByteArrayToImage(ss.AsByteArray);
            }
            catch (Exception e)
            {
                _isLocked = false;
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<Bitmap> RenderUrlAsync(Uri url)
        {
            return await RenderUrlAsync(url.ToString());
        }
    }
}