using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Drawers;
using CrytonCoreNext.Enums;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Interfaces.Files;
using CrytonCoreNext.Properties;
using CrytonCoreNext.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Wpf.Ui;


namespace CrytonCoreNext.ViewModels
{
    public partial class CryptingViewModel : InteractiveViewBase
    {
        private readonly ICryptingService _cryptingService;

        private readonly IPasswordProvider _passwordProvider;

        private readonly IFileService _fileService;

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

        [ObservableProperty]
        public EStrength? minimalPasswordStrenght;

        [ObservableProperty]
        public WriteableBitmap gradientColorMat;

        public delegate void HandleFileChanged(CryptFile file);

        public event HandleFileChanged OnFileChanged;


        public CryptingViewModel(IFileService fileService,
            ICryptingService cryptingService,
            ISnackbarService snackbarService,
            List<ICryptingView<CryptingMethodViewModel>> cryptingViews,
            IPasswordProvider passwordProvider,
            DialogService dialogService)
            : base(fileService, snackbarService, dialogService)
        {
            ProgressService = new ProgressService();

            _fileService = fileService;
            _cryptingService = cryptingService;
            _passwordProvider = passwordProvider;
            files = [];

            CryptingViewsItemSource = new(cryptingViews);
            RegisterFileChangedEvent();

            MinimalPasswordStrenght = _passwordProvider.GetPasswordValidationStrength();
            SelectedCryptingView = CryptingViewsItemSource.First();
        }

        private void RegisterFileChangedEvent()
        {
            foreach (var view in CryptingViewsItemSource)
            {
                OnFileChanged += view.ViewModel.HandleFileChanged;
            }
        }

        partial void OnSelectedFileChanged(CryptFile value)
        {
            OnFileChanged.Invoke(value);
        }

        partial void OnFilePasswordChanged(string value)
        {
            MinimalPasswordStrenght = _passwordProvider.GetPasswordValidationStrength();
            var newFilePassword = _passwordProvider.SetPassword(value);
            var result = _passwordProvider.GetPasswordStrenght();
            PasswordStrenght = result > EStrength.None ? result : null;
            if (PasswordStrenght != null)
            {
                var bStartValue = 45;
                var gStartValue = 80;
                var rStartValue = 95;
                var aStartValue = 0;
                var bEndValue = 61;
                var gEndValue = 202;
                var rEndValue = 255;
                var aEndValue = 255;
                var bStep = (bEndValue - bStartValue) / 6;
                var gStep = (gEndValue - gStartValue) / 6;
                var rStep = (rEndValue - rStartValue) / 6;
                GradientColorMat = ColorGradientGenerator.GenerateGradient(
                    new OpenCvSharp.Size(400, 20),
                    new OpenCvSharp.Scalar(
                        bStartValue,
                        gStartValue,
                        rStartValue,
                        aStartValue),
                    new OpenCvSharp.Scalar(
                        bStartValue + (bStep * (int)PasswordStrenght),
                        gStartValue + (gStep * (int)PasswordStrenght),
                        rStartValue + (rStep * (int)PasswordStrenght),
                        aEndValue));
            }
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
            SaveFile(SelectedFile);
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
        private async Task PerformCrypting()
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
                SelectedFile.Recognition.SetKeys(((ICryptingViewModel)SelectedCryptingView.ViewModel).ExportObjects());
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
