using CommunityToolkit.Mvvm.ComponentModel;
using CrytonCoreNext.Models;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Collections.ObjectModel;

namespace CrytonCoreNext.ViewModels
{
    public partial class AIViewerViewModel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<SimpleImageItemContainer> detectecCurrentLabelsImages;

        [ObservableProperty]
        public ObservableCollection<SimpleImageItemContainer> images;

        [ObservableProperty]
        public SimpleImageItemContainer selectedImage;

        public AIViewerViewModel()
        {
            Images = new ObservableCollection<SimpleImageItemContainer>()
            {
                new SimpleImageItemContainer()
                {
                    Label = "Zrzut ekranu (2)",
                    Image = Cv2.ImRead("C:\\Users\\gizmo\\OneDrive\\Obrazy\\Zrzuty ekranu\\Zrzut ekranu (2).png").ToWriteableBitmap()
                },
                new SimpleImageItemContainer()
                {
                    Label = "Zrzut ekranu (3)",
                    Image = Cv2.ImRead("C:\\Users\\gizmo\\OneDrive\\Obrazy\\Zrzuty ekranu\\Zrzut ekranu (3).png").ToWriteableBitmap()
                },
            };
        }
    }
}
