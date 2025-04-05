using Columbus.CcUden.Divisiespel.Models;

namespace Columbus.CcUden.Divisiespel.Calculator
{
    public class StandingsCalculator : IStandingsCalculator
    {
        public IEnumerable<OwnerResult> GetOwnerResultsFromSingleFlight(IEnumerable<ResultLine> results)
        {
            Dictionary<string, OwnerResult> ownerResults = [];

            foreach (ResultLine line in results)
            {
                ownerResults.TryAdd(line.Name, new OwnerResult(line.Name));
                OwnerResult ownerResult = ownerResults[line.Name];
                ownerResult.Occurences = Math.Clamp(ownerResult.Occurences + 1, 0, 5);
                if (line.Rank is 1 or 2)
                    ownerResult.HasDesignated = true;
            }

            return ownerResults.Values;
        }

        public StandingsYear GetUpdatedStandingsFromResults(StandingsYear standingsYear, FlightCode flightCode, IEnumerable<OwnerResult> ownerResults)
        {
            var newResultsPerFlight = standingsYear.OwnerResultByFlight.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            newResultsPerFlight.Add(flightCode, ownerResults);

            return new StandingsYear
            {
                Year = standingsYear.Year,
                OwnerResultByFlight = newResultsPerFlight,
            };
        }
    }
}
