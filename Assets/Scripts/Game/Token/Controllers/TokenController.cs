using System;
using Game.Token.Interfaces;

namespace Game.Token.Controllers
{
    public class TokenController:ITokenController
    {
        private string _separator = "_";
    
        public String GenerateToken(int id, int bbsKey)
        {
            return String.Join(_separator, id, bbsKey);
        }

        public (int id, int bbsKey) GetTokenData(string token)
        {
            var pieces = token.Split(_separator);
            if (pieces.Length != 2)
            {
                throw new Exception("token exception");
            }

            var id     = Int32.Parse(pieces[0]);
            var bbsKey = Int32.Parse(pieces[1]);

            return (id, bbsKey);
        }
    }
}