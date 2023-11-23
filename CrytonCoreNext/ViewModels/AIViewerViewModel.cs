using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.AI.Interfaces;
using CrytonCoreNext.AI.Models;
using CrytonCoreNext.Drawers;
using CrytonCoreNext.Views;
using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using static CrytonCoreNext.Models.WindowDialog;

namespace CrytonCoreNext.ViewModels
{
    public partial class AIViewerViewModel : ObservableObject
    {
        private const int DefaultCompareSliderValue = 50;

        private readonly IYoloModelService _yoloModelService;

        private readonly PdfViewModel _pdfViewModel;

        private readonly INavigationService _navigationService;

        private readonly ImageDrawer _imageDrawer;

        public delegate void TabControlChanged();

        public event TabControlChanged OnTabControlChanged;

        [ObservableProperty]
        private ObservableCollection<INavigationControl> navigationItems = [];

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
            ImageDrawer drawer)
        {
            _pdfViewModel = pdfViewModel;
            _navigationService = navigationService; 
            _imageDrawer = drawer;
            DetectedCurrentImages = [];
            _yoloModelService = yoloModelService;
            _yoloModelService.LoadYoloModel();
            _yoloModelService.LoadLabels();
            Images = [];
            NavigationItems =
            [
                new NavigationItem
                {
                    Content = "Processes",
                    PageTag = "processes",
                    Icon = SymbolRegular.Apps24,
                    PageType = typeof(PdfView)
                }
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
            var fileDialog = new SaveFileDialog()
            {
                Title = "Save file",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };
            if (fileDialog.ShowDialog() == true)
            {
                var outputFilePath = fileDialog.FileName;
                if (outputFilePath == null)
                {
                    return;
                }
                else if (Path.GetExtension(outputFilePath) == string.Empty)
                {
                    outputFilePath = Path.ChangeExtension(outputFilePath, ".png");
                }
                
                Cv2.ImWrite(outputFilePath, SelectedImage.AdjusterImage.ToMat());
            }
        }

        [RelayCommand]
        private void LoadImages()
        {
            var folderDialog = new OpenFileDialog()
            {
                Title = "Select Folder",
                Multiselect = true,
                Filter = Static.Extensions.FilterToPrompt(Static.Extensions.DialogFilters.Images),
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            if (folderDialog.ShowDialog() == true)
            {
                var newFiles = new List<AIImage>();
                foreach (var item in folderDialog.FileNames)
                {
                    newFiles.Add(new(item, _imageDrawer));
                }
                foreach (var image in newFiles)
                {
                    image.SetPredicitons(_yoloModelService.GetPredictions(image.Image.ToMat()));
                }
                var oldList = Images.ToList();
                oldList.AddRange(newFiles);
                Images = new (oldList);
                SelectedImage = Images.First();
            }
        }

        partial void OnSelectedDetectionImageChanged(AIDetectionImage? value)
        {
            SelectedImage.DetectionImage = Drawers.YoloDetectionDrawer.DrawDetection(SelectedImage, value);
            UserMouseIsInDetectedObject = value != null;
        }
    }
}
