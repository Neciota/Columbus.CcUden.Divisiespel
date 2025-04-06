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
        /// The divisions in which the owners are placed.
        /// </summary>
        public List<League> Leagues { get; set; } = [];
        /// <summary>
        /// The results per flight.
        /// </summary>
        public Dictionary<FlightCode, IEnumerable<OwnerResult>> OwnerResultByFlight { get; set; } = [];

        public FlightCode[] GetFlights() => OwnerResultByFlight
            .Keys
            .Order()
            .ToArray();
        public string[] GetAllOwners() => Leagues
            .SelectMany(league => league.Owners)
            .Distinct()
            .ToArray();
        public Dictionary<(FlightCode Flight, string OwnerName), int> GetPointsByOwnerAndFlight() => OwnerResultByFlight
            .SelectMany(flight => flight.Value.Select(result => (flight.Key, result)))
            .ToDictionary(x => (x.Key, x.result.Name), x => x.result.GetPoints());
    }
}
