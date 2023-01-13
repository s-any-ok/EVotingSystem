#nullable enable
using System.Collections.Generic;
using System.Linq;
using Game.Users.Data;
using Game.Users.Interfaces;
using UnityEngine;

namespace Game.Users.Scriptable
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Database/UserDatabase", fileName = "UserDatabase")]
    public class UserDatabase : ScriptableObject , IUserDatabase
    {
#pragma warning disable 649
        [SerializeField] private List<User> _users;
#pragma warning restore 649
        
        public List<User> Users => _users;
        
        public User? GetUserById(int id) => _users.FirstOrDefault(u => u.Id == id);
        public List<User> GetVoters(int limit = -1) => _users.FindAll(u => u.CanVote).Take(limit).ToList();

        public void UpdateUser(User user)
        {
            var oldUser = _users.FirstOrDefault(x => user.Id == x.Id);
            oldUser.Login = user.Login;
            oldUser.Password = user.Password;
            oldUser.Token = user.Token;
            oldUser.CanVote = user.CanVote;
        }
    }
}
