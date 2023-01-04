using System;
using System.Security.Cryptography;
using Game.DataProvider.Interfaces;
using Game.Users.Data;
using Game.Vote.Enum;
using Game.Vote.Interfaces;
using Game.XOR.Interfaces;

namespace Game.Vote.Strategies.Client
{
    public class ClientSimpleVoteStrategy: BaseClientStrategy
    {
        public override event Action<string> OnError;
        private readonly IDataProviderController _dataProviderController;
        private readonly IServerVoteController _serverVoteController;
        private readonly IXORCipherController _xorCipherController;
        
        public override EStrategy Type => EStrategy.SIMPLE;

        public ClientSimpleVoteStrategy(IDataProviderController dataProviderController, IServerVoteController serverVoteController,
            IXORCipherController xorCipherController)
        {
            _dataProviderController = dataProviderController;
            _serverVoteController = serverVoteController;
            _xorCipherController = xorCipherController;
        }
    
        public override void Vote(User user, int candidateId)
        {
            var bulletinId = _dataProviderController.CreateBulletinId(user.Id, candidateId);

            var hashed = _xorCipherController.Encrypt(BitConverter.GetBytes(bulletinId));
            var signedMsg = user.SignWithPrivateKey(hashed);
        
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(_serverVoteController.PublicKey);

            var encrypted = rsa.Encrypt(BitConverter.GetBytes(bulletinId), RSAEncryptionPadding.Pkcs1);
        
            _serverVoteController.Vote(encrypted, signedMsg);
        }
    }
}