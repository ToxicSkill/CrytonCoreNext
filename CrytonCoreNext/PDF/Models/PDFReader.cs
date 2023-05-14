using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Enums;
using CrytonCoreNext.PDF.Interfaces;
using Docnet.Core;
using Docnet.Core.Exceptions;
using Docnet.Core.Models;
using Docnet.Core.Readers;

namespace CrytonCoreNext.PDF.Models
{
    public class PDFReader : IPDFReader
    {
        private readonly double _dimensions = 1.0d;

        public PDFFile ReadPdf(File file, string password = "")
        {
            using IDocLib pdfLibrary = DocLib.Instance;
            IDocReader? reader = null;
            var status = EPdfStatus.Opened;
            var dimensions = _dimensions;

            if (file.Path.Equals(string.Empty))
            {
                return CreateNewPdfFile(file, reader, status);
            }

            try
            {
                reader = password.Equals(string.Empty, default) ?
                    pdfLibrary.GetDocReader(file.Bytes, new PageDimensions(dimensions)) :
                    pdfLibrary.GetDocReader(file.Bytes, password, new PageDimensions(dimensions));
            }
            catch (DocnetLoadDocumentException)
            {
                status = EPdfStatus.Protected;
            }
            catch (DocnetException)
            {
                status = EPdfStatus.Damaged;
            }

            return CreateNewPdfFile(file, reader, status);
        }

        public void UpdatePdfFileInformations(ref PDFFile pdfFile)
        {
            using IDocLib pdfLibrary = DocLib.Instance;
            IDocReader? reader = null;
            pdfFile.Dimensions = _dimensions;
            try
            {
                reader = pdfFile.Password.Equals(string.Empty, default) ?
                    pdfLibrary.GetDocReader(pdfFile.Bytes, new PageDimensions(pdfFile.Dimensions)) :
                    pdfLibrary.GetDocReader(pdfFile.Bytes, pdfFile.Password, new PageDimensions(pdfFile.Dimensions));
            }
            catch (DocnetLoadDocumentException)
            {
                pdfFile.PdfStatus = EPdfStatus.Protected;
                return;
            }
            catch (DocnetException)
            {
                pdfFile.PdfStatus = EPdfStatus.Damaged;
                return;
            }

            pdfFile.Version = reader.GetPdfVersion();
            pdfFile.NumberOfPages = reader.GetPageCount();
            pdfFile.IsOpened = true;
        }

        private PDFFile CreateNewPdfFile(File file, IDocReader? reader, EPdfStatus status)
        {
            return reader == null
                ? new PDFFile(file, status)
                : new PDFFile(
                file: file,
                version: reader.GetPdfVersion(),
                reader: reader,
                pdfStatus: status,
                password: string.Empty,
                dimensions: _dimensions,
                numberOfPages: reader.GetPageCount());
        }
    }
}
