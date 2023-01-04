#nullable enable
using System;

namespace Game.Vote.Interfaces
{
    public interface IServerStrategy
    {
        event Action<string> OnError;
    }
}