using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Interfaces.Files;
using CrytonCoreNext.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Wpf.Ui.Common;
using Wpf.Ui.Mvvm.Contracts;
using IDialogService = CrytonCoreNext.Interfaces.IDialogService;

namespace CrytonCoreNext.ViewModels
{
    public partial class CryptingViewModel : InteractiveViewBase
    {
        private readonly ICryptingService _cryptingService;

        private readonly IFileService _fileService;

        private readonly List<ICryptingView<CryptingMethodViewModel>> _cryptingViews;

        [ObservableProperty]
        public IProgressService progressService;

        [ObservableProperty]
        public ObservableCollection<ICryptingView<CryptingMethodViewModel>> cryptingViewsItemSource;

        [ObservableProperty]
        public ICryptingView<CryptingMethodViewModel> selectedCryptingView;

        [ObservableProperty]
        public ObservableCollection<CryptFile> files;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Files))]
        public CryptFile selectedFile;

        public delegate void HandleFileChanged(CryptFile file);

        public event HandleFileChanged OnFileChanged;

        public CryptingViewModel(IFileService fileService,
            IDialogService dialogService,
            ICryptingService cryptingService,
            ISnackbarService snackbarService,
            List<ICryptingView<CryptingMethodViewModel>> cryptingViews)
            : base(fileService, dialogService, snackbarService)
        {
            ProgressService = new ProgressService();

            _fileService = fileService;
            _cryptingService = cryptingService;
            _cryptingViews = cryptingViews;

            files = new();

            UpdateCryptingViews();
            RegisterFileChangedEvent();

            SelectedCryptingView = CryptingViewsItemSource.First();
        }

        private void RegisterFileChangedEvent()
        {
            foreach (var view in _cryptingViews)
            {
                OnFileChanged += view.ViewModel.HandleFileChanged;
            }
        }

        private void UpdateCryptingViews()
        {
            CryptingViewsItemSource = new(_cryptingViews);
        }

        partial void OnSelectedFileChanged(CryptFile value)
        {
            OnFileChanged.Invoke(value);
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
            await foreach (var file in LoadFiles())
            {
                Files.Add(_cryptingService.ReadCryptFile(file));
                SelectedFile = Files.Last();
            }
            if (SelectedFile == null && Files.Any())
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


        [RelayCommand]
        private async void PerformCrypting()
        {
            Lock();
            if (!_fileService.HasBytes(SelectedFile) || !_cryptingService.IsCorrectMethod(SelectedFile, SelectedCryptingView))
            {
                PostErrorSnackbar(Language.Post("WrongMethod"));
                return;
            }

            var progressReport = ProgressService.SetProgress<string>(SelectedCryptingView.ViewModel.Crypting.ProgressCount);
            var result = await _cryptingService.RunCrypting(SelectedCryptingView.ViewModel.Crypting, SelectedFile, progressReport);

            if (!result.Equals(Array.Empty<byte>()) && Files.Any())
            {
                _cryptingService.ModifyFile(SelectedFile, result, _cryptingService.GetOpositeStatus(SelectedFile.Status), SelectedCryptingView.ViewModel.Crypting.Method);
                UpdateStateOfSelectedFile();
                PostSuccessSnackbar(Language.Post("Success"));
            }
            else
            {
                PostErrorSnackbar(Language.Post("CryptingError"));
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
    }
}
