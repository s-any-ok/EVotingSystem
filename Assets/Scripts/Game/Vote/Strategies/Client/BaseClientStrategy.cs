using System;
using System.Collections.Generic;
using Game.Users.Data;
using Game.Vote.Enum;
using Game.Vote.Interfaces;

namespace Game.Vote.Strategies.Client
{
    public class BaseClientStrategy: IClientStrategy
    {
        public virtual event Action<string> OnError;
        public virtual EStrategy Type => EStrategy.SIMPLE;
        public virtual void Vote(User user, int candidateId) { }
        public virtual Dictionary<int, int> GetResults()
        {
            return new Dictionary<int, int>();
        }
    }
}