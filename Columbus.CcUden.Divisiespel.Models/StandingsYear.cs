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
        /// The results per flight.
        /// </summary>
        public Dictionary<string, IEnumerable<OwnerResult>> OwnerResultByFlight { get; set; } = [];

        public string[] GetFlights() => OwnerResultByFlight
            .Keys
            .Order()
            .ToArray();
        public string[] GetAllOwners() => OwnerResultByFlight
            .SelectMany(results => results.Value.Select(result => result.Name))
            .Distinct()
            .Order()
            .ToArray();
        public Dictionary<(string Flight, string OwnerName), int> GetPointsByOwnerAndFlight() => OwnerResultByFlight
            .SelectMany(flight => flight.Value.Select(result => (flight.Key, result)))
            .ToDictionary(x => (x.Key, x.result.Name), x => x.result.GetPoints());
    }
}
