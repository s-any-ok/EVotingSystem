using System.Collections.Generic;
using Game.Bulletins.Data;

namespace Game.Bulletins.Interfaces
{
    public interface IBulletinDatabase
    {
        List<Bulletin> Bulletins { get; } 
        void SaveBulletin(Bulletin b);
    }
}