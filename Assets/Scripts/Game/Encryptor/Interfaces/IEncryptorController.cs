#nullable enable
namespace Game.Encryptor.Interfaces
{
    public interface IEncryptorController
    {
        byte[] Encrypt(byte[] msg);

        byte[] Decrypt(byte[] msg);

        byte[] Encrypt2(byte[] msg);

        byte[] Decrypt2(byte[] msg);
    }
}