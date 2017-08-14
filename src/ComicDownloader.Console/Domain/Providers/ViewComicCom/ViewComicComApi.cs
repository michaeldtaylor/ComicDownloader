using System;

namespace ComicDownloader.Console.Domain.Providers.ViewComicCom
{
    public static class ViewComicComApi
    {
        private static readonly Uri BaseUri = new Uri("http://view-comic.com/");

        public static Uri BuildComicIssueUri(string title, string issue)
        {
            return new Uri(BaseUri, $"/{title}-{issue}");
        }
    }
}