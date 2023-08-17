using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Crypting.Cryptors;
using CrytonCoreNext.Crypting.Helpers;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Drawers;
using CrytonCoreNext.Enums;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Interfaces.Files;
using CrytonCoreNext.Properties;
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

        private readonly IPasswordProvider _passwordProvider;

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

        [ObservableProperty]
        public string filePassword;

        [ObservableProperty]
        public EStrength? passwordStrenght;

        public delegate void HandleFileChanged(CryptFile file);

        public event HandleFileChanged OnFileChanged;


        public CryptingViewModel(IFileService fileService,
            IDialogService dialogService,
            ICryptingService cryptingService,
            ISnackbarService snackbarService,
            List<ICryptingView<CryptingMethodViewModel>> cryptingViews,
            IPasswordProvider passwordProvider)
            : base(fileService, dialogService, snackbarService)
        {
            ColorGradientGenerator.GenerateGradientImage(new OpenCvSharp.Size(200, 200), new OpenCvSharp.Scalar(255, 100, 100), new OpenCvSharp.Scalar(255, 100, 0));
            ProgressService = new ProgressService();

            _fileService = fileService;
            _cryptingService = cryptingService;
            _cryptingViews = cryptingViews;
            _passwordProvider = passwordProvider;

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

        partial void OnFilePasswordChanged(string value)
        {
            var newFilePassword = _passwordProvider.SetPassword(value);
            var result = _passwordProvider.GetPasswordStrenght();
            PasswordStrenght = result > EStrength.None ? result : null;
            FilePassword = newFilePassword;
        } 

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
            ProgressService.ClearProgress();
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
            SetFileSuffix();
            base.SaveFile(SelectedFile);
        }

        private void SetFileSuffix()
        {
            var suffix = SelectedFile.Status == Static.CryptingStatus.Status.Encrypted ? Settings.Default.EncryptionSuffix : Settings.Default.DecryptionSuffix;
            if (SelectedFile.Name.EndsWith(Settings.Default.EncryptionSuffix) || SelectedFile.Name.EndsWith(Settings.Default.DecryptionSuffix) && SelectedFile.Name.Length > 3)
            {
                var newFileName = SelectedFile.Name[..^suffix.Length] + suffix;
                SelectedFile.Rename(newFileName);
            }
        }

        [RelayCommand]
        private async void PerformCrypting()
        {
            Lock();
            if (
                !_fileService.HasBytes(SelectedFile) || 
                !_passwordProvider.ValidatePassword() ||
                !_cryptingService.IsCorrectMethod(SelectedFile, SelectedCryptingView))
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
