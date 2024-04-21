using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.AI.Interfaces;
using CrytonCoreNext.AI.Models;
using CrytonCoreNext.Drawers;
using CrytonCoreNext.Enums;
using CrytonCoreNext.Views;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Wpf.Ui;
using Wpf.Ui.Controls;
using DialogService = CrytonCoreNext.Services.DialogService;

namespace CrytonCoreNext.ViewModels
{
    public partial class AIViewerViewModel : ObservableObject
    {
        private const int MaxImageSavingRepeatCount = 5;

        private const int ImageSavingTriesDelayInMiliseconds = 500;

        private const int DefaultCompareSliderValue = 50;

        private readonly IYoloModelService _yoloModelService;

        private readonly PdfViewModel _pdfViewModel;

        private readonly ISnackbarService _snackbarService;

        private readonly INavigationService _navigationService;

        private readonly ImageDrawer _imageDrawer;

        private readonly DialogService _dialogService;

        public delegate void TabControlChanged();

        public event TabControlChanged OnTabControlChanged;

        [ObservableProperty]
        private ObservableCollection<object> navigationItems = [];

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
        public bool userMouseIsInDetectedObject;

        [ObservableProperty]
        public bool showOriginal;

        [ObservableProperty]
        public int imageCompareSliderValue = DefaultCompareSliderValue;

        public AIViewerViewModel(
            IYoloModelService yoloModelService,
            PdfViewModel pdfViewModel,
            INavigationService navigationService,
            ISnackbarService snackbarService,
            ImageDrawer drawer,
            DialogService dialogService)
        {
            _snackbarService = snackbarService;
            _pdfViewModel = pdfViewModel;
            _navigationService = navigationService;
            _imageDrawer = drawer;
            _dialogService = dialogService;
            DetectedCurrentImages = [];
            _yoloModelService = yoloModelService;
            Images = [];
            NavigationItems =
            [
                new NavigationViewItem("Processes", SymbolRegular.Apps24, typeof(PdfView))
            ];
        }

        [RelayCommand]
        private void ExportImageToPDF()
        {
            if (_pdfViewModel.ExportImageToPDF(
                new Models.File(
                    SelectedImage.Path,
                    DateTime.Now,
                    EImageExtensions.png.ToString(),
                    0, SelectedImage.AdjusterImage.ToMat().ToBytes()),
                SelectedImage.AdjusterImage.ToMat()))
            {
                _navigationService.Navigate(typeof(PdfView));
            }
        }

        [RelayCommand]
        private void DeleteImage()
        {
            var index = Images.IndexOf(SelectedImage);
            Images = new(Images.Except([SelectedImage]));
            if (index > 0)
            {
                SelectedImage = Images[index - 1];
            }
            else if (index == 0 && Images.Any())
            {
                SelectedImage = Images[0];
            }
        }

        [RelayCommand]
        private async Task SaveImage()
        {
            SelectedImage.RenderFinal = true;
            SelectedImage.UpdateImage();
            var isReady = false;
            var outputFileName = _dialogService.GetFileNameToSave(".png", Environment.SpecialFolder.Desktop);
            if (outputFileName != string.Empty)
            {
                for (var i = 0; i < MaxImageSavingRepeatCount; i++)
                {
                    await Task.Delay(ImageSavingTriesDelayInMiliseconds);
                    isReady = SelectedImage.IsImageReady();
                    if (isReady)
                    {
                        break;
                    }
                }
                if (isReady)
                {
                    Cv2.ImWrite(outputFileName, SelectedImage.AdjusterImage.ToMat());
                    _snackbarService.Show("Success", "Image has been successfully saved", ControlAppearance.Success, new SymbolIcon(SymbolRegular.CheckmarkCircle20), TimeSpan.FromSeconds(2));
                }
                else
                {
                    _snackbarService.Show("Error", "Error occured during image saving", ControlAppearance.Danger, new SymbolIcon(SymbolRegular.ErrorCircle20), TimeSpan.FromSeconds(2));
                }
            }
            SelectedImage.RenderFinal = false;
        }

        [RelayCommand]
        private void LoadImages()
        {
            var filesToOpen = _dialogService.GetFilesNamesToOpen(Static.Extensions.DialogFilters.Images, Environment.SpecialFolder.Desktop);
            if (filesToOpen.Count != 0)
            {
                var newFiles = new List<AIImage>();
                foreach (var item in filesToOpen)
                {
                    newFiles.Add(new(item, _imageDrawer));
                }
                foreach (var image in newFiles)
                {
                    image.SetPredicitons(_yoloModelService.GetPredictions(image.Image.ToMat()));
                }
                var oldList = Images.ToList();
                oldList.AddRange(newFiles);
                Images = new(oldList);
                SelectedImage = Images.Last();
            }
        }

        [RelayCommand]
        private void SetDefaultValue(string parameter)
        {
            switch (parameter)
            {
                case "Exposure":
                    SelectedImage.Exposure.SetDefault();
                    break;
                case "Contrast":
                    SelectedImage.Contrast.SetDefault();
                    break;
                case "Brightness":
                    SelectedImage.Brightness.SetDefault();
                    break;
                default:
                    break;
            }
        }

        partial void OnSelectedDetectionImageChanged(AIDetectionImage? value)
        {
            SelectedImage.DetectionImage = YoloDetectionDrawer.DrawDetection(SelectedImage, value);
            if (value != null)
            {
                SelectedImage.DetectionLabel = value.Prediction.Label.Name ?? string.Empty;
            }
            else
            {
                SelectedImage.DetectionLabel = string.Empty;
            }
            UserMouseIsInDetectedObject = value != null;
        }
    }
}
