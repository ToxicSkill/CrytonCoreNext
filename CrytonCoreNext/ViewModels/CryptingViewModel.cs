﻿using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using CrytonCoreNext.Static;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class CryptingViewModel : InteractiveViewBase<CryptFile>
    {
        private readonly ICryptingService _cryptingService;

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
            _cryptingService = cryptingService;
            ProgressViewModel.ChangeProgressType(BusyIndicator.IndicatorType.ThreeDots);

            CurrentCryptingViewModel = new();
            CryptingComboBox = new();

            CryptCommand = new Command(PerformCrypting, CanExecute);
            LoadFilesCommand = new Command(LoadFiles, CanExecute);
            SaveFileCommand = new Command(SaveFile, CanExecute);

            InitializeCryptingComboBox();
            UpdateCurrentCrypting();

            FilesViewModel.FilesChanged += HandleFileChanged;

            //var t = new Crypting.Crypting(new() { new(new AES(), ECrypting.EnumToString(ECrypting.Methods.aes)) });
            //t.Encrypt(FilesViewViewModel.FilesView[0].Bytes, ECrypting.EnumToString(ECrypting.Methods.aes));
        }

        public override bool CanExecute()
        {
            return !IsBusy && !CurrentCryptingViewModel.IsBusy;
        }

        private new void HandleFileChanged(object? sender, EventArgs? e)
        {
            OnPropertyChanged(nameof(CryptButtonName));
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
            if (!_fileService.HasBytes(CurrentFile) || CurrentFile == null)
            {
                return;
            }

            var result = await _cryptingService.RunCrypting((CryptFile)CurrentFile, progressReport);

            if (result != null && result.Length > 0 && FilesViewModel.AnyFiles())
            {
                ModifyFile(CurrentFile, result, GetOpositeStatus(((CryptFile)CurrentFile).Status), _cryptingService.GetCurrentCrypting()?.GetName());
                OnPropertyChanged(nameof(CryptButtonName));
            }

            ActionTimer.InitializeTimerWithAction(ProgressViewModel.ClearProgress, 2);
        }

        private static CryptingStatus.Status GetOpositeStatus(CryptingStatus.Status curremtStatus)
        {
            return curremtStatus.Equals(CryptingStatus.Status.Decrypted) ?
                CryptingStatus.Status.Encrypted :
                CryptingStatus.Status.Decrypted;
        }

        private string GetCryptName()
        {
            if (CurrentFile == null)
            {
                return string.Empty;
            }

            return GetOpositeStatus(((CryptFile)CurrentFile).Status).ToDescription();
        }
    }
}
