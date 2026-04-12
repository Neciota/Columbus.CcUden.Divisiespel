using Columbus.CcUden.Divisiespel.Models;

namespace Columbus.CcUden.Divisiespel.Calculator
{
    public class StandingsCalculator : IStandingsCalculator
    {
        public IEnumerable<OwnerResult> GetOwnerResultsFromSingleFlight(IEnumerable<ResultLine> results)
        {
            const int maxRankToCount = 5;
            const int maxRankToCountExtra = 2;

            Dictionary<Owner, OwnerResult> ownerResults = [];

            foreach (ResultLine line in results.Where(r => r.Rank <= maxRankToCount))
            {
                Owner owner = new(line.Name);
                ownerResults.TryAdd(owner, new OwnerResult(owner));
                OwnerResult ownerResult = ownerResults[owner];
                ownerResult.Occurences++;
                if (line.Rank <= maxRankToCountExtra)
                    ownerResult.HasDesignated = true;
            }

            return ownerResults.Values;
        }
    }
}
