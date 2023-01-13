using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using Game.Auth.Interfaces;
using Game.BlumBlumShub.Interfaces;
using Game.Bulletins.Data;
using Game.DataProvider.Interfaces;
using Game.Encryptor.Interfaces;
using Game.Primes.Interfaces;
using Game.Token.Interfaces;
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
        
        private Dictionary<int, (int p, int q)> _votersKeys = new();

        private List<byte[]> bulletins = new();

        private readonly IDataProviderController _dataProviderController;
        private readonly IAuthController _authController;
        private readonly IXORCipherController _xorCipherController;
        private readonly IPrimesController _primesController;
        private readonly IEncryptorController _encryptorController;
        private readonly IBlumBlumShubController _blumBlumShubController;
        private readonly ITokenController _tokenController;

        private IServerStrategy _strategy;
        private List<ServerSeparatedStrategy> _separatedVotingStrategies;
        
        public List<ServerSeparatedStrategy> SeparatedVotingStrategies => _separatedVotingStrategies;

        public ServerVoteController(IDataProviderController dataProviderController, IAuthController authController,
            IXORCipherController xorCipherController,IPrimesController primesController, IEncryptorController encryptorController,
            IBlumBlumShubController blumBlumShubController, ITokenController tokenController)
        {
            _dataProviderController = dataProviderController;
            _authController = authController;
            _xorCipherController = xorCipherController;
            _primesController = primesController;
            _encryptorController = encryptorController;
            _blumBlumShubController = blumBlumShubController;
            _tokenController = tokenController;

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

        public void GenerateKeys(List<int> ids)
        {
            var primes = _primesController.GeneratePrimesNaive(ids.Count * 2, 3, 5);
            var pos = 0;
            
            foreach (var id in ids)
            {
                if (_votersKeys.ContainsKey(id))
                {
                    throw new Exception("not unique id");
                }
                
                _votersKeys.Add(id, (primes[pos++], primes[pos++]));
            }
        }

        public List<string> GetTokens()
        {
            return _votersKeys.Select(pair => _tokenController.GenerateToken(pair.Key, pair.Value.p * pair.Value.q)).ToList();
        }

        public void Vote(byte[] msg)
        {
            bulletins.Add(msg);
        }

        public IDictionary<int, int> ComputeResults()
        {
            var ids = new HashSet<int>();
            var res = new Dictionary<int, int>();

            foreach (var msg in bulletins)
            {
                var (id, candidate) = GetVoteResult(msg);

                if (ids.Contains(id)) continue; // voter already voted

                if (res.ContainsKey(candidate))
                {
                    res[candidate]++;
                }
                else
                {
                    res.Add(candidate, 1);
                }

                ids.Add(id);
            }

            return res;
        }

        private (int id, int candidate) GetVoteResult(byte[] msg)
        {
            var decrypted = _encryptorController.Decrypt2(msg);
            var decryptedWithoutId = decrypted;

            var keys = (-1, -1);

            foreach (var pair in _votersKeys)
            {
                var idBytes = BitConverter.GetBytes(pair.Key);

                var last = decrypted.TakeLast(idBytes.Length);

                if (last.SequenceEqual(idBytes))
                {
                    keys = pair.Value;

                    decryptedWithoutId = decrypted.SkipLast(idBytes.Length).ToArray();
                    
                    break;
                }
            }

            if (keys.Item1 == -1 || keys.Item2 == -1)
            {
                throw new Exception("voter not found");
            }

            var bit = BitConverter.GetBytes(_blumBlumShubController.GetBit(keys.Item1 * keys.Item2));

            if (!decryptedWithoutId.TakeLast(bit.Length).SequenceEqual(bit))
            {
                throw new Exception("mismatch");
            }

            var bulletinEncrypted = decryptedWithoutId.SkipLast(bit.Length).ToArray();
            var bulletinDecrypted = _encryptorController.Decrypt(bulletinEncrypted);

            var bulletin = _dataProviderController.GetBulletinId(BitConverter.ToInt32(bulletinDecrypted));

            return (bulletin.UserId, bulletin.CandidateId);
        }

        public void Dispose()
        {
            _strategy.OnError -= OnError;
        }
    }
}