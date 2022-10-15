using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Interfaces;
using System.Collections.ObjectModel;

namespace CrytonCoreNext.ViewModels
{
    public class CryptingViewModel : InteractiveViewBase
    {
        private const int ReportDelay = 2000;

        private readonly ICryptingService _cryptingService;

        private int _progressCounter = 0;

        public ICommand LoadFilesCommand { get; init; }

        public ICommand SaveFileCommand { get; init; }

        public ICommand CryptCommand { get; init; }

        public ObservableCollection<ICrypting> CryptingComboBox { get; private set; }

        public string CryptButtonName => GetCryptName();

        public string ProgessMessage { get; set; } = string.Empty;

        public Visibility ProgressVisibility { get; set; } = Visibility.Hidden;

        public Visibility PreparingVisibility { get; set; }

        public Visibility StartingVisibility { get; set; }

        public Visibility FinishedVisibility { get; set; }

        public Visibility UpdatingVisibility { get; set; }

        public Visibility SuccessVisibility { get; set; }

        public ICrypting CurrentCryptingName
        {
            get => _cryptingService.GetCurrentCrypting();
            set
            {
                if (value != _cryptingService.GetCurrentCrypting())
                {
                    _cryptingService.SetCurrentCrypting(value);
                    UpdateCurrentCrypting();
                    OnPropertyChanged(nameof(CurrentCryptingName));
                }
            }
        }

        public ViewModelBase CurrentCryptingViewModel { get; private set; }

        public CryptingViewModel(IFileService fileService, IDialogService dialogService, ICryptingService cryptingService) : base(fileService, dialogService)
        {
            _cryptingService = cryptingService;
            CurrentCryptingViewModel = new();
            CryptingComboBox = new();
            CryptCommand = new Command(DoCrypt, true);
            LoadFilesCommand = new Command(LoadFiles, true);
            SaveFileCommand = new Command(SaveFile, true);
            InitializeCryptingComboBox();
            FilesViewViewModel.FilesChanged += HandleFileChanged;
            ResetProgressVisibility();
            UpdateCurrentCrypting();

            //var t = new Crypting.Crypting(new() { new(new AES(), ECrypting.EnumToString(ECrypting.Methods.aes)) });
            //t.Encrypt(FilesViewViewModel.FilesView[0].Bytes, ECrypting.EnumToString(ECrypting.Methods.aes));
        }

        private new void HandleFileChanged(object? sender, EventArgs? e)
        {
            OnPropertyChanged(nameof(CryptButtonName));
        }

        private void InitializeCryptingComboBox()
        {
            CryptingComboBox = new (_cryptingService.GetCryptors().ToList());
        }

        private void UpdateCurrentCrypting()
        {
            CurrentCryptingViewModel = _cryptingService.GetCurrentCrypting().GetViewModel();
            OnPropertyChanged(nameof(CurrentCryptingViewModel));
        }

        private async void DoCrypt()
        {
            ProgressVisibility = Visibility.Visible;
            OnPropertyChanged(nameof(ProgressVisibility));
            var progressReport = new Progress<string>(ReportCryptingProgress);
            if (FilesViewViewModel.CurrentFile != null)
            {
                if (FilesViewViewModel.CurrentFile.Bytes != null)
                {
                    var result = FilesViewViewModel.CurrentFile.Status ?
                        await _cryptingService.GetCurrentCrypting().Decrypt(FilesViewViewModel.CurrentFile.Bytes, progressReport) :
                        await _cryptingService.GetCurrentCrypting().Encrypt(FilesViewViewModel.CurrentFile.Bytes, progressReport);
                    if (result != null && FilesViewViewModel.FilesView != null)
                    {
                        ModifyFile(FilesViewViewModel.FilesView, FilesViewViewModel.CurrentFile.Guid, result, !FilesViewViewModel.CurrentFile.Status, _cryptingService.GetCurrentCrypting()?.GetName());
                        OnPropertyChanged(nameof(CryptButtonName));
                    }
                }
            }
            _ = Task.Delay(ReportDelay).ContinueWith(t => ResetProgressVisibility());
        }

        private void ReportCryptingProgress(string progressMessage)
        {
            ProgessMessage = progressMessage;
            OnPropertyChanged(nameof(ProgessMessage));
            switch (_progressCounter)
            {
                case 0:
                    PreparingVisibility = Visibility.Visible;
                    OnPropertyChanged(nameof(PreparingVisibility));
                    break;
                case 1:
                    StartingVisibility = Visibility.Visible;
                    OnPropertyChanged(nameof(StartingVisibility));
                    break;
                case 2:
                    FinishedVisibility = Visibility.Visible;
                    OnPropertyChanged(nameof(FinishedVisibility));
                    break;
                case 3:
                    UpdatingVisibility = Visibility.Visible;
                    OnPropertyChanged(nameof(UpdatingVisibility));
                    break;
                case 4:
                    SuccessVisibility = Visibility.Visible;
                    OnPropertyChanged(nameof(SuccessVisibility));
                    break;
                default:
                    break;
            }

            _progressCounter += 1; 
        }

        private void ResetProgressVisibility()
        {
            ProgressVisibility = Visibility.Hidden;
            OnPropertyChanged(nameof(ProgressVisibility));
            PreparingVisibility = Visibility.Hidden;
            OnPropertyChanged(nameof(PreparingVisibility));
            StartingVisibility = Visibility.Hidden;
            OnPropertyChanged(nameof(StartingVisibility));
            FinishedVisibility = Visibility.Hidden;
            OnPropertyChanged(nameof(FinishedVisibility));
            UpdatingVisibility = Visibility.Hidden;
            OnPropertyChanged(nameof(UpdatingVisibility));
            SuccessVisibility = Visibility.Hidden;
            OnPropertyChanged(nameof(SuccessVisibility));
            _progressCounter = 0;
        }

        private string GetCryptName()
        {
            if (FilesViewViewModel != null)
            {
                if (FilesViewViewModel.CurrentFile != null)
                {
                    return FilesViewViewModel.CurrentFile.Status ? "Decrypt" : "Encrypt";
                }
            }
            return "Encrypt";
        }
    }
}
