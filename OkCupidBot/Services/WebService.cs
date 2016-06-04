using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace OkCupidBot.Services
{
    public class WebService : BaseService
    {
        private FirefoxDriver _driver;

        public FirefoxDriver Browser
        {
            get
            {
                return _driver;
            }
        }

        public void Initialize()
        {
            this.Services.LogService.WriteHeader("Initalizing WebService");
            _driver = new FirefoxDriver();
        }

        public void WaitForTime(int seconds)
        {
            DateTime endTime = DateTime.Now.AddSeconds(seconds);
            this.Services.LogService.WriteLine("Pausing for {0} seconds...", ConsoleColor.Yellow, seconds);
            while (DateTime.Now <= endTime)
            {

            }
        }

        public void WaitForRandomTime(int min, int max)
        {
            Random rand = new Random();
            this.WaitForTime(rand.Next(min, max));
        }

        public void RefreshPage()
        {
            var javaScriptExecutor = _driver as IJavaScriptExecutor;
            javaScriptExecutor.ExecuteScript("document.location.reload()");
        }

        public bool AlreadyOnPage(Uri uri)
        {
            return _driver.Url == uri.ToString();
        }

        public void WaitForPageReady(TimeSpan? timeout = null)
        {
            var javaScriptExecutor = _driver as IJavaScriptExecutor;
            var wait = new WebDriverWait(_driver, timeout.HasValue ? timeout.Value : new TimeSpan(0, 0, 30));

            // Check if document is ready
            Func<IWebDriver, bool> readyCondition = webDriver => (bool)javaScriptExecutor.ExecuteScript("return (document.readyState == 'complete' && jQuery.active == 0)");

            try
            {
                wait.Until(readyCondition);
            }
            catch (TimeoutException ex)
            {
                RefreshPage();
                wait.Until(readyCondition);
            }
        }

        public void ScrollTo(By by)
        {
            ServiceManager.Services.LogService.WriteLine("Scrolling to \"{0}\".", by.ToString());
            bool foundItem = false;

            while (!foundItem)
            {
                _driver.Keyboard.SendKeys(Keys.PageDown);
                this.WaitForPageReady();

                if (_driver.FindElements(by).Count > 0)
                {
                    foundItem = true;
                    break;
                }
            }
        }

        public void NavigateTo(string url)
        {
            ServiceManager.Services.LogService.WriteLine("Navigating to \"{0}\".", url);
            _driver.Navigate().GoToUrl(url);
            this.WaitForPageReady();
        }

        public void NavigateTo(Uri uri)
        {
            ServiceManager.Services.LogService.WriteLine("Navigating to \"{0}\".", uri.ToString());
            _driver.Navigate().GoToUrl(uri.ToString());
            this.WaitForPageReady();
        }

        public void Dispose()
        {
            _driver.Close();
            _driver.Dispose();
        }
    }
}
