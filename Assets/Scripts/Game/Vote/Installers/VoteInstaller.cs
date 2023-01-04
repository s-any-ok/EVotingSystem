using Game.Vote.Controllers;
using Game.Vote.Strategies.Client;
using UnityEngine;
using Zenject;

namespace Game.Vote.Installers
{
    [CreateAssetMenu(fileName = "VoteInstaller", menuName = "ScriptableObjects/Installers/VoteInstaller")]
    public class VoteInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ClientStrategies>().AsSingle();
            Container.BindInterfacesTo<ClientVoteController>().AsSingle();
            Container.BindInterfacesTo<ServerVoteController>().AsSingle();
        }
    }
}