using Game.Primes.Controllers;
using UnityEngine;
using Zenject;

namespace Game.Primes.Installers
{
    [CreateAssetMenu(fileName = "PrimesInstaller", menuName = "ScriptableObjects/Installers/PrimesInstaller")]
    public class PrimesInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<PrimesController>().AsSingle();
        }
    }
}