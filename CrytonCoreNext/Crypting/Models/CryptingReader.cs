using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Models;
using System.Linq;
using static CrytonCoreNext.Static.CryptingStatus;

namespace CrytonCoreNext.Crypting.Models
{
    public class CryptingReader : ICryptingReader
    {
        public CryptFile ReadCryptFile(File file, Recognition recognition)
        {
            var status = Status.Decrypted;
            var method = EMethod.AES;
            if (recognition.Status == CrytonCoreNext.Enums.EStatus.Success)
            {
                status = Status.Encrypted;
                file.Bytes = file.Bytes.Skip(64).ToArray();
                file.Extension = recognition.Extension;
                method = recognition.Method;
            }
            return new CryptFile(file, status, method, recognition.Keys, file.Guid);
        }
    }
}
