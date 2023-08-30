using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.PDF.Interfaces
{
    public interface IPDFManager
    {
        Task ProtectFile(PDFFile pdfFile);

        WriteableBitmap LoadImage(PDFFile pdfFile);

        Task<PDFFile> Split(PDFFile pdfFile, int fromPage, int toPage, int newId);

        Task<PDFFile> Merge(List<PDFFile> pdfFiles);

        PDFFile ImageToPdf(ImageFile image, int newId);

        Task<PDFFile> MergeAllImagesToPDF(List<ImageFile> images, int newId);
    }
}
