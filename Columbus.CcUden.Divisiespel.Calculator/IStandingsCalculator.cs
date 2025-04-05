using Columbus.CcUden.Divisiespel.Models;

namespace Columbus.CcUden.Divisiespel.Calculator
{
    public interface IStandingsCalculator
    {
        IEnumerable<OwnerResult> GetOwnerResultsFromSingleFlight(IEnumerable<ResultLine> results);
        StandingsYear GetUpdatedStandingsFromResults(StandingsYear standingsYear, FlightCode flightCode, IEnumerable<OwnerResult> ownerResults);
    }
}