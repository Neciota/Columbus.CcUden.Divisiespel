using Columbus.CcUden.Divisiespel.Models;

namespace Columbus.CcUden.Divisiespel.Fetcher
{
    public class ResultsPage
    {
        public required IEnumerable<ResultLine> ResultLines { get; set; }
        public required IEnumerable<string> OtherPages { get; set; }
    }
}
