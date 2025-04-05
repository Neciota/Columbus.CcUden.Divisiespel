namespace Columbus.CcUden.Divisiespel.Models
{
    /// <summary>
    /// The standings for a current year.
    /// </summary>
    public class StandingsYear
    {
        /// <summary>
        /// The year for which the standings apply.
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// Owners that may appear in flight results but who do not participate in the championship.
        /// </summary>
        public List<string> ExcludedOwners { get; set; } = [];
        /// <summary>
        /// The results per flight.
        /// </summary>
        public Dictionary<FlightCode, IEnumerable<OwnerResult>> OwnerResultByFlight { get; set; } = [];

        public FlightCode[] GetFlights() => OwnerResultByFlight
            .Keys
            .Order()
            .ToArray();
        public string[] GetAllOwners() => OwnerResultByFlight
            .SelectMany(results => results.Value.Select(result => result.Name))
            .Distinct()
            .Except(ExcludedOwners)
            .Order()
            .ToArray();
        public Dictionary<(FlightCode Flight, string OwnerName), int> GetPointsByOwnerAndFlight() => OwnerResultByFlight
            .SelectMany(flight => flight.Value
                .Where(result => !ExcludedOwners.Contains(result.Name))
                .Select(result => (flight.Key, result))
            )
            .ToDictionary(x => (x.Key, x.result.Name), x => x.result.GetPoints());
    }
}
