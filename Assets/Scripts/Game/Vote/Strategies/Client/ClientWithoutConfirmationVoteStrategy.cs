using System;
using System.Collections.Generic;
using System.Linq;
using Game.BlumBlumShub.Interfaces;
using Game.DataProvider.Interfaces;
using Game.Encryptor.Interfaces;
using Game.Token.Interfaces;
using Game.Users.Data;
using Game.Vote.Enum;
using Game.Vote.Interfaces;

namespace Game.Vote.Strategies.Client
{
    public class ClientWithoutConfirmationVoteStrategy: BaseClientStrategy
    {
        public override event Action<string> OnError;
        private readonly IDataProviderController _dataProviderController;
        private readonly IServerVoteController _serverVoteController;
        private readonly ITokenController _tokenController;
        private readonly IBlumBlumShubController _blumBlumShubController;
        private readonly IEncryptorController _encryptorController;

        public override EStrategy Type => EStrategy.WITHOUT_CONFIRM;

        public ClientWithoutConfirmationVoteStrategy(IDataProviderController dataProviderController, IServerVoteController serverVoteController,
            ITokenController tokenController, IBlumBlumShubController blumBlumShubController, IEncryptorController encryptorController)
        {
            _dataProviderController = dataProviderController;
            _serverVoteController = serverVoteController;
            _tokenController = tokenController;
            _blumBlumShubController = blumBlumShubController;
            _encryptorController = encryptorController;
        }

        public override void Vote(User user, int candidateId)
        {
            var msg = CreateMessage(user.Token, candidateId);
            _serverVoteController.Vote(msg);
        }

        private byte[] CreateMessage(string token, int candidateId)
        {
            var data = _tokenController.GetTokenData(token);

            var bulletinId = _dataProviderController.CreateBulletinId(data.id, candidateId);
        
            var encrypted = _encryptorController.Encrypt(BitConverter.GetBytes(bulletinId));
        
            return _blumBlumShubController.Encrypt(encrypted, data.bbsKey, data.id);
        }

        public override Dictionary<int, int> GetResults()
        {
            var res = _serverVoteController.ComputeResults();
        
            return res.Select(r => new 
                {
                    Score = r.Value,
                    CandidateId = _dataProviderController.Candidates.FirstOrDefault(c => c.Id == r.Key).Id
                })
                .ToDictionary(e => e.CandidateId, e => e.Score);
        }
    }
}