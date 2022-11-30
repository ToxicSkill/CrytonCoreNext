using CrytonCoreNext.Models;
using Docnet.Core.Models;
using Docnet.Core.Readers;

namespace CrytonCoreNext.PDF.Models
{
    public class PDFFile : File
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

        public PDFFile(File file, PdfVersion version, IDocReader reader, string password, double dimensions, string owner, int numberOfPages, int lastPage, bool isProtectedByPassword, string format) : base(file)
        {
            Version = version;
            Reader = reader;
            Password = password;
            Dimensions = dimensions;
            Owner = owner;
            NumberOfPages = numberOfPages;
            LastPage = lastPage;
            IsProtectedByPassword = isProtectedByPassword;
            Format = format;
        }
    }
}
