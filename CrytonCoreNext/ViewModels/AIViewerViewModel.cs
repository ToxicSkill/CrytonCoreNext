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
using Wpf.Ui;
using Wpf.Ui.Controls;


using DialogService = CrytonCoreNext.Services.DialogService;

namespace CrytonCoreNext.ViewModels
{
    public partial class AIViewerViewModel : ObservableObject
    {
        private const int DefaultCompareSliderValue = 50;

        private readonly IYoloModelService _yoloModelService;

        private readonly PdfViewModel _pdfViewModel;

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
            ImageDrawer drawer,
            DialogService dialogService)
        {
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
            Images = new(Images.Except(new List<AIImage>() { SelectedImage }));
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
        private void SaveImage()
        {
            var outputFileName = _dialogService.GetFileNameToSave(".png", Environment.SpecialFolder.Recent);
            if (outputFileName != string.Empty)
            {
                Cv2.ImWrite(outputFileName, SelectedImage.AdjusterImage.ToMat());
            }
        }

        [RelayCommand]
        private void LoadImages()
        {
            var filesToOpen = _dialogService.GetFilesNamesToOpen(Static.Extensions.DialogFilters.Images, Environment.SpecialFolder.Recent);
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
                SelectedImage = Images.First();
            }
        }

        partial void OnSelectedDetectionImageChanged(AIDetectionImage? value)
        {
            SelectedImage.DetectionImage = YoloDetectionDrawer.DrawDetection(SelectedImage, value);
            UserMouseIsInDetectedObject = value != null;
        }
    }
}
