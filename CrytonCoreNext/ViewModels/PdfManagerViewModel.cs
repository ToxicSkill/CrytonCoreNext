using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CrytonCoreNext.ViewModels
{
    public class PdfManagerViewModel : InteractiveViewBase
    {
        private int _iterator = 0;
        public WriteableBitmap Bitmap { get; set; }

        public ICommand PostFilesCommand { get; set; }

        public ICommand ClearFilesCommand { get; set; }

        public ICommand DeleteCurrentFileCommand { get; set; }

        public ICommand SetFileAsFirstCommand { get; set; }

        public ICommand SetFileAsLastCommand { get; set; }

        public ICommand MoveFileUpCommand { get; set; }

        public ICommand MoveFileDownCommand { get; set; }

        public PdfManagerViewModel(IFileService fileService, IDialogService dialogService, IFilesView filesView, IProgressView progressView) : base(fileService, dialogService, filesView, progressView)
        {
            PostFilesCommand = new AsyncCommand(AddFiles, CanExecute);
            ClearFilesCommand = new AsyncCommand(Up, CanExecute);
            //DeleteCurrentFileCommand = new Command(DeleteFile, true);
            //SetFileAsFirstCommand = new Command(SetFileAsFirst, true);
            //SetFileAsLastCommand = new Command(SetFileAsLast, true);
            //MoveFileUpCommand = new Command(MoveFileUp, true);
            //MoveFileDownCommand = new Command(MoveFileDown, true);
        }
        private bool CanExecute()
        {
            return true;
        }

        private async Task Up()
        {
            _iterator++;
            await AddFiles();
        }

        private async Task AddFiles()
        {
            var PDFReader = new PDFReader();
            var pdfManager = new PDFManager();
            var pdf = PDFReader.ReadPdf("E:\\Code\\C#\\CrytonCoreNext\\CrytonCoreNextTests\\TestingFiles\\1.pdf");
            await Run(pdfManager, pdf, _iterator);
        }

        private async Task Run(PDFManager pdfManager, PDFBase pdf, int numberOfPage)
        {
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                Bitmap = pdfManager.GetImageFromPdf(pdf, numberOfPage);
                OnPropertyChanged(nameof(Bitmap));
            }));
        }
    }
}
