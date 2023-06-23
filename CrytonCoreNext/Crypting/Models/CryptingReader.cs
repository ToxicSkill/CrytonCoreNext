using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Models;
using System.Linq;
using static CrytonCoreNext.Static.CryptingStatus;

namespace CrytonCoreNext.Crypting.Models
{
    public class CryptingReader : ICryptingReader
    {
        public CryptFile ReadCryptFile(File file, (bool succes, (EMethod method, string extension)) cryptingRecognitionResult)
        {
            var status = Status.Decrypted;
            var method = EMethod.AES;
            if (cryptingRecognitionResult.succes)
            {
                status = Status.Encrypted;
                file.Bytes = file.Bytes.Skip(64).ToArray();
                file.Extension = cryptingRecognitionResult.Item2.extension;
                method = cryptingRecognitionResult.Item2.method;
            }
            return new CryptFile(file, status, method, file.Guid);
        }
    }
}
