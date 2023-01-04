using Game.Bulletins.Interfaces;
using Game.Bulletins.Scriptable;
using UnityEngine;
using Zenject;

namespace Game.Bulletins.Installers
{
    [CreateAssetMenu(fileName = "BulletinInstaller", menuName = "ScriptableObjects/Installers/BulletinInstaller")]
    public class BulletinInstaller : ScriptableObjectInstaller
    {
#pragma warning disable 649
        [SerializeField] private BulletinDatabase _bulletinDatabase;
#pragma warning restore 649
        public override void InstallBindings()
        {
            Container.Bind<IBulletinDatabase>().FromInstance(_bulletinDatabase);
        }
    }
}