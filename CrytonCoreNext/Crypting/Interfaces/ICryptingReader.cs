using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Models;

namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICryptingReader
    {
        CryptFile ReadCryptFile(File file, (bool succes, (EMethod method, string extension, string keys)) cryptingRecognitionResult);
    }
}
