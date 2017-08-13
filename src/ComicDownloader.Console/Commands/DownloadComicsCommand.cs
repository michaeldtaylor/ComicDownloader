using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ComicDownloader.Console.Domain;
using ComicDownloader.Console.Domain.Providers;
using ManyConsole;

namespace ComicDownloader.Console.Commands
{
    public class DownloadComicsCommand : ConsoleCommand
    {
        private readonly Dictionary<string, IComicProvider> _comicProviderMap;
        private readonly CbzCreator _cbzCreator;
        
        public DownloadComicsCommand(IEnumerable<IComicProvider> comicProviders, CbzCreator cbzCreator)
        {
            IsCommand("DownloadComics", "Download specifc comic issue(s) for a title from various comic providers.");

            _comicProviderMap = comicProviders.ToDictionary(c => c.ServiceName, c => c);
            _cbzCreator = cbzCreator;
            
            HasRequiredOption("ct|comic-title=", "The comic title as it appears in the URL (e.g. batman).", c => Title = c);
            HasRequiredOption("cp|comic-provider=", "The comic provider to use (e.g. read-comics-tv).", c => ComicProvider = ValidateComicProvider(_comicProviderMap, c));
            HasRequiredOption("df|download-folder=", "The folder to download into (e.g. C:\\Comics).", d => DownloadFolder = d);

            HasOption("ci|comic-issues=", "The comic issues to download (e.g. 1, 3, 5-12).", i => Issues = IssueRangeParser.Parse(i));
        }

        public string Title { get; set; }

        public IEnumerable<int> Issues { get; set; }

        public string ComicProvider { get; set; }

        public string DownloadFolder { get; set; }

        public override int Run(string[] remainingArguments)
        {
            var comicProvider = _comicProviderMap[ComicProvider];

            try
            {
                if (!Directory.Exists(DownloadFolder))
                {
                    Directory.CreateDirectory(DownloadFolder);
                }

                if (Issues == null || !Issues.Any())
                {
                    var totalIssueCount = comicProvider.DownloadAllIssues(Title, DownloadFolder);

                    _cbzCreator.CreateIssues(Title, Enumerable.Range(1, totalIssueCount), DownloadFolder);

                    System.Console.WriteLine($"Downloaded all issues of the comic book '{Title}'.");
                }
                else
                {
                    comicProvider.DownloadIssues(Title, Issues, DownloadFolder);
                    _cbzCreator.CreateIssues(Title, Issues, DownloadFolder);

                    System.Console.WriteLine($"Downloaded issue(s) {string.Join(", ", Issues)} of the comic book '{Title}'.");
                }
            }
            catch (Exception ex)
            {
                throw new ConsoleHelpAsException(ex.Message);
            }

            return 0;
        }

        private static string ValidateComicProvider(IDictionary<string, IComicProvider> comicProvidersMap, string comicProvider)
        {
            if (comicProvidersMap.ContainsKey(comicProvider))
            {
                return comicProvider;
            }

            throw new ConsoleHelpAsException($"The comic provider '{comicProvider}' is unknown. Valid options are: {string.Join(", ", comicProvidersMap.Keys)}");
        }
    }
}