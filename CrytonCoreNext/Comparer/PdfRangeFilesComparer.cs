using CrytonCoreNext.PDF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CrytonCoreNext.Comparer
{
    public class PdfRangeFilesComparer : IEqualityComparer<PdfRangeFile>
    {
        public bool Equals(PdfRangeFile? x, PdfRangeFile? y)
        {
            if (x is null || y is null)
            {
                return false;
            }
            return x.From == y.From && x.To == y.To;
        }

        public int GetHashCode([DisallowNull] PdfRangeFile obj)
        {
            throw new NotImplementedException();
        }
    }
}
