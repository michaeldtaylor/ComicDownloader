namespace ComicDownloader.Console.Domain.Providers
{
    public interface IComicProvider
    {
        string ServiceName { get; }

        void DownloadIssues(string title, int[] issues, string downloadFolder);

        int DownloadAllIssues(string title, string downloadFolder);
    }
}