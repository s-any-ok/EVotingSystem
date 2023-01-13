using System;
using System.Collections.Generic;
using Game.Primes.Interfaces;

namespace Game.Primes.Controllers
{
    public class PrimesController:IPrimesController
    {
        private readonly int[] PRIMES = 
        {
            11, 13, 17, 19, 23, 31, 37, 41, 43, 5, 7, 47, 53, 57, 59, 61, 67, 71
        };
        
        public List<int> GeneratePrimes(int n)
        {
            var primes = new List<int>();
            var random = new Random();
            var rnd = random.Next(0, PRIMES.Length - 1);

            int firstPrime = PRIMES[rnd];
            int secondPrime = PRIMES[rnd + 1];
            
            primes.Add(firstPrime);
            var nextPrime = secondPrime;
            
            while (primes.Count < n)
            {
                var sqrt = (int)Math.Sqrt(nextPrime);
                var isPrime = true;
                for (var i = 0; primes[i] <= sqrt; i++)
                {
                    if (nextPrime % primes[i] == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime)
                {
                    primes.Add(nextPrime);
                }
                nextPrime += 2;
            }
            return primes;
        }
        
        public List<int> GeneratePrimesNaive(int n, int firstPrime, int secondPrime)
        {
            var primes = new List<int>();
            primes.Add(firstPrime);
            var nextPrime = secondPrime;
            while (primes.Count < n)
            {
                var sqrt = (int)Math.Sqrt(nextPrime);
                var isPrime = true;
                for (var i = 0; primes[i] <= sqrt; i++)
                {
                    if (nextPrime % primes[i] == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime)
                {
                    primes.Add(nextPrime);
                }
                nextPrime += 2;
            }
            return primes;
        }
    }
}