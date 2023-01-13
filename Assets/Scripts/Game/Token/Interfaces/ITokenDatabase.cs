#nullable enable
using System.Collections.Generic;

namespace Game.Tokens.Interfaces
{
    public interface ITokenDatabase
    {
        List<string> Tokens { get; }
    }
}