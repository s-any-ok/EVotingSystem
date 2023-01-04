using System.Collections.Generic;
using Game.VoteResults.Interfaces;
using UnityEngine;

namespace Game.VoteResults.Scriptable
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Database/ElectionResultDatabase", fileName = "ElectionResultDatabase")]
    public class ElectionResultDatabase : ScriptableObject, IElectionResultDatabase
    {
#pragma warning disable 649
        [SerializeField] private List<ElectionResult.Data.ElectionResult> _electionResults;
#pragma warning restore 649
        
        public List<ElectionResult.Data.ElectionResult> ElectionResults => _electionResults;
        public void SaveElectionResult(ElectionResult.Data.ElectionResult el) => _electionResults.Add(el);
    }
}
