using System;

namespace Game.ElectionResult.Data
{
    [Serializable]
    public class ElectionResult
    {
        public int UserId;
        public int CandidateId;

        public ElectionResult(int userId, int candidateId)
        {
            UserId = userId;
            CandidateId = candidateId;
        }
    }
}