using System;
using System.Collections.Generic;
using System.Linq;
using Game.Auth.Interfaces;
using Game.DataProvider.Interfaces;

namespace Game.Auth.Controllers
{
    public class AuthController: IAuthController
    {
        private const long MIN_LONG = 100000000000000000;
        private const long MAX_LONG = 100000000000000050;
        
        public event Action<string> OnError;
        
        private readonly IDataProviderController _dataProviderController;
        private Dictionary<int, long> _usersNumbers = new();
        private HashSet<long> _numbers = new();
        private HashSet<long> _numbersBackup = new();
        
        public List<long> RegistrationNumbers => _numbersBackup.ToList();

        public AuthController(IDataProviderController dataProviderController)
        {
            _dataProviderController = dataProviderController;
            for (int i = 0; i < _dataProviderController.Users.Count + _dataProviderController.Users.Count / 4; i++)
            {
                long r = LongRandom(MIN_LONG, MAX_LONG, new Random());

                while (!_numbers.Add(r))
                {
                    r = LongRandom(MIN_LONG, MAX_LONG, new Random());
                }

                _numbersBackup.Add(r);
            }
        }

        public long GenerateRegNumber(int id)
        {
            if (_usersNumbers.ContainsKey(id))
            {
                OnError?.Invoke("user already got reg number");
            }

            var r = _numbers.First();
            _usersNumbers.Add(id, r);
            _numbers.Remove(r);

            return r;
        }
        
        private long LongRandom(long min, long max, Random rand) {
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min)) + min);
        }
    }
}