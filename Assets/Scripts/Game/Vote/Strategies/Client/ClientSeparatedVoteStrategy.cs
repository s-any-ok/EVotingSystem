using System;
using Game.Users.Data;
using Game.Vote.Enum;
using Game.Vote.Interfaces;

namespace Game.Vote.Strategies.Client
{
    public class ClientSeparatedVoteStrategy: BaseClientStrategy
    {
        public override event Action<string> OnError;
        private readonly IServerVoteController _serverVoteController;
        
        public override EStrategy Type => EStrategy.SEPARATE;

        public ClientSeparatedVoteStrategy(IServerVoteController serverVoteController)
        {
            _serverVoteController = serverVoteController;
        }
    
        public override void Vote(User user, int candidateId)
        {
            var (a, b) = BreakIntoMultipliers(candidateId);

            var encrypted1 = _serverVoteController.Encrypt(a).ToByteArray();
            var encrypted2 = _serverVoteController.Encrypt(b).ToByteArray();

            var signed1 = _serverVoteController.SeparatedVotingStrategies[0].Sign(encrypted1);
            var signed2 = _serverVoteController.SeparatedVotingStrategies[1].Sign(encrypted2);

            _serverVoteController.SeparatedVotingStrategies[0].Accept(user.Id, encrypted1, signed1);
            _serverVoteController.SeparatedVotingStrategies[1].Accept(user.Id, encrypted2, signed2);
        }
        
        private (int a, int b) BreakIntoMultipliers(int n)
        {
            for (int i = 2; i <= n; i++)
            {
                if (n % i == 0) return (i, n / i);
            }

            return (1, n);
        }
        
    }
}