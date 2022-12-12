using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.PDF.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace CrytonCoreNext.ViewModels
{
    public class ImageViewerViewModel : ViewModelBase, IImageView
    {
        public ObservableCollection<PDFImage> Images { get; set; }

        public ImageViewerViewModel()
        {
            Images = new();
        }

        public void Add(PDFImage pdfImage)
        {
            Images.Add(pdfImage);
            OnPropertyChanged(nameof(Images));
        }

        public void Remove(PDFImage pdfImage)
        {
            Images.Remove(pdfImage);
        }

        public PDFImage? GetPDFImage(int pageNumber)
        {
            if (pageNumber < 0 || Images.Count < pageNumber || !Images.Any())
            {
                return default;
            }
            return Images[pageNumber];
        }
    }
}
