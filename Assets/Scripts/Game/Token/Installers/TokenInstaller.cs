using Game.Token.Controllers;
using Game.Tokens.Interfaces;
using Game.Tokens.Scriptable;
using UnityEngine;
using Zenject;

namespace Game.Token.Installers
{
    [CreateAssetMenu(fileName = "TokenInstaller", menuName = "ScriptableObjects/Installers/TokenInstaller")]
    public class TokenInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private TokenDatabase _tokenDatabase;
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<TokenController>().AsSingle();
            Container.Bind<ITokenDatabase>().FromInstance(_tokenDatabase);
        }
    }
}