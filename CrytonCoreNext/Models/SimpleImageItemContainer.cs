using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.Models
{
    public abstract partial class SimpleImageItemContainer : ObservableObject
    {
        public string DateOfCreation { get; set; }

        public string Size { get; set; }

        public string Label { get; set; }

        public WriteableBitmap Image { get; set; }

        public string Path { get; init; }
    }
}
