#nullable enable
using System;
using Game.Vote.Enum;

namespace Game.Vote.Interfaces
{
    public interface IClientStrategies
    {
        event Action<string> OnErrorE;
        IClientStrategy GetStrategy(EStrategy strategy);
    }
}