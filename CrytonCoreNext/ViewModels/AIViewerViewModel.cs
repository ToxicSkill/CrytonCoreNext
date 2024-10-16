using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.AI.Interfaces;
using CrytonCoreNext.AI.Models;
using CrytonCoreNext.AI.Services;
using CrytonCoreNext.Drawers;
using CrytonCoreNext.Interfaces.Files;
using CrytonCoreNext.Views;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;
using static CrytonCoreNext.Static.Extensions;
using DialogService = CrytonCoreNext.Services.DialogService;

namespace CrytonCoreNext.ViewModels
{
    public partial class AIViewerViewModel : InteractiveViewBase
    {
        private const int MaxImageSavingRepeatCount = 5;

        private const int ImageSavingTriesDelayInMiliseconds = 500;

        private const int DefaultCompareSliderValue = 50;

        private readonly IYoloModelService _yoloModelService;

        private readonly PdfViewModel _pdfViewModel;

        private readonly ISnackbarService _snackbarService;

        private readonly INavigationService _navigationService;

        private readonly DialogService _dialogService;

        private readonly AIImageLoader _aiImageLoader;

        private readonly IProgress<double> _progress;

        private readonly IProgress<double> _aiProgress;

        public delegate void TabControlChanged();

        public event TabControlChanged OnTabControlChanged;

        [ObservableProperty]
        private int _aiProgressValue;

        [ObservableProperty]
        private ObservableCollection<object> _navigationItems = [];

        [ObservableProperty]
        private ObservableCollection<AIDetectionImage> _detectedCurrentImages;

        [ObservableProperty]
        private AIDetectionImage _detectedCurrentImage;

        [ObservableProperty]
        private AIDetectionImage? _selectedDetectionImage;

        [ObservableProperty]
        private ObservableCollection<AIImage> _images;

        [ObservableProperty]
        private AIImage _selectedImage;

        [ObservableProperty]
        private bool _userMouseIsInDetectedObject;

        [ObservableProperty]
        private bool _isLoadingInProgress;

        [ObservableProperty]
        private bool _showOriginal;

        [ObservableProperty]
        private int _imageCompareSliderValue = DefaultCompareSliderValue;

        public AIViewerViewModel(
            IYoloModelService yoloModelService,
            PdfViewModel pdfViewModel,
            INavigationService navigationService,
            ISnackbarService snackbarService,
            IFilesSaver filesSaver,
            IFilesLoader filesLoader,
            AIImageLoader aiImageLoader,
            DialogService dialogService) : base(filesLoader, filesSaver, snackbarService, dialogService)
        {
            _snackbarService = snackbarService;
            _pdfViewModel = pdfViewModel;
            _navigationService = navigationService;
            _dialogService = dialogService;
            DetectedCurrentImages = [];
            _aiImageLoader = aiImageLoader;
            _yoloModelService = yoloModelService;
            _progress = new Progress<double>(UpdateProgress);
            _aiProgress = new Progress<double>(UpdateAiProgress);
            Images = [];
            NavigationItems =
            [
                new NavigationViewItem("Processes", SymbolRegular.Apps24, typeof(PdfView))
            ];
        }

        partial void OnImagesChanged(ObservableCollection<AIImage> value)
        {
            IsLoadingInProgress = false;
        }

        [RelayCommand]
        private void ExportImageToPDF()
        {
            if (_pdfViewModel.ExportImageToPDF(
                new Models.File(
                    SelectedImage.Path,
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

        private async Task<bool> RenderFinal()
        {
            var isReady = false;
            SelectedImage.RenderFinal = true;
            SelectedImage.UpdateImage();

            for (var i = 0; i < MaxImageSavingRepeatCount; i++)
            {
                await Task.Delay(ImageSavingTriesDelayInMiliseconds);
                isReady = SelectedImage.IsImageReady();
                if (isReady)
                {
                    break;
                }
            }
            if (!isReady)
            {

                _snackbarService.Show("Error", "Error occured during image rendering", ControlAppearance.Danger, new SymbolIcon(SymbolRegular.ErrorCircle20), TimeSpan.FromSeconds(2));
            }
            return isReady;
        }

        [RelayCommand]
        private async Task CoptyImageToClipboard()
        {
            if (await RenderFinal())
            {
                Clipboard.SetImage(SelectedImage.Image.ToMat().ToBitmapSource());
                _snackbarService.Show("Success", "Image has been copied to clipboard", ControlAppearance.Success, new SymbolIcon(SymbolRegular.CheckmarkCircle20), TimeSpan.FromSeconds(2));
            }
        }

        [RelayCommand]
        private async Task SaveImage()
        {
            if (!await RenderFinal())
            {
                return;
            }
            var outputFileName = _dialogService.GetFileNameToSave(
                System.IO.Path.GetFileNameWithoutExtension(SelectedImage.Path),
                ".png",
                Environment.SpecialFolder.Desktop,
                DialogFilters.Images);
            if (outputFileName != string.Empty)
            {
                Cv2.ImWrite(outputFileName, SelectedImage.AdjusterImage.ToMat());
                _snackbarService.Show("Success", "Image has been successfully saved", ControlAppearance.Success, new SymbolIcon(SymbolRegular.CheckmarkCircle20), TimeSpan.FromSeconds(2));
            }
        }

        [RelayCommand]
        private async Task LoadImages()
        {
            IsLoadingInProgress = true;
            var newFiles = new List<AIImage>();
            await foreach (var file in LoadFiles(_progress, DialogFilters.Images))
            {
                newFiles.Add(_aiImageLoader.InitializeFile(file));
            }
            if (newFiles.Count == 0)
            {
                return;
            }
            var iterator = 0;
            foreach (var image in newFiles)
            {
                using (var mat = image.Image.ToMat())
                {
                    image.SetPredicitons(await _yoloModelService.GetPredictions(mat), mat);
                }
                iterator++;
                _aiProgress.Report((double)iterator / (double)newFiles.Count);
            }
            _aiProgress.Report(0);
            _progress.Report(0);
            var oldList = Images.ToList();
            oldList.AddRange(newFiles);
            Images = new(oldList);
            SelectedImage = Images.Last();
        }

        [RelayCommand]
        private void SetDefaultValue(string parameter)
        {
            switch (parameter)
            {
                case nameof(SelectedImage.Exposure):
                    SelectedImage.Exposure.SetDefault();
                    break;
                case nameof(SelectedImage.Contrast):
                    SelectedImage.Contrast.SetDefault();
                    break;
                case nameof(SelectedImage.Brightness):
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

        private void UpdateAiProgress(double value)
        {
            AiProgressValue = (int)(value * 100);
        }
    }
}
