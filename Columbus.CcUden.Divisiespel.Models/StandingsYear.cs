namespace Columbus.CcUden.Divisiespel.Models
{
    /// <summary>
    /// The standings for a current year.
    /// </summary>
    public class StandingsYear
    {
        public int Year { get; set; }
        public List<string> FlightCodes { get; set; } = [];
        public List<StandingsOwner> OwnerStandings { get; set; } = [];
    }
}
