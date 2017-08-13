using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ComicDownloader.Console.Domain.Providers.ReadComicsTv
{
    public class ReadComicsTvComicProvider : IComicProvider
    {
        private static readonly string ChromeDriverDirectory = Path.Combine(Environment.CurrentDirectory, "lib");

        public string ServiceName => "read-comics-tv";

        public void DownloadIssues(string title, IEnumerable<int> issues, string downloadFolder)
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
                driver.Navigate().GoToUrl(ReadComicsTvApi.BuildComicIssueUri(title, 1));

                var issueSelect = GetIssueSelector(driver);
                var totalIssueCount = issueSelect.Options.Count;

                for (var issueCount = 1; issueCount <= totalIssueCount; issueCount++)
                {
                    DownloadAllIssuePages(driver, title, issueCount, downloadFolder);
                }

                return totalIssueCount;
            }
        }

        private void DownloadAllIssuePages(IWebDriver driver, string title, int issue, string downloadFolder)
        {
            var downloadPath = Path.Combine(downloadFolder, title, issue.ToString("D3"));

            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }

            driver.Navigate().GoToUrl(ReadComicsTvApi.BuildComicIssueUri(title, issue));

            var pageSelect = GetPageSelector(driver);
            var totalPageCount = pageSelect.Options.Count;

            for (var pageNumber = 1; pageNumber <= totalPageCount; pageNumber++)
            {
                DownloadPage(title, issue, pageNumber, downloadPath);
            }
        }

        private static void DownloadPage(string title, int issue, int page, string downloadPath)
        {
            var imageUri = ReadComicsTvApi.BuildComicPageUri(title, issue, page);
            var localFileName = $"{title}_{issue:D3}_{page:D3}.jpg";
            var localPath = Path.Combine(downloadPath, localFileName);

            using (var client = new WebClient())
            {
                client.DownloadFile(imageUri, localPath);
            }
        }

        private static SelectElement GetPageSelector(ISearchContext driver)
        {
            return new SelectElement(driver.FindElement(By.Name("page_select")));
        }

        private static SelectElement GetIssueSelector(ISearchContext driver)
        {
            return new SelectElement(driver.FindElement(By.Name("chapter_select")));
        }
    }
}