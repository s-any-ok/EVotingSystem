using Game.DataProvider.Controllers;
using UnityEngine;
using Zenject;

namespace Game.DataProvider.Installers
{
    [CreateAssetMenu(fileName = "DataProviderInstaller", menuName = "ScriptableObjects/Installers/DataProviderInstaller")]
    public class DataProviderInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<DataProviderController>().AsSingle();
        }
    }
}