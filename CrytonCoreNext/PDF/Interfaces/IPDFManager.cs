using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.PDF.Interfaces
{
    public interface IPDFManager
    {
        IAsyncEnumerable<BitmapImage> LoadAllPDFImages(PDFFile pdfFile);

        Task<CrytonCoreNext.Models.File> Split(PDFFile pdfFile, int fromPage, int toPage);

        Task<File> Merge(List<PDFFile> pdfFiles);
    }
}
