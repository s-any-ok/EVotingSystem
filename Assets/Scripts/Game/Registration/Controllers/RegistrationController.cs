using System;
using System.Collections.Generic;
using System.Linq;
using Game.DataProvider.Interfaces;
using Game.Registration.Interfaces;
using Game.Users.Data;
using Game.Vote.Classes;
using Game.Vote.Interfaces;

namespace Game.Registration.Controllers
{
    public class RegistrationController:IRegistrationController
    {
        private readonly IDataProviderController _dataProviderController;
        private readonly IServerVoteController _serverVoteController;

        public RegistrationController(IDataProviderController dataProviderController, IServerVoteController serverVoteController)
        {
            _dataProviderController = dataProviderController;
            _serverVoteController = serverVoteController;

            var ids = GenerateIds();
            _serverVoteController.GenerateKeys(ids);
            
            SaveTokens(_serverVoteController.GetTokens());
        }

        public User RegisterVoter(int ipn)
        {
            var u = _dataProviderController.GetUserById(ipn);
            if (u == null || !u.CanVote)
            {
                throw new Exception("user cannot vote");
            }

            var user = _dataProviderController.Users.FirstOrDefault(p => p.Id == ipn);
            if (_dataProviderController.Tokens.Contains(user.Token))
            {
                return user;
            }

            var voter = new User();
            voter.Login = StringGenerator.GenerateRandom();
            voter.Password = StringGenerator.GenerateRandom();
            voter.Id = ipn;
            voter.CanVote = u.CanVote;

            var usersTokens = _dataProviderController.Users.ConvertAll(x => x.Token);
            var token = _dataProviderController.Tokens.FirstOrDefault(x => !usersTokens.Contains(x));
            voter.Token = token;
            
            _dataProviderController.UpdateUser(voter);

            return voter;
        }

        public string LoginVoter(string login, string password)
        {
            var token = GetToken(login, password);
            var user = _dataProviderController.Users.FirstOrDefault(x => x.Token == token);
            
            if (user != null)
            {
                user.IsLogedIn = true;
                _dataProviderController.UpdateUser(user);
            }

            return token;
        }

        private string GetToken(string login, string password)
        {
            var user = _dataProviderController.Users.FirstOrDefault(p => p.Login == login && p.Password == password);
            if (_dataProviderController.Tokens.FirstOrDefault(x => x == user.Token) == null)
            {
                throw new Exception("invalid credentials");
            }

            return user.Token;
        }
        
        private List<int> GenerateIds()
        {
            var count = _dataProviderController.GetVoters(100).Count;

            var rnd = new Random();
            
            return Enumerable.Range(0, count)
                .Select(i => new Tuple<int, int>(rnd.Next(count), i))
                .OrderBy(i => i.Item1)
                .Select(i => i.Item2)
                .ToList();
        }

        private void SaveTokens(List<string> tokens)
        {
            var i = 1;
            
            tokens.ForEach(t => _dataProviderController.Tokens.Add(t));
        }
    }
}