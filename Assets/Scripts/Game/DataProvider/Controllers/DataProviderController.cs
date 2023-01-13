#nullable enable
using System.Collections.Generic;
using System.Linq;
using Game.Bulletins.Data;
using Game.Bulletins.Interfaces;
using Game.Candidates.Data;
using Game.Candidates.Interfaces;
using Game.DataProvider.Interfaces;
using Game.Tokens.Interfaces;
using Game.Users.Data;
using Game.Users.Interfaces;
using Game.Vote.Data;
using Game.VoteResults.Interfaces;
using Zenject;

namespace Game.DataProvider.Controllers
{
    public class DataProviderController:IDataProviderController, IInitializable
    {
        private const int DELTA = 10000;
        private readonly IUserDatabase _userDatabase;
        private readonly ICandidateDatabase _candidateDatabase;
        private readonly IBulletinDatabase _bulletinDatabase;
        private readonly IElectionResultDatabase _electionResultDatabase;
        private readonly ITokenDatabase _tokenDatabase;

        public List<Candidate> Candidates => _candidateDatabase.Candidates;
        public List<User> Users => _userDatabase.Users;
        public List<ElectionResult.Data.ElectionResult> ElectionResults => _electionResultDatabase.ElectionResults;
        public List<string> Tokens => _tokenDatabase.Tokens;

        protected DataProviderController(IUserDatabase userDatabase, ICandidateDatabase candidateDatabase, 
            IBulletinDatabase bulletinDatabase, IElectionResultDatabase electionResultDatabase,
            ITokenDatabase tokenDatabase)
        {
            _userDatabase = userDatabase;
            _candidateDatabase = candidateDatabase;
            _bulletinDatabase = bulletinDatabase;
            _electionResultDatabase = electionResultDatabase;
            _tokenDatabase = tokenDatabase;

            Seed();
        }
        
        public void Initialize() { }
        
        public Candidate? GetCandidateById(int id) => _candidateDatabase.GetCandidateById(id);
        public Candidate? GetCandidateByName(string name) => _candidateDatabase.GetCandidateByName(name);
        public List<User> GetVoters(int limit = -1) => _userDatabase.GetVoters(limit);
        public void SaveBulletin(Bulletin b) => _bulletinDatabase.SaveBulletin(b);
        public void UpdateUser(User user)
        {
            _userDatabase.UpdateUser(user);
        }

        public User? GetUserById(int id) => _userDatabase.Users.FirstOrDefault(u => u.Id == id);
        public void SaveElectionResult(ElectionResult.Data.ElectionResult el) => _electionResultDatabase.ElectionResults.Add(el);

        public int CreateBulletinId(int userId, int candidateId) => userId * DELTA + candidateId;
        public (int UserId, int CandidateId) GetBulletinId(int id) => (id / DELTA, id % DELTA);

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
            _tokenDatabase.Tokens.Clear();
            
            _candidateDatabase.Candidates.AddRange(new[]
            {
                new Candidate() {Id = 1, Name = "Abaddon"},
                new Candidate() {Id = 2, Name = "Alchemist"},
                new Candidate() {Id = 3, Name = "Invoker"},
            });

            for (int i = 1; i <= 10; i++)
            {
                var user = new User(1024 + 256 * (10 - i), 2048 + 512 * (10 - i))
                {
                    Id = 1000 + i,
                    CanVote = i < 5
                };
                _userDatabase.Users.Add(user);
            }
        }
    }
}