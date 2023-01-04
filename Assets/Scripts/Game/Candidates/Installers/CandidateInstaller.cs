using Game.Candidates.Interfaces;
using Game.Candidates.Scriptable;
using UnityEngine;
using Zenject;

namespace Game.Candidates.Installers
{
    [CreateAssetMenu(fileName = "CandidateInstaller", menuName = "ScriptableObjects/Installers/CandidateInstaller")]
    public class CandidateInstaller : ScriptableObjectInstaller
    {
#pragma warning disable 649
        [SerializeField] private CandidateDatabase _candidateDatabase;
#pragma warning restore 649
        public override void InstallBindings()
        {
            Container.Bind<ICandidateDatabase>().FromInstance(_candidateDatabase);
        }
    }
}