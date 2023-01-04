using System;

namespace Game.Bulletins.Data
{
    [Serializable]
    public class Bulletin
    {
        public int Id;

        public byte[] Sign;

        public byte[] Message;
    }
}