#nullable enable
using System;
using System.Collections.Generic;
using Game.Users.Data;
using Game.Vote.Data;

namespace Game.Vote.Interfaces
{
    public interface IClientVoteController
    {
        event Action<string> OnError;
        IEnumerable<ElectionResultsData> GetElectionResults();
        void Vote(int userId, int candidateId);
        void SetStrategy(IClientStrategy clientStrategy);
        User Register(int ipn);
        string LogIn(string login, string password);
    }
}