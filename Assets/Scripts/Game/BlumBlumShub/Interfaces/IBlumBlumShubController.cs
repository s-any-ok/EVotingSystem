#nullable enable
namespace Game.BlumBlumShub.Interfaces
{
    public interface IBlumBlumShubController
    {
        int GetBit(int m);
        byte[] Encrypt(byte[] msg, int m, int id);
    }
}