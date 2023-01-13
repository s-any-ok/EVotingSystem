#nullable enable
using System.Collections.Generic;
using System.Linq;
using Game.Candidates.Data;
using Game.Candidates.Interfaces;
using Game.Tokens.Interfaces;
using UnityEngine;

namespace Game.Tokens.Scriptable
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Database/TokenDatabase", fileName = "TokenDatabase")]
    public class TokenDatabase : ScriptableObject, ITokenDatabase
    {
#pragma warning disable 649
        [SerializeField] private List<string> _tokens;
#pragma warning restore 649

        public List<string> Tokens => _tokens;
    }
}
