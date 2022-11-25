using CrytonCoreNext.Interfaces;
using Docnet.Core;
using Docnet.Core.Models;

namespace CrytonCoreNext.Models
{
    public class PDFReader : IPDFReader
    {
        private readonly double _dimensions = 2.0d;

        public PDFBase? ReadPdf(File file, string password = "")
        {
            if (file.Path.Equals(string.Empty))
            {
                return null;
            }

            using (IDocLib pdfLibrary = DocLib.Instance)
            {
                var dimensions = _dimensions;
                var reader = password.Equals(string.Empty, default) ?
                    pdfLibrary.GetDocReader(file.Bytes, new PageDimensions(dimensions)) :
                    pdfLibrary.GetDocReader(file.Bytes, password, new PageDimensions(dimensions));

                return new PDFBase(file)
                {
                    Reader = reader,
                    Password = string.Empty,
                    Dimensions = _dimensions,
                    Version = reader.GetPdfVersion(),
                    NumberOfPages = reader.GetPageCount()
                };
            }
        }
    }
}
