using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ComicDownloader.Console.Domain;
using ManyConsole;

namespace ComicDownloader.Console.Commands
{
    public class DownloadComicsCommand : ConsoleCommand
    {
        public DownloadComicsCommand()
        {
            IsCommand("DownloadComics", "Download specifc comic issue(s) for a title.");

            HasRequiredOption("ct|comic-title=", "The comic title as it appears in the URL (e.g. batman).", c => Title = c);
            HasRequiredOption("df|download-folder=", "The folder to download into (e.g. C:\\Comics).", r => DownloadFolder = r);

            HasOption("ci|comic-issues=", "The comic issues to download (e.g. 1, 3, 5-12).", i => Issues = IssueRangeParser.Parse(i));
        }

        public string Title { get; set; }
        public IEnumerable<int> Issues { get; set; }
        public string DownloadFolder { get; set; }

        public override int Run(string[] remainingArguments)
        {
            try
            {
                if (!Directory.Exists(DownloadFolder))
                {
                    Directory.CreateDirectory(DownloadFolder);
                }

                var comicNavigator = new ComicNavigator(DownloadFolder);

                if (Issues == null || !Issues.Any())
                {
                    comicNavigator.DownloadAllIssues(Title);

                    System.Console.WriteLine($"Downloaded all issues of the comic book '{Title}'.");
                }
                else
                {
                    comicNavigator.DownloadIssues(Title, Issues);

                    System.Console.WriteLine($"Downloaded issue(s) {string.Join(", ", Issues)} of the comic book '{Title}'.");
                }
            }
            catch (Exception ex)
            {
                throw new ConsoleHelpAsException(ex.Message);
            }

            return 0;
        }
    }
}