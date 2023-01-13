using System.Collections.Generic;

#nullable enable
namespace Game.Primes.Interfaces
{
    public interface IPrimesController
    {
        List<int> GeneratePrimes(int n);
        List<int> GeneratePrimesNaive(int n, int firstPrime, int secondPrime);
    }
}