using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Enums;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Static;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class CryptingViewModel : InteractiveViewBase
    {
        private readonly ICryptingService _cryptingService;

        private List<CryptFile> _files;

        public CryptFile CurrentFile { get; set; }

        public ICommand LoadFilesCommand { get; init; }

        public ICommand SaveFileCommand { get; init; }

        public ICommand CryptCommand { get; init; }

        public ObservableCollection<ICrypting> CryptingComboBox { get; private set; }

        public ViewModelBase CurrentCryptingViewModel { get; private set; }

        public string ProgessMessage { get; set; } = string.Empty;

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
        public CryptingViewModel(IFileService fileService, IDialogService dialogService, ICryptingService cryptingService, IFilesView filesView, IProgressView progressView) : base(fileService, dialogService, filesView, progressView)
        {
            _files = new();
            _cryptingService = cryptingService;
            ProgressViewModel.ChangeProgressType(BusyIndicator.IndicatorType.ThreeDots);

            CurrentCryptingViewModel = new();
            CryptingComboBox = new();

            CryptCommand = new Command(PerformCrypting, CanExecute);
            LoadFilesCommand = new AsyncCommand(LoadCryptFiles, CanExecute);
            SaveFileCommand = new Command(SaveCryptFile, CanExecute);

            InitializeCryptingComboBox();
            UpdateCurrentCrypting();

            FilesViewModel.CurrentFileChanged += HandleCurrentFileChanged;
            FilesViewModel.FileDeleted += HandleFileDeleted;
            FilesViewModel.AllFilesDeleted += HandleAllFilesDeleted;
        }

        public override bool CanExecute()
        {
            return !IsBusy && !CurrentCryptingViewModel.IsBusy;
        }

        private void HandleAllFilesDeleted(object? sender, EventArgs e)
        {
            _files.Clear();
        }

        private void HandleFileDeleted(object? sender, EventArgs e)
        {
            var deletedFileGuid = FilesViewModel.GetDeletedFileGuid();
            _files.Remove(_files.Select(x => x).Where(x => x.Guid == deletedFileGuid).First());
        }

        private void HandleCurrentFileChanged(object? sender, EventArgs? e)
        {
            var file = _files.FirstOrDefault(x => x?.Guid == FilesViewModel.GetCurrentFileGuid());
            if (file != null)
            {
                CurrentFile = file;
                OnPropertyChanged(nameof(CurrentFile));
                OnPropertyChanged(nameof(CryptButtonName));
            }
        }

        private async Task LoadCryptFiles()
        {
            Lock();
            await foreach (var file in base.LoadFiles())
            {
                FilesViewModel.AddFile(file);
                _files.Add(_cryptingService.ReadCryptFile(file));
            }

            FilesViewModel.UpdateFiles();
            Unlock();
        }

        private void SaveCryptFile()
        {
            _cryptingService.AddRecognitionBytes(CurrentFile);
            base.SaveFile(CurrentFile);
        }

        private void InitializeCryptingComboBox()
        {
            CryptingComboBox = new(_cryptingService.GetCryptors());
        }

        private void UpdateCurrentCrypting()
        {
            CurrentCryptingViewModel = _cryptingService.GetCurrentCrypting().GetViewModel();
            OnPropertyChanged(nameof(CurrentCryptingViewModel));
        }

        private async void PerformCrypting()
        {
            Lock();
            if (!_fileService.HasBytes(CurrentFile) || IsCorrectMethod())
            {
                CurrentCryptingViewModel.Log(ELogLevel.Error, Language.Post("WrongMethod"));
                return;
            }

            var progressReport = ProgressViewModel.InitializeProgress<string>(_cryptingService.GetCurrentCryptingProgressCount());
            var result = await _cryptingService.RunCrypting(CurrentFile, progressReport);

            if (result != null && result.Length > 0 && _files.Any())
            {
                _cryptingService.ModifyFile(CurrentFile, result, GetOpositeStatus(CurrentFile.Status), _cryptingService.GetCurrentCrypting().GetName());
                OnPropertyChanged(nameof(CurrentFile));
                OnPropertyChanged(nameof(CryptButtonName));
            }

            ActionTimer.InitializeTimerWithAction(ProgressViewModel.ClearProgress);
            Unlock();
        }

        private bool IsCorrectMethod()
        {
            return CurrentFile.Status == CryptingStatus.Status.Encrypted &&
                CurrentCryptingViewModel.PageName != CurrentFile.Method;
        }

        private static CryptingStatus.Status GetOpositeStatus(CryptingStatus.Status currentStatus)
        {
            return currentStatus.Equals(CryptingStatus.Status.Decrypted) ?
                CryptingStatus.Status.Encrypted :
                CryptingStatus.Status.Decrypted;
        }

        private string GetCryptName()
        {
            if (CurrentFile == null)
            {
                return string.Empty;
            }

            return GetOpositeStatus(CurrentFile.Status).ToDescription();
        }
    }
}
