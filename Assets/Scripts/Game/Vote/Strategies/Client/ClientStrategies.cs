using System;
using System.Collections.Generic;
using Game.Auth.Interfaces;
using Game.DataProvider.Interfaces;
using Game.Primes.Interfaces;
using Game.Users.Data;
using Game.Vote.Classes;
using Game.Vote.Enum;
using Game.Vote.Interfaces;
using Game.XOR.Interfaces;

namespace Game.Vote.Strategies.Client
{
    public class ClientStrategies: IClientStrategies, IDisposable
    {
        public event Action<string> OnErrorE;
        private readonly IDataProviderController _dataProviderController;
        private readonly IAuthController _authController;
        private readonly IServerVoteController _serverVoteController;
        private readonly IXORCipherController _xorCipherController;
        private readonly IPrimesController _primesController;
        
        public ClientStrategies(
            IDataProviderController dataProviderController,
            IAuthController authController,
            IServerVoteController serverVoteController,
            IXORCipherController xorCipherController, IPrimesController primesController
        )
        {
            _dataProviderController = dataProviderController;
            _authController = authController;
            _serverVoteController = serverVoteController;
            _xorCipherController = xorCipherController;
            _primesController = primesController;

            _authController.OnError += OnError;
        }
        
        private void OnError(string val)
        {
            OnErrorE?.Invoke(val);
        }

        public IClientStrategy GetStrategy(EStrategy strategy)
        {
            switch (strategy)
            {
                case EStrategy.POLY:
                    return new ClientPolyVoteStrategy(_dataProviderController, _serverVoteController, _authController);
                case EStrategy.BLIND:
                    return new ClientBlindVoteStrategy(_dataProviderController, _serverVoteController,
                        _xorCipherController, _primesController);
                case EStrategy.USERS:
                {
                    List<User> voters = _dataProviderController.GetVoters(4);
                    if (voters.Count != 4)
                    {
                        OnErrorE?.Invoke("not enough voters");
                    }
                
                    var a = new UserChain(voters[0]);
                    var b = new UserChain(voters[1]);
                    var c = new UserChain(voters[2]);
                    var d = new UserChain(voters[3]);

                    d.Next = c;
                    c.Next = b;
                    b.Next = a;
        
                    a.Prev = b;
                    b.Prev = c;
                    c.Prev = d;
        
                    return new ClientUsersVoteStrategy(_dataProviderController, d);
                }
                    
                case EStrategy.SEPARATE:
                    return new ClientSeparatedVoteStrategy(_serverVoteController);
            }
            
            return new ClientSimpleVoteStrategy(_dataProviderController, _serverVoteController, _xorCipherController);
        }

        public void Dispose()
        {
            _authController.OnError -= OnError;
        }
    }
}