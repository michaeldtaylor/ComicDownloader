using System.Collections.Generic;

namespace ComicDownloader.Console.Domain.Providers
{
    public interface IComicProvider
    {
        string ServiceName { get; }

        void DownloadIssues(string title, IEnumerable<int> issues, string downloadFolder);

        int DownloadAllIssues(string title, string downloadFolder);
    }
}