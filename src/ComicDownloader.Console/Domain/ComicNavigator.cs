using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ComicDownloader.Console.Domain
{
    public class ComicNavigator
    {
        static readonly string ChromeDriverDirectory = Path.Combine(Environment.CurrentDirectory, "lib");

        readonly string _downloadFolder;

        public ComicNavigator(string downloadFolder)
        {
            _downloadFolder = downloadFolder;
        }

        public void DownloadIssues(string title, IEnumerable<int> issues)
        {
            using (var driver = new ChromeDriver(ChromeDriverDirectory))
            {
                foreach (var issue in issues)
                {
                    DownloadAllIssuePages(driver, title, issue);
                }
            }
        }

        public void DownloadAllIssues(string title)
        {
            using (var driver = new ChromeDriver(ChromeDriverDirectory))
            {
                driver.Navigate().GoToUrl(ReadComicsTvApi.BuildComicIssueUri(title, 1));

                var issueSelect = GetIssueSelector(driver);
                var totalIssueCount = issueSelect.Options.Count;

                for (var issueCount = 1; issueCount <= totalIssueCount; issueCount++)
                {
                    DownloadAllIssuePages(driver, title, issueCount);
                }
            }
        }

        void DownloadAllIssuePages(IWebDriver driver, string title, int issue)
        {
            var downloadPath = Path.Combine(_downloadFolder, title, issue.ToString());

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

        static void DownloadPage(string title, int issue, int page, string downloadPath)
        {
            var imageUri = ReadComicsTvApi.BuildComicPageUri(title, issue, page);
            var localFileName = Path.GetFileName(imageUri.LocalPath);
            var localPath = Path.Combine(downloadPath, localFileName);

            using (var client = new WebClient())
            {
                client.DownloadFile(imageUri, localPath);
            }
        }

        static SelectElement GetPageSelector(ISearchContext driver)
        {
            return new SelectElement(driver.FindElement(By.Name("page_select")));
        }

        static SelectElement GetIssueSelector(ISearchContext driver)
        {
            return new SelectElement(driver.FindElement(By.Name("chapter_select")));
        }
    }
}