using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.PDF.Interfaces
{
    public interface IPDFManager
    {
        bool ProtectFile(PDFFile pdfFile, int permissions, int encryption);

        WriteableBitmap LoadImage(PDFFile pdfFile);

        IAsyncEnumerable<WriteableBitmap> LoadImages(PDFFile pdfFile);

        Task<PDFFile> Split(PDFFile pdfFile, int fromPage, int toPage, int newId);

        Task<PDFFile> Merge(List<PDFFile> pdfFiles);

        PDFFile ImageToPdf(ImageFile image, int newId);

        Task<PDFFile> MergeAllImagesToPDF(List<ImageFile> images, int newId);

        List<string> GetAvailableEncryptionOptions();

        List<string> GetAvailableEncryptionAllowOptions();
    }
}
