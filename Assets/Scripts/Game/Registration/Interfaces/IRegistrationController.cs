using Game.Users.Data;

#nullable enable
namespace Game.Registration.Interfaces
{
    public interface IRegistrationController
    {
        public User RegisterVoter(int ipn); 
        public string LoginVoter(string login, string password);
    }
}