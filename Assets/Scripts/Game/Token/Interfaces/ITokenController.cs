using System;

#nullable enable
namespace Game.Token.Interfaces
{
    public interface ITokenController
    {
        String GenerateToken(int id, int bbsKey);
        (int id, int bbsKey) GetTokenData(string token);
    }
}