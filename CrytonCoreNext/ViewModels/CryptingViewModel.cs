﻿using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using CrytonCoreNext.Static;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            LoadFilesCommand = new Command(LoadCryptFiles, CanExecute);
            SaveFileCommand = new Command(SaveCryptFile, CanExecute);

            InitializeCryptingComboBox();
            UpdateCurrentCrypting();

            FilesViewModel.CurrentFileChanged += HandleCurrentFileChanged;
            FilesViewModel.FileDeleted += HandleFileDeleted;
            FilesViewModel.AllFilesDeleted += HandleAllFilesDeleted;
        }

        private void HandleAllFilesDeleted(object? sender, EventArgs e)
        {
            _files.Clear();
            CurrentFile = null;
            OnPropertyChanged(nameof(CurrentFile));
        }

        private void HandleFileDeleted(object? sender, EventArgs e)
        {
            var deletedFileGuid = FilesViewModel.GetDeletedFileGuid();
            _files.Remove(_files.Select(x => x).Where(x => x.Guid == deletedFileGuid).First());
            OnPropertyChanged(nameof(CurrentFile));
        }

        private void HandleCurrentFileChanged(object? sender, EventArgs? e)
        {
            var file = _files.FirstOrDefault(x => x?.Guid == FilesViewModel.GetCurrentFile()?.Guid);
            var files = FilesViewModel.GetFiles();
            //UpdateFiles(files);

            if (file == null)
            {
                return;
            }

            if (!file.Guid.Equals(CurrentFile?.Guid))
            {
                CurrentFile = file;
                OnPropertyChanged(nameof(CurrentFile));
                OnPropertyChanged(nameof(CryptButtonName));
            }
        }

        private void LoadCryptFiles()
        {
            IsBusy = true;
            var files = base.LoadFiles();
            foreach (var file in files)
            {
                _files.Add(_cryptingService.ReadCryptFile(file));
            }

            FilesViewModel.UpdateFiles(files);
            IsBusy = false;
        }

        private void SaveCryptFile()
        {
            _cryptingService.AddRecognitionBytes(CurrentFile);
            base.SaveFile(CurrentFile);
        }

        public override bool CanExecute()
        {
            return !IsBusy && !CurrentCryptingViewModel.IsBusy;
        }

        private void UpdateFiles(IEnumerable<File> files)
        {
            if (IsBusy)
            {
                return;
            }

            foreach (var file in _files)
            {
                if (!files.Contains(file))
                {
                    _files.Remove(file);
                }
            }
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
            var progressReport = ProgressViewModel.InitializeProgress<string>(_cryptingService.GetCurrentCryptingProgressCount());
            if (!_fileService.HasBytes(CurrentFile))
            {
                return;
            }

            var result = await _cryptingService.RunCrypting(CurrentFile, progressReport);

            if (result != null && result.Length > 0 && _files.Any())
            {
                _cryptingService.ModifyFile(CurrentFile, result, GetOpositeStatus(CurrentFile.Status), _cryptingService.GetCurrentCrypting().GetName());
                OnPropertyChanged(nameof(CurrentFile));
                OnPropertyChanged(nameof(CryptButtonName));
            }

            ActionTimer.InitializeTimerWithAction(ProgressViewModel.ClearProgress, 2);
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
