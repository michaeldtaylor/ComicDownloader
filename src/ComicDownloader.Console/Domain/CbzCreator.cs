using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace ComicDownloader.Console.Domain
{
    public class CbzCreator
    {
        public void CreateIssues(string title, IEnumerable<int> issues, string issueFolder)
        {
            foreach (var issue in issues)
            {
                var issuePath = Path.Combine(issueFolder, title, issue.ToString("D3"));
                var destinationFileName = $"{title}_{issue:D3}.cbz";
                var destinationArchiveFileName = Path.Combine(issueFolder, title, destinationFileName);

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