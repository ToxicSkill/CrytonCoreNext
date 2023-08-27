using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.AI.Interfaces;
using CrytonCoreNext.AI.Models;
using OpenCvSharp.WpfExtensions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace CrytonCoreNext.ViewModels
{
    public partial class AIViewerViewModel : ObservableObject
    {
        private const int DefaultCompareSliderValue = 50;

        private const double DefaultAutoColorValue = 0.5;

        private const string YoloModelONNXPath = "AI/YoloModels/yolov7-tiny.onnx";

        private readonly IYoloModelService _yoloModelService;

        public delegate void TabControlChanged();

        public event TabControlChanged OnTabControlChanged;

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
        public bool userIsInAdjusterTab;

        [ObservableProperty]
        public int imageCompareSliderValue = DefaultCompareSliderValue;

        [ObservableProperty]
        public bool drawAllBoxSelected;

        [ObservableProperty]
        public bool autoColorSwitch;

        [ObservableProperty]
        public double autoColorValue = DefaultAutoColorValue;

        [ObservableProperty]
        public int selectedTabControlIndex;

        public AIViewerViewModel(IYoloModelService yoloModelService)
        {
            DetectedCurrentImages = new();
            _yoloModelService = yoloModelService;
            _yoloModelService.LoadYoloModel(YoloModelONNXPath);
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
            SelectedImage = Images.First();
        }

        partial void OnSelectedTabControlIndexChanged(int value)
        {
            UserIsInAdjusterTab = value == 1;
            if (value == 1)
            {
                OnTabControlChanged.Invoke();
            }
        }

        partial void OnAutoColorValueChanged(double value)
        {
            OnAutoColorSwitchChanged(AutoColorSwitch);
        }

        partial void OnAutoColorSwitchChanged(bool value)
        {
            SelectedImage.AdjusterImage = value ? 
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
