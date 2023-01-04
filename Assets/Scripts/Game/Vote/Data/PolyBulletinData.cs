namespace Game.Vote.Data
{
    public class PolyBulletinData
    {
        public int Id { get; set; }
        public long RegNumber { get; set; }
        public byte[] Data { get; set; }
        public byte[] SignedData { get; set; }
    }
}