namespace Columbus.CcUden.Divisiespel.Models
{
    public class League
    {
        public required int Rank { get; set; }
        public required string Name { get; set; }
        public List<string> Owners { get; set; } = [];

        public bool AddOwner(string owner)
        {
            if (Owners.Contains(owner))
                return false;

            Owners.Add(owner);
            return true;
        }

        public bool RemoveOwner(string owner) => Owners.Remove(owner);

        public override string ToString() => Name;
    }
}
