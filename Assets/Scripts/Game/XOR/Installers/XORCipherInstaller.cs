using Game.XOR.Controllers;
using UnityEngine;
using Zenject;

namespace Game.XOR.Installers
{
    [CreateAssetMenu(fileName = "XORCipherInstaller", menuName = "ScriptableObjects/Installers/XORCipherInstaller")]
    public class XORCipherInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<XORCipherController>().AsSingle();
        }
    }
}