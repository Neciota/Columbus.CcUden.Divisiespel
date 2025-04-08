namespace Columbus.CcUden.Divisiespel.Models
{
    public class League
    {
        public required int Rank { get; set; }
        public required string Name { get; set; }
        public List<Owner> Owners { get; set; } = [];

        public bool AddOwner(Owner owner)
        {
            if (Owners.Contains(owner))
                return false;

            Owners.Add(owner);
            return true;
        }

        public bool RemoveOwner(Owner owner) => Owners.Remove(owner);

        public Dictionary<Owner, int> GetTotalPointsByLeagueOwner(Dictionary<(FlightCode Flight, Owner Owner), int> pointsByOwnerFlight)
        {
            return Owners.Select(owner => (owner, pointsByOwnerFlight.Where(pof => pof.Key.Owner == owner)))
                .ToDictionary(op => op.owner, op => op.Item2.Sum(pof => pof.Value));
        }

        public override string ToString() => Name;
    }
}
