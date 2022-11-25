using Docnet.Core.Models;
using Docnet.Core.Readers;
using System.Collections.Generic;

namespace CrytonCoreNext.Models
{
    public class PDFBase : File
    {
        public PdfVersion Version { get; set; }

        public IDocReader Reader { get; set; }

        public string Password { get; set; }

        public double Dimensions { get; set; }

        public string Owner { get; set; }

        public int NumberOfPages { get; set; }

        public int LastPage { get; set; } = 0;

        public bool IsProtectedByPassword { get; set; }

        public string Format { get; set; }

        public List<PDFPage> Pages { get; set; }

        public PDFBase()
        {

        }

        public PDFBase(File file) : base(file)
        {

        }
    }
}
