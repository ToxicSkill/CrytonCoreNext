using System.Windows.Media.Imaging;

namespace CrytonCoreNext.Models
{
    public class SimpleImageItemContainer
    {
        public string DateOfCreation { get; set; }

        public string Size { get; set; }

        public string Label { get; set; }

        public WriteableBitmap Image { get; init; }

        public SimpleImageItemContainer() { } 
    }
}
