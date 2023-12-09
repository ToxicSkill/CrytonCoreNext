using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Models;

namespace CrytonCoreNext.PDF.Interfaces
{
    public interface IPDFReader
    {
        PDFFile ReadPdf(File file);

        void OpenProtectedPdf(ref PDFFile pdfFile);
    }
}
