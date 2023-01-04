#nullable enable
using System.Collections.Generic;
using Game.Candidates.Data;

namespace Game.VoteResults.Interfaces
{
    public interface IElectionResultDatabase
    {
        List<ElectionResult.Data.ElectionResult> ElectionResults { get; } 
        void SaveElectionResult(ElectionResult.Data.ElectionResult vr);
    }
}