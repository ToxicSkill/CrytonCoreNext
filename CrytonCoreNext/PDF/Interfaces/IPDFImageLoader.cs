using CrytonCoreNext.PDF.Models;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.PDF.Interfaces
{
    public interface IPDFImageLoader
    {
        WriteableBitmap LoadImage(PDFFile pdfFile);

        IAsyncEnumerable<WriteableBitmap> LoadImages(PDFFile pdfFile);
    }
}
