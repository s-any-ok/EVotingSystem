using System.Collections.Generic;
using Game.Bulletins.Data;
using Game.Bulletins.Interfaces;
using UnityEngine;

namespace Game.Bulletins.Scriptable
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Database/BulletinDatabase", fileName = "BulletinDatabase")]
    public class BulletinDatabase : ScriptableObject, IBulletinDatabase
    {
#pragma warning disable 649
        [SerializeField] private List<Bulletin> _bulletins;
#pragma warning restore 649
        
        public List<Bulletin> Bulletins => _bulletins;
        public void SaveBulletin(Bulletin b) => _bulletins.Add(b);
    }
}
