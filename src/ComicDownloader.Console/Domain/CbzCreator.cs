using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace ComicDownloader.Console.Domain
{
    public class CbzCreator
    {
        readonly string _downloadFolder;

        public CbzCreator(string downloadFolder)
        {
            _downloadFolder = downloadFolder;
        }

        public void CreateIssues(string title, IEnumerable<int> issues)
        {
            foreach (var issue in issues)
            {
                var issuePath = Path.Combine(_downloadFolder, title, issue.ToString("D3"));
                var destinationFileName = $"{title}_{issue:D3}.cbz";
                var destinationArchiveFileName = Path.Combine(_downloadFolder, title, destinationFileName);

                if (File.Exists(destinationArchiveFileName))
                {
                    File.Delete(destinationArchiveFileName);
                }

                ZipFile.CreateFromDirectory(issuePath, destinationArchiveFileName);

                if (Directory.Exists(issuePath))
                {
                    Directory.Delete(issuePath, true);
                }
            }
        }
    }
}