using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Game.Auth.Interfaces;
using Game.DataProvider.Interfaces;
using Game.Users.Data;
using Game.Vote.Data;
using Game.Vote.Enum;
using Game.Vote.Interfaces;
using Random = System.Random;

namespace Game.Vote.Strategies.Client
{
    public class ClientPolyVoteStrategy: BaseClientStrategy
    {
        public override event Action<string> OnError;
        private const int MAX_INT = 2147483;
        private readonly IDataProviderController _dataProviderController;
        private readonly IServerVoteController _serverVoteController;
        private readonly IAuthController _authController;

        private readonly Dictionary<int, int> _usersRandomId;
        private readonly Random _random;
        
        public override EStrategy Type => EStrategy.POLY;

        public ClientPolyVoteStrategy(
            IDataProviderController dataProviderController, 
            IServerVoteController serverVoteController,
            IAuthController authController)
        {
            _dataProviderController = dataProviderController;
            _serverVoteController = serverVoteController;
            _authController = authController;

            _usersRandomId = new Dictionary<int, int>();
            _random = new Random();
        }

        public override void Vote(User user, int candidateId)
        {
            var regNumber = _authController.GenerateRegNumber(user.Id);

            if (!_usersRandomId.ContainsKey(user.Id))
            {
                var randomId = _random.Next(1, MAX_INT);
                while (_usersRandomId.ContainsValue(randomId))
                {
                    randomId = _random.Next(1, MAX_INT);
                }
            
                _usersRandomId.Add(user.Id, randomId);
            }

            var id = _usersRandomId[user.Id];

            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(_serverVoteController.PublicKey);
            
            var bulletinId = _dataProviderController.CreateBulletinId(id, candidateId);
            var hashed = rsa.Encrypt(BitConverter.GetBytes(bulletinId), RSAEncryptionPadding.Pkcs1);
            var signed = _serverVoteController.SignWithPrivateKey(hashed);

            _serverVoteController.Vote(new PolyBulletinData()
            {
                Id = id,
                RegNumber = regNumber,
                Data = hashed,
                SignedData = signed
            });
        }
    }
}