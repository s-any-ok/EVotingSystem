#nullable enable
using System.Collections.Generic;
using System.Linq;
using Game.Candidates.Data;
using Game.Candidates.Interfaces;
using UnityEngine;

namespace Game.Candidates.Scriptable
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Database/CandidateDatabase", fileName = "CandidateDatabase")]
    public class CandidateDatabase : ScriptableObject, ICandidateDatabase
    {
#pragma warning disable 649
        [SerializeField] private List<Candidate> _candidates;
#pragma warning restore 649

        public List<Candidate> Candidates => _candidates;
        public Candidate? GetCandidateById(int id) => _candidates.FirstOrDefault(c => c.Id == id);
        public Candidate? GetCandidateByName(string name) => _candidates.FirstOrDefault(c => c.Name == name);
    }
}
