#nullable enable
using System.Collections.Generic;
using Game.Candidates.Data;

namespace Game.Candidates.Interfaces
{
    public interface ICandidateDatabase
    {
        List<Candidate> Candidates { get; } 
        Candidate? GetCandidateById(int id);
        Candidate? GetCandidateByName(string name);
    }
}