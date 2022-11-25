using CrytonCoreNext.Abstract;
using System.Collections.Generic;
using System.Drawing;

namespace CrytonCoreNext.ViewModels
{
    public class ImageViewerViewModel : ViewModelBase
    {
        public List<Bitmap> Images { get; set; }

        public ImageViewerViewModel(List<Bitmap> images)
        {
            Images = images;
        }
    }
}
