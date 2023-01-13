using Game.BlumBlumShub.Controllers;
using Game.Registration.Controllers;
using UnityEngine;
using Zenject;

namespace Game.Registration.Installers
{
    [CreateAssetMenu(fileName = "RegistrationInstaller", menuName = "ScriptableObjects/Installers/RegistrationInstaller")]
    public class RegistrationInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<RegistrationController>().AsSingle();
        }
    }
}