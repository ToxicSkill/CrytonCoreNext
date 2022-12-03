using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Models;

namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICryptingReader
    {
        CryptFile ReadCryptFile(File file, (bool succes, (string method, string extension)) cryptingRecognitionResult);
    }
}
