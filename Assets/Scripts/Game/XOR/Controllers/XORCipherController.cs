using System;
using Game.XOR.Interfaces;

namespace Game.XOR.Controllers
{
    public class XORCipherController:IXORCipherController
    {
        public byte[] Encrypt(byte[] data) => Cipher(data);
        
        public byte[] Decrypt(byte[] data) => Cipher(data);

        private byte[] GetGamma(byte[] data)
        {
            byte[] gamma = BitConverter.GetBytes(0);
            Array.Resize(ref gamma, data.Length);
            return gamma;
        }
        
        private byte[] Cipher(byte[] data)
        {
            var gama = GetGamma(data);

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ gama[i]);
            }

            return data;
        }
    }
}