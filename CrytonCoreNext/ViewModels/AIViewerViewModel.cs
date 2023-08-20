using CommunityToolkit.Mvvm.ComponentModel;
using CrytonCoreNext.AI.Interfaces;
using CrytonCoreNext.AI.Models;
using CrytonCoreNext.Models;
using OpenCvSharp.WpfExtensions;
using System.Collections.ObjectModel;

namespace CrytonCoreNext.ViewModels
{
    public partial class AIViewerViewModel : ObservableObject
    {
        private readonly IYoloModelService _yoloModelService;

        [ObservableProperty]
        public ObservableCollection<AIDetectionImage> detectedCurrentImages;

        [ObservableProperty]
        public AIDetectionImage detectedCurrentImage;

        [ObservableProperty]
        public ObservableCollection<AIImage> images;

        [ObservableProperty]
        public SimpleImageItemContainer selectedImage;

        public AIViewerViewModel(IYoloModelService yoloModelService)
        {
            DetectedCurrentImages = new();
            _yoloModelService = yoloModelService;
            _yoloModelService.LoadYoloModel("AI/YoloModels/yolov7-tiny.onnx");
            _yoloModelService.LoadLabels();

            Images = new ()
            {
                new ("C:\\Users\\gizmo\\OneDrive\\Obrazy\\Zrzuty ekranu\\Zrzut ekranu (12).png"),
                new ( "C:\\Users\\gizmo\\OneDrive\\Obrazy\\Zrzuty ekranu\\Zrzut ekranu 2023-06-14 140801.png"),
                new ( "C:\\Users\\gizmo\\OneDrive\\Obrazy\\tough-crowd.png")
            };

            foreach (var image in Images)
            {
                image.SetPredicitons(_yoloModelService.GetPredictions(image.Image.ToMat()));
                DetectedCurrentImages.Add(new(image));
            }
            DetectedCurrentImage = DetectedCurrentImages[0];
        }

        partial void OnSelectedImageChanged(SimpleImageItemContainer value)
        {
            var index = Images.IndexOf((AIImage)value);
            if (index <= DetectedCurrentImages.Count)
            {
                DetectedCurrentImage = DetectedCurrentImages[index];
            }

        }
    }
}
