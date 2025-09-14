namespace Columbus.CcUden.Divisiespel.Models
{
    public class OwnerStandings(Owner owner, IEnumerable<OwnerResult> ownerResults)
    {
        public Owner Owner { get; set; } = owner;
        public IEnumerable<OwnerResult> OwnerResults { get; set; } = ownerResults;

        public int TotalPoints => OwnerResults.Sum(or => or.GetPoints());
        public int GetAmountOfPointResults(int points) => OwnerResults.Count(or => or.GetPoints() == points);
    }
}
