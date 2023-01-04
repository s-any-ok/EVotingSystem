using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Game.DataProvider.Interfaces;
using Game.Primes.Interfaces;
using Game.Users.Data;
using Game.Vote.Data;
using Game.Vote.Enum;
using Game.Vote.Interfaces;
using Game.XOR.Interfaces;

namespace Game.Vote.Strategies.Client
{
    public class ClientBlindVoteStrategy: BaseClientStrategy
    {
        public override event Action<string> OnError;
        
        private readonly IDataProviderController _dataProviderController;
        private readonly IServerVoteController _serverVoteController;
        private readonly IXORCipherController _xorCipherController;
        private readonly IPrimesController _primesController;

        public override EStrategy Type => EStrategy.BLIND;

        public ClientBlindVoteStrategy(IDataProviderController dataProviderController, IServerVoteController serverVoteController,
            IXORCipherController xorCipherController, IPrimesController primesController)
        {
            _dataProviderController = dataProviderController;
            _serverVoteController = serverVoteController;
            _xorCipherController = xorCipherController;
            _primesController = primesController;
        }
    
        public override void Vote(User user, int candidateId)
        {
            var packages = CreatePackages(user);
        
            var id = BitConverter.ToInt32(_xorCipherController.Encrypt(BitConverter.GetBytes(user.Id)));

            var bundle = new PackagesData()
            {
                Packages = packages,
                Id = id
            };

            var signed = (BlindPackageData)_serverVoteController.Sign(bundle);

            if (signed.Messages!.Count != signed.SignedBulletins!.Count)
            {
                OnError?.Invoke("Center voting system hasn't signed messages");
            }
        
            for (int i = 0; i < signed.Messages.Count; i++)
            {
                var msg = signed.Messages[i];
                var signedBulletin = signed.SignedBulletins[i];
                
                var bulletinId = user.ApplyPrivateKey(_xorCipherController.Decrypt(msg));

                var bulletin = _dataProviderController.GetBulletinId(BitConverter.ToInt32(bulletinId));

                if (bulletin.CandidateId != candidateId) continue;

                if (bulletin.UserId != id) continue;

                var rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(_serverVoteController.PublicKey);

                var preparedBulletin = rsa.Encrypt(bulletinId, RSAEncryptionPadding.Pkcs1);
            
                _serverVoteController.Vote(preparedBulletin, signedBulletin, signed.Bulletins![i]);
            }
        }
        
        private List<BlindPackageData> CreatePackages(User u)
        {
            var packages = new List<BlindPackageData>();
            var candidates = _dataProviderController.Candidates;
            var id = BitConverter.ToInt32(_xorCipherController.Encrypt(BitConverter.GetBytes(u.Id)));
            
            for (int i = 0; i < 10; i++)
            {
                var maskedBulletins = new List<byte[]>();
                var bulletins = new List<byte[]>();
                var random = new Random();
                
                var generatedPrimes = _primesController.GeneratePrimes(random.Next(50, 150));
                var rnd = random.Next(0, generatedPrimes.Count);
                var r = generatedPrimes[rnd];

                while (BitConverter.ToInt64(u.PublicKey.Modulus) % r == 0)
                {
                    r = generatedPrimes[++rnd % generatedPrimes.Count];
                }

                foreach (var c in candidates)
                {
                    var bulletinId = u.ApplyPublicKey(BitConverter.GetBytes(_dataProviderController.CreateBulletinId(id, c.Id)));
                    var maskedBulletin = _xorCipherController.Encrypt(bulletinId);

                    maskedBulletins.Add(maskedBulletin);
                    bulletins.Add(bulletinId);
                }
            
                packages.Add(new BlindPackageData()
                {
                    Id = r,
                    Messages = maskedBulletins,
                    Bulletins = bulletins
                });
            }

            return packages;
        }
    }
}