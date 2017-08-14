using System;
using System.IO;
using System.Net;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ComicDownloader.Console.Domain.Providers.ReadComicsTv
{
    public class ReadComicsTvComicProvider : IComicProvider
    {
        private static readonly Func<int, string> IssueFormat = x => $"{x:D3}";
        private static readonly Func<int, string> LocalIssueFormat = x => $"{x:D3}";
        private static readonly Func<int, string> PageFormat = x => $"{x:D3}";

        private static readonly string ChromeDriverDirectory = Path.Combine(Environment.CurrentDirectory, "lib");

        public string ServiceName => "read-comics-tv";

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
                driver.Navigate().GoToUrl(ReadComicsTvApi.BuildComicIssueUri(title, IssueFormat(1)));

                var issueSelect = GetIssueSelector(driver);
                var totalIssueCount = issueSelect.Options.Count;

                for (var issue = 1; issue <= totalIssueCount; issue++)
                {
                    DownloadAllIssuePages(driver, title, issue, downloadFolder);
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

            driver.Navigate().GoToUrl(ReadComicsTvApi.BuildComicIssueUri(title, IssueFormat(issue)));

            var pageSelect = GetPageSelector(driver);
            var totalPageCount = pageSelect.Options.Count;

            for (var pageNumber = 1; pageNumber <= totalPageCount; pageNumber++)
            {
                DownloadPage(title, issue, pageNumber, downloadPath);
            }
        }

        private static void DownloadPage(string title, int issue, int page, string downloadPath)
        {
            var issueFormat = LocalIssueFormat(issue);
            var pageFormat = PageFormat(page);

            var imageUri = ReadComicsTvApi.BuildComicPageUri(title, issueFormat, pageFormat);
            var localFileName = $"{title}_{issueFormat}_{pageFormat}.jpg";
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