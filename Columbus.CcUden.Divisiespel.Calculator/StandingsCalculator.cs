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

        public StandingsYear GetUpdatedStandingsFromResults(StandingsYear standingsYear, string flightCode, IEnumerable<OwnerResult> ownerResults)
        {
            IEnumerable<string> allOwnerNames = standingsYear.OwnerStandings.Select(x => x.Name)
                .Concat(ownerResults.Select(x => x.Name));
            Dictionary<string, OwnerResult> ownerResultsByName = ownerResults.ToDictionary(x => x.Name);
            Dictionary<string, StandingsOwner> standingsOwnersByName = standingsYear.OwnerStandings.ToDictionary(x => x.Name);

            List<StandingsOwner> newOwnerStandings = allOwnerNames.Select(name => new StandingsOwner
            {
                Name = name,
                Points = ownerResultsByName.GetValueOrDefault(name)?.GetPoints() ?? 0 + (standingsOwnersByName.GetValueOrDefault(name)?.Points ?? 0),
            }).ToList();

            return new StandingsYear
            {
                Year = standingsYear.Year,
                FlightCodes = [flightCode, .. standingsYear.FlightCodes],
                OwnerStandings = newOwnerStandings,
            };
        }
    }
}
