using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces.Files;
using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Wpf.Ui.Mvvm.Contracts;
using IDialogService = CrytonCoreNext.Interfaces.IDialogService;

namespace CrytonCoreNext.ViewModels
{
    public partial class PdfViewModel : InteractiveViewBase
    {
        private readonly IPDFService _pdfService;

        private List<(int pdfIndex, int pdfPage)> _pdfMergeImageRules;

        private int _currentPdfToMergeImageIndex = 0;

        [ObservableProperty]
        public string pageMergeCountStatus;

        [ObservableProperty]
        public bool hasMoreThanOnePageToMerge;
        
        [ObservableProperty]
        public string pdfPassword;

        [ObservableProperty]
        public ObservableCollection<PDFFile> pdfFiles;

        [ObservableProperty]
        public ObservableCollection<PDFFile> openedPdfFiles;

        [ObservableProperty]
        public ObservableCollection<PDFFile> selectedPdfFilesToMerge;

        [ObservableProperty] 
        public ObservableCollection<ImageFile> imageFiles;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PdfFiles))]
        public PDFFile selectedPdfFile;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImageFiles))]
        public ImageFile selectedImageFile;
        
        [ObservableProperty]
        public PDFFile selectedPdfFileToMerge;

        [ObservableProperty]
        public PDFFile openedPdfSelectedFile;

        [ObservableProperty]
        public WriteableBitmap pdfToMergeImage;

        [ObservableProperty]
        public int selectedTabIndex;

        [ObservableProperty]
        public bool isOnLastMergePage = true;

        [ObservableProperty]
        public bool isOnFirstMergePage = true;

        public PdfViewModel(IPDFService pdfService,
            IFileService fileService,
            IDialogService dialogService,
            ISnackbarService snackbarService) : base(fileService, dialogService, snackbarService)
        {
            _pdfService = pdfService;
            _pdfMergeImageRules = new();

            pdfFiles = new();
            imageFiles = new();
            openedPdfFiles = new(); 
            selectedPdfFilesToMerge = new();
        }

        partial void OnSelectedTabIndexChanged(int value)
        {
            if (SelectedTabIndex == 1)
            {
                OpenedPdfFiles.Clear();
                foreach (var file in PdfFiles)
                {
                    if (file.IsOpened)
                    {
                        OpenedPdfFiles.Add(file);
                    }
                }
            }
        }

        [RelayCommand]
        private void AddFileToMergeList()
        {
            if (!SelectedPdfFilesToMerge.Contains(OpenedPdfSelectedFile))
            {
                SelectedPdfFilesToMerge.Add(OpenedPdfSelectedFile);
                UpdatePdfToMergeImage();
            }
        }

        [RelayCommand]
        private void RemoveFileFromMergeList()
        {
            if (SelectedPdfFilesToMerge.Contains(SelectedPdfFileToMerge))
            {
                SelectedPdfFilesToMerge.Remove(SelectedPdfFileToMerge);
                UpdatePdfToMergeImage();
            }
        }

        private void UpdatePdfToMergeImage()
        {
            _pdfMergeImageRules.Clear();
            var pdfIndex = 0;
            foreach (var mergeFile in SelectedPdfFilesToMerge)
            {
                foreach (var pageIndex in Enumerable.Range(0, mergeFile.NumberOfPages))
                {
                    _pdfMergeImageRules.Add((pdfIndex, pageIndex));
                }
                pdfIndex++;
            }
            if (_currentPdfToMergeImageIndex >= _pdfMergeImageRules.Count)
            {
                _currentPdfToMergeImageIndex = _pdfMergeImageRules.Count - 1;
                if (_currentPdfToMergeImageIndex < 0)
                {
                    _currentPdfToMergeImageIndex = 0;
                }
            }
            if (!_pdfMergeImageRules.Any())
            {
                PdfToMergeImage = null;
            }
            else
            {
                var pdfFile = SelectedPdfFilesToMerge[_pdfMergeImageRules[_currentPdfToMergeImageIndex].pdfIndex];
                pdfFile.LastPage = _pdfMergeImageRules[_currentPdfToMergeImageIndex].pdfPage;
                PdfToMergeImage = _pdfService.LoadImage(pdfFile);
            }
            IsOnFirstMergePage = !_pdfMergeImageRules.Any() ? true : _currentPdfToMergeImageIndex == 0;
            IsOnLastMergePage = !_pdfMergeImageRules.Any() ? true : _currentPdfToMergeImageIndex == _pdfMergeImageRules.Count - 1;
            HasMoreThanOnePageToMerge = _pdfMergeImageRules.Any();
            PageMergeCountStatus = $"{_currentPdfToMergeImageIndex + 1} / {_pdfMergeImageRules.Count}";
        }

        partial void OnSelectedPdfFileChanged(PDFFile value)
        {
            if (value != null)
            {
                value.PageImage = _pdfService.LoadImage(value);
            }
        }

        [RelayCommand]
        private void DeleteFile()
        {
            var oldIndex = PdfFiles.IndexOf(SelectedPdfFile);
            PdfFiles.Remove(SelectedPdfFile);
            if (PdfFiles.Any())
            {
                SelectedPdfFile = PdfFiles.ElementAt(oldIndex > 0 ? --oldIndex : oldIndex);
            }
        }

        [RelayCommand]
        private new async Task LoadPdfFiles()
        {
            Lock();
            var protectedFilesCount = 0;
            var damagedFilesCount = 0;
            await foreach (var file in base.LoadFiles(Static.Extensions.DialogFilters.Pdf))
            {
                var pdfFile = _pdfService.ReadPdf(file);
                if (pdfFile != null)
                {
                    PdfFiles.Add(pdfFile);
                }
                if (pdfFile.PdfStatus == PDF.Enums.EPdfStatus.Protected)
                {
                    protectedFilesCount++;
                }
                if (pdfFile.PdfStatus == PDF.Enums.EPdfStatus.Damaged)
                {
                    damagedFilesCount++;
                }
                SelectedPdfFile = PdfFiles.Last();
            }
            if (SelectedPdfFile == null && PdfFiles.Any())
            {
                SelectedPdfFile = PdfFiles.First();
            }
            if (protectedFilesCount > 0 && damagedFilesCount == 0)
            {
                PostSnackbar("Warning",
                    (protectedFilesCount > 1 ?
                    $"{protectedFilesCount} of {protectedFilesCount + pdfFiles.Count} loaded files" :
                    "One file") + " requires password",
                    Wpf.Ui.Common.SymbolRegular.Warning20,
                    Wpf.Ui.Common.ControlAppearance.Caution);
            }
            else if (damagedFilesCount > 0 && protectedFilesCount == 0)
            {
                PostSnackbar("Warning",
                    (protectedFilesCount > 1 ?
                    $"{protectedFilesCount} of {protectedFilesCount + pdfFiles.Count} loaded files" :
                    "One file") + " is damaged",
                    Wpf.Ui.Common.SymbolRegular.Warning20,
                    Wpf.Ui.Common.ControlAppearance.Caution);
            }
            else if (damagedFilesCount > 0 && protectedFilesCount > 0)
            {
                PostSnackbar("Warning",
                    "At least one file is protected by password and at least one file is damaged",
                    Wpf.Ui.Common.SymbolRegular.Warning20,
                    Wpf.Ui.Common.ControlAppearance.Caution);
            }
            Unlock();
        }

        [RelayCommand]
        private new async Task LoadImageFiles()
        {
            Lock();
            await foreach (var imageFile in base.LoadFiles(Static.Extensions.DialogFilters.Images))
            {
                ImageFiles.Add(new ImageFile(imageFile));
                SelectedImageFile = ImageFiles.Last();
            }
            if (SelectedImageFile == null && ImageFiles.Any())
            {
                SelectedImageFile = ImageFiles.First();
            }
            Unlock();
        }

        [RelayCommand]
        private void ConfirmPassword()
        {
            SelectedPdfFile.Password = PdfPassword;
            UpdateProtectedPdf();
        }

        [RelayCommand]
        private void GoPreviousPage()
        {
            if (SelectedPdfFile == null)
            {
                return;
            }
            if (SelectedPdfFile.NumberOfPages > 0 && SelectedPdfFile.LastPage > 0)
            {
                SelectedPdfFile.LastPage -= 1;
            }
            SelectedPdfFile.PageImage = _pdfService.LoadImage(SelectedPdfFile);
            OnPropertyChanged(nameof(SelectedPdfFile));
        }

        [RelayCommand]
        private void GoNextPage()
        {
            if (SelectedPdfFile == null)
            {
                return;
            }
            if (SelectedPdfFile.NumberOfPages > 0 && SelectedPdfFile.LastPage < SelectedPdfFile.NumberOfPages - 1)
            {
                SelectedPdfFile.LastPage += 1;
            }
            SelectedPdfFile.PageImage = _pdfService.LoadImage(SelectedPdfFile);
            OnPropertyChanged(nameof(SelectedPdfFile));
        }

        [RelayCommand]
        private void GoNextPagePdfToMergeIndex()
        {
            if(_currentPdfToMergeImageIndex < _pdfMergeImageRules.Count - 1)
            {
                _currentPdfToMergeImageIndex++;
            }
            UpdatePdfToMergeImage();
            OnPropertyChanged(nameof(PdfToMergeImage));
        }

        [RelayCommand]
        private void GoPreviousPagePdfToMergeIndex()
        {
            if (_currentPdfToMergeImageIndex > 0)
            {
                _currentPdfToMergeImageIndex--;
            }
            UpdatePdfToMergeImage();
            OnPropertyChanged(nameof(PdfToMergeImage));
        }

        private void UpdateProtectedPdf()
        {
            SelectedPdfFile.PdfStatus = PDF.Enums.EPdfStatus.Opened;
            _pdfService.UpdatePdfFileInformations(ref selectedPdfFile);
            if (SelectedPdfFile.PdfStatus == PDF.Enums.EPdfStatus.Protected ||
                SelectedPdfFile.PdfStatus == PDF.Enums.EPdfStatus.Damaged)
            {
                SelectedPdfFile.Password = string.Empty;
                PostSnackbar("Caution",
                    "Incorrect password",
                    Wpf.Ui.Common.SymbolRegular.ErrorCircle20,
                    Wpf.Ui.Common.ControlAppearance.Danger);
            }
            else
            {
                PostSnackbar("Success",
                    "Pdf document has been open successfully",
                    Wpf.Ui.Common.SymbolRegular.Checkmark20,
                    Wpf.Ui.Common.ControlAppearance.Success);
                RefreshCollection();
            }
        }

        private void RefreshCollection()
        {
            var oldFile = SelectedPdfFile;
            var oldIndex = PdfFiles.IndexOf(SelectedPdfFile);
            PdfFiles.Remove(SelectedPdfFile);
            PdfFiles.Insert(oldIndex, oldFile);
            SelectedPdfFile = oldFile;
        }

        private static string GetValueFromMetadata(string metadata, string propertyName)
        {
            var index = metadata.IndexOf(propertyName, StringComparison.Ordinal);

            if (index < 0)
            {
                return string.Empty;
            }

            var startIndex = index + propertyName.Length + 1;
            var endIndex = metadata.IndexOf('\n', startIndex);

            if (endIndex < 0)
            {
                endIndex = metadata.Length;
            }

            return metadata[startIndex..endIndex].Trim();
        }
    }
}
