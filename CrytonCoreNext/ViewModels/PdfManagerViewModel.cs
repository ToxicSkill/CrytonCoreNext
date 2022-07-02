using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces;

namespace CrytonCoreNext.ViewModels
{
    public class PdfManagerViewModel : InteractiveViewBase
    {
        //public ICommand PostFilesCommand { get; set; }

        //public ICommand ClearFilesCommand { get; set; }

        //public ICommand DeleteCurrentFileCommand { get; set; }

        //public ICommand SetFileAsFirstCommand { get; set; }

        //public ICommand SetFileAsLastCommand { get; set; }

        //public ICommand MoveFileUpCommand { get; set; }

        //public ICommand MoveFileDownCommand { get; set; }

        public PdfManagerViewModel(IFilesManager filesManager) : base(filesManager)
        {
            //PostFilesCommand = new Command(AddFiles, true);
            //ClearFilesCommand = new Command(ClearAllFiles, true);
            //DeleteCurrentFileCommand = new Command(DeleteFile, true);
            //SetFileAsFirstCommand = new Command(SetFileAsFirst, true);
            //SetFileAsLastCommand = new Command(SetFileAsLast, true);
            //MoveFileUpCommand = new Command(MoveFileUp, true);
            //MoveFileDownCommand = new Command(MoveFileDown, true);
        }
    }
}
