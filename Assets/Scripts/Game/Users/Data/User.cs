using System;
using System.Security.Cryptography;

namespace Game.Users.Data
{
    [Serializable]
    public class User
    {
        public int Id;
        public bool CanVote;
        public bool IsLogedIn;
        public string Token;
        public string Login;
        public string Password;
        private RSA _rsa;
        private RSA _secondRsa;
        
        public RSAParameters PublicKey => _rsa.ExportParameters(false);

        public User(int keysize = 1024, int secondKeySize = 1024)
        {
            _rsa = RSA.Create(keysize);
            _secondRsa = RSA.Create(secondKeySize);
        }

        public byte[] ApplyPrivateKey(byte[] msg)
        {
            return _rsa.Decrypt(msg, RSAEncryptionPadding.Pkcs1);
        }
    
        public byte[] ApplyPublicKey(byte[] msg)
        {
            return _rsa.Encrypt(msg, RSAEncryptionPadding.Pkcs1);
        }
    
        public byte[] ApplySecondPrivateKey(byte[] msg)
        {
            return _secondRsa.Decrypt(msg, RSAEncryptionPadding.Pkcs1);
        }
    
        public byte[] ApplySecondPublicKey(byte[] msg)
        {
            return _secondRsa.Encrypt(msg, RSAEncryptionPadding.Pkcs1);
        }
    
        public byte[] SignWithPrivateKey(byte[] msg)
        {
            return _rsa.SignData(msg, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
        }
    
        public bool CheckIfSigned(byte[] signature, byte[] data)
        {
            return _rsa.VerifyData(data, signature, HashAlgorithmName.SHA512,
                RSASignaturePadding.Pkcs1);
        }
    }
}