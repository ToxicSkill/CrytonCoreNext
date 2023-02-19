using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.PDF.Models;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace CrytonCoreNext.ViewModels
{
    public class ImageViewerViewModel : ViewModelBase, IImageView
    {
        public ObservableCollection<ImageSource> Images { get; set; }

        public ImageViewerViewModel()
        {
            Images = new();
        }

        public void PostImages(PDFImage images)
        {
            Images = new(images.Images);
            OnPropertyChanged(nameof(Images));
        }
    }
}
