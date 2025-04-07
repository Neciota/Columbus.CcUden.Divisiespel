using Columbus.CcUden.Divisiespel.Models;

namespace Columbus.CcUden.Divisiespel.Calculator
{
    public class StandingsCalculator : IStandingsCalculator
    {
        public IEnumerable<OwnerResult> GetOwnerResultsFromSingleFlight(IEnumerable<ResultLine> results)
        {
            Dictionary<Owner, OwnerResult> ownerResults = [];

            foreach (ResultLine line in results)
            {
                Owner owner = new(line.Name);
                ownerResults.TryAdd(owner, new OwnerResult(owner));
                OwnerResult ownerResult = ownerResults[owner];
                ownerResult.Occurences = Math.Clamp(ownerResult.Occurences + 1, 0, 5);
                if (line.Rank is 1 or 2)
                    ownerResult.HasDesignated = true;
            }

            return ownerResults.Values;
        }
    }
}
