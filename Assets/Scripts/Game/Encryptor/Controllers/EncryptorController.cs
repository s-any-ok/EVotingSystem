using System.Security.Cryptography;
using Game.Encryptor.Interfaces;
using Game.Primes.Interfaces;

namespace Game.Encryptor.Controllers
{
    public class EncryptorController:IEncryptorController
    {
        private RSA rsa = RSA.Create();

        private RSA rsa2 = RSA.Create(2048 + 512);
    
        public byte[] Encrypt(byte[] msg)
        {
            return rsa.Encrypt(msg, RSAEncryptionPadding.Pkcs1);
        }
    
        public byte[] Decrypt(byte[] msg)
        {
            return rsa.Decrypt(msg, RSAEncryptionPadding.Pkcs1);
        }
    
        public byte[] Encrypt2(byte[] msg)
        {
            return rsa2.Encrypt(msg, RSAEncryptionPadding.Pkcs1);
        }
    
        public byte[] Decrypt2(byte[] msg)
        {
            return rsa2.Decrypt(msg, RSAEncryptionPadding.Pkcs1);
        }
    }
}