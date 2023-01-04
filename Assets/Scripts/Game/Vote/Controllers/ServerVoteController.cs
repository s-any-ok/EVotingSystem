using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using Game.Auth.Interfaces;
using Game.Bulletins.Data;
using Game.DataProvider.Interfaces;
using Game.Vote.Data;
using Game.Vote.Interfaces;
using Game.Vote.Strategies.Server;
using Game.XOR.Interfaces;

namespace Game.Vote.Controllers
{
    public class ServerVoteController: IServerVoteController, IDisposable
    {
        public event Action<string> OnErrorE;
        private RSA _rsa;
        public RSAParameters PublicKey => _rsa.ExportParameters(false);

        private readonly IDataProviderController _dataProviderController;
        private readonly IAuthController _authController;
        private readonly IXORCipherController _xorCipherController;

        private IServerStrategy _strategy;
        private List<ServerSeparatedStrategy> _separatedVotingStrategies;
        
        public List<ServerSeparatedStrategy> SeparatedVotingStrategies => _separatedVotingStrategies;

        public ServerVoteController(IDataProviderController dataProviderController, IAuthController authController,
            IXORCipherController xorCipherController)
        {
            _dataProviderController = dataProviderController;
            _authController = authController;
            _xorCipherController = xorCipherController;

            _rsa = RSA.Create();

            _separatedVotingStrategies = new List<ServerSeparatedStrategy>() {new(), new()};
        }
        
        private void OnError(string value)
        {
            OnErrorE?.Invoke(value);
        }
        
        // 1
        public void Vote(byte[] msg, byte[] sign)
        {
            var strategy = new ServerSimpleStrategy(_dataProviderController, this, _xorCipherController);
            _strategy = strategy;
            _strategy.OnError += OnError;
            strategy.Vote(new Bulletin { Message = msg, Sign = sign});
        }

        // 2
        public void Vote(byte[] msg, byte[] sign, byte[] signedData)
        {
            var strategy = new ServerBlindStrategy(_dataProviderController, this, _xorCipherController);
            _strategy = strategy;
            _strategy.OnError += OnError;
            strategy.Vote(msg, sign, signedData);
        }

        // 3
        public void Vote(PolyBulletinData msg)
        {
            var strategy = new ServerPolyStrategy(_dataProviderController, this, _authController);
            _strategy = strategy;
            _strategy.OnError += OnError;
            strategy.Vote(new Bulletin()
            {
                Id = msg.Id,
                Message = msg.Data,
                Sign = msg.SignedData
            }, msg.RegNumber);
        }

        // 2
        public object Sign(object msgs)
        {
            var strategy = new ServerBlindStrategy(_dataProviderController, this, _xorCipherController);
            _strategy = strategy;
            _strategy.OnError += OnError;
            return strategy.Sign(msgs);
        }
    
        public byte[] SignWithPrivateKey(byte[] msg)
        {
            return _rsa.SignData(msg, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
        }
    
        public bool CheckIfSigned(byte[] msg, byte[] signature)
        {
            return _rsa.VerifyData(msg, signature, HashAlgorithmName.SHA512,
                RSASignaturePadding.Pkcs1);
        }
    
        public byte[] ApplyPublicKey(byte[] msg)
        {
            return _rsa.Encrypt(msg, RSAEncryptionPadding.Pkcs1);
        }

        public byte[] ApplyPrivateKey(byte[] msg)
        {
            return _rsa.Decrypt(msg, RSAEncryptionPadding.Pkcs1);
        }

        public BigInteger Encrypt(BigInteger x)
        {
            return BigInteger.Pow(x, 7) % 33;
        }
    
        private BigInteger Decrypt(BigInteger x)
        {
            return BigInteger.Pow(x, 3) % 33;
        }

        public Dictionary<int, int> GetSeparatedResults()
        {
            var a = _separatedVotingStrategies[0];
            var b = _separatedVotingStrategies[1];
        
            var res = new Dictionary<int, int>();

            foreach (var key in a.Bulletins.Keys)
            {
                if (!b.Bulletins.ContainsKey(key))
                {
                    OnErrorE?.Invoke("missing bulletin part");
                }
            
                var x = new BigInteger(a.Bulletins[key]);
                var y = new BigInteger(b.Bulletins[key]);

                var multiplied = x * y;
            
                var decrypted = Decrypt(multiplied);
                var candidateId = (int)decrypted;
            
                if (res.ContainsKey(candidateId))
                {
                    res[candidateId] += 1;
                }
                else
                {
                    res.Add(candidateId, 1);
                }
            }

            return res;
        }

        public void Dispose()
        {
            _strategy.OnError -= OnError;
        }
    }
}