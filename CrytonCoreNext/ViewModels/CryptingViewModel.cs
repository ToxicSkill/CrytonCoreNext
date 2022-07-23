using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Interfaces;
using System;
using System.Collections.Generic;
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

        private readonly IEnumerable<ICrypting> _cryptors;

        private string _currentCryptingName = "";

        private int _progressCounter = 0;

        public ICommand LoadFilesCommand { get; init; }

        public ICommand SaveFileCommand { get; init; }

        public ICommand CryptCommand { get; init; }

        public ObservableCollection<string> CryptingComboBox { get; private set; }

        public ICrypting? CurrentCrypting { get; private set; }

        public string CryptButtonName => GetCryptName();

        public string ProgessMessage { get; set; }

        public Visibility ProgressVisibility { get; set; } = Visibility.Hidden;

        public Visibility PreparingVisibility { get; set; }

        public Visibility StartingVisibility { get; set; }

        public Visibility FinishedVisibility { get; set; }

        public Visibility UpdatingVisibility { get; set; }

        public Visibility SuccessVisibility { get; set; }


        public string CurrentCryptingName
        {
            get => _currentCryptingName;
            set
            {
                if (value != _currentCryptingName)
                {
                    _currentCryptingName = value;
                    UpdateCurrentCrypting();
                    OnPropertyChanged(nameof(CurrentCryptingName));
                }
            }
        }

        public ViewModelBase CurrentCryptingViewModel { get; private set; }

        public CryptingViewModel(IFileService fileService, IEnumerable<ICrypting> cryptors) : base(fileService)
        {
            CryptCommand = new Command(DoCrypt, true);
            LoadFilesCommand = new Command(LoadFiles, true);
            SaveFileCommand = new Command(SaveFile, true);
            CurrentCryptingViewModel = new ();
            CryptingComboBox = new ();
            _cryptors = cryptors;
            InitializeCryptingComboBox();
            FilesViewViewModel.FilesChanged += HandleFileChanged;
            ResetProgressVisibility();

            //var t = new Crypting.Crypting(new() { new(new AES(), ECrypting.EnumToString(ECrypting.Methods.aes)) });
            //t.Encrypt(FilesViewViewModel.FilesView[0].Bytes, ECrypting.EnumToString(ECrypting.Methods.aes));
        }

        private new void HandleFileChanged(object? sender, EventArgs? e)
        {
            OnPropertyChanged(nameof(CryptButtonName));
        }

        private bool InitializeCryptingComboBox()
        {
            foreach (var cryptor in _cryptors)
            {
                CryptingComboBox.Add(cryptor.GetName());
            }

            if (CryptingComboBox.Count > 0)
            {
                CurrentCryptingName = _cryptors.ToList().First().GetName();
                UpdateCurrentCrypting();
                return true;
            }

            return false;
        }

        private void UpdateCurrentCrypting()
        {
            CurrentCrypting = _cryptors.Where(x => x.GetName() == _currentCryptingName).First();
            CurrentCryptingViewModel = CurrentCrypting.GetViewModel();

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
                        await CurrentCrypting.Decrypt(FilesViewViewModel.CurrentFile.Bytes, progressReport) :
                        await CurrentCrypting.Encrypt(FilesViewViewModel.CurrentFile.Bytes, progressReport);
                    if (result != null && FilesViewViewModel.FilesView != null)
                    {
                        ModifyFile(FilesViewViewModel.FilesView, FilesViewViewModel.CurrentFile.Guid, result, !FilesViewViewModel.CurrentFile.Status, CurrentCrypting?.GetName());
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
