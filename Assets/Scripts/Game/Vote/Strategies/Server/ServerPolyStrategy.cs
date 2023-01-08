using System;
using System.Collections.Generic;
using Game.Auth.Interfaces;
using Game.Bulletins.Data;
using Game.DataProvider.Interfaces;
using Game.Vote.Interfaces;

namespace Game.Vote.Strategies.Server
{
    public class ServerPolyStrategy:IServerStrategy
    {
        public event Action<string> OnError;
        private readonly IDataProviderController _dataProviderController;
        private readonly IServerVoteController _serverVoteController;

        private List<long> _regNumbers;

        private List<int> _votedUsers = new();
    
        public ServerPolyStrategy(
            IDataProviderController dataProviderController, IServerVoteController serverVoteController, 
            IAuthController authController)
        {
            _dataProviderController = dataProviderController;
            _serverVoteController = serverVoteController;

            _regNumbers = new List<long>(authController.RegistrationNumbers.ToArray());
        }

        public void Vote(Bulletin b, long regNumber)
        {
            if (_votedUsers.Contains(b.Id))
            {
                OnError?.Invoke("User already voted");
            }
            
            if (_dataProviderController.ElectionResults.Exists(vr => vr.UserId == b.Id))
            {
                OnError?.Invoke("User already voted");
            }
        
            if (!_regNumbers.Contains(regNumber))
            {
                OnError?.Invoke("Registration number does not exist");
            }

            if (!_serverVoteController.CheckIfSigned(b.Message, b.Sign))
            {
                OnError?.Invoke("Message is not signed");
            }

            var bulletinId = _serverVoteController.ApplyPrivateKey(b.Message);
            var bulletin = _dataProviderController.GetBulletinId(BitConverter.ToInt32(bulletinId));

            if (bulletin.UserId != b.Id)
            {
                OnError?.Invoke("User ids mismatch");
            }
        
            _votedUsers.Add(b.Id);
            _regNumbers.Remove(regNumber);
            
            _dataProviderController.SaveElectionResult(new ElectionResult.Data.ElectionResult(b.Id, bulletin.CandidateId));
        }
    }
}