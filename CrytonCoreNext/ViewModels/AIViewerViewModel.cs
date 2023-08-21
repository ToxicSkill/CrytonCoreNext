using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.AI.Interfaces;
using CrytonCoreNext.AI.Models;
using CrytonCoreNext.Models;
using OpenCvSharp.WpfExtensions;
using System.Collections.ObjectModel;
using System.Windows.Threading;

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
        public AIDetectionImage? selectedDetectionImage;

        [ObservableProperty]
        public ObservableCollection<AIImage> images;

        [ObservableProperty]
        public AIImage selectedImage;

        [ObservableProperty]
        public bool drawAllBoxSelected; 

        public AIViewerViewModel(IYoloModelService yoloModelService)
        {
            DetectedCurrentImages = new();
            _yoloModelService = yoloModelService;
            _yoloModelService.LoadYoloModel("AI/YoloModels/yolov7-tiny.onnx");
            _yoloModelService.LoadLabels();
            Images = new();
        }

        [RelayCommand]
        private void LoadImages()
        {
            Images = new()
            {
                new ("C:\\Users\\gizmo\\OneDrive\\Obrazy\\2022-02-04-test_image.jpg"),
                new ( "C:\\Users\\gizmo\\OneDrive\\Obrazy\\tough-crowd.png")
            };

            foreach (var image in Images)
            {
                image.SetPredicitons(_yoloModelService.GetPredictions(image.Image.ToMat()));
            }
        }

        partial void OnSelectedImageChanged(AIImage value)
        {
            OnDrawAllBoxSelectedChanged(DrawAllBoxSelected);
        }

        partial void OnSelectedDetectionImageChanged(AIDetectionImage? value)
        {
            DrawAllBoxSelected = false;
            if (value == null)
            {
                SelectedImage.DetectionImage = SelectedImage.Image;
            }
            else
            {
                SelectedImage.DetectionImage = Drawers.YoloDetectionDrawer.DrawDetection(SelectedImage, value);
            }
        }

        partial void OnDrawAllBoxSelectedChanged(bool value)
        {
            if (value)
            {
                SelectedImage.DetectionImage = Drawers.YoloDetectionDrawer.DrawAllDetections(SelectedImage);
            }
            else
            {
                SelectedImage.DetectionImage = SelectedImage.Image;
            }
        }
    }
}
