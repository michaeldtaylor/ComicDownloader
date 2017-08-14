using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ComicDownloader.Console.Domain.Providers.ViewComicCom
{
    public class ViewComicComComicProvider : IComicProvider
    {
        private static readonly Func<int, string> IssueFormat1 = x => $"{x:D2}-of-04-2011";
        private static readonly Func<int, string> LocalIssueFormat = x => $"{x:D3}";

        private static readonly string ChromeDriverDirectory = Path.Combine(Environment.CurrentDirectory, "lib");

        public string ServiceName => "view-comic-com";

        public void DownloadIssues(string title, int[] issues, string downloadFolder)
        {
            using (var driver = new ChromeDriver(ChromeDriverDirectory))
            {
                foreach (var issue in issues)
                {
                    DownloadAllIssuePages(driver, title, issue, downloadFolder);
                }
            }
        }

        public int DownloadAllIssues(string title, string downloadFolder)
        {
            using (var driver = new ChromeDriver(ChromeDriverDirectory))
            {
                driver.Navigate().GoToUrl(ViewComicComApi.BuildComicIssueUri(title, IssueFormat1(1)));

                var issueSelect = GetIssueSelector(driver);
                var totalIssueCount = issueSelect.Options.Count;

                for (var issueCount = 1; issueCount <= totalIssueCount; issueCount++)
                {
                    DownloadAllIssuePages(driver, title, issueCount, downloadFolder);
                }

                return totalIssueCount;
            }
        }

        private static void DownloadAllIssuePages(IWebDriver driver, string title, int issue, string downloadFolder)
        {
            var downloadPath = Path.Combine(downloadFolder, title, LocalIssueFormat(issue));

            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }

            driver.Navigate().GoToUrl(ViewComicComApi.BuildComicIssueUri(title, IssueFormat1(issue)));

            var pageImageUris = GetPageImageUris(driver);

            var page = 1;

            foreach (var pageImageUri in pageImageUris)
            {
                var localFileName = $"{title}_{issue:D3}_{page:D3}.jpg";
                var localPath = Path.Combine(downloadPath, localFileName);

                using (var client = new WebClient())
                {
                    client.DownloadFile(pageImageUri, localPath);
                }

                page++;
            }
        }

        private static IEnumerable<string> GetPageImageUris(ISearchContext driver)
        {
            return driver.FindElements(By.XPath("//div[@class='separator']/a/img")).Select(x => x.GetAttribute("src")).ToList();
        }

        private static SelectElement GetIssueSelector(ISearchContext driver)
        {
            return new SelectElement(driver.FindElement(By.Id("selectbox")));
        }
    }
}