using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Interfaces;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class CryptingViewModel : InteractiveViewBase
    {
        public ICommand PostFilesCommand { get; set; }

        public ICommand ClearFilesCommand { get; set; }

        public ICommand DeleteCurrentFileCommand { get; set; }

        public CryptingViewModel(IFilesManager filesManager) : base(filesManager)
        {
            PostFilesCommand = new Command(AddFiles, true);
            ClearFilesCommand = new Command(ClearAllFiles, true);
            DeleteCurrentFileCommand = new Command(DeleteItem, true);
        }
    }
}
