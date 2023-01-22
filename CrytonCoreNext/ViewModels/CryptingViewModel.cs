using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Services;
using CrytonCoreNext.Static;
using System;
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

        private readonly IFileService _fileService;

        [ObservableProperty]
        public IProgressService progressService;

        [ObservableProperty]
        public ObservableCollection<ICrypting> cryptingMethods;

        [ObservableProperty]
        public ICrypting selectedCryptingMethod;

        [ObservableProperty]
        public ObservableCollection<CryptFile> files;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Files))]
        public CryptFile selectedFile;

        [ObservableProperty]
        public INavigableView<ViewModelBase> currentCryptingViewModel;


        public CryptingViewModel(IFileService fileService, IDialogService dialogService, ICryptingService cryptingService, IFilesView filesView, ISnackbarService snackbarService)
            : base(fileService, dialogService, snackbarService)
        {
            ProgressService = new ProgressService();
            _fileService = fileService;
            _cryptingService = cryptingService;
            files = new ObservableCollection<CryptFile>();

            InitializeCryptingComboBox();
        }

        partial void OnSelectedFileChanged(CryptFile value)
        {
            CurrentCryptingViewModel = _cryptingService.GetCurrentCrypting().GetViewModel();
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

        //private void HandleSelectedFileChanged(object? sender, EventArgs? e)
        //{
        //    var file = _files.FirstOrDefault(x => x?.Guid == FilesViewModel.GetSelectedFileGuid());
        //    if (file != null)
        //    {
        //        SelectedFile = file;
        //        OnPropertyChanged(nameof(SelectedFile));
        //        OnPropertyChanged(nameof(CryptButtonName));
        //    }
        //}


        [RelayCommand]
        private void ClearFiles()
        {
            Files.Clear();
        }

        [RelayCommand]
        private void DeleteFile()
        {
            var oldIndex = Files.IndexOf(SelectedFile);
            Files.Remove(SelectedFile);
            if (Files.Any())
            {
                SelectedFile = Files.ElementAt(oldIndex > 0 ? --oldIndex : oldIndex);
            }
        }

        [RelayCommand]
        private async Task LoadCryptFiles()
        {
            Lock();
            await foreach (var file in base.LoadFiles())
            {
                Files.Add(_cryptingService.ReadCryptFile(file));
            }
            if (SelectedFile == null)
            {
                SelectedFile = Files.First();
            }
            Unlock();
        }

        [RelayCommand]
        private void SaveCryptFile()
        {
            _cryptingService.AddRecognitionBytes(SelectedFile);
            base.SaveFile(SelectedFile);
        }

        private void InitializeCryptingComboBox()
        {
            CryptingMethods = new(_cryptingService.GetCryptors());
            SelectedCryptingMethod = CryptingMethods.First();
            OnPropertyChanged(nameof(SelectedCryptingMethod));
        }


        [RelayCommand]
        private async void PerformCrypting()
        {
            Lock();
            if (!_fileService.HasBytes(SelectedFile) || IsCorrectMethod())
            {
                PostSnackbar("Error", Language.Post("WrongMethod"), SymbolRegular.ErrorCircle20, ControlAppearance.Danger);
                return;
            }

            var progressReport = ProgressService.SetProgress<string>(_cryptingService.GetCurrentCryptingProgressCount());
            var result = await _cryptingService.RunCrypting(SelectedFile, progressReport);

            if (!result.Equals(Array.Empty<byte>()) && Files.Any())
            {
                _cryptingService.ModifyFile(SelectedFile, result, GetOpositeStatus(SelectedFile.Status), _cryptingService.GetCurrentCrypting().GetName());
                UpdateStateOfSelectedFile();
                PostSnackbar("Success", Language.Post("Success"), SymbolRegular.Checkmark20, ControlAppearance.Success);
            }
            else
            {
                PostSnackbar("Error", Language.Post("CryptingError"), SymbolRegular.ErrorCircle20, ControlAppearance.Danger);
            }
            Unlock();
        }

        private void UpdateStateOfSelectedFile()
        {
            var oldIndex = Files.IndexOf(SelectedFile);
            var temp = SelectedFile;
            Files.RemoveAt(oldIndex);
            Files.Insert(oldIndex, temp);
            SelectedFile = temp;
            OnPropertyChanged(nameof(Files));
            OnPropertyChanged(nameof(SelectedFile));
        }

        private bool IsCorrectMethod()
        {
            return SelectedFile.Status == CryptingStatus.Status.Encrypted &&
                CurrentCryptingViewModel.ViewModel.PageName != SelectedFile.Method;
        }

        private static CryptingStatus.Status GetOpositeStatus(CryptingStatus.Status currentStatus)
        {
            return currentStatus.Equals(CryptingStatus.Status.Decrypted) ?
                CryptingStatus.Status.Encrypted :
                CryptingStatus.Status.Decrypted;
        }
    }
}
