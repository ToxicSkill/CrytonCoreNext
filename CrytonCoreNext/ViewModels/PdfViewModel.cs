﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Enums;
using CrytonCoreNext.Extensions;
using CrytonCoreNext.Interfaces.Files;
using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Wpf.Ui.Mvvm.Contracts;
using File = CrytonCoreNext.Models.File;
using IDialogService = CrytonCoreNext.Interfaces.IDialogService;

namespace CrytonCoreNext.ViewModels
{
    public partial class PdfViewModel : InteractiveViewBase
    {
        private readonly IPDFService _pdfService;

        private List<(int pdfIndex, int pdfPage)> _pdfToMergePagesIndexes;

        private List<(int pdfIndex, int pdfPage)> _pdfExcludedMergeIndexes;

        private int _currentPdfToMergeImageIndex = 0;

        [ObservableProperty]
        public bool anyLoadedFile;

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
        public ObservableCollection<PDFFile> selectedPdfFilesToSplit;

        [ObservableProperty]
        public ObservableCollection<PDFFile> outcomeFilesFromSplit;

        [ObservableProperty]
        public ObservableCollection<PdfImageContainer> pdfToSplitImages;

        [ObservableProperty]
        public ObservableCollection<PdfRangeFile> pdfSplitRangeFiles;

        [ObservableProperty]
        public PdfImageContainer? selectedPdfToSplitImage;

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
        public PDFFile selectedPdfFileToSplit;

        [ObservableProperty]
        public PDFFile openedPdfSelectedFile;

        [ObservableProperty]
        public WriteableBitmap pdfToMergeImage;

        [ObservableProperty]
        public int selectedTabIndex;

        [ObservableProperty]
        public bool isSplitImageFirst;

        [ObservableProperty]
        public bool isSplitImageLast;

        [ObservableProperty]
        public bool hasSplit;

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
            _pdfToMergePagesIndexes = new();

            pdfFiles = new();
            imageFiles = new();
            openedPdfFiles = new(); 
            selectedPdfFilesToMerge = new();
            selectedPdfFilesToSplit = new();
            outcomeFilesFromSplit = new ();
            _pdfExcludedMergeIndexes = new();
            pdfToSplitImages = new();
            pdfSplitRangeFiles = new();
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
        private void AddFileToSplitList()
        {
            if (!SelectedPdfFilesToSplit.Any() && OpenedPdfSelectedFile.NumberOfPages > 1)
            {
                SelectedPdfFilesToSplit.Add(OpenedPdfSelectedFile);
                SelectedPdfFileToSplit = SelectedPdfFilesToSplit.First();
                UpdatePdfToSplitImage();
            }
        }

        [RelayCommand]
        private void RemoveFileFromMergeList()
        {
            RemoveFileFromMergeList(null, true);
        }


        [RelayCommand]
        private void DeleteSplit()
        {
            DrawSplitLine(EDirection.Both, true);
            UpdateSplitOutcomeFiles();
        }

        [RelayCommand]
        private void SplitSelectedPdfImageLeft()
        {
            DrawSplitLine(EDirection.Left);
            UpdateSplitOutcomeFiles();
        }

        [RelayCommand]
        private void SplitSelectedPdfImageRight()
        {
            DrawSplitLine(EDirection.Right);
            UpdateSplitOutcomeFiles();
        }

        [RelayCommand]
        private void RemoveFileFromSplitList()
        {
            if (SelectedPdfFilesToSplit.Any())
            {
                SelectedPdfFilesToSplit.Clear();
                PdfToSplitImages.Clear();
                PdfToSplitImages = new ();
                SelectedPdfToSplitImage = null;
            }
        }

        [RelayCommand]
        private async Task Split()
        {
            var pdfFile = await _pdfService.Split(SelectedPdfFileToSplit, 0, 0, OpenedPdfFiles.Max(x => x.Id) + 1);
            AddPdfToPdfList(pdfFile);
        }

        [RelayCommand]
        private async Task Merge()
        {
            var pdfFile = await _pdfService.Merge(SelectedPdfFilesToMerge.ToList());
            AddPdfToPdfList(pdfFile);
        }

        [RelayCommand]
        private void InsertPdf()
        {
            var mergedImagesPdf = _pdfService.MergeAllImagesToPDF(ImageFiles.ToList(), ImageFiles.Max(x => x.Id) + 1);
            AddPdfToPdfList(mergedImagesPdf);
        }

        [RelayCommand]
        private void ConvertSelectedImageToPdf()
        {
            var convertedPdf = _pdfService.ImageToPdf(SelectedImageFile, ImageFiles.Max(x => x.Id) + 1);
            AddPdfToPdfList(convertedPdf);
        }

        [RelayCommand]
        private void DeleteFile()
        {
            var oldIndex = PdfFiles.IndexOf(SelectedPdfFile);
            OnFileDeleteBefore();
            PdfFiles.Remove(SelectedPdfFile);
            OnFileDeleteAfter(oldIndex);
        }

        [RelayCommand]
        private async Task LoadPdfAndImageFiles(string? andImages = null)
        {
            Lock();
            var protectedFilesCount = 0;
            var damagedFilesCount = 0;
            await foreach (var file in base.LoadFiles(andImages == null ? Static.Extensions.DialogFilters.PdfAndImages : Static.Extensions.DialogFilters.Pdf))
            {
                if (file.Extension.ToLower().Contains("pdf"))
                {
                    LoadPdfFile(ref protectedFilesCount, ref damagedFilesCount, file);
                }
                else
                {
                    LoadImageFile(file);
                }
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
            CheckAnyFileLoaded();
            Unlock();
        }

        [RelayCommand]
        private async Task LoadImageFiles()
        {
            Lock();
            await foreach (var imageFile in base.LoadFiles(Static.Extensions.DialogFilters.Images))
            {
                LoadImageFile(imageFile);
            }
            Unlock();
        }

        [RelayCommand]
        private void SavePdfFile()
        {
            if (SelectedPdfFile == null)
            {
                return;
            }
            base.SaveFile(SelectedPdfFile);
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
            if (_currentPdfToMergeImageIndex < _pdfToMergePagesIndexes.Count - 1)
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

        [RelayCommand]
        private void EraseCurrentMergePage()
        {
            _pdfExcludedMergeIndexes.Add(_pdfToMergePagesIndexes[_currentPdfToMergeImageIndex]);
            UpdatePdfToMergeImage();
        }

        private void AddPdfToPdfList(PDFFile pdfFile)
        {
            _pdfService.UpdatePdfFileInformations(ref pdfFile);
            CheckNameConflicts(pdfFile);
            PdfFiles.Add(pdfFile);
        }

        private void RemoveFileFromMergeList(PDFFile? file = null, bool updateEachStep = true)
        {
            if (file == null)
            {
                file = SelectedPdfFileToMerge;
            }
            if (SelectedPdfFilesToMerge.Contains(file))
            {
                var toRemoveExcluded = new List<(int pdfIndex, int pdfPage)>();
                var indexOfSelectedPdf = SelectedPdfFilesToMerge.IndexOf(file);
                foreach (var excludedIndex in _pdfExcludedMergeIndexes)
                {
                    if (excludedIndex.pdfIndex == indexOfSelectedPdf)
                    {
                        toRemoveExcluded.Add(excludedIndex);
                    }
                }
                _pdfExcludedMergeIndexes = _pdfExcludedMergeIndexes.Except(toRemoveExcluded).ToList();
                SelectedPdfFilesToMerge.Remove(file);
                if (updateEachStep)
                {
                    UpdatePdfToMergeImage();
                }
            }
        }


        private void UpdatePdfToSplitImage()
        {
        }

        private void UpdatePdfToMergeImage()
        {
            _pdfToMergePagesIndexes.Clear();
            PDFFile removedByExclusion = default;
            var pdfIndex = 0;
            foreach (var mergeFile in SelectedPdfFilesToMerge)
            {
                var removed = true;
                foreach (var pageIndex in Enumerable.Range(0, mergeFile.NumberOfPages))
                {
                    var tupleToAdd = (pdfIndex, pageIndex);
                    if (!_pdfExcludedMergeIndexes.Contains(tupleToAdd))
                    {
                        removed = false;
                        _pdfToMergePagesIndexes.Add((pdfIndex, pageIndex));
                    }
                }
                if (removed)
                {
                    removedByExclusion = mergeFile;
                }
                pdfIndex++;
            }
            if (_currentPdfToMergeImageIndex >= _pdfToMergePagesIndexes.Count)
            {
                _currentPdfToMergeImageIndex = _pdfToMergePagesIndexes.Count - 1;
                if (_currentPdfToMergeImageIndex < 0)
                {
                    _currentPdfToMergeImageIndex = 0;
                }
            }
            if (!_pdfToMergePagesIndexes.Any())
            {
                PdfToMergeImage = null;
            }
            else
            {
                var pdfFile = SelectedPdfFilesToMerge[_pdfToMergePagesIndexes[_currentPdfToMergeImageIndex].pdfIndex];
                pdfFile.LastPage = _pdfToMergePagesIndexes[_currentPdfToMergeImageIndex].pdfPage;
                PdfToMergeImage = _pdfService.LoadImage(pdfFile);
                SelectedPdfFileToMerge = pdfFile;
            }
            IsOnFirstMergePage = !_pdfToMergePagesIndexes.Any() || _currentPdfToMergeImageIndex == 0;
            IsOnLastMergePage = !_pdfToMergePagesIndexes.Any() || _currentPdfToMergeImageIndex == _pdfToMergePagesIndexes.Count - 1;
            HasMoreThanOnePageToMerge = _pdfToMergePagesIndexes.Any();
            PageMergeCountStatus = $"{_currentPdfToMergeImageIndex + 1} / {_pdfToMergePagesIndexes.Count}";
            if (removedByExclusion != null)
            {
                RemoveFileFromMergeList(removedByExclusion, false);
                UpdatePdfToMergeImage();
            }
            OnPropertyChanged(nameof(SelectedPdfFilesToMerge));
        }

        partial void OnSelectedTabIndexChanged(int value)
        {
            if (SelectedTabIndex == 1 || SelectedTabIndex == 2)
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

        partial void OnSelectedPdfFileChanged(PDFFile value)
        {
            if (value != null)
            {
                value.PageImage = _pdfService.LoadImage(value);
            }
        }

        partial void OnSelectedPdfFileToSplitChanged(PDFFile value)
        {
            if (value == null)
            {
                PdfToSplitImages.Clear();
                return;
            }
            if (PdfToSplitImages.Any())
            {
                return;
            }
            var images = new List<PdfImageContainer>();
            for (var i = 0; i < value.NumberOfPages; i++)
            {
                value.LastPage = i;
                images.Add(new PdfImageContainer(i+1, _pdfService.LoadImage(value)));
            }
            PdfToSplitImages = new ObservableCollection<PdfImageContainer>(images);
            SelectedPdfToSplitImage = PdfToSplitImages.First();
        }

        partial void OnSelectedPdfToSplitImageChanged(PdfImageContainer? value)
        {
            if (value != null)
            {
                var notNullableValue = (PdfImageContainer)value;
                IsSplitImageLast = notNullableValue.PageNumber == SelectedPdfFileToSplit.NumberOfPages;
                IsSplitImageFirst = notNullableValue.PageNumber == 1;
                HasSplit = notNullableValue.IsVerticalSplitLineLeftVisible || notNullableValue.IsVerticalSplitLineRightVisible;
            }
        }

        partial void OnSelectedPdfFileToMergeChanged(PDFFile value)
        {
            if (value != null)
            {
                _currentPdfToMergeImageIndex = FindSectedFileIndex();
                UpdatePdfToMergeImage();
            }
        }

        private int FindSectedFileIndex()
        {
            var selectedPdfIndex = SelectedPdfFilesToMerge.IndexOf(SelectedPdfFileToMerge);
            var index = 0;
            foreach (var fileMerge in _pdfToMergePagesIndexes)
            {
                if (fileMerge.pdfIndex == selectedPdfIndex)
                {
                    return index;
                }
                index++;
            }
            return 0;
        }

        private void OnFileDeleteAfter(int oldIndex)
        {
            if (PdfFiles.Any())
            {
                SelectedPdfFile = PdfFiles.ElementAt(oldIndex > 0 ? --oldIndex : oldIndex);
            }
            CheckAnyFileLoaded();
        }

        private void OnFileDeleteBefore()
        {
            SelectedPdfFilesToMerge.Remove(SelectedPdfFile);
            SelectedPdfFilesToSplit.Remove(SelectedPdfFile);
        }

        private void CheckAnyFileLoaded()
        {
            if (PdfFiles.Any() && !ImageFiles.Any())
            {
                SelectedTabIndex = 0;
            }
            else if (!PdfFiles.Any() && ImageFiles.Any())
            {
                SelectedTabIndex = 3;
            }
            AnyLoadedFile = PdfFiles.Any() || ImageFiles.Any();
        }

        private void LoadPdfFile(ref int protectedFilesCount, ref int damagedFilesCount, File file)
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

        private void DrawSplitLine(EDirection direction, bool delete = false)
        {
            if (SelectedPdfToSplitImage == null)
            {
                return;
            }
            var selectedImageIndex = PdfToSplitImages.IndexOf((PdfImageContainer)SelectedPdfToSplitImage);
            var originSplitDirection = PdfToSplitImages[selectedImageIndex].SplitDirection;
            UpdateImageOnGivenIndex(direction, delete, selectedImageIndex);

            if (delete)
            {
                if (originSplitDirection == EDirection.Left || originSplitDirection == EDirection.Both)
                {
                    UpdateImageOnGivenIndex(EDirection.Right, delete, selectedImageIndex - 1);
                }
                if (originSplitDirection == EDirection.Right || originSplitDirection == EDirection.Both)
                {
                    UpdateImageOnGivenIndex(EDirection.Left, delete, selectedImageIndex + 1);
                }
            }
            else
            {
                var adjacentImageIndex = direction == EDirection.Left ? selectedImageIndex - 1 : selectedImageIndex + 1;
                UpdateImageOnGivenIndex(direction.Opposite(), delete, adjacentImageIndex);
            }
        }

        private void UpdateImageOnGivenIndex(EDirection direction, bool delete, int index)
        {
            if (index >= 0 && index < PdfToSplitImages.Count)
            {
                var image = PdfToSplitImages[index];
                PdfToSplitImages.RemoveAt(index);
                image = ChangeSplitVisibility(direction, delete, image);
                PdfToSplitImages.Insert(index, image);
            }
        }

        private static PdfImageContainer ChangeSplitVisibility(EDirection direction, bool delete, PdfImageContainer image)
        {
            if (delete)
            {
                switch (direction)
                {
                    case EDirection.Left:
                        image.IsVerticalSplitLineLeftVisible = false;
                        break;
                    case EDirection.Right:
                        image.IsVerticalSplitLineRightVisible = false;
                        break;
                    case EDirection.Both:
                        image.IsVerticalSplitLineLeftVisible = false;
                        image.IsVerticalSplitLineRightVisible = false;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (direction)
                {
                    case EDirection.Left:
                        image.IsVerticalSplitLineLeftVisible = true;
                        image.SplitDirection = EDirection.Left;
                        break;
                    case EDirection.Right:
                        image.IsVerticalSplitLineRightVisible = true;
                        image.SplitDirection = EDirection.Right;
                        break;
                    case EDirection.Both:
                        image.IsVerticalSplitLineLeftVisible = true;
                        image.IsVerticalSplitLineRightVisible = true;
                        image.SplitDirection = EDirection.Both;
                        break;
                    default:
                        break;
                }
            }
            if (image.IsVerticalSplitLineLeftVisible &&  image.IsVerticalSplitLineRightVisible)
            {
                image.SplitDirection = EDirection.Both;
            }

            return image;
        }

        private void UpdateSplitOutcomeFiles()
        {
            var splitResultFiles = new List<PdfRangeFile>();
            var indexes = new Dictionary<int, bool>() { { 0, true } };
            var index = 0;
            foreach (var imageFile in PdfToSplitImages)
            {
                if (imageFile.IsVerticalSplitLineLeftVisible)
                {
                    if (!indexes.ContainsKey(index - 1))
                    {
                        indexes.Add(index - 1, true);
                    }
                }
                if (imageFile.IsVerticalSplitLineRightVisible)
                {
                    if (!indexes.ContainsKey(index + 1))
                    {
                        indexes.Add(index + 1, true);
                    }
                }
                index++;
            }
            indexes.Add(PdfToSplitImages.Count() - 1, true);

            var statusToggle = false;
            index = 0;
            foreach (var to in indexes.Skip(1))
            {
                var from = indexes.ElementAt(index);
                splitResultFiles.Add(new PdfRangeFile(from.Key, to.Key, $"From{from.Key}_To{to.Key}.pdf"));
                index++;
            }
            index = 0;
        }

        private void LoadImageFile(File imageFile)
        {
            ImageFiles.Add(new ImageFile(imageFile));
            SelectedImageFile = ImageFiles.Last();
        }

        private void CheckNameConflicts(Models.File file)
        {
            var index = 1;
            while (PdfFiles.Any(x => x.Name == file.Name))
            {
                file.Rename(file.Name+$"_{index}");
                index++;
            }                
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
            CheckAnyFileLoaded();
        }
    }
}
