#nullable enable
namespace Game.XOR.Interfaces
{
    public interface IXORCipherController
    {
        byte[] Encrypt(byte[] data);
        byte[] Decrypt(byte[] data);
    }
}