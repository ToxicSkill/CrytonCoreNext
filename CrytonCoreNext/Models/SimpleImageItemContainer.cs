using System;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.Models
{
    public struct SimpleImageItemContainer
    {
        public string DateOfCreation { get; set; }

        public string Size { get; set; }

        public string Label { get; init; }

        public WriteableBitmap Image { get; init; }

        public SimpleImageItemContainer() { }

        public SimpleImageItemContainer(string path)
        {
            
        }
    }
}
