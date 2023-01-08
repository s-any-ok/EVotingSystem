using System;
using Game.Bulletins.Data;
using Game.DataProvider.Interfaces;
using Game.Vote.Interfaces;
using Game.XOR.Interfaces;

namespace Game.Vote.Strategies.Server
{
    public class ServerSimpleStrategy:IServerStrategy
    {
        public event Action<string> OnError;
        
        private readonly IDataProviderController _dataProviderController;
        private readonly IServerVoteController _serverVoteController;
        private readonly IXORCipherController _xorCipherController;

        public ServerSimpleStrategy(IDataProviderController dataProviderController, IServerVoteController serverVoteController,
            IXORCipherController xorCipherController)
        {
            _dataProviderController = dataProviderController;
            _serverVoteController = serverVoteController;
            _xorCipherController = xorCipherController;
        }
    
        public void Vote(Bulletin b)
        {
            var decrypted = _serverVoteController.ApplyPrivateKey(b.Message);
            var bulletinId = BitConverter.ToInt32(decrypted);

            var bulletin = _dataProviderController.GetBulletinId(bulletinId);

            var user = _dataProviderController.GetUserById(bulletin.UserId);
            if (user == null)
            {
                OnError?.Invoke("User not found");
            }

            if (b.Id != 0 && b.Id != bulletinId)
            {
                OnError?.Invoke("Bulletin id data missmatch");
            }
        
            if (_dataProviderController.ElectionResults.Exists(vr => vr.UserId == bulletin.UserId))
            {
                OnError?.Invoke("User already voted");
            }

            if (!user.CheckIfSigned(b.Sign, _xorCipherController.Encrypt(decrypted)))
            {
                OnError?.Invoke("Sign data and message data mismatched; signed incorrectly");
            }

            b.Id = bulletinId;
        
            _dataProviderController.SaveBulletin(b);
            _dataProviderController.SaveElectionResult(new ElectionResult.Data.ElectionResult(bulletin.UserId, bulletin.CandidateId));
        }
    }
}