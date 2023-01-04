using System.Collections.Generic;

namespace Game.Vote.Data
{
    public class BlindPackageData
    {
        public long Id { get; set; }
        public List<byte[]> Messages { get; set; }
        public List<byte[]> SignedBulletins { get; set; }
        public List<byte[]> Bulletins { get; set; }
    }
}