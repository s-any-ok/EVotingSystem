using System;
using System.Collections.Generic;
using System.Linq;
using Game.BlumBlumShub.Interfaces;
using Game.DataProvider.Interfaces;
using Game.Registration.Interfaces;
using Game.Users.Data;
using Game.Vote.Data;
using Game.Vote.Enum;
using Game.Vote.Interfaces;

namespace Game.Vote.Controllers
{
    public class ClientVoteController: IClientVoteController
    {
        public event Action<string> OnError;
        private readonly IDataProviderController _dataProviderController;
        private readonly IServerVoteController _serverVoteController;
        private readonly IRegistrationController _registrationController;

        private IClientStrategy _strategy;

        private List<User> _voters;

        public ClientVoteController(IDataProviderController dataProviderController, IServerVoteController serverVoteController, IRegistrationController registrationController)
        {
            _dataProviderController = dataProviderController;
            _serverVoteController = serverVoteController;
            _registrationController = registrationController;
        }

        public void SetStrategy(IClientStrategy clientStrategy)
        {
            _strategy = clientStrategy;
        }
        
        public User Register(int ipn)
        {
            return _registrationController.RegisterVoter(ipn);
        }

        public string LogIn(string login, string password)
        {
            return _registrationController.LoginVoter(login, password);
        }

        public IEnumerable<ElectionResultsData> GetElectionResults()
        {
            switch (_strategy.Type)
            {
                // lab 5
                case EStrategy.SEPARATE:
                    return _serverVoteController.GetSeparatedResults().Select(r => new ElectionResultsData()
                    {
                        Votes = r.Value,
                        CandidateId = _dataProviderController.Candidates.FirstOrDefault(c => c.Id == r.Key).Id,
                        CandidateName = _dataProviderController.Candidates.FirstOrDefault(c => c.Id == r.Key)?.Name
                    });
                // lab 4
                case EStrategy.USERS:
                    return _strategy.GetResults().Select(r => new ElectionResultsData()
                    {
                        Votes = r.Value,
                        CandidateId = _dataProviderController.Candidates.FirstOrDefault(c => c.Id == r.Key).Id,
                        CandidateName = _dataProviderController.Candidates.FirstOrDefault(c => c.Id == r.Key)?.Name
                    });
            }
            return _dataProviderController.GetElectionResults();
        }
    
        public void Vote(int userId, int candidateId)
        {
            var user = _dataProviderController.GetUserById(userId);
            if (user == null)
            {
                OnError?.Invoke("User with such personal id does not exist");
            }
        
            ValidateData(user, candidateId);

            _strategy.Vote(user, candidateId);
        }

        private void ValidateData(User user, int candidateId)
        {
            if (!user.CanVote)
            {
                OnError?.Invoke("User with such personal id can not vote");
            }

            if (_dataProviderController.GetCandidateById(candidateId) == null)
            {
                OnError?.Invoke("Candidate with such id does not exist");
            }
        }
    }
}