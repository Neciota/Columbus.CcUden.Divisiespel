using Columbus.CcUden.Divisiespel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Columbus.CcUden.Divisiespel.Calculator
{
    public static class Calculator
    {
        public static IEnumerable<OwnerResult> GetOwnerResultsFromSingleFlight(IEnumerable<ResultLine> results)
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
    }
}
