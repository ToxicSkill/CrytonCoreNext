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
using CrytonCoreNext.Services;
using iText.Kernel.Pdf;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Wpf.Ui.Mvvm.Contracts;
using File = CrytonCoreNext.Models.File;

namespace CrytonCoreNext.ViewModels
{
    public partial class PdfViewModel : InteractiveViewBase
    {

        private readonly SemaphoreSlim _semaphore;

        private readonly IPDFManager _pdfManager;

        private readonly IPDFReader _pdfReader;

        private readonly IPDFImageLoader _imageLoader;

        private CancellationTokenSource _asyncImageLoadingCalncelationToken;

        private List<(int pdfIndex, int pdfPage)> _pdfToMergePagesIndexes;

        private List<(int pdfIndex, int pdfPage)> _pdfExcludedMergeIndexes;

        private int _currentPdfToMergeImageIndex = 0;

        private object _locker;

        [ObservableProperty]
        public string webViewSource;

        [ObservableProperty]
        public string webViewSourceFileName;

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
        public PDFFile openedPdfSelectedFile;

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

        [ObservableProperty]
        public string loadedViewFile;

        [ObservableProperty]
        public List<string> availableEncryptionOptions;

        [ObservableProperty]
        public List<string> availablePermissionsOptions;

        [ObservableProperty]
        public string selectedPermissionOption;

        [ObservableProperty]
        public string selectedEncryptionOption;

        public PdfViewModel(IPDFManager pdfManager,
            IPDFReader pdfReader,
            IPDFImageLoader pdfImageLoader,
            IFileService fileService,
            ISnackbarService snackbarService,
            DialogService dialogService) : base(fileService, snackbarService, dialogService)
        {
            _pdfManager = pdfManager;
            _pdfReader = pdfReader;
            _imageLoader = pdfImageLoader;
            _pdfToMergePagesIndexes = [];

            ImageFiles = [];
            PdfToProtectFiles = [];
            OutcomeFilesFromSplit = [];
            SelectedPdfFilesToMerge = [];
            SelectedPdfFilesToSplit = [];
            OpenedPdfFiles = [];
            PdfToSplitImages = [];
            PdfToSplitRangeFiles = [];
            PdfFiles = [];

            InitializeComboBoxes();

            _pdfExcludedMergeIndexes = [];
            _locker = new object();
            _asyncImageLoadingCalncelationToken = new();
            _semaphore = new SemaphoreSlim(1, 1);

            BindingOperations.EnableCollectionSynchronization(PdfFiles, _locker);
        }

        private void OnPdfFilesChanged()
        {
            OpenedPdfFiles = new(PdfFiles.Where(x => x.IsOpened).ToList());
            PdfToProtectFiles = new(OpenedPdfFiles.Where(x => x.HasPassword == false).ToList());
            var splitFilesToRemove = new List<PDFFile>();
            var mergeFilesToRemove = new List<PDFFile>();
            foreach (var splitFile in SelectedPdfFilesToSplit)
            {
                if (!OpenedPdfFiles.Contains(splitFile))
                {
                    splitFilesToRemove.Add(splitFile);
                }
            }
            foreach (var mergeFile in SelectedPdfFilesToMerge)
            {
                if (!OpenedPdfFiles.Contains(mergeFile))
                {
                    mergeFilesToRemove.Add(mergeFile);
                }
            }
            SelectedPdfFilesToSplit = new(SelectedPdfFilesToSplit.Except(splitFilesToRemove));
            SelectedPdfFilesToMerge = new(SelectedPdfFilesToMerge.Except(mergeFilesToRemove));
            if (SelectedPdfFile == null && PdfFiles.Any())
            {
                SelectedPdfFile = PdfFiles.Last();
            }
            if (SelectedPdfFileToMerge == null && SelectedPdfFilesToMerge.Any())
            {
                SelectedPdfFileToMerge = SelectedPdfFilesToMerge.Last();
            }
            if (SelectedPdfFileToSplit == null && SelectedPdfFilesToSplit.Any())
            {
                SelectedPdfFileToSplit = SelectedPdfFilesToSplit.Last();
            }
            if (!PdfFiles.Any() && ImageFiles.Any())
            {
                SelectedTabIndex = (int)EPdfTabControls.Convert;
            }
            AnyLoadedFile = PdfFiles.Any() || ImageFiles.Any();
        }

        public bool ExportImageToPDF(File file, Mat image)
        {
            return LoadImageFile(file, image);
        }

        private void InitializeComboBoxes()
        {
            AvailablePermissionsOptions = _pdfManager.GetAvailableEncryptionAllowOptions();
            AvailableEncryptionOptions = _pdfManager.GetAvailableEncryptionOptions();
            SelectedPermissionOption = AvailablePermissionsOptions.Single(x => x.Equals(nameof(EncryptionConstants.ALLOW_COPY)));
            SelectedEncryptionOption = AvailableEncryptionOptions.Single(x => x.Equals(nameof(EncryptionConstants.ENCRYPTION_AES_256)));
        }

        public void SetPdfPassword(string password)
        {
            SelectedPdfFile.Password = password;
        }

        [RelayCommand]
        private void LoadViewFile()
        {
            Lock();
            WebViewSource = GetFileFromDialog(Static.Extensions.DialogFilters.Pdf);
            WebViewSourceFileName = Path.GetFileName(WebViewSource);
            Unlock();
        }

        [RelayCommand]
        private void AddFileToMergeList()
        {
            if (!SelectedPdfFilesToMerge.Contains(SelectedPdfFile))
            {
                SelectedPdfFilesToMerge.Add(SelectedPdfFile);
                UpdatePdfToMergeImage();
            }
        }

        [RelayCommand]
        private async Task AddFileToSplitList()
        {
            if (!SelectedPdfFilesToSplit.Any() && OpenedPdfSelectedFile.NumberOfPages > 1)
            {
                SelectedPdfFilesToSplit.Add(OpenedPdfSelectedFile);
                SelectedPdfFileToSplit = SelectedPdfFilesToSplit.First();
                var index = 0;
                PdfToSplitImages = [];
                await _semaphore.WaitAsync();
                await foreach (var bitmap in _imageLoader.LoadImages(SelectedPdfFileToSplit))
                {
                    index++;
                    if (_asyncImageLoadingCalncelationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    await System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        PdfToSplitImages.Add(new PdfImageContainer(index, bitmap));
                    });
                }
                if (!_asyncImageLoadingCalncelationToken.IsCancellationRequested)
                {
                    SelectedPdfToSplitImage = PdfToSplitImages.First();
                }
                else
                {
                    _asyncImageLoadingCalncelationToken = new();
                }
                _semaphore.Release();
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
            var permissions = typeof(EncryptionConstants).GetField(SelectedPermissionOption).GetValue(null);
            var encryption = typeof(EncryptionConstants).GetField(SelectedEncryptionOption).GetValue(null);
            if (permissions != null && encryption != null && PdfToProtectSelectedFile != null)
            {
                if (_pdfManager.ProtectFile(PdfToProtectSelectedFile, (int)permissions, (int)encryption))
                {
                    OnPdfFilesChanged();
                    PostSuccessSnackbar("Pdf file has been protected successfully");
                }
                else
                {
                    PostWarningSnackbar("Error during protecting PDF, try other settings or password");
                }
            }
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
                PdfToSplitImages = [];
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
                var pdfFile = await _pdfManager.Split(SelectedPdfFileToSplit, from, to, OpenedPdfFiles.Max(x => x.Id) + idAdd);
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
            var pdfFile = await _pdfManager.Merge(SelectedPdfFilesToMerge.ToList());
            if (AddPdfToPdfList(pdfFile))
            {
                PostSuccessSnackbar($"Merged {SelectedPdfFilesToMerge.Count} files into new: {pdfFile.Name}");
            }
        }

        [RelayCommand]
        private async Task MergeAllImagesToPdf()
        {
            var mergedImagesPdf = await _pdfManager.MergeAllImagesToPDF(ImageFiles.ToList(), ImageFiles.Max(x => x.Id) + 1);
            if (AddPdfToPdfList(mergedImagesPdf))
            {
                PostSuccessSnackbar($"Merged {ImageFiles.Count} files into PDF: {mergedImagesPdf.Name}");
            }
        }

        [RelayCommand]
        private void ConvertSelectedImageToPdf()
        {
            var convertedPdf = _pdfManager.ImageToPdf(SelectedImageFile, ImageFiles.Max(x => x.Id) + 1);
            if (AddPdfToPdfList(convertedPdf))
            {
                if (PdfFiles.Count == 1)
                {
                    SelectedTabIndex = 0;
                }
                PostSuccessSnackbar($"Converted image into PDF: {convertedPdf.Name}");
            }
        }

        [RelayCommand]
        private void DeleteFile()
        {
            PdfFiles.Remove(SelectedPdfFile);
            OnPdfFilesChanged();
        }

        [RelayCommand]
        private void DeleteImageFile()
        {
            var oldIndex = ImageFiles.IndexOf(SelectedImageFile);
            ImageFiles.Remove(SelectedImageFile);
            OnImageFileDeleteAfter(oldIndex);
        }


        [RelayCommand]
        private async Task LoadPdfAndImageFiles(string param)
        {
            Lock();
            var protectedFilesCount = 0;
            var damagedFilesCount = 0;
            var nofNotLoadedFiles = 0;
            var nofFilesBefore = PdfFiles.Count + ImageFiles.Count;
            var withImages = param.ToLowerInvariant() == true.ToString().ToLowerInvariant();
            await foreach (var file in LoadFiles(withImages ? Static.Extensions.DialogFilters.PdfAndImages : Static.Extensions.DialogFilters.Pdf))
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
            Unlock();
        }

        private void PostInformations(int protectedFilesCount, int damagedFilesCount, int nofNotLoadedFiles, int nofLoadedFiles)
        {
            var message = "";
            if (protectedFilesCount > 0)
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
            if (nofLoadedFiles == 0 && damagedFilesCount == 0 && protectedFilesCount == 0 && nofNotLoadedFiles == 0)
            {
                return;
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
            var oldPageIndex = SelectedPdfFile.LastPage;
            if (SelectedPdfFile == null)
            {
                return;
            }
            if (SelectedPdfFile.NumberOfPages > 0 && SelectedPdfFile.LastPage > 0)
            {
                SelectedPdfFile.LastPage -= 1;
            }
            if (oldPageIndex == SelectedPdfFile.LastPage)
            {
                return;

            }
            SelectedPdfFile.PageImage = _imageLoader.LoadImage(SelectedPdfFile);
            OnPropertyChanged(nameof(SelectedPdfFile));
        }

        [RelayCommand]
        private void GoNextPage()
        {
            var oldPageIndex = SelectedPdfFile.LastPage;
            if (SelectedPdfFile == null)
            {
                return;
            }
            if (SelectedPdfFile.NumberOfPages > 0 && SelectedPdfFile.LastPage < SelectedPdfFile.NumberOfPages - 1)
            {
                SelectedPdfFile.LastPage += 1;
            }
            if (oldPageIndex == SelectedPdfFile.LastPage)
            {
                return;

            }
            SelectedPdfFile.PageImage = _imageLoader.LoadImage(SelectedPdfFile);
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
                _pdfReader.OpenProtectedPdf(ref pdfFile);
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
                PdfToMergeImage = default;
            }
            else
            {
                var pdfFile = SelectedPdfFilesToMerge[_pdfToMergePagesIndexes[_currentPdfToMergeImageIndex].pdfIndex];
                pdfFile.LastPage = _pdfToMergePagesIndexes[_currentPdfToMergeImageIndex].pdfPage;
                PdfToMergeImage = _imageLoader.LoadImage(pdfFile);
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

        partial void OnSelectedPdfFileChanged(PDFFile value)
        {
            if (value != null)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    value.PageImage = _imageLoader.LoadImage(value);
                });
            }
        }

        partial void OnSelectedPdfFileToSplitChanged(PDFFile value)
        {
            if (value == null)
            {
                _asyncImageLoadingCalncelationToken.Cancel();
            }
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


        private void OnImageFileDeleteAfter(int oldIndex)
        {
            if (ImageFiles.Any())
            {
                SelectedImageFile = ImageFiles.ElementAt(oldIndex > 0 ? --oldIndex : oldIndex);
            }
            OnPdfFilesChanged();
        }

        private void OnFileDeleteBefore()
        {
            SelectedPdfFilesToMerge.Remove(SelectedPdfFile);
            SelectedPdfFilesToSplit.Remove(SelectedPdfFile);
        }


        private int LoadPdfFile(ref int protectedFilesCount, ref int damagedFilesCount, File file)
        {
            lock (_locker)
            {
                var pdfFile = _pdfReader.ReadPdf(file);
                if (pdfFile != null)
                {
                    if (!PdfFiles.Contains(pdfFile, new FileComparer()))
                    {
                        PdfFiles.Add(pdfFile);
                        OnPdfFilesChanged();
                    }
                    else
                    {
                        return 1;
                    }
                }
                if (pdfFile!.PdfStatus == EPdfStatus.Protected)
                {
                    protectedFilesCount++;
                }
                if (pdfFile.PdfStatus == EPdfStatus.Damaged)
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
            if (image.IsVerticalSplitLineLeftVisible && image.IsVerticalSplitLineRightVisible)
            {
                image.SplitDirection = EDirection.Both;
            }

            return image;
        }

        private void UpdateSplitOutcomeFiles()
        {
            var splitResultFiles = new List<PdfRangeFile>();
            var indexes = new List<(int from, int to)>();
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
                PdfToSplitRangeFiles = [];
            }
        }

        private static string GetSubPDFFileName((int from, int to) indx)
        {
            return indx.from == indx.to ? $"Only{indx.from}" : $"From{indx.from}_To{indx.to}.pdf";
        }

        private bool LoadImageFile(File file, Mat image)
        {
            var imageFile = new ImageFile(file, image);
            if (imageFile != null)
            {
                ImageFiles.Add(imageFile);
                SelectedImageFile = ImageFiles.Last();
                OnPdfFilesChanged();
                return true;
            }
            return false;
        }

        private int LoadImageFile(File imageFile)
        {
            if (!ImageFiles.Contains(imageFile, new FileComparer()))
            {
                ImageFiles.Add(new ImageFile(imageFile));
                SelectedImageFile = ImageFiles.Last();
                OnPdfFilesChanged();
                return 0;
            }
            return 1;
        }

        private void CheckNameConflicts(File file)
        {
            var index = 1;
            while (PdfFiles.Any(x => x.Name == file.Name))
            {
                file.Rename(file.Name + $"_{index}");
                index++;
            }
        }

        private void UpdateProtectedPdf()
        {
            _pdfReader.OpenProtectedPdf(ref selectedPdfFile);
            OnSelectedPdfFileChanged(SelectedPdfFile);
            if (SelectedPdfFile.PdfStatus == EPdfStatus.Protected ||
                SelectedPdfFile.PdfStatus == EPdfStatus.Damaged)
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
            OnPdfFilesChanged();
        }
    }
}
