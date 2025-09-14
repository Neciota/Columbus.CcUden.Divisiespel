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

        public Owner[] GetAllOwners() => Leagues
            .SelectMany(league => league.Owners)
            .Distinct()
            .ToArray();

        public Dictionary<(FlightCode, Owner), int> GetPointsByOwnerAndFlight() => OwnerResultByFlight
            .SelectMany(flight => flight.Value.Select(result => (flight.Key, result)))
            .ToDictionary(x => (x.Key, x.result.Owner), x => x.result.GetPoints());

        public OwnerStandings[] GetOwnerStandingsByOwner() => Leagues.SelectMany(l => l.Owners)
            .Select(GetOwnerStandingsForOwner)
            .OrderByDescending(os => os.TotalPoints)
            .ThenByDescending(os => os.GetAmountOfPointResults(6))
            .ThenByDescending(os => os.GetAmountOfPointResults(5))
            .ThenByDescending(os => os.GetAmountOfPointResults(4))
            .ThenByDescending(os => os.GetAmountOfPointResults(3))
            .ThenByDescending(os => os.GetAmountOfPointResults(2))
            .ThenByDescending(os => os.GetAmountOfPointResults(1))
            .ToArray();

        private OwnerStandings GetOwnerStandingsForOwner(Owner owner) => new OwnerStandings(owner, 
            OwnerResultByFlight
            .SelectMany(flight => flight.Value)
            .Where(or => or.Owner == owner));

        public Owner[] GetUnregisteredOwners()
        {
            HashSet<Owner> ownersInLeagues = Leagues.SelectMany(league => league.Owners)
                .ToHashSet();

            return OwnerResultByFlight
                .SelectMany(ownerFlight => ownerFlight.Value)
                .Select(ownerResult => ownerResult.Owner)
                .Distinct()
                .Where(owner => !ownersInLeagues.Contains(owner))
                .ToArray();
        }
    }
}
