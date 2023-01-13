#nullable enable
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using Game.Vote.Data;
using Game.Vote.Strategies.Server;

namespace Game.Vote.Interfaces
{
    public interface IServerVoteController
    {
        event Action<string> OnErrorE;
        RSAParameters PublicKey { get; }
        List<ServerSeparatedStrategy> SeparatedVotingStrategies { get; }
        void Vote(byte[] msg, byte[] sign, byte[] signedData);
        void Vote(PolyBulletinData data);
        object Sign(object msgs);
        byte[] SignWithPrivateKey(byte[] msg);
        bool CheckIfSigned(byte[] msg, byte[] signature);
        byte[] ApplyPublicKey(byte[] msg);
        byte[] ApplyPrivateKey(byte[] msg);
        BigInteger Encrypt(BigInteger x);
        Dictionary<int, int> GetSeparatedResults();
        void Vote(byte[] msg, byte[] sign);
        void GenerateKeys(List<int> ids);

        List<string> GetTokens();

        void Vote(byte[] msg);

        IDictionary<int, int> ComputeResults();
    }
}