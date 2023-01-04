#nullable enable
using System;
using System.Collections.Generic;

namespace Game.Auth.Interfaces
{
    public interface IAuthController
    {
        event Action<string> OnError;
        List<long> RegistrationNumbers { get; }
        long GenerateRegNumber(int id);
    }
}