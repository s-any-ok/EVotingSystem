using Game.Encryptor.Controllers;
using Game.Primes.Controllers;
using UnityEngine;
using Zenject;

namespace Game.Encryptor.Installers
{
    [CreateAssetMenu(fileName = "EncryptorInstaller", menuName = "ScriptableObjects/Installers/EncryptorInstaller")]
    public class EncryptorInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<EncryptorController>().AsSingle();
        }
    }
}