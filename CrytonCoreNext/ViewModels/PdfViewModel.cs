using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Comparer;
using CrytonCoreNext.Enums;
using CrytonCoreNext.Extensions;
using CrytonCoreNext.Interfaces.Files;
using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Enums;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
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


        private object _locker;

        [ObservableProperty]
        public bool anyLoadedFile;

        [ObservableProperty]
        public string pageMergeCountStatus;

        [ObservableProperty]
        public bool hasMoreThanOnePageToMerge; 

        [ObservableProperty]
        public ObservableCollection<PDFFile> pdfFiles;

        [ObservableProperty]
        public ObservableCollection<PDFFile> openedPdfFiles;

        [ObservableProperty]
        public ObservableCollection<PDFFile> selectedPdfFilesToMerge;

        [ObservableProperty]
        public ObservableCollection<PDFFile> selectedPdfFilesToSplit;

        [ObservableProperty]
        public ObservableCollection<PDFFile> pdfToProtectFiles;

        [ObservableProperty]
        public PDFFile pdfToProtectSelectedFile;

        [ObservableProperty]
        public ObservableCollection<PDFFile> outcomeFilesFromSplit;

        [ObservableProperty]
        public ObservableCollection<PdfImageContainer> pdfToSplitImages;

        [ObservableProperty]
        public ObservableCollection<PdfRangeFile> pdfToSplitRangeFiles;

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

            PdfFiles = new();
            ImageFiles = new();
            OpenedPdfFiles = new(); 
            PdfToProtectFiles = new();
            OutcomeFilesFromSplit = new ();
            SelectedPdfFilesToMerge = new();
            SelectedPdfFilesToSplit = new();
            PdfToSplitImages = new();
            PdfToSplitRangeFiles = new();

            _pdfExcludedMergeIndexes = new();
            _locker = new object();

            BindingOperations.EnableCollectionSynchronization(PdfFiles, _locker);
        }

        public void SetPdfPassword(string password)
        {
            SelectedPdfFile.Password = password;
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
        private void ProtectFile()
        {
            _pdfService.ProtectFile(SelectedPdfFile);
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
            var idAdd = 1;
            var nofSplittedFiles = 0;
            var snackbarText = new StringBuilder();
            foreach (var subPdfFile in PdfToSplitRangeFiles)
            {
                if (!subPdfFile.IsSelectedToSplit)
                {
                    continue;
                }
                var from = subPdfFile.From;
                var to = subPdfFile.To;
                var pdfFile = await _pdfService.Split(SelectedPdfFileToSplit, from, to, OpenedPdfFiles.Max(x => x.Id) + idAdd);
                if (AddPdfToPdfList(pdfFile))
                {
                    nofSplittedFiles++;
                    if (from == to)
                    {
                        snackbarText.AppendLine($"{from}   {pdfFile.Name}");
                    }
                    else
                    {
                        snackbarText.AppendLine($"{from} - {to}   {pdfFile.Name}");
                    }
                }
                idAdd++;
            }
            PostSuccessSnackbar($"Successfully splited {nofSplittedFiles} files: \n{snackbarText}");
        }

        [RelayCommand]
        private async Task Merge()
        {
            var pdfFile = await _pdfService.Merge(SelectedPdfFilesToMerge.ToList());
            if (AddPdfToPdfList(pdfFile))
            {
                PostSuccessSnackbar($"Merged {SelectedPdfFilesToMerge.Count} files into new: {pdfFile.Name}");
            }
        }

        [RelayCommand]
        private async Task MergeAllImagesToPdf()
        {
            var mergedImagesPdf = await _pdfService.MergeAllImagesToPDF(ImageFiles.ToList(), ImageFiles.Max(x => x.Id) + 1);
            if (AddPdfToPdfList(mergedImagesPdf))
            {
                PostSuccessSnackbar($"Merged {ImageFiles.Count} files into PDF: {mergedImagesPdf.Name}");
            }
        }

        [RelayCommand]
        private void ConvertSelectedImageToPdf()
        {
            var convertedPdf = _pdfService.ImageToPdf(SelectedImageFile, ImageFiles.Max(x => x.Id) + 1);
            if (AddPdfToPdfList(convertedPdf))
            {
                PostSuccessSnackbar($"Converted image into PDF: {convertedPdf.Name}");
            }
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
        private void DeleteImageFile()
        {
            var oldIndex = ImageFiles.IndexOf(SelectedImageFile);
            ImageFiles.Remove(SelectedImageFile);
            OnImageFileDeleteAfter(oldIndex);
        }
        

        [RelayCommand]
        private async Task LoadPdfAndImageFiles(string? andImages = null)
        {
            Lock();
            var protectedFilesCount = 0;
            var damagedFilesCount = 0;
            var nofNotLoadedFiles = 0;
            var nofFilesBefore = PdfFiles.Count + ImageFiles.Count;
            await foreach (var file in LoadFiles(andImages == null ? Static.Extensions.DialogFilters.PdfAndImages : Static.Extensions.DialogFilters.Pdf))
            {
                if (file.Extension.ToLower().Contains("pdf"))
                {
                    nofNotLoadedFiles += LoadPdfFile(ref protectedFilesCount, ref damagedFilesCount, file);
                }
                else
                {
                    nofNotLoadedFiles += LoadImageFile(file);
                }
            }
            if (SelectedPdfFile == null && PdfFiles.Any())
            {
                SelectedPdfFile = PdfFiles.First();
            }
            var nofFilesAfter = PdfFiles.Count + ImageFiles.Count;
            PostInformations(protectedFilesCount, damagedFilesCount, nofNotLoadedFiles, nofFilesAfter - nofFilesBefore);
            CheckAnyFileLoaded();
            Unlock();
        }

        private void PostInformations(int protectedFilesCount, int damagedFilesCount, int nofNotLoadedFiles, int nofLoadedFiles)
        {
            var message = "";
            if (protectedFilesCount > 0 )
            {
                message += (protectedFilesCount > 1 ?
                    $"{protectedFilesCount} of {protectedFilesCount + PdfFiles.Count - nofNotLoadedFiles} loaded files. " :
                    "One file") + " requires password. \n";
            }
            if (damagedFilesCount > 0)
            {
                message += (protectedFilesCount > 1 ?
                    $"{protectedFilesCount} of {protectedFilesCount + PdfFiles.Count - nofNotLoadedFiles} loaded files. " :
                    "One file") + " is damaged. \n";
            }
            if (nofNotLoadedFiles > 0)
            {
                message += nofNotLoadedFiles == 1 ?
                 "One file is already loaded. \n" :
                 $"At least {nofNotLoadedFiles} files are aleardy loaded or has same content. \n";
            }
            if (message != "")
            {
                PostWarningSnackbar(message);
            }
            else
            {
                PostSuccessSnackbar($"Successfully loaded {nofLoadedFiles} files");
            }
        }

        [RelayCommand]
        private async Task LoadImageFiles()
        {
            Lock();
            var nofNotLoadedFiles = 0;
            var nofFilesBefore = ImageFiles.Count;
            await foreach (var imageFile in LoadFiles(Static.Extensions.DialogFilters.Images))
            {
                nofNotLoadedFiles += LoadImageFile(imageFile);
            }
            PostInformations(0, 0, nofNotLoadedFiles, ImageFiles.Count - nofFilesBefore);
            Unlock();
        }

        [RelayCommand]
        private void SavePdfFile()
        {
            if (SelectedPdfFile == null)
            {
                return;
            }
            SaveFile(SelectedPdfFile);
        }

        [RelayCommand]
        private void ConfirmPassword()
        {
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

        private bool AddPdfToPdfList(PDFFile pdfFile)
        {
            try
            {
                _pdfService.UpdatePdfFileInformations(ref pdfFile);
                CheckNameConflicts(pdfFile);
                PdfFiles.Add(pdfFile);
                return true;
            }
            catch (Exception ex)
            {
                PostErrorSnackbar($"Something went wrong: {ex.Message}");
                return false;
            }
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
            if (SelectedTabIndex != (int)EPdfTabControls.Manage)
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
            if (SelectedTabIndex == (int)EPdfTabControls.Split)
            {
                if (SelectedPdfFileToSplit == null)
                {
                    PdfToSplitImages.Clear();
                    PdfToSplitRangeFiles.Clear();
                    PdfToSplitImages = new();
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
            foreach (var (pdfIndex, pdfPage) in _pdfToMergePagesIndexes)
            {
                if (pdfIndex == selectedPdfIndex)
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

        private void OnImageFileDeleteAfter(int oldIndex)
        {
            if (ImageFiles.Any())
            {
                SelectedImageFile = ImageFiles.ElementAt(oldIndex > 0 ? --oldIndex : oldIndex);
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
            if (!PdfFiles.Any() && ImageFiles.Any())
            {
                SelectedTabIndex = 3;
            }
            else
            {
                SelectedTabIndex = 0;
            }
            AnyLoadedFile = PdfFiles.Any() || ImageFiles.Any();
        }

        private int LoadPdfFile(ref int protectedFilesCount, ref int damagedFilesCount, File file)
        {
            lock(_locker)
            {
                var pdfFile = _pdfService.ReadPdf(file);
                if (pdfFile != null)
                {
                    if (!PdfFiles.Contains(pdfFile, new FileComparer()))
                    {
                        PdfFiles.Add(pdfFile);
                    }
                    else
                    {
                        return 1;
                    }
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
            return 0;
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
            var indexes = new List<(int from, int to) >();
            var index = 0;
            var tempList = new List<int>() { 0 };
            foreach (var imageFile in PdfToSplitImages)
            {
                if (imageFile.IsVerticalSplitLineRightVisible)
                {
                    tempList.Add(index);
                }
                if (tempList.Count == 2) 
                {
                    var lastItem = tempList[1] + 1;
                    indexes.Add((tempList[0], tempList[1]));
                    tempList.Clear();
                    tempList.Add(lastItem);
                }
                index++;
            }
            if (tempList.Count == 1)
            {
                tempList.Add(PdfToSplitImages.Count - 1);
                indexes.Add((tempList[0], tempList[1]));
            }
            foreach (var indx in indexes)
            {
                var rangeFile = new PdfRangeFile(indx.from, indx.to, GetSubPDFFileName(indx));
                if (PdfToSplitRangeFiles.Contains(rangeFile, new PdfRangeFilesComparer()))
                {
                    rangeFile.IsSelectedToSplit = PdfToSplitRangeFiles.Where(x => x.From == indx.from).First().IsSelectedToSplit;
                }
                splitResultFiles.Add(rangeFile);
            }
            if (splitResultFiles.Count > 1)
            {
                PdfToSplitRangeFiles = new(splitResultFiles);
            }
            else
            {
                PdfToSplitRangeFiles = new();
            }
        }

        private static string GetSubPDFFileName((int from, int to) indx)
        {
            return indx.from == indx.to ? $"Only{indx.from}" : $"From{indx.from}_To{indx.to}.pdf";
        }

        private int LoadImageFile(File imageFile)
        {
            if (!ImageFiles.Contains(imageFile, new FileComparer()))
            { 
                ImageFiles.Add(new ImageFile(imageFile));
                SelectedImageFile = ImageFiles.Last();
                return 0;
            }
            return 1;
        }

        private void CheckNameConflicts(File file)
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
                PostWarningSnackbar("Incorrect password");
            }
            else
            {
                PostSuccessSnackbar("Pdf document has been open successfully");
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
