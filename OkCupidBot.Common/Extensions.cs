using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidBot.Common
{
    public static class Extensions
    {
        public static SecureString ToSecureString(this string strPassword)
        {
            var secureStr = new SecureString();
            if (strPassword.Length > 0)
            {
                foreach (var c in strPassword.ToCharArray()) secureStr.AppendChar(c);
            }
            return secureStr;
        }

        public static IWebElement FindElementByPartialClass(this IWebDriver driver, string partialClass)
        {
            return driver.FindElement(By.XPath(string.Format(@"//*[@class[contains(., '{0}')]]", partialClass)));
        }

        public static IWebElement FindElementByPartialClass(this IWebElement element, string partialClass)
        {
            return element.FindElement(By.XPath(string.Format(@"//*[@class[contains(., '{0}')]]", partialClass)));
        }

        public static bool ElementExists(this IWebElement element, By by)
        {
            return element.FindElements(by).Count > 0;
        }

        public static int TryParseToInt(this string str)
        {
            int val;
            if(int.TryParse(str, out val))
            {
                return val;
            }

            return -1;
        }
    }
}
