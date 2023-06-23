using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using File = CrytonCoreNext.Models.File;

namespace CrytonCoreNext.Comparer
{
    public class FileComparer : IEqualityComparer<File>
    {
        public bool Equals(File? x, File? y)
        {
            if (x is null || y is null)
            {
                return false;
            }
            return CompareByteArraySpan(x.Bytes, y.Bytes);
        } 

        public int GetHashCode([DisallowNull] File obj)
        {
            throw new NotImplementedException();
        }

        private bool CompareByteArraySpan(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
        {
            return a.SequenceEqual(b);
        }
    }
}
