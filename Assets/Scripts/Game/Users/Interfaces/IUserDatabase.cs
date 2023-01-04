#nullable enable
using System.Collections.Generic;
using Game.Users.Data;

namespace Game.Users.Interfaces
{
    public interface IUserDatabase
    {
        List<User> Users { get; } 
        User? GetUserById(int id);
        List<User> GetVoters(int limit = -1);
    }
}