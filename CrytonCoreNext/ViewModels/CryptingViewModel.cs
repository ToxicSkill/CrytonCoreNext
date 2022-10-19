using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Static;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class CryptingViewModel : InteractiveViewBase
    {
        private const int ReportDelay = 2000;


        private readonly ICryptingService _cryptingService;

        public ICommand LoadFilesCommand { get; init; }

        public ICommand SaveFileCommand { get; init; }

        public ICommand CryptCommand { get; init; }

        public ObservableCollection<ICrypting> CryptingComboBox { get; private set; }

        public ViewModelBase CurrentCryptingViewModel { get; private set; }

        public IProgressService ProgressService { get; init; }

        public string ProgessMessage { get; set; } = string.Empty;

        public Visibility ProgressVisibility { get; set; } = Visibility.Hidden;

        public string CryptButtonName => GetCryptName();

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

        public CryptingViewModel(IFileService fileService, IDialogService dialogService, ICryptingService cryptingService, IFilesView filesView, IProgressService progressService) : base(fileService, dialogService, filesView)
        {
            _cryptingService = cryptingService;
            ProgressService = progressService;

            CurrentCryptingViewModel = new();
            CryptingComboBox = new();

            CryptCommand = new Command(PerformCrypting, true);
            LoadFilesCommand = new Command(LoadFiles, true);
            SaveFileCommand = new Command(SaveFile, true);

            InitializeCryptingComboBox();
            UpdateCurrentCrypting();

            FilesView.FilesChanged += HandleFileChanged;

            //var t = new Crypting.Crypting(new() { new(new AES(), ECrypting.EnumToString(ECrypting.Methods.aes)) });
            //t.Encrypt(FilesViewViewModel.FilesView[0].Bytes, ECrypting.EnumToString(ECrypting.Methods.aes));
        }

        private new void HandleFileChanged(object? sender, EventArgs? e)
        {
            OnPropertyChanged(nameof(CryptButtonName));
        }

        private void InitializeCryptingComboBox()
        {
            CryptingComboBox = new(_cryptingService.GetCryptors().ToList());
        }

        private void UpdateCurrentCrypting()
        {
            CurrentCryptingViewModel = _cryptingService.GetCurrentCrypting().GetViewModel();
            OnPropertyChanged(nameof(CurrentCryptingViewModel));
        }

        private void SetProgressVisibility(Visibility visibility)
        {
            ProgressVisibility = visibility;
            OnPropertyChanged(nameof(ProgressVisibility));
        }

        private async void PerformCrypting()
        {
            SetProgressVisibility(Visibility.Visible);
            var progressReport = new Progress<string>(ReportCryptingProgress);
            if (!_fileService.HasBytes(CurrentFile) || CurrentFile == null)
            {
                return;
            }

            var result = await _cryptingService.RunCrypting(CurrentFile, progressReport);

            if (result != null && FilesView.AnyFiles())
            {
                ModifyFile(CurrentFile, result, GetOpositeStatus(CurrentFile.Status), _cryptingService.GetCurrentCrypting()?.GetName());
                OnPropertyChanged(nameof(CryptButtonName));
            }

            _ = Task.Delay(ReportDelay).ContinueWith(t => ResetProgressVisibility());
        }

        private CryptingStatus.Status GetOpositeStatus(CryptingStatus.Status curremtStatus)
        {
            return curremtStatus.Equals(CryptingStatus.Status.Decrypted) ?
                CryptingStatus.Status.Encrypted :
                CryptingStatus.Status.Decrypted;
        }

        private void ReportCryptingProgress(string progressMessage)
        {
            ProgessMessage = progressMessage;
            OnPropertyChanged(nameof(ProgessMessage));
            ProgressService.UpdateProgress(Visibility.Visible);
        }

        private void ResetProgressVisibility()
        {
            ProgressService.HideAllProgress();
        }

        private string GetCryptName()
        {
            if (CurrentFile == null)
            {
                return string.Empty;
            }

            return GetOpositeStatus(CurrentFile.Status).ToString();
        }
    }
}
