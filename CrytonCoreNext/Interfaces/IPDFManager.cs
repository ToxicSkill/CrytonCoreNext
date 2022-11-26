using CrytonCoreNext.Models;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.Interfaces
{
    public interface IPDFManager
    {
        WriteableBitmap GetImage(PDFFile pdf);

        List<BitmapImage> GetAllPdfImages(PDFFile pdf);

        WriteableBitmap GetImageFromPdf(PDFFile pdf, int pageNumber);
    }
}
