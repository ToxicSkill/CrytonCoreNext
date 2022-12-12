using System.Windows.Media;

namespace CrytonCoreNext.PDF.Models
{
    public class PDFImage
    {
        public ImageSource Source { get; set; }

        public int PageNumber { get; set; }

        public PDFImage(ImageSource source, int pageNumber)
        {
            Source = source;
            PageNumber = pageNumber;
        }
    }
}
