using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.AI.Interfaces;
using CrytonCoreNext.AI.Models;
using OpenCvSharp.WpfExtensions;
using System.Collections.ObjectModel;
using System.Windows;

namespace CrytonCoreNext.ViewModels
{
    public partial class AIViewerViewModel : ObservableObject
    {
        private const int DefaultCompareSliderValue = 50;

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
        public Visibility compareSliderVisibility;

        [ObservableProperty]
        public int imageCompareSliderValue = DefaultCompareSliderValue;

        [ObservableProperty]
        public bool drawAllBoxSelected;

        [ObservableProperty]
        public bool autoColorSwitch;

        [ObservableProperty]
        public double autoColorValue;

        [ObservableProperty]
        public int selectedTabControlIndex;

        public AIViewerViewModel(IYoloModelService yoloModelService)
        {
            DetectedCurrentImages = new();
            _yoloModelService = yoloModelService;
            _yoloModelService.LoadYoloModel("AI/YoloModels/yolov7-tiny.onnx");
            _yoloModelService.LoadLabels();
            Images = new();
            OnSelectedTabControlIndexChanged(0);
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

        partial void OnSelectedTabControlIndexChanged(int value)
        {
            CompareSliderVisibility = value == 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        partial void OnAutoColorValueChanged(double value)
        {
            OnAutoColorSwitchChanged(AutoColorSwitch);
        }

        partial void OnAutoColorSwitchChanged(bool value)
        {
            SelectedImage.DetectionImage = value ? 
                Drawers.ImageDrawer.DrawAutoColor(SelectedImage.Image.ToMat(), AutoColorValue).ToWriteableBitmap() : 
                SelectedImage.Image;
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
