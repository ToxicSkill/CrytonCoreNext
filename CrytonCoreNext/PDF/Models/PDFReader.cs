using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Interfaces;
using Docnet.Core;
using Docnet.Core.Models;

namespace CrytonCoreNext.PDF.Models
{
    public class PDFReader : IPDFReader
    {
        private readonly double _dimensions = 1.0d;

        public PDFFile? ReadPdf(File file, string password = "")
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

                return new PDFFile(
                    file: file,
                    version: reader.GetPdfVersion(),
                    reader: reader,
                    password: string.Empty,
                    dimensions: _dimensions,
                    owner: string.Empty,
                    numberOfPages: reader.GetPageCount() - 1,
                    lastPage: 0,
                    isProtectedByPassword: false,
                    format: "A4",
                    file.Guid);
            }
        }
    }
}
