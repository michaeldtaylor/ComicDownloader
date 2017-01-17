using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ComicDownloader.Console.Infrastructure
{
    public static class WebDriverExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeout)
        {
            if (timeout <= 0)
            {
                return driver.FindElement(by);
            }

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));

            return wait.Until(drv => drv.FindElement(by));
        }

        public static bool ElementExists(this IWebDriver driver, By by)
        {
            return driver.FindElements(by).Any();
        }

        public static bool TryFindElement(this IWebDriver driver, By by, out IWebElement element)
        {
            if (ElementExists(driver, by))
            {
                element = driver.FindElement(by);
                return true;
            }

            element = null;
            return false;
        }
    }
}