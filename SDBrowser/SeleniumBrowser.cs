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
        private readonly IWebDriver WebDriver;
        private bool IsLocked = false;

        public SeleniumBrowser(IWebDriver driver)
        {
            WebDriver = driver;
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
            if (browsertype == Browsertype.Chrome)
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArguments("headless");

                WebDriver = new ChromeDriver(location, chromeOptions);
                WebDriver.Manage().Window.Size = new Size(1920, 1080);
            }
            else if (browsertype == Browsertype.Firefox)
            {
                var firefoxOptions = new FirefoxOptions();
                firefoxOptions.AddArguments("--headless");

                WebDriver = new FirefoxDriver(location, firefoxOptions);
                WebDriver.Manage().Window.Size = new Size(1920, 1080);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public async Task<Bitmap> RenderHtmlAsync(string html)
        {
            while (IsLocked)
            {
                await Task.Delay(500);
            }
            IsLocked = true;
            string basehtml = Convert.ToBase64String(Encoding.UTF8.GetBytes(html));
            WebDriver.Url = "data:html;base64," + html;
            WebDriver.Navigate();
            IWait<IWebDriver> wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(30.00));

            wait.Until(driver1 => ((IJavaScriptExecutor)WebDriver).ExecuteScript("return document.readyState").Equals("complete")); Screenshot ss = ((ITakesScreenshot)WebDriver).GetScreenshot();
            IsLocked = false;
            return Utils.ByteArrayToImage(ss.AsByteArray);
        }

        public async Task<Bitmap> RenderUrlAsync(string url)
        {
            while (IsLocked)
            {
                await Task.Delay(500);
            }
            IsLocked = true;
            try
            {
                WebDriver.Url = url;
                WebDriver.Navigate();
                IWait<IWebDriver> wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(30.00));

                wait.Until(driver1 => ((IJavaScriptExecutor)WebDriver).ExecuteScript("return document.readyState").Equals("complete")); Screenshot ss = ((ITakesScreenshot)WebDriver).GetScreenshot();
                IsLocked = false;
                return Utils.ByteArrayToImage(ss.AsByteArray);
            }
            catch (Exception e)
            {
                IsLocked = false;
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