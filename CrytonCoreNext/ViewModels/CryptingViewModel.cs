using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using IDialogService = CrytonCoreNext.Interfaces.IDialogService;

namespace CrytonCoreNext.ViewModels
{
    public partial class CryptingViewModel : InteractiveViewBase
    {
        private readonly ICryptingService _cryptingService;

        [ObservableProperty]
        public ObservableCollection<ICrypting> cryptingMethods;

        [ObservableProperty]
        public ICrypting selectedCryptingMethod;

        [ObservableProperty]
        public ObservableCollection<CryptFile> files;

        [ObservableProperty]
        public CryptFile selectedFile;

        [ObservableProperty]
        public INavigableView<ViewModelBase> currentCryptingViewModel;

        //private List<CryptFile> _files;

        //public CryptFile CurrentFile { get; set; }


        //public ICommand SaveFileCommand { get; init; }

        //public ICommand CryptCommand { get; init; }

        //public ObservableCollection<ICrypting> CryptingComboBox { get; private set; }

        //public ViewModelBase CurrentCryptingViewModel { get; private set; }

        //public string ProgessMessage { get; set; } = string.Empty;

        //public string CryptButtonName => GetCryptName();

        //public ICrypting CurrentCryptingName
        //{
        //    get => _cryptingService.GetCurrentCrypting();
        //    set
        //    {
        //        if (value != _cryptingService.GetCurrentCrypting())
        //        {
        //            _cryptingService.SetCurrentCrypting(value);
        //            UpdateCurrentCrypting();
        //            OnPropertyChanged(nameof(CurrentCryptingName));
        //        }
        //    }
        //}
        public CryptingViewModel(IFileService fileService, IDialogService dialogService, ICryptingService cryptingService, IFilesView filesView, IProgressView progressView, ISnackbarService snackbarService)
            : base(fileService, dialogService, snackbarService)
        {
            //_files = new();
            _cryptingService = cryptingService;
            files = new ObservableCollection<CryptFile>();
            //CurrentCryptingViewModel = new();
            //CryptingComboBox = new();

            //CryptCommand = new Command(PerformCrypting, CanExecute);
            //SaveFileCommand = new Command(SaveCryptFile, CanExecute);

            InitializeCryptingComboBox();
            //UpdateCurrentCrypting();

            //FilesViewModel.CurrentFileChanged += HandleCurrentFileChanged;
            //FilesViewModel.FileDeleted += HandleFileDeleted;
            //FilesViewModel.AllFilesDeleted += HandleAllFilesDeleted;
        }

        partial void OnSelectedFileChanged(CryptFile value)
        {
            CurrentCryptingViewModel = _cryptingService.GetCurrentCrypting().GetViewModel();
            OnPropertyChanged(nameof(CurrentCryptingViewModel));
        }

        partial void OnSelectedCryptingMethodChanged(ICrypting value)
        {
            _cryptingService.SetCurrentCrypting(value);
            CurrentCryptingViewModel = _cryptingService.GetCurrentCrypting().GetViewModel();
        }
        //public override bool CanExecute()
        //{
        //    return !IsBusy && !CurrentCryptingViewModel.IsBusy;
        //}

        //private void HandleAllFilesDeleted(object? sender, EventArgs e)
        //{
        //    _files.Clear();
        //}

        //private void HandleFileDeleted(object? sender, EventArgs e)
        //{
        //    var deletedFileGuid = FilesViewModel.GetDeletedFileGuid();
        //    _files.Remove(_files.Select(x => x).Where(x => x.Guid == deletedFileGuid).First());
        //}

        //private void HandleCurrentFileChanged(object? sender, EventArgs? e)
        //{
        //    var file = _files.FirstOrDefault(x => x?.Guid == FilesViewModel.GetCurrentFileGuid());
        //    if (file != null)
        //    {
        //        CurrentFile = file;
        //        OnPropertyChanged(nameof(CurrentFile));
        //        OnPropertyChanged(nameof(CryptButtonName));
        //    }
        //}

        [RelayCommand]
        private async Task LoadCryptFiles()
        {
            Lock();
            await foreach (var file in base.LoadFiles())
            {
                Files.Add(_cryptingService.ReadCryptFile(file));
            }
            Unlock();
        }

        //private void SaveCryptFile()
        //{
        //    _cryptingService.AddRecognitionBytes(CurrentFile);
        //    base.SaveFile(CurrentFile);
        //}

        private void InitializeCryptingComboBox()
        {
            CryptingMethods = new(_cryptingService.GetCryptors());
            SelectedCryptingMethod = CryptingMethods.First();
            OnPropertyChanged(nameof(SelectedCryptingMethod));
        }

        //private void UpdateCurrentCrypting()
        //{
        //    CurrentCryptingViewModel = _cryptingService.GetCurrentCrypting().GetViewModel();
        //    OnPropertyChanged(nameof(CurrentCryptingViewModel));
        //}

        //private async void PerformCrypting()
        //{
        //    Lock();
        //    if (!_fileService.HasBytes(CurrentFile) || IsCorrectMethod())
        //    {
        //        CurrentCryptingViewModel.Log(ELogLevel.Error, Language.Post("WrongMethod"));
        //        return;
        //    }

        //    var progressReport = ProgressViewModel.InitializeProgress<string>(_cryptingService.GetCurrentCryptingProgressCount());
        //    var result = await _cryptingService.RunCrypting(CurrentFile, progressReport);

        //    if (!result.Equals(Array.Empty<byte>()) && _files.Any())
        //    {
        //        _cryptingService.ModifyFile(CurrentFile, result, GetOpositeStatus(CurrentFile.Status), _cryptingService.GetCurrentCrypting().GetName());
        //        OnPropertyChanged(nameof(CurrentFile));
        //        OnPropertyChanged(nameof(CryptButtonName));
        //    }

        //    ActionTimer.InitializeTimerWithAction(ProgressViewModel.ClearProgress);
        //    Unlock();
        //}

        //private bool IsCorrectMethod()
        //{
        //    return CurrentFile.Status == CryptingStatus.Status.Encrypted &&
        //        CurrentCryptingViewModel.PageName != CurrentFile.Method;
        //}

        //private static CryptingStatus.Status GetOpositeStatus(CryptingStatus.Status currentStatus)
        //{
        //    return currentStatus.Equals(CryptingStatus.Status.Decrypted) ?
        //        CryptingStatus.Status.Encrypted :
        //        CryptingStatus.Status.Decrypted;
        //}

        //private string GetCryptName()
        //{
        //    if (CurrentFile == null)
        //    {
        //        return string.Empty;
        //    }

        //    return GetOpositeStatus(CurrentFile.Status).ToDescription();
        //}
    }
}
