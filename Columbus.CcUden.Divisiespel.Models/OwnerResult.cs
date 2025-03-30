namespace Columbus.CcUden.Divisiespel.Models
{
    /// <summary>
    /// Models the total entries of an owner in a single result.
    /// </summary>
    public class OwnerResult(string name)
    {
        /// <summary>
        /// Name as it appears on the result.
        /// </summary>
        public string Name { get; set; } = name;
        /// <summary>
        /// How often the owner occurs in the result.
        /// </summary>
        public int Occurences { get; set; }
        /// <summary>
        /// True if one of the top 2 designated pigeons has occurred in the results, false if not.
        /// </summary>
        public bool HasDesignated { get; set; }
    }
}
