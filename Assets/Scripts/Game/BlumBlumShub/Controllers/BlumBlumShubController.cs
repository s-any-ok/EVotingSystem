using System;
using System.Linq;
using Game.BlumBlumShub.Interfaces;
using Game.Encryptor.Interfaces;

namespace Game.BlumBlumShub.Controllers
{
    public class BlumBlumShubController:IBlumBlumShubController
    {
        private readonly IEncryptorController _encryptorController;


        public BlumBlumShubController(IEncryptorController encryptorController)
        {
            _encryptorController = encryptorController;
        }
        
        public int GetBit(int m)
        {
            var seed = 6367859;

            int[] bits = new int[100];

            var xprev = seed;
            for(int k = 0; k != 100; ++k) {
                var xnext = Next(xprev, m);
                int bit = Parity(xnext); // extract random bit from generated BBS number using parity,
                // or just int bit = LSB(xnext);

                bits[k] = bit;

                xprev = xnext;
            }

            return bits[0];
        }
        
        private int Next(int prev, int m) {
            return prev * prev % m;
        }

        private int Parity(int n) {
            var q = n;
            var count = 0;
            while (q != 0) {
                count += q & 1;
                q >>= 1;
            }
            return ((count & 1) != 0) ? 1 : 0; // even parity
        }

        private int LSB(int n) {
            return ((n & 1) != 0) ? 1 : 0;
        }

        public byte[] Encrypt(byte[] msg, int m, int id)
        {
            var bit = GetBit(m);
        
            msg = msg.Concat(BitConverter.GetBytes(bit)).ToArray();
            msg = msg.Concat(BitConverter.GetBytes(id)).ToArray();

            return _encryptorController.Encrypt2(msg);
        }
    }
}