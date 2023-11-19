using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.AI.Interfaces;
using CrytonCoreNext.AI.Models;
using CrytonCoreNext.Drawers;
using CrytonCoreNext.Enums;
using CrytonCoreNext.Extensions;
using CrytonCoreNext.Views;
using Microsoft.Win32;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Documents;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;

namespace CrytonCoreNext.ViewModels
{
    public partial class AIViewerViewModel : ObservableObject
    {
        private const int DefaultCompareSliderValue = 50;

        private readonly IYoloModelService _yoloModelService;

        private ImageDrawer _imageDrawer;

        public delegate void TabControlChanged();

        public event TabControlChanged OnTabControlChanged;

        [ObservableProperty]
        private ObservableCollection<INavigationControl> navigationItems = new();

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

        public AIViewerViewModel(IYoloModelService yoloModelService, ImageDrawer drawer)
        {
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
