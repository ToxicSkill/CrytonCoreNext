using System;

namespace CrytonCoreNext.Models
{
    public class RecognitionValues
    {
        public Guid Unique;
        public string Method;
        public string Extension;
        public byte[]? CheckSum;

        public RecognitionValues(Guid appKey)
        {
            Unique = appKey;
            Method = string.Empty;
            Extension = string.Empty;
        }
    }
}
