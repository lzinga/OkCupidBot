using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;

namespace OkCupidBot.Services
{
    public class WebService : BaseService
    {
        private ChromeDriver _driver;

        public ChromeDriver Browser
        {
            get
            {
                return _driver;
            }
        }

        public void Initialize()
        {
            this.Services.LogService.WriteHeader("Initalizing WebService");
            _driver = new ChromeDriver(@"C:\Program Files (x86)\Microsoft Web Driver\");
        }

        public void WaitForTime(int seconds)
        {
            DateTime endTime = DateTime.Now.AddSeconds(seconds);
            this.Services.LogService.WriteLine("Pausing for {0} second{1}...", ConsoleColor.Yellow, seconds, seconds > 1 ? "s" : "");
            int secondsPassed = 0;
            int curSecond = DateTime.Now.Second;
            while (DateTime.Now <= endTime)
            {
                if(curSecond != DateTime.Now.Second)
                {
                    secondsPassed++;
                    curSecond = DateTime.Now.Second;
                    this.Services.LogService.ClearCurrentConsoleLine();

                    int newSeconds = seconds - secondsPassed;
                    this.Services.LogService.WriteLine("Pausing for {0} more second{1}...", ConsoleColor.Yellow, newSeconds, newSeconds > 1 ? "s" : "");

                    if(newSeconds <= 5 && this.Services.ArgumentService.Arguments.DiscordMessage && this.Services.DiscordService.ReceiveMessages)
                    {
                        this.Services.DiscordService.SendMessage($"Pausing for {newSeconds} more second{(newSeconds > 1 ? "s" : "")}...");
                    }

                }
            }

            // Since it replaced how long it was originally paused for, place it back into the console.
            this.Services.LogService.ClearCurrentConsoleLine();
            this.Services.LogService.WriteLine("Paused for {0} second{1} from {2} to {3}", ConsoleColor.Yellow, seconds, seconds > 1 ? "s" : "", DateTime.Now.AddSeconds(-seconds).ToLongTimeString(), DateTime.Now.ToLongTimeString());
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

        public void ScrollTo(By by, int timeout = 60)
        {
            ServiceManager.Services.LogService.WriteLine("Scrolling to \"{0}\".", by.ToString());
            bool foundItem = false;
            DateTime endTime = DateTime.Now.AddSeconds(timeout);

            while (!foundItem)
            {
                _driver.Keyboard.SendKeys(Keys.PageDown);
                this.WaitForPageReady();

                if (_driver.FindElements(by).Count > 0)
                {
                    foundItem = true;
                    break;
                }

                if(DateTime.Now >= endTime)
                {
                    ServiceManager.Services.LogService.WriteLine("Scroll timed out.", by.ToString());
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
