using CrytonCoreNext.Models;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.Interfaces
{
    public interface IPDFManager
    {
        WriteableBitmap GetImage(PDFBase pdf);

        List<BitmapImage> GetAllPdfImages(PDFBase pdf);

        WriteableBitmap GetImageFromPdf(PDFBase pdf, int pageNumber);
    }
}
