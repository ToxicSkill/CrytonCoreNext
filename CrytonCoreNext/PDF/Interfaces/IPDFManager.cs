using CrytonCoreNext.PDF.Models;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.PDF.Interfaces
{
    public interface IPDFManager
    {
        void ExtractPages(PDFFile file);

        IAsyncEnumerable<BitmapImage> LoadAllPDFImages(PDFFile pdfFile);

        WriteableBitmap GetImage(PDFFile pdf);

        List<BitmapImage> GetAllPdfImages(PDFFile pdf);

        WriteableBitmap GetImageFromPdf(PDFFile pdf, int pageNumber);
    }
}
