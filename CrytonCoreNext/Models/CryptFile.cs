using CrytonCoreNext.Static;

namespace CrytonCoreNext.Models
{
    public class CryptFile : File
    {
        public CryptingStatus.Status Status { get; set; }

        public string Method { get; set; }

        public CryptFile(File file) : base(file)
        {

        }
    }
}
