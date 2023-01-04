#nullable enable
using System;
using System.Collections.Generic;
using Game.Users.Data;
using Game.Vote.Enum;

namespace Game.Vote.Interfaces
{
    public interface IClientStrategy
    {
        event Action<string> OnError;
        EStrategy Type { get; }
        void Vote(User user, int candidateId);
        Dictionary<int, int> GetResults();
    }
}