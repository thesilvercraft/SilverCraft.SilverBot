using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace SDBrowser
{
    public sealed class SeleniumBrowser : IBrowser
    {
        private readonly IWebDriver _webDriver;
        private bool _isLocked;

        public SeleniumBrowser(IWebDriver driver) => _webDriver = driver;

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

        public static SeleniumBrowser FromBrowserType(Browsertype browsertype)
        {
            return new SeleniumBrowser(browsertype, Environment.CurrentDirectory);
        }

        public async Task<Stream> RenderHtmlAsync(string html)
        {
            while (_isLocked)
            {
                await Task.Delay(500);
            }
            _isLocked = true;
            _webDriver.Url = "data:text/html;base64," + Convert.ToBase64String(Encoding.UTF8.GetBytes(html));
            _webDriver.Navigate();
            IWait<IWebDriver> wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(30.00));
            wait.Until(driver1 => ((IJavaScriptExecutor)_webDriver).ExecuteScript("return document.readyState").Equals("complete")); var ss = ((ITakesScreenshot)_webDriver).GetScreenshot();
            _isLocked = false;
            return new MemoryStream(ss.AsByteArray);
        }

        public async Task<Stream> RenderUrlAsync(string url)
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
                return new MemoryStream(ss.AsByteArray);
            }
            catch (Exception)
            {
                _isLocked = false;
                throw;
            }
        }
    }
}