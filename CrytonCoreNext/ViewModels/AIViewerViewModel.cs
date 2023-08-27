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
        public bool userMouseIsInDetectedObject;

        [ObservableProperty]
        public int imageCompareSliderValue = DefaultCompareSliderValue;

        [ObservableProperty]
        public int selectedTabControlIndex;

        public AIViewerViewModel(IYoloModelService yoloModelService)
        {
            DetectedCurrentImages = new();
            _yoloModelService = yoloModelService;
            _yoloModelService.LoadYoloModel();
            _yoloModelService.LoadLabels();
            Images = new();
            OnSelectedTabControlIndexChanged(0);
        }

        [RelayCommand]
        private void LoadImages()
        {
#if DEBUG
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

#endif
        }

        partial void OnSelectedTabControlIndexChanged(int value)
        {
            UserIsInAdjusterTab = value == 1;
            if (UserIsInAdjusterTab)
            {
                OnTabControlChanged.Invoke();
            }
        }

        partial void OnSelectedDetectionImageChanged(AIDetectionImage? value)
        {
            SelectedImage.DetectionImage = value == null ? 
                SelectedImage.Image : 
                Drawers.YoloDetectionDrawer.DrawDetection(SelectedImage, value);
            UserMouseIsInDetectedObject = value != null;
        }
    }
}
