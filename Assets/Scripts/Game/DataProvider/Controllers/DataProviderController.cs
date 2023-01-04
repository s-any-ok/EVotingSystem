#nullable enable
using System.Collections.Generic;
using System.Linq;
using Game.Bulletins.Data;
using Game.Bulletins.Interfaces;
using Game.Candidates.Data;
using Game.Candidates.Interfaces;
using Game.DataProvider.Interfaces;
using Game.Users.Data;
using Game.Users.Interfaces;
using Game.Vote.Data;
using Game.VoteResults.Interfaces;
using Zenject;

namespace Game.DataProvider.Controllers
{
    public class DataProviderController:IDataProviderController, IInitializable
    {
        private readonly IUserDatabase _userDatabase;
        private readonly ICandidateDatabase _candidateDatabase;
        private readonly IBulletinDatabase _bulletinDatabase;
        private readonly IElectionResultDatabase _electionResultDatabase;
        
        public List<Candidate> Candidates => _candidateDatabase.Candidates;
        public List<User> Users => _userDatabase.Users;
        public List<ElectionResult.Data.ElectionResult> ElectionResults => _electionResultDatabase.ElectionResults;

        protected DataProviderController(IUserDatabase userDatabase, ICandidateDatabase candidateDatabase, 
            IBulletinDatabase bulletinDatabase, IElectionResultDatabase electionResultDatabase)
        {
            _userDatabase = userDatabase;
            _candidateDatabase = candidateDatabase;
            _bulletinDatabase = bulletinDatabase;
            _electionResultDatabase = electionResultDatabase;
            
            Seed();
        }
        
        public void Initialize() { }
        
        public Candidate? GetCandidateById(int id) => _candidateDatabase.GetCandidateById(id);
        public Candidate? GetCandidateByName(string name) => _candidateDatabase.GetCandidateByName(name);
        public List<User> GetVoters(int limit = -1) => _userDatabase.GetVoters(limit);
        public void SaveBulletin(Bulletin b) => _bulletinDatabase.SaveBulletin(b);
        public User? GetUserById(int id) => _userDatabase.Users.FirstOrDefault(u => u.Id == id);
        public void SaveElectionResult(ElectionResult.Data.ElectionResult el) => _electionResultDatabase.ElectionResults.Add(el);

        public int CreateBulletinId(int userId, int candidateId)
        {
            return userId * _candidateDatabase.Candidates.Count + candidateId;
        }

        public (int UserId, int CandidateId) GetBulletinId(int id)
        {
            return (id / _candidateDatabase.Candidates.Count, id % _candidateDatabase.Candidates.Count);
        }

        public IEnumerable<ElectionResultsData> GetElectionResults()
        {
            var res = _electionResultDatabase.ElectionResults.ToList()
                .GroupBy(b => b.CandidateId,
                    b => b.CandidateId,
                    (key, g) => new { CandidateId = key, Score = g.Count() })
                .ToList();

            return res.Select(r => new ElectionResultsData()
            {
                Votes = r.Score,
                CandidateId = r.CandidateId,
                CandidateName = _candidateDatabase.Candidates.FirstOrDefault(c => c.Id == r.CandidateId)?.Name,
            });
        }

        private void Seed()
        {
            _userDatabase.Users.Clear();
            _bulletinDatabase.Bulletins.Clear();
            _candidateDatabase.Candidates.Clear();
            _electionResultDatabase.ElectionResults.Clear();
            
            _candidateDatabase.Candidates.AddRange(new[]
            {
                new Candidate() {Id = 1, Name = "Abaddon"},
                new Candidate() {Id = 6, Name = "Alchemist"},
                new Candidate() {Id = 9, Name = "Invoker"},
            });

            _userDatabase.Users.AddRange(new[]
            {
                new User(1024 + 256 * 3, 2048 + 512 * 3)
                {
                    Id = 11111111,
                    CanVote = true
                },
                new User(1024 + 256 * 2, 2048 + 512 * 2)
                {
                    Id = 11111112,
                    CanVote = true
                },
                new User(1024 + 256, 2048 + 512)
                {
                    Id = 11111113,
                    CanVote = true
                },
                new User(1024, 2048)
                {
                    Id = 11111116,
                    CanVote = true
                },
                new User
                {
                    Id = 11111121,
                    CanVote = true
                },
                new User
                {
                    Id = 11111124,
                    CanVote = true
                },
                new User
                {
                    Id = 16,
                    CanVote = false
                },
                new User
                {
                    Id = 17,
                    CanVote = true
                },
            });
        }


    }
}