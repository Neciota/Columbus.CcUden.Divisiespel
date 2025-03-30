using Columbus.CcUden.Divisiespel.Models;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Columbus.CcUden.Divisiespel.Fetcher
{
    public partial class HtmlParser : IHtmlParser
    {
        public string GetCcResultsPath(string htmlText)
        {
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(htmlText);

            HtmlNode ccLinkNode = htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"ebul_cbmenu_afd_sam_uitklap_2\"]/li[14]/a");

            return ccLinkNode.Attributes["href"].Value;
        }

        public IEnumerable<string> GetFlightPaths(string htmlText)
        {
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(htmlText);

            HtmlNode flightsListNode = htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"ebul_cbhoriz_menu_6\"]");
            return flightsListNode.Descendants("a")
                .Select(a => a.GetAttributeValue("href", string.Empty))
                .Where(href => !string.IsNullOrEmpty(href))
                .ToList();
        }

        public ResultsPage GetResultsPage(string htmlText)
        {
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(htmlText);

            return new ResultsPage
            {
                ResultLines = GetOwnerResults(htmlDocument),
                OtherPages = GetPagePaths(htmlDocument),
            };
        }

        private static List<ResultLine> GetOwnerResults(HtmlDocument htmlDocument)
        {
            HtmlNode resultsOverviewNode = htmlDocument.DocumentNode.SelectSingleNode("//pre[1]");
            // The result is a table of preformatted text (<pre></pre>) with lines split by <br>-elements.
            string[] resultLines = resultsOverviewNode.InnerHtml.Split("<br>");

            return resultLines.Skip(2) // First line is column headers, second line is separator chars
                .SkipLast(1) // Oddly enough, AgilityPack thinks the final <tr> is in the <pre>, even though it really isn't, so skip it in the lines.
                .Select(line =>
                {
                    // Find all <a> elements and replace them with their inner content to ensure the spacing is the same
                    string sanitizedLine = LinkElementRegex().Replace(line, "$1");

                    int[] arrivalTimeComponents = sanitizedLine.Substring(94, 8)
                        .Trim()
                        .Split('.')
                        .Select(value => Convert.ToInt32(value))
                        .ToArray();
                    int[] timeDifferenceComponents = sanitizedLine.Substring(122, 8)
                        .Trim()
                        .Split(':')
                        .Select(value => Convert.ToInt32(value))
                        .ToArray();

                    return new ResultLine
                    {
                        Position = Convert.ToInt32(sanitizedLine.Substring(1, 5).Trim()),
                        Name = sanitizedLine.Substring(8, 20).Trim(),
                        City = sanitizedLine.Substring(30, 20).Trim(),
                        ClubId = Convert.ToInt32(sanitizedLine.Substring(52, 4).Trim()),
                        AmountInFlight = Convert.ToInt32(sanitizedLine.Substring(58, 7).Trim().Split('/')[0]),
                        PigeonId = sanitizedLine.Substring(67, 10).Trim(),
                        Rank = Convert.ToInt32(sanitizedLine.Substring(79, 3).Trim()),
                        Distance = Convert.ToDouble(sanitizedLine.Substring(84, 8).Trim()),
                        Arrival = new TimeOnly(arrivalTimeComponents[0], arrivalTimeComponents[1], arrivalTimeComponents[2]),
                        Speed = Convert.ToDouble(sanitizedLine.Substring(104, 8).Trim()),
                        Points = Convert.ToDouble(sanitizedLine.Substring(114, 6).Trim()),
                        TimeDifference = new TimeSpan(timeDifferenceComponents[0], timeDifferenceComponents[1], timeDifferenceComponents[2])
                    };
                })
                .ToList();
        }

        private static List<string> GetPagePaths(HtmlDocument htmlDocument)
        {
            HtmlNode pageOverviewNode = htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"ebul_cbhoriz_menu_5\"]");
            return pageOverviewNode.Descendants("a")
                .Select(a => a.GetAttributeValue("href", string.Empty))
                .Where(href => !string.IsNullOrEmpty(href))
                .ToList();
        }

        [GeneratedRegex(@"<a[^>]*>(.*?)</a>")]
        private static partial Regex LinkElementRegex();
    }
}
