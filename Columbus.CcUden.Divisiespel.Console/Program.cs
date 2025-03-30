using Columbus.CcUden.Divisiespel.Fetcher;
using Columbus.CcUden.Divisiespel.Models;
using System.Globalization;
using System.Web;

IHtmlParser htmlParser = new HtmlParser();
CompuClubFetcher fetcher = new(htmlParser);
await fetcher.TryUpdateSessionIdAsync();

int year;
while (!int.TryParse(Console.ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out year))
{
    Console.WriteLine("Select year: ");
}
await fetcher.SetYear(year);

IEnumerable<string> paths = await fetcher.GetCcFlightLinks();
Console.WriteLine("Available flights for CC Uden: ");
foreach (string path in paths)
{
    var queryParams = HttpUtility.ParseQueryString(path);
    Console.WriteLine(queryParams["vlc"]);
}

string? selectedPath = null;
while (selectedPath is null)
{
    Console.WriteLine("Select a flight: ");
    string? input = Console.ReadLine();
    selectedPath = paths.FirstOrDefault(p => p.Contains($"vlc={input}", StringComparison.InvariantCultureIgnoreCase));
}

IEnumerable<ResultLine> results = await fetcher.GetResults(selectedPath);
foreach (ResultLine result in results)
    Console.WriteLine($"{result.Position,-3} {result.Name,-20} {result.City,-20} {result.ClubId,-4} {result.AmountInFlight,-5} {result.PigeonId,-10} {result.Rank,-3} {result.Distance,-7} {result.Arrival:HH-mm-ss,-8} {result.Speed,-8} {result.Points,-7} {result.TimeDifference,-8}");