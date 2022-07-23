using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class CryptingViewModel : InteractiveViewBase
    {
        private readonly IEnumerable<ICrypting> _cryptors;

        private string _currentCryptingName = "";

        public ICommand LoadFilesCommand { get; init; }

        public ICommand SaveFileCommand { get; init; }

        public ICommand CryptCommand { get; init; }

        public ObservableCollection<string> CryptingComboBox { get; private set; }

        public ICrypting? CurrentCrypting { get; private set; }

        public string CryptButtonName => GetCryptName();

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

        async void DoCrypt()
        {
            if (FilesViewViewModel.CurrentFile != null)
            {
                if (FilesViewViewModel.CurrentFile.Bytes != null)
                {
                    var result = FilesViewViewModel.CurrentFile.Status ?
                        await CurrentCrypting?.Decrypt(FilesViewViewModel.CurrentFile.Bytes) :
                        await CurrentCrypting?.Encrypt(FilesViewViewModel.CurrentFile.Bytes);
                    if (result != null && FilesViewViewModel.FilesView != null)
                    {
                        ModifyFile(FilesViewViewModel.FilesView, FilesViewViewModel.CurrentFile.Guid, result, !FilesViewViewModel.CurrentFile.Status, CurrentCrypting?.GetName());
                        OnPropertyChanged(nameof(CryptButtonName));
                    }
                }
            }
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
