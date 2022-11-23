using System.Collections.Generic;

namespace CrytonCoreNext.Models
{
    public class PDFBase
    {
        public string Path { get; set; }

        public string Password { get; set; }

        public double Dimensions { get; set; }

        public string Title { get; set; }

        public string Owner { get; set; }

        public int NumberOfPages { get; set; }

        public bool IsProtectedByPassword { get; set; }

        public string Format { get; set; }

        public List<PDFPage> Pages { get; set; }

        public byte[] Bytes { get; set; }
    }
}
