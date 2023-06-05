using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.PDF.Interfaces
{
    public interface IPDFManager
    {
        //IAsyncEnumerable<BitmapImage> LoadAllPDFImages(PDFFile pdfFile);

        WriteableBitmap LoadImage(PDFFile pdfFile);

        Task<File> Split(PDFFile pdfFile, int fromPage, int toPage, int newId);

        Task<File> Merge(List<PDFFile> pdfFiles);

        PDFFile ImageToPdf(ImageFile image, int newId);
    }
}
