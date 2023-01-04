using System.Collections.Generic;

namespace Game.Vote.Data
{
    public class PackagesData
    {
        public long Id { get; set; }
        public List<BlindPackageData> Packages { get; set; }
    }
}