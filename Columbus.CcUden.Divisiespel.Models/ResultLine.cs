namespace Columbus.CcUden.Divisiespel.Models
{
    /// <summary>
    /// Models a single line in the results.
    /// </summary>
    public class ResultLine
    {
        public required int Position { get; set; }
        public required string Name { get; set; }
        public required string City { get; set; }
        public required int ClubId { get; set; }
        public required int AmountInFlight { get; set; }
        public required string PigeonId { get; set; }
        public required int Rank { get; set; }
        public required double Distance { get; set; }
        public required TimeOnly Arrival { get; set; }
        public required double Speed { get; set; }
        public required double Points { get; set; }
        public required TimeSpan TimeDifference { get; set; }
    }
}
