namespace CrytonCoreNext.Interfaces
{
    public interface ICrypting
    {
        byte[] Encrypt(byte[] data);

        byte[] Decrypt(byte[] data);
    }
}
