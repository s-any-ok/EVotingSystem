using Game.Auth.Controllers;
using UnityEngine;
using Zenject;

namespace Game.Auth.Installers
{
    [CreateAssetMenu(fileName = "AuthInstaller", menuName = "ScriptableObjects/Installers/AuthInstaller")]
    public class AuthInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<AuthController>().AsSingle();
        }
    }
}