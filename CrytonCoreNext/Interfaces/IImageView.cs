using CrytonCoreNext.PDF.Models;

namespace CrytonCoreNext.Interfaces
{
    public interface IImageView
    {
        void Add(PDFImage ipdfImage);

        PDFImage? GetPDFImage(int pageNumber);
    }
}
