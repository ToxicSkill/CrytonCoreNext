using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class CryptingViewModel : InteractiveViewBase
    {
        private IEnumerable<ICrypting> _cryptors;

        private string _currentCryptingName;

        public ICommand PostFilesCommand { get; set; }

        public ICommand ClearFilesCommand { get; set; }

        public ICommand DeleteCurrentFileCommand { get; set; }

        public ICommand SetFileAsFirstCommand { get; set; }

        public ICommand SetFileAsLastCommand { get; set; }

        public ICommand MoveFileUpCommand { get; set; }

        public ICommand MoveFileDownCommand { get; set; }

        public ObservableCollection<string> CryptingComboBox { get; private set; }

        public ICrypting CurrentCrypting { get; private set; }

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

        public CryptingViewModel(IFilesManager filesManager, IEnumerable<ICrypting> cryptors) : base(filesManager)
        {
            PostFilesCommand = new Command(AddFiles, true);
            ClearFilesCommand = new Command(ClearAllFiles, true);
            DeleteCurrentFileCommand = new Command(DeleteFile, true);
            SetFileAsFirstCommand = new Command(SetFileAsFirst, true);
            SetFileAsLastCommand = new Command(SetFileAsLast, true);
            MoveFileUpCommand = new Command(MoveFileUp, true);
            MoveFileDownCommand = new Command(MoveFileDown, true);
            CurrentCryptingViewModel = new();
            CryptingComboBox = new();
            _cryptors = cryptors;
            InitializeCryptingComboBox();
            
            //var t = new Crypting.Crypting(new () { new (new AES(), ECrypting.EnumToString(ECrypting.Methods.aes)) });
            //t.Encrypt(FilesViewViewModel.FilesView[0].Bytes, ECrypting.EnumToString(ECrypting.Methods.aes));
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
    }
}
