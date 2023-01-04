using Game.Users.Interfaces;
using Game.Users.Scriptable;
using UnityEngine;
using Zenject;

namespace Game.Users.Installers
{
    [CreateAssetMenu(fileName = "UserInstaller", menuName = "ScriptableObjects/Installers/UserInstaller")]
    public class UserInstaller : ScriptableObjectInstaller
    {
#pragma warning disable 649
        [SerializeField] private UserDatabase _userDatabase;
#pragma warning restore 649
        public override void InstallBindings()
        {
            Container.Bind<IUserDatabase>().FromInstance(_userDatabase);
        }
    }
}