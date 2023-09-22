using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Models;

namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICryptingReader
    {
        CryptFile ReadCryptFile(File file, Recognition recognition);
    }
}
