#nullable enable
using System.Collections.Generic;
using Game.Bulletins.Data;
using Game.Candidates.Data;
using Game.Users.Data;
using Game.Vote.Data;

namespace Game.DataProvider.Interfaces
{
    public interface IDataProviderController
    {
        List<Candidate> Candidates { get; }
        List<string> Tokens { get; }
        List<User> Users { get; }
        List<ElectionResult.Data.ElectionResult> ElectionResults { get; }
        User? GetUserById(int id);
        Candidate? GetCandidateById(int id);
        Candidate? GetCandidateByName(string name);
        List<User> GetVoters(int limit);
        IEnumerable<ElectionResultsData> GetElectionResults();
        int CreateBulletinId(int userId, int candidateId);
        (int UserId, int CandidateId) GetBulletinId(int id);
        void SaveElectionResult(ElectionResult.Data.ElectionResult vr);
        void SaveBulletin(Bulletin b);
        void UpdateUser(User user);
    }
}