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

        Task<File> Merge(List<PDFFile> pdfFiles);
    }
}
