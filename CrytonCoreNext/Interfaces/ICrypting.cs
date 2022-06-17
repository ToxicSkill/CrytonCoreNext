using CrytonCoreNext.Abstract;

namespace CrytonCoreNext.Interfaces
{
    public interface ICrypting
    {
        byte[] Encrypt(byte[] data);

        byte[] Decrypt(byte[] data);

        ViewModelBase GetViewModel();
    }
}
