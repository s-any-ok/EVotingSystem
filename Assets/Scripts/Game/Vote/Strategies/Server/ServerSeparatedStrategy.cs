using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Game.Vote.Interfaces;

namespace Game.Vote.Strategies.Server
{
    public class ServerSeparatedStrategy:IServerStrategy
    {
        public event Action<string> OnError;
    
        private RSA _rsa;

        public Dictionary<int, byte[]> Bulletins { get; } = new();

        public ServerSeparatedStrategy()
        {
            _rsa = RSA.Create();
        }
    
        public byte[] Sign(byte[] msg)
        {
            return _rsa.SignData(msg, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
        }
    
        private bool VerifySign(byte[] msg, byte[] signature)
        {
            return _rsa.VerifyData(msg, signature, HashAlgorithmName.SHA512,
                RSASignaturePadding.Pkcs1);
        }

        public void Accept(int id, byte[] msg, byte[] signature)
        {
            if (!VerifySign(msg, signature))
            {
                OnError?.Invoke("Sign is incorrect");
            }

            if (Bulletins.ContainsKey(id))
            {
                OnError?.Invoke("User already sent bulletin");
            }
        
            Bulletins.Add(id, msg);
        }
    }
}