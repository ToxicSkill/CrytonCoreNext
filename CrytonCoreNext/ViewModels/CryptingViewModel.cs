using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class CryptingViewModel : InteractiveViewBase
    {
        public ICommand PostFilesCommand { get; set; }

        public ICommand ClearFilesCommand { get; set; }

        public ICommand DeleteCurrentFileCommand { get; set; }

        public ICommand SetFileAsFirstCommand { get; set; }

        public ICommand SetFileAsLastCommand { get; set; }

        public ICommand MoveFileUpCommand { get; set; }

        public ICommand MoveFileDownCommand { get; set; }

        public CryptingViewModel(IFilesManager filesManager, IEnumerable<ICrypting> cryptors) : base(filesManager)
        {
            PostFilesCommand = new Command(AddFiles, true);
            ClearFilesCommand = new Command(ClearAllFiles, true);
            DeleteCurrentFileCommand = new Command(DeleteFile, true);
            SetFileAsFirstCommand = new Command(SetFileAsFirst, true);
            SetFileAsLastCommand = new Command(SetFileAsLast, true);
            MoveFileUpCommand = new Command(MoveFileUp, true);
            MoveFileDownCommand = new Command(MoveFileDown, true);
            CryptingOptionsViewModel = cryptors.ToList()[0].GetViewModel();
            OnPropertyChanged(nameof(CryptingOptionsViewModel));

            //var t = new Crypting.Crypting(new () { new (new AES(), ECrypting.EnumToString(ECrypting.Methods.aes)) });
            //t.Encrypt(FilesViewViewModel.FilesView[0].Bytes, ECrypting.EnumToString(ECrypting.Methods.aes));
        }
    }
}
