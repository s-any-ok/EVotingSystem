using Game.BlumBlumShub.Controllers;
using Game.Encryptor.Controllers;
using UnityEngine;
using Zenject;

namespace Game.BlumBlumShub.Installers
{
    [CreateAssetMenu(fileName = "BlumBlumShubInstaller", menuName = "ScriptableObjects/Installers/BlumBlumShubInstaller")]
    public class BlumBlumShubInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<BlumBlumShubController>().AsSingle();
        }
    }
}