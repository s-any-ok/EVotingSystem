using System;
using System.Collections.Generic;
using Game.DataProvider.Interfaces;
using Game.Vote.Data;
using Game.Vote.Interfaces;
using Game.XOR.Interfaces;

namespace Game.Vote.Strategies.Server
{
    public class ServerBlindStrategy:IServerStrategy
    {
        public event Action<string> OnError;
        private List<long> _checkedUsers = new();

        private readonly IDataProviderController _dataProviderController;
        private readonly IServerVoteController _serverVoteController;
        private readonly IXORCipherController _xorCipherController;

        public ServerBlindStrategy(IDataProviderController dataProviderController, IServerVoteController serverVoteController,
            IXORCipherController xorCipherController)
        {
            _dataProviderController = dataProviderController;
            _serverVoteController = serverVoteController;
            _xorCipherController = xorCipherController;
        }

        public BlindPackageData Sign(object obj)
        {
            var bundle = (PackagesData)obj;
        
            if (_checkedUsers.Contains(bundle.Id))
            {
                OnError?.Invoke("User has already sent bulletins");
            }
        
            var random = new Random();
            var uncheckedIdx = random.Next(0, bundle.Packages.Count);

            for (int i = 0; i < bundle.Packages.Count; i++)
            {
                if (i == uncheckedIdx) continue;

                var pkg = bundle.Packages[i];

                for (int j = 0; j < pkg.Messages!.Count; j++)
                {
                    if (_xorCipherController.Decrypt(pkg.Messages[j]) != pkg.Bulletins![j])
                    {
                        OnError?.Invoke("Packages are not valid");
                    }
                }
            }
        
            _checkedUsers.Add(bundle.Id);

            var signedBulletins = new List<byte[]>();
            foreach (var m in bundle.Packages[uncheckedIdx].Bulletins!)
            {
                signedBulletins.Add(_serverVoteController.SignWithPrivateKey(m));
            }

            return new BlindPackageData
            {
                Id = bundle.Packages[uncheckedIdx].Id,
                Messages = bundle.Packages[uncheckedIdx].Messages,
                Bulletins = bundle.Packages[uncheckedIdx].Bulletins,
                SignedBulletins = signedBulletins
            };
        }

        public void Vote(byte[] msg, byte[] signed, byte[] _)
        {
            var bulletinId = _serverVoteController.ApplyPrivateKey(msg);
            var bulletin = _dataProviderController.GetBulletinId(BitConverter.ToInt32(bulletinId));

            ValidateUserAlreadyVoted(bulletin.UserId);
        
            _dataProviderController.SaveElectionResult(new ElectionResult.Data.ElectionResult(bulletin.UserId, bulletin.CandidateId));
        }
    
        private void ValidateUserAlreadyVoted(int userId)
        {
            if (_dataProviderController.ElectionResults.Exists(vr => vr.UserId == userId))
            {
                OnError?.Invoke("User already voted");
            }
        }
    }
}