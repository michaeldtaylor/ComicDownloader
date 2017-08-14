using System;

namespace ComicDownloader.Console.Domain.Providers.ReadComicsTv
{
    public static class ReadComicsTvApi
    {
        private static readonly Uri BaseUri = new Uri("http://www.readcomics.tv/");

        public static Uri BuildComicIssueUri(string title, string issue)
        {
            return new Uri(BaseUri, $"/{title}/chapter-{issue}");
        }

        public static Uri BuildComicPageUri(string title, string issue, string page)
        {
            return new Uri(BaseUri, $"images/manga/{title}/{issue}/{page}.jpg");
        }
    }
}