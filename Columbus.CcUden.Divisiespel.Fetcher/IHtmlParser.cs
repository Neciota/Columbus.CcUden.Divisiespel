
namespace Columbus.CcUden.Divisiespel.Fetcher
{
    public interface IHtmlParser
    {
        string GetCcResultsPath(string htmlText);
        IEnumerable<string> GetFlightPaths(string htmlText);
        ResultsPage GetResultsPage(string htmlText);
    }
}
