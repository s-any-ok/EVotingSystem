using System;
using System.Collections.Generic;
using System.Text;
using Game.DataProvider.Interfaces;
using Game.Users.Data;
using Game.Vote.Classes;
using Game.Vote.Enum;

namespace Game.Vote.Strategies.Client
{
    public class ClientUsersVoteStrategy: BaseClientStrategy
    {
        public override event Action<string> OnError;
        private readonly IDataProviderController _dataProviderController;
        private readonly UserChain _userChain;

        private Dictionary<int, string> _usersStrings;
        
        public override EStrategy Type => EStrategy.USERS;

        public ClientUsersVoteStrategy(IDataProviderController dataProviderController, UserChain userChain)
        {
            _dataProviderController = dataProviderController;
            _userChain = userChain;

            _usersStrings = new Dictionary<int, string>();
        }
    
        public override void Vote(User user, int candidateId)
        {
            var bulletinId = _dataProviderController.CreateBulletinId(user.Id, candidateId);
            var randomStr = StringGenerator.GenerateRandom();

            if (_usersStrings.ContainsKey(user.Id))
            {
                OnError?.Invoke("User already voted");
            }
            
            _usersStrings.Add(user.Id, randomStr);

            var bulletinStr = bulletinId + randomStr;
            var bytes = Encoding.UTF8.GetBytes(bulletinStr);

            var encrypted = _userChain.EncryptRSA(bytes);
            var encryptedWithStrings = _userChain.EncryptRSAWithStr(encrypted, user.Id);

            var userChain = _userChain;
            while (_userChain.Next != null)
            {
                userChain = userChain.Next;
            }
        
            userChain.AcceptBulletin(encryptedWithStrings, user.Id);
        }
    
        public override Dictionary<int, int> GetResults()
        {
            var results = new Dictionary<int, int>();
        
            var userChain = _userChain;
            while (userChain.Next != null)
            {
                userChain = userChain.Next;
            }

            var bulletins = userChain.Bulletins;
            var withoutStrBatch = userChain.DencryptRSARemoveStrBatch(bulletins.ToArray());

            var signed = userChain.DencryptRSAAndSignBatch(withoutStrBatch);

            foreach (var signedBulletin in signed)
            {
                if (!_userChain.VerifySign(signedBulletin.msg, signedBulletin.signed))
                {
                    OnError?.Invoke("Invalid sign");
                }

                var str = _usersStrings[signedBulletin.id];
            
                var withoutStr = Encoding.UTF8.GetString(signedBulletin.msg);

                if (!withoutStr.Contains(str))
                {
                    OnError?.Invoke("Missing bulletins");
                }

                withoutStr = withoutStr.Replace(str, "");
                int bulletinId;

                if (!Int32.TryParse(withoutStr, out bulletinId))
                {
                    OnError?.Invoke("Something went wrong");
                }

                var bulletin = _dataProviderController.GetBulletinId(bulletinId);

                if (results.ContainsKey(bulletin.CandidateId))
                {
                    results[bulletin.CandidateId] += 1;
                }
                else
                {
                    results.Add(bulletin.CandidateId, 1);
                }
            }

            return results;
        }
    }
}