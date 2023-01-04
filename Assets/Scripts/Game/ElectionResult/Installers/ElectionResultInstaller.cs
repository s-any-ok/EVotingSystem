using Game.VoteResults.Interfaces;
using Game.VoteResults.Scriptable;
using UnityEngine;
using Zenject;

namespace Game.ElectionResult.Installers
{
    [CreateAssetMenu(fileName = "ElectionResultInstaller", menuName = "ScriptableObjects/Installers/ElectionResultInstaller")]
    public class ElectionResultInstaller : ScriptableObjectInstaller
    {
#pragma warning disable 649
        [SerializeField] private ElectionResultDatabase _electionResultDatabase;
#pragma warning restore 649
        public override void InstallBindings()
        {
            Container.Bind<IElectionResultDatabase>().FromInstance(_electionResultDatabase);
        }
    }
}