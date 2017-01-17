using System;
using System.Collections.Generic;
using System.Linq;
using ManyConsole;

namespace ComicDownloader.Console.Commands
{
    public static class IssueRangeParser
    {
        public static IEnumerable<int> Parse(string issueSelector)
        {
            if (string.IsNullOrEmpty(issueSelector))
            {
                return Enumerable.Empty<int>();
            }

            var results = new List<int>();
            int singleIssue;
            
            if (int.TryParse(issueSelector, out singleIssue))
            {
                results.Add(singleIssue);

                return results;
            }

            var multipleIssueParts = issueSelector.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();

            foreach (var multipleIssuePart in multipleIssueParts)
            {
                if (int.TryParse(multipleIssuePart, out singleIssue))
                {
                    AddIfNotExists(results, singleIssue);
                    continue;
                }

                var rangeIssueParts = multipleIssuePart.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries);

                if (rangeIssueParts.Length != 2)
                {
                    throw new ConsoleHelpAsException($"The issue selector is invalid (Range = '{multipleIssuePart}').");
                }

                int minRange;

                if (!int.TryParse(rangeIssueParts[0], out minRange))
                {
                    throw new ConsoleHelpAsException($"The issue selector is invalid (Range = '{multipleIssuePart}', Minimum = '{rangeIssueParts[0]}').");
                }

                int maxRange;
                
                if (!int.TryParse(rangeIssueParts[1], out maxRange))
                {
                    throw new ConsoleHelpAsException($"The issue selector is invalid (Range = '{multipleIssuePart}', Maximum = '{rangeIssueParts[1]}').");
                }

                for (var i = minRange; i <= maxRange; i++)
                {
                    AddIfNotExists(results, i);
                }
            }

            return results;
        }

        static void AddIfNotExists(ICollection<int> results, int result)
        {
            if (!results.Contains(result))
            {
                results.Add(result);
            }
        }
    }
}