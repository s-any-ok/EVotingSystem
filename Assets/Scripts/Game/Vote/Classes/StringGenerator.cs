using System;
using System.Linq;

namespace Game.Vote.Classes
{
    public static class StringGenerator
    {
        private static Random random = new();

        private const int LENGTH = 5;
        private const string LETTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    
        public static string GenerateRandom()
        {
            return new string(Enumerable.Repeat(LETTERS, LENGTH)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}