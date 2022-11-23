using CrytonCoreNext.Interfaces;

namespace CrytonCoreNext.Models
{
    public class PDFReader : IPDFReader
    {
        public PDFBase? ReadPdf(string path)
        {
            if (path.Equals(string.Empty))
            {
                return null;
            }

            var pdf = new PDFBase()
            {
                Path = path,
                Bytes = System.IO.File.ReadAllBytes(path),
                Dimensions = 2.0d
            };

            var pdfManager = new PDFManager();
            return pdf;
        }
    }
}
